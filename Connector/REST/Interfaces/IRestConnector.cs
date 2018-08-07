using Connector.REST.Entities;

namespace Connector.REST.Interfaces
{
    public interface IRestConnector
    {
        string ApiKey { get; }
        string ApiSecret { get; }

        UserObject Connect();
        RestResponse Disconnect();
        OrderItem RegisterOrder(OrderItem order);
        RestResponse CancelOrder(string orderId, string comment=null);
        object SubscribeOrderbook();
    }
}
