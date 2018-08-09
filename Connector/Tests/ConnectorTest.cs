using Connector.REST;
using Connector.REST.Entities;
using NUnit.Framework;

namespace Connector.Tests
{
    [TestFixture]
    public class ConnectorTest
    {
        private const string BITMEX_URL = "https://testnet.bitmex.com/api/v1";
        private RestApiConnector _connector;
        private OrderItem _order;

        public ConnectorTest()
        {
            _order = new OrderItem
            {
                side = "Buy",
                symbol = Symbols.Eth,
                orderQty = 1,
                price = 402.2M,
                ordType = "Limit"
            };
        }

        [Order(1), TestCase("mcklN9Ir8LSimr0cj3vxYMvw", "CP9ieGvkSsk0T55dMnZeG55XEpoYNMFtNNhyl6y7Nih9oFhl")]
        public void Connect(string key, string secret)
        {
            _connector = new RestApiConnector(BITMEX_URL, key, secret);
            var user = _connector.Connect();
            Assert.IsTrue(user.IsSuccess, "Connection failed");
            Assert.IsNotNull(user.Id, "User id was not fetched");
        }

        [Order(2), TestCase("mcklN9Ir8LSimr0cj3vxYMvw", "CP9ieGvkSsk0T55dMnZeG55XEpoYNMFtNNhyl6y7Nih9oFhl")]
        public void PlaceOrder(string key, string secret)
        {
            _connector = new RestApiConnector(BITMEX_URL, key, secret);
            var result = _connector.RegisterOrder(_order);
            Assert.True(result.IsSuccess, "Order allocation failed");
            Assert.AreEqual(_order.price, result.price, "Order allocated with bad price");
            Assert.AreEqual(_order.side, result.side, "Order allocated with bad side");
            Assert.AreEqual(_order.orderQty, result.orderQty, "Order allocated with bad qty");
            Assert.AreEqual(_order.symbol, result.symbol, "Order allocated with bad instrument");
            Assert.AreEqual(_order.ordType, result.ordType, "Order allocated with bad order type");
            _order = result;
        }

        [Order(3), TestCase("Z3So8fRWLaejVhuqKJikSj8B", "malgMjihESAAJrVtzs96ydD1f_m4zoS6YK_vwFFRLitQZzGa")]
        public void CancelOrder(string key, string secret)
        {
            _connector = new RestApiConnector(BITMEX_URL, key, secret);
            _connector.Connect();
            var result = _connector.CancelOrder(_order.orderID);
            Assert.True(result.IsSuccess, "Order cancellation failed");
        }

        [Order(4), TestCase("iIFvva629YnJ1LmjEflRfhWA", "QKWRrSRXSBF2sLneIHG118MiBZxAmP5m3L0tel_zYFdPIOvf")]
        public void Disconnect(string key, string secret)
        {
            _connector = new RestApiConnector(BITMEX_URL, key, secret);
            _connector.Connect();
            var response = _connector.Disconnect();
            Assert.IsTrue(response.IsSuccess, "Disconnect failed");
        }
    }
}