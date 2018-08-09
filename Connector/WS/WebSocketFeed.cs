using System;
using System.Collections.Generic;
using System.Linq;
using Connector.Extensions;
using Connector.REST;
using Connector.WS.Entities;
using Newtonsoft.Json;
using WebSocketSharp;

namespace Connector.WS
{
    public class WebSocketFeed : IDisposable
    {
        private WebSocket _webSocket;
        private long _expires;
        private const string FEED_RESOURCE = "realtime";
        private const int EXPIRATION_PERIOD = 10;
        public string ConnectionString { get; set; }

        public event EventHandler<InfoResponse> NewInfoMessage;
        public event EventHandler<TradeResponse> NewTradeMessage;
        public event EventHandler<Order> NewOrderMessage;

        private Dictionary<Guid, Order> _orders;

        public WebSocketFeed(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool Connect(string key, string secret, out string error)
        {
            try
            {
                _expires = DateTime.Now.ToUnixTime() + EXPIRATION_PERIOD;
                _orders = new Dictionary<Guid, Order>();

                var endpointUrl = new Uri(ConnectionString);
                var sign = HttpHelper.CalculateSignature(endpointUrl, _expires, secret, "GET", FEED_RESOURCE);
                var authrorizedUrl = string.Format("{0}realtime?api-expires={1}&api-signature={2}&api-key={3}", endpointUrl, _expires, sign, key);
                _webSocket = new WebSocket(authrorizedUrl) { Log = { Level = LogLevel.Error } };
                _webSocket.OnMessage += MessageReceived;
                _webSocket.Connect();

                error = string.Empty;
                return true;
            }
            catch (WebSocketException wex)
            {
                error = wex.Message;
                return false;
            }
        }

        public bool Subscribe(List<string> channels, string instrument)
        {
            var message = new WebSocketCommand()
            {
                op = "subscribe",
                args = channels.ToArray()
            };
            _webSocket.Send(JsonConvert.SerializeObject(message));
            return true;
        }

        public void Disconnect()
        {
            _webSocket.Close(CloseStatusCode.Normal);
        }

        public void Dispose()
        {
            if (_webSocket != null)
            {
                _webSocket.Close(CloseStatusCode.Normal);
            }
        }

        protected virtual void OnNewInfoMessage(InfoResponse e)
        {
            var handler = NewInfoMessage;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnNewTradeMessage(TradeResponse e)
        {
            var handler = NewTradeMessage;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnNewOrderMessage(Order e)
        {
            var handler = NewOrderMessage;
            if (handler != null) handler(this, e);
        }


        private static void UpdateObject<T>(T source, T target) where T : class
        {
            var t = typeof(T);
            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            foreach (var prop in properties)
            {
                var sourceValue = prop.GetValue(source, null);
                var targetValue = prop.GetValue(target, null);
                if (sourceValue != null)
                {
                    if (targetValue == null || !targetValue.Equals(0))
                    {
                        prop.SetValue(target, sourceValue, null);
                    }
                }
                //else
                //{
                //    if (targetValue != null && targetValue != (object)0)
                //    {
                //        prop.SetValue(target, null, null);
                //    }
                //}
            }
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(e.Data);
            if (jsonData.action != null && jsonData.action == "partial")
            {
                return;
            }
            ResponseBase response;
            if (jsonData.info != null)
            {
                response = new InfoResponse
                {
                    Info = jsonData.info,
                    Version = jsonData.version,
                    Timestamp = jsonData.time
                };
                OnNewInfoMessage((InfoResponse)response);
            }
            else if (jsonData.table == "trade")
            {
                response = JsonConvert.DeserializeObject<TradeResponse>(jsonData.ToString());
                OnNewTradeMessage((TradeResponse)response);
            }
            else if (jsonData.table == "order")
            {
                var orderResponse = JsonConvert.DeserializeObject<OrderResponse>(jsonData.ToString());
                foreach (Order order in orderResponse.Data)
                {
                    if (order.OrdStatus == "New")
                    {
                        if (!_orders.ContainsKey(order.OrderId))
                        {
                            _orders.Add(order.OrderId, order);
                        }
                    }
                    else
                    {
                        var oldOrder = _orders[order.OrderId];
                        UpdateObject(oldOrder, order);
                    }
                    OnNewOrderMessage(order);
                }
            }
        }

    }
}
