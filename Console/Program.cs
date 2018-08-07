using System;
using System.Configuration;
using Connector.REST;
using Connector.REST.Entities;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("BitMEX Test Net Connector");
            try
            {
                var restEndpoint = ConfigurationManager.AppSettings.Get("RestEndpoint");
                var wsEndpoint = ConfigurationManager.AppSettings.Get("WebSocketEndpoint");
                var key = ConfigurationManager.AppSettings.Get("BitmexApiKey");
                var secret = ConfigurationManager.AppSettings.Get("BitmexApiSecret");

                var client = new RestApiConnector(restEndpoint, key, secret);
                var user = client.Connect();
                if (user.IsSuccess)
                {
                    System.Console.WriteLine("Client connected: {0}, {1}", user.FirstName, user.LastName);
                    System.Console.WriteLine("Trying to set order");
                    var order = new OrderItem
                    {
                        side = "Buy",
                        symbol = Symbols.BuySellEth,
                        orderQty = 1,
                        price = 402.2M,
                        ordType = "Limit"
                    };

                    var orderSubmitResult = client.RegisterOrder(order);
                    if (orderSubmitResult.IsSuccess)
                    {
                        System.Console.WriteLine("[REST API][{0}] {1} {2} by {3:#.00} - order sent ({4})", order.side, order.orderQty, order.symbol, order.price, orderSubmitResult.orderID);
                    }
                    else
                    {
                        System.Console.WriteLine("[REST API]Could not allocate order: {0}", orderSubmitResult.Error);
                    }

                    var cancelOrderResult = client.CancelOrder(orderSubmitResult.orderID);
                    if (cancelOrderResult.IsSuccess)
                    {
                        System.Console.WriteLine("[REST API]Order {0} cancelled.", orderSubmitResult.orderID);
                    }
                    var disconnectionResult = client.Disconnect();
                    if (disconnectionResult.IsSuccess)
                    {
                        System.Console.WriteLine("[REST API]Client {0}, {1} disconnected at {2:T}", user.FirstName, user.LastName, DateTime.Now);
                    }
                }
                else
                {
                    System.Console.WriteLine("[REST API]Connection error: {0}", user.Error);
                }
            }
            catch (ConfigurationErrorsException e)
            {
                System.Console.WriteLine("[APP]Configuration error: {0} in line {1}", e.Message, e.Line);
                System.Console.ReadLine();
            }

            System.Console.ReadLine();
        }
    }
}
