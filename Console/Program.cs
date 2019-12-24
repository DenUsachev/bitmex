﻿using System;
using System.Configuration;

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
                    var order = new OrderObject
                    {
                        Side = "Buy",
                        Symbol = Symbols.BuySellEth,
                        OrderQty = 10,
                        OrderType = "Market"
                    };
                    //var orderSubmitResult = client.RegisterOrder(order);
                    //if (orderSubmitResult.IsSuccess)
                    //{
                    //    System.Console.WriteLine("Order to buy {0} {1} allocated.", order.OrderQty, order.Symbol);
                    //}
                    //else
                    //{
                    //    System.Console.WriteLine("Could not allocate order: {0}.", orderSubmitResult.Error);
                    //}
                    var disconnectionResult = client.Disconnect();
                    if (disconnectionResult.IsSuccess)
                    {
                        System.Console.WriteLine("Client {0}, {1} disconnected at {2:T}", user.FirstName, user.LastName, DateTime.Now);
                    }
                }
                else
                {
                    System.Console.WriteLine("Connection error: {0}", user.Error);
                }
            }
            catch (ConfigurationErrorsException e)
            {
                System.Console.WriteLine("Configuration error: {0} in line {1}", e.Message, e.Line);
                System.Console.ReadLine();
            }

            System.Console.ReadLine();
        }
    }
}
