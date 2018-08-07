using Newtonsoft.Json;

namespace Connector.REST.Entities
{
    public class OrderObject : BaseRestObject
    {
        public string symbol { get; set; }

        public string side { get; set; }

        public decimal orderQty { get; set; }

        public string ordType { get; set; }
    }
}