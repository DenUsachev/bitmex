using System.Net;

<<<<<<< HEAD:Connector/REST/Entities/RestResponse.cs
namespace Connector.REST.Entities
=======
namespace MarketConnectivity.REST
>>>>>>> * Bitmex.Runner.csproj: Projects ported to netcore:MarketConnectivity/REST/RestResponse.cs
{
    public class RestResponse
    {
        public dynamic Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public RestError Error { get; set; }
        public bool IsSuccess { get; set; }
    }
}
