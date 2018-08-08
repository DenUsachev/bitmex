namespace Connector.WS
{
    public class WebSocketCommand
    {
        public string op { get; set; }
        public string[] args { get; set; }
        public bool success { get; set; }
    }
}
