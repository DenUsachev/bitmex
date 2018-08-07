using Newtonsoft.Json;

namespace Connector.REST.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OrderItem : BaseRestObject
    {
        public string orderID { get; set; }

        public string currency { get; set; }

        public decimal price { get; set; }

        public string symbol { get; set; }

        public string side { get; set; }

        public decimal orderQty { get; set; }

        public string ordType { get; set; }
    }
}