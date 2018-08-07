using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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

        public OrderObject RegisterOrder(OrderObject order)
        {
            try
            {
                var registeredOrderResult = DoRequest<OrderObject>(RestMethod.POST, ORDER, order, true);
                var registeredOrder = (OrderObject)registeredOrderResult.Data;
                if (registeredOrder == null)
                {
                    throw new Exception(registeredOrderResult.Error.Message);
                }
                return registeredOrder;
            }
            catch (Exception ex)
            {
                return new OrderObject { Error = ex.Message, IsSuccess = false };
            }
        }

        public RestResponse CancelOrder(string orderId, string comment = null)
        {
            try
            {
                return DoRequest(RestMethod.DELETE, ORDER);
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

        private RestResponse DoRequest<T>(RestMethod method, string resource, object requestData = null, bool json = false)
            where T : BaseRestObject
        {
            var request = new RestRequest
            {
                IsJson = json,
                Method = Enum.GetName(typeof(RestMethod), method),
                Url = $"{_endpointUri}{resource}",
            };
            request.Headers = new Dictionary<string, string>
            {
                {"api-expires", Expires.ToString() },
                {"api-key", ApiKey },
                {"api-signature", CalculateSignature(request.Method, resource, requestData) }
            };

            RestResponse response;
            if (method != RestMethod.GET)
            {
                var queryDataString = request.IsJson ? JsonConvert.SerializeObject(requestData).ToString() : BuildQueryData(requestData.ToStringDicrionary());
                var queryDataBytes = Encoding.UTF8.GetBytes(queryDataString);
                request.Data = queryDataBytes;
            }
            response = HttpHelper.RawHttpRestQuery<T>(request);
            return response;
        }

        private RestResponse DoRequest(RestMethod method, string resource, object requestData = null, bool json = false)
        {
            var request = new RestRequest
            {
                IsJson = json,
                Method = Enum.GetName(typeof(RestMethod), method),
                Url = $"{_endpointUri}{resource}",
            };
            request.Headers = new Dictionary<string, string>
            {
                {"api-expires", Expires.ToString() },
                {"api-key", ApiKey },
                {"api-signature", CalculateSignature(request.Method, resource, requestData) }
            };

            RestResponse response;
            if (method != RestMethod.GET)
            {
                var queryDataString = request.IsJson ? JsonConvert.SerializeObject(requestData).ToString() : BuildQueryData(requestData.ToStringDicrionary());
                var queryDataBytes = Encoding.UTF8.GetBytes(queryDataString);
                request.Data = queryDataBytes;
            }
            response = HttpHelper.RawHttpRestQuery(request);
            return response;
        }
        private string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        private string CalculateSignature(string method, string resource, object data = null)
        {
            var sb = new StringBuilder();
            sb
                .Append(method)
                .Append(_endpointUri.LocalPath)
                .Append(resource)
                .Append(Expires);

            if (data != null)
            {
                var requestString = JsonConvert.SerializeObject(data);
                sb.Append(requestString);
            }
            var tmp = sb.ToString();

            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(ApiSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(tmp));
                var sign = BitConverter.ToString(hash).Replace("-", "");
                return sign;
            }
        }
    }
}