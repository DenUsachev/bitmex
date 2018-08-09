using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Connector.REST;
using Connector.REST.Entities;
using Connector.WS;
using Connector.WS.Entities;

namespace Console
{
    class Program
    {
        private static string _instrument = Symbols.Bitcoin;
        private static string _restEndpoint;
        private static string _wsEndpoint;
        private static string _secret;
        private static string _key;
        private static volatile RestApiConnector _client;
        private static WebSocketFeed _wsf;

        static void Main(string[] args)
        {
            System.Console.WriteLine("BitMEX Test Net Connector.\nPress CTRL+Break to exit.");
            try
            {
                _restEndpoint = ConfigurationManager.AppSettings.Get("RestEndpoint");
                _wsEndpoint = ConfigurationManager.AppSettings.Get("WebSocketEndpoint");
                _key = ConfigurationManager.AppSettings.Get("BitmexApiKey");
                _secret = ConfigurationManager.AppSettings.Get("BitmexApiSecret");

                _client = new RestApiConnector(_restEndpoint, _key, _secret);

                // intinite loop wraparound
                System.Console.CancelKeyPress += (sender, e) =>
                {
                    var isCtrlC = e.SpecialKey == ConsoleSpecialKey.ControlC;
                    if (isCtrlC)
                    {
                        var disconnectionResult = _client.Disconnect();
                        _wsf.Disconnect();
                        _wsf.Dispose();
                        if (disconnectionResult.IsSuccess)
                        {
                            System.Console.WriteLine("[REST API]Client disconnected.");
                        }
                        else
                        {
                            System.Console.WriteLine("[REST API]Could not terminate session: {0}", disconnectionResult.Error);
                        }
                        System.Console.WriteLine("Press Enter key to close the app.");
                        System.Console.ReadLine();
                        e.Cancel = true;
                    }
                };

                Task.Factory.StartNew(ListenWebSocket);

                // Wait for websocket to connect
                Thread.Sleep(300);
                var user = _client.Connect();
                if (user.IsSuccess)
                {
                    System.Console.WriteLine("Client connected: {0}, {1}", user.FirstName, user.LastName);
                    System.Console.WriteLine("Trying to set order");

                    // to prevent anti-spam filtering of BitMex
                    var random = new Random(int.MaxValue);
                    var priceDivergence = random.Next(1, 30);
                    var valueSign = priceDivergence % 2 == 0 ? 1 : -1;
                    var order = new OrderItem
                    {
                        side = "Buy",
                        symbol = _instrument,
                        orderQty = random.Next(1, 5),
                        price = (decimal)(6330 + priceDivergence * valueSign),
                        ordType = "Limit"
                    };

                    var orderSubmitResult = _client.RegisterOrder(order);
                    if (orderSubmitResult.IsSuccess)
                    {
                        System.Console.WriteLine("[REST API][{0}] {1} {2} by {3:#.00} - order sent ({4})", order.side, order.orderQty, order.symbol, order.price, orderSubmitResult.orderID);
                    }
                    else
                    {
                        System.Console.WriteLine("[REST API]Could not allocate order: {0}", orderSubmitResult.Error);
                    }

                    var cancelOrderResult = _client.CancelOrder(orderSubmitResult.orderID);
                    if (cancelOrderResult.IsSuccess)
                    {
                        System.Console.WriteLine("[REST API]Order {0} cancelled.", orderSubmitResult.orderID);
                    }
                }
                else
                {
                    System.Console.WriteLine("[API]Connection error: {0}", user.Error);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Application error: {0}.", e.Message);
                System.Console.ReadLine();
            }

            while (true)
            {
                // infinite loop to keep websocket open
            }
        }

        private static void ListenWebSocket()
        {
            _wsf = new WebSocketFeed(_wsEndpoint);
            var webSoketChannels = new List<string> { "order", "trade" };
            _wsf.NewInfoMessage += WebSocketInfoMessage;
            _wsf.NewTradeMessage += WebSocketTableMessage;
            _wsf.NewOrderMessage += WebSocketOrderMessage;
            string socketError;
            if (_wsf.Connect(_key, _secret, out socketError))
            {
                _wsf.Subscribe(webSoketChannels, _instrument);
            }
            else
            {
                System.Console.WriteLine("[Web socket] Error: {0}", socketError);
            }
        }

        private static void WebSocketOrderMessage(object sender, Order e)
        {
            System.Console.WriteLine("[Web Socket: {2} Order] ID: {0}, Type: {1}, Symbol:{3}, Price:{4}, Qty: {5}", e.OrderId, e.OrdType, e.OrdStatus, e.Symbol, e.Price, e.OrderQty);
        }

        private static void WebSocketTableMessage(object sender, TradeResponse e)
        {
            foreach (var trade in e.Data)
            {
                System.Console.WriteLine("[Web Socket: New trade] {0} {1} {2} for price: {3}", trade.Side, trade.Size, trade.Symbol, trade.Price);
            }
        }

        static void WebSocketInfoMessage(object sender, InfoResponse e)
        {
            System.Console.WriteLine("[Web socket Info] {0}", e.Info);
        }
    }
}
