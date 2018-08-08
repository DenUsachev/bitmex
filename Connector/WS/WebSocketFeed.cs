using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Connector.WS
{
    public class WebSocketFeed
    {
        private WebSocket _webSocket;

        public string ConnectionString { get; set; }


        public WebSocketFeed(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool Connect()
        {
            _webSocket = new WebSocket(ConnectionString);
            _webSocket.OnMessage += MessageReceived;
            _webSocket.Connect();
            return true;
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        public bool Subscribe(List<string> channels, string instrument)
        {
            var message = new WebSocketCommand()
            {
                op = "subscribe",
                args = (new List<string> { "orderBookL2", "instrument" }).ToArray()
            };
            return true;
        }
    }
}
