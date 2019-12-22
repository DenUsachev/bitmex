using MarketConnectivity.REST.Entities;

namespace MarketConnectivity.REST.Interfaces
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
