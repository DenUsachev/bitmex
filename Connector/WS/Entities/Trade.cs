namespace Connector.WS.Entities
{
    public class Trade
    {
        public string Symbol { get; set; }
        public string Side { get; set; }
        public decimal Size { get; set; }
        public decimal Price { get; set; }
    }
}
