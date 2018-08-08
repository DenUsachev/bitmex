using System.Net;

namespace Connector.REST.Entities
{
    public class RestResponse
    {
        public dynamic Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public RestError Error { get; set; }
        public bool IsSuccess { get; set; }
    }
}
