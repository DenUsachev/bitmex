using Newtonsoft.Json;

namespace Connector.REST.Entities
{
    public class BaseRestObject
    {
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        
        [JsonIgnore]
        public string Error { get; set; }
    }
}
