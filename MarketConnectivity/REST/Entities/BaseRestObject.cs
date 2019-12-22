using Newtonsoft.Json;

namespace MarketConnectivity.REST.Entities
{
    public class BaseRestObject
    {
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        
        [JsonIgnore]
        public string Error { get; set; }
    }
}
