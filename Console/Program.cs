using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Connector.REST;
using Connector.REST.Entities;
using Connector.WS;
using Connector.WS.Entities;

namespace Console
{
    class Program
    {
        private static string _instrument = Symbols.Eth;
        private static string _restEndpoint;
        private static string _wsEndpoint;
        private static string _secret;
        private static string _key;

        static void Main(string[] args)
        {
            System.Console.WriteLine("BitMEX Test Net Connector.\nPress CTRL+Break to exit.");
            try
            {
                _restEndpoint = ConfigurationManager.AppSettings.Get("RestEndpoint");
                _wsEndpoint = ConfigurationManager.AppSettings.Get("WebSocketEndpoint");
                _key = ConfigurationManager.AppSettings.Get("BitmexApiKey");
                _secret = ConfigurationManager.AppSettings.Get("BitmexApiSecret");

                // intinite loop wraparound
                System.Console.CancelKeyPress += (sender, e) =>
                {
                    var isCtrlC = e.SpecialKey == ConsoleSpecialKey.ControlC;
                    if (isCtrlC)
                    {
                        e.Cancel = true;
                    }
                };

                Task.Factory.StartNew(ListenWebSocket);

                //var client = new RestApiConnector(_restEndpoint, _key, _secret);
                //var user = client.Connect();
                //if (user.IsSuccess)
                //{
                //    System.Console.WriteLine("Client connected: {0}, {1}", user.FirstName, user.LastName);
                //    System.Console.WriteLine("Trying to set order");

                //    // to prevent anti-spam filtering of BitMex
                //    var random = new Random(int.MaxValue);
                //    var priceDivergence = random.Next(1, 10);
                //    var valueSign = priceDivergence % 2 == 0 ? 1 : -1;
                //    var order = new OrderItem
                //    {
                //        side = "Buy",
                //        symbol = _instrument,
                //        orderQty = random.Next(10, 1000),
                //        price = (decimal)(402.2 + priceDivergence * valueSign),
                //        ordType = "Limit"
                //    };

                //    var orderSubmitResult = client.RegisterOrder(order);
                //    if (orderSubmitResult.IsSuccess)
                //    {
                //        System.Console.WriteLine("[REST API][{0}] {1} {2} by {3:#.00} - order sent ({4})", order.side, order.orderQty, order.symbol, order.price, orderSubmitResult.orderID);
                //    }
                //    else
                //    {
                //        System.Console.WriteLine("[REST API]Could not allocate order: {0}", orderSubmitResult.Error);
                //    }

                //    var cancelOrderResult = client.CancelOrder(orderSubmitResult.orderID);
                //    if (cancelOrderResult.IsSuccess)
                //    {
                //        System.Console.WriteLine("[REST API]Order {0} cancelled.", orderSubmitResult.orderID);
                //    }
                //    //var disconnectionResult = client.Disconnect();
                //    //if (disconnectionResult.IsSuccess)
                //    //{
                //    //    System.Console.WriteLine("[REST API]Client {0}, {1} disconnected at {2:T}", user.FirstName, user.LastName, DateTime.Now);
                //    //}
                //}
                //else
                //{
                //    System.Console.WriteLine("[API]Connection error: {0}", user.Error);
                //}
            }
            catch (ConfigurationErrorsException e)
            {
                System.Console.WriteLine("[APP]Configuration error: {0} in line {1}", e.Message, e.Line);
                System.Console.ReadLine();
            }

            while (true)
            {
                // infinite loop to keep websocket open
            }
        }

        private static void ListenWebSocket()
        {
            var wsf = new WebSocketFeed(_wsEndpoint);
            var webSoketChannels = new List<string>
                {
                    "position"
                };
            wsf.NewInfoMessage += WebSocketInfoMessage;
            wsf.NewTradeMessage += WebSocketTableMessage;
            string socketError;
            if (wsf.Connect(_key, _secret, out socketError))
            {
                wsf.Subscribe(webSoketChannels, _instrument);
            }
            else
            {
                System.Console.WriteLine("[Web socket] Error: {0}", socketError);
            }

        }

        private static void WebSocketTableMessage(object sender, TradeResponse e)
        {
            foreach (var trade in e.Data)
            {
                System.Console.WriteLine("[Web Socket: New trade] {0} {1} {2} for price: {3}", trade.Side, trade.Size, trade.Symbol, trade.Price);
            }
        }

        static void WebSocketInfoMessage(object sender, Connector.WS.Entities.InfoResponse e)
        {
            System.Console.WriteLine("[Web socket Info] {0}", e.Info);
        }
    }
}
