using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Connector.Extensions;
using Connector.REST.Entities;
using Connector.REST.Interfaces;
using Newtonsoft.Json;

namespace Connector.REST
{
    public class RestApiConnector : IRestConnector
    {
        private const string USER = "/user";
        private const string LOGOUT = "/user/logout";
        private const string ORDER = "/order";

        private readonly Uri _endpointUri;

        public string ApiKey { get; private set; }
        public string ApiSecret { get; private set; }
        public long Expires { get; private set; }

        public RestApiConnector(string endpoint, string apiKey, string apiSecret, int expirationPeriod = 10)
        {
            _endpointUri = new Uri(endpoint);
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            Expires = DateTime.Now.ToUnixTime() + expirationPeriod;
        }

        public UserObject Connect()
        {
            try
            {
                var result = DoRequest<UserObject>(RestMethod.GET, USER);
                if (!result.IsSuccess)
                {
                    throw new Exception(result.Error.Message);
                }
                UserObject userObject = result.Data;
                userObject.IsSuccess = result.IsSuccess;
                return userObject;
            }
            catch (Exception ex)
            {
                return new UserObject { Error = ex.Message, IsSuccess = false };
            }
        }

        public RestResponse Disconnect()
        {
            try
            {
                return DoRequest(RestMethod.POST, LOGOUT);
            }
            catch (Exception ex)
            {
                return new RestResponse { Error = new RestError { Message = ex.Message } };
            }
        }

        public OrderItem RegisterOrder(OrderItem order)
        {
            try
            {
                var registeredOrderResult = DoRequest<OrderItem>(RestMethod.POST, ORDER, order);
                var registeredOrder = (OrderItem)registeredOrderResult.Data;
                if (registeredOrder == null)
                {
                    throw new Exception(registeredOrderResult.Error.Message);
                }
                registeredOrder.IsSuccess = true;
                return registeredOrder;
            }
            catch (Exception ex)
            {
                return new OrderItem { Error = ex.Message, IsSuccess = false };
            }
        }

        public RestResponse CancelOrder(string orderId, string comment = null)
        {
            try
            {
                return DoRequest(RestMethod.DELETE, ORDER, new { orderID = orderId });
            }
            catch (Exception ex)
            {
                return new RestResponse { Error = new RestError { Message = ex.Message } };
            }
        }

        public object SubscribeOrderbook()
        {
            throw new NotImplementedException();
        }

        private RestResponse DoRequest<T>(RestMethod method, string resource, object requestData = null, bool json = true)
            where T : BaseRestObject
        {
            var request = new RestRequest
            {
                IsJson = json,
                Method = Enum.GetName(typeof(RestMethod), method),
                Url = string.Format("{0},{1}", _endpointUri, resource)
            };
            request.Headers = new Dictionary<string, string>
            {
                {"api-expires", Expires.ToString() },
                {"api-key", ApiKey },
                {"api-signature", HttpHelper.CalculateSignature(_endpointUri, Expires,ApiSecret, request.Method, resource, requestData) }
            };

            if (method != RestMethod.GET)
            {
                var queryDataString = request.IsJson ? JsonConvert.SerializeObject(requestData) : BuildQueryData(requestData.ToStringDicrionary());
                var queryDataBytes = Encoding.UTF8.GetBytes(queryDataString);
                request.Data = queryDataBytes;
            }
            return HttpHelper.RawHttpRestQuery<T>(request);
        }

        private RestResponse DoRequest(RestMethod method, string resource, object requestData = null, bool json = true)
        {
            var request = new RestRequest
            {
                IsJson = json,
                Method = Enum.GetName(typeof(RestMethod), method),
                Url = string.Format("{0},{1}", _endpointUri, resource)
            };
            request.Headers = new Dictionary<string, string>
            {
                {"api-expires", Expires.ToString() },
                {"api-key", ApiKey },
                {"api-signature", HttpHelper.CalculateSignature(_endpointUri, Expires, ApiSecret, request.Method, resource, requestData) }
            };
            if (method != RestMethod.GET)
            {
                if (requestData != null)
                {
                    var queryDataString = request.IsJson ? JsonConvert.SerializeObject(requestData) : BuildQueryData(requestData.ToStringDicrionary());
                    var queryDataBytes = Encoding.UTF8.GetBytes(queryDataString);
                    request.Data = queryDataBytes;
                }
            }
            return HttpHelper.RawHttpRestQuery(request);
        }

        private string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            var b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            try
            {
                return b.ToString().Substring(1);
            }
            catch (Exception)
            {
                return "";
            }
        }


    }
}