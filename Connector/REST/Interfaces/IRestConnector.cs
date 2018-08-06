using Connector.REST.Entities;

namespace Connector.REST.Interfaces
{
    public interface IRestConnector
    {
        string ApiKey { get; }
        string ApiSecret { get; }

        UserObject Connect();
        RestResponse Disconnect();
        OrderObject RegisterOrder();
        object CancelOrder(OrderObject order);
        object SubscribeOrderbook();
    }
}
