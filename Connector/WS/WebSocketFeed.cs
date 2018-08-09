using System;
using System.Collections.Generic;
using Connector.REST;
using Connector.WS.Entities;
using Newtonsoft.Json;
using WebSocketSharp;

namespace Connector.WS
{
    public class WebSocketFeed : IDisposable
    {
        private WebSocket _webSocket;
        private const long EXPIRES = 10;
        private const string FEED_RESOURCE = "/realtime";

        public string ConnectionString { get; set; }

        public event EventHandler<InfoResponse> NewInfoMessage;
        public event EventHandler<TradeResponse> NewTradeMessage;

        public WebSocketFeed(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool Connect(string key, string secret, out string error)
        {
            try
            {
                var endpointUrl = new Uri(ConnectionString);
                var sign = HttpHelper.CalculateSignature(endpointUrl, EXPIRES, secret, "GET", FEED_RESOURCE);
                //var authrorizedUrl = string.Format("{0}?api-expires={1}&api-signature={2}&api-key={3}", endpointUrl, EXPIRES, sign, key);
                var authrorizedUrl = string.Format("{0}{1}", ConnectionString, FEED_RESOURCE);
                _webSocket = new WebSocket(authrorizedUrl) { Log = { Level = LogLevel.Debug } };
                _webSocket.OnMessage += MessageReceived;
                _webSocket.Connect();
                Auth(key, sign);

                error = string.Empty;
                return true;
            }
            catch (WebSocketException wex)
            {
                error = wex.Message;
                return false;
            }
        }

        private void Auth(string key, string sign)
        {
            var message = new WebSocketCommand()
            {
                op = "authKeyExpires",
                args = new object[] { key, EXPIRES, sign }
            };
            var m = JsonConvert.SerializeObject(message);
            _webSocket.Send(m);
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(e.Data);
            Console.WriteLine(jsonData);
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
    }
}
