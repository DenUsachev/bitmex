using System.Net;
using Connector.REST;
using NUnit.Framework;

namespace Connector.Tests
{
    [TestFixture]
    public class ConnectionTest
    {
        private const string BitmexUrl = "https://testnet.bitmex.com/api/v1";
        private RestApiConnector _connector;
        
        [TestCase("iIFvva629YnJ1LmjEflRfhWA","QKWRrSRXSBF2sLneIHG118MiBZxAmP5m3L0tel_zYFdPIOvf")]
        public void ConnectTest(string key, string secret)
        {
            _connector = new RestApiConnector(BitmexUrl, key, secret);
            var user = _connector.Connect();
            Assert.IsTrue(user.IsSuccess, "Connection failed", user);
            Assert.IsNotNull(user.Id, "User id was not fetched");
        }
        
        
        [Test]
        public void DisconnectTest()
        {
            var response =_connector.Disconnect();
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}