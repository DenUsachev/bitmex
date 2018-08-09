namespace Connector.WS.Entities
{
    public class OrderResponse : ResponseBase
    {
        public string Action { get; set; }
        public string Table { get; set; }
        public Order[] Data { get; set; }
    }
}
