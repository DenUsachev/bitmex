using System;
using Newtonsoft.Json;

namespace Connector.REST.Entities
{
    public class OrderObject : BaseRestObject
    {
        public string Symbol { get; set; }
        
        public string Side { get; set; }
        
        public decimal OrderQty { get; set; }
        
        [JsonProperty("ordType")]
        public string  OrderType { get; set; }
        
        public long Timestamp { get; }
        
        public long TransactTime { get; }

        public OrderObject()
        {
            Timestamp = DateTime.UtcNow.ToUnixTime();
            TransactTime = Timestamp;
        }
    }
}