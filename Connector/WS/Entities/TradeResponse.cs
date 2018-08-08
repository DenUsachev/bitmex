namespace Connector.WS.Entities
{
    public class TradeResponse : ResponseBase
    {
        public string Table { get; set; }
        public Trade[] Data { get; set; }
    }
}
