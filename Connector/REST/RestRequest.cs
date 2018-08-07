using System.Collections.Generic;

namespace Connector.REST
{
    public class RestRequest
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public bool IsJson { get; set; }
        public byte[] Data { get; set; }
    }
}
