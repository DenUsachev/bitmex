using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Connector.REST.Entities;
using Connector.REST.Interfaces;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using RestSharp;

namespace Connector.REST
{
    public class RestApiConnector : IRestConnector
    {
        private const string USER = "/user";
        private const string LOGOUT = "/user/logout";
        private const string ORDER = "/order";

        private readonly RestClient _client;
        private readonly Uri _endpointUri;

        public string ApiKey { get; private set; }
        public string ApiSecret { get; private set; }
        public long Expires { get; private set; }

        public RestApiConnector(string endpoint, string apiKey, string apiSecret, int expirationPeriod = 10)
        {
            _client = new RestClient(endpoint);
            _endpointUri = new Uri(endpoint);
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            Expires = DateTime.Now.ToUnixTime() + expirationPeriod;
        }

        public UserObject Connect()
        {
            try
            {
                var result = DoRequest<UserObject>(Method.GET, USER);
                UserObject userObject = result.Data;
                userObject.IsSuccess = result.IsSuccess;
                return userObject;
            }
            catch (Exception ex)
            {
                return new UserObject {Error = ex.Message, IsSuccess = false};
            }
        }

        public RestResponse Disconnect()
        {
            try
            {
                return DoRequest(Method.POST, LOGOUT);
            }
            catch (Exception ex)
            {
                return new RestResponse {Error = new RestError {Message = ex.Message}};
            }
        }

        public OrderObject RegisterOrder(OrderObject order)
        {
            try
            {
                var registeredOrderResult = DoRequest<OrderObject>(Method.POST, ORDER, order);
                var registeredOrder = (OrderObject) registeredOrderResult.Data;
                return registeredOrder;
            }
            catch (Exception ex)
            {
                return new OrderObject {Error = ex.Message, IsSuccess = false};
            }
        }

        public RestResponse CancelOrder(string orderId, string comment = null)
        {
            try
            {
                return DoRequest(Method.DELETE, ORDER);
            }
            catch (Exception ex)
            {
                return new RestResponse {Error = new RestError {Message = ex.Message}};
            }
        }

        public object SubscribeOrderbook()
        {
            throw new NotImplementedException();
        }

        private RestResponse DoRequest<T>(Method method, string resource, object requestData = null, bool json = false)
            where T : BaseRestObject
        {
            var req = new RestRequest {Method = method, Resource = resource};
            if (requestData != null &&
                (method == Method.POST
                 || method == Method.PUT
                 || method == Method.PATCH
                 || method == Method.DELETE))
            {
                if (json)
                {
                    req.AddJsonBody(requestData);
                }
                else
                {
                    req.AddHeader("ContentType", "application/x-www-form-urlencoded");
                    var dataDicrionary = requestData.ToStringDicrionary();
                    foreach (var param in dataDicrionary)
                    {
                        req.AddParameter(param.Key, param.Value);
                    }
                }
            }
            
            req.AddHeader("api-expires", Expires.ToString());
            req.AddHeader("api-key", ApiKey);
            req.AddHeader("api-signature", CalculateSignature(method, resource, requestData));
            var httpResponse = _client.Execute(req);

            RestResponse res;
            if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                res = JsonConvert.DeserializeObject<RestResponse>(httpResponse.Content);
                res.IsSuccess = false;
            }
            else
            {
                res = new RestResponse
                {
                    Data = JsonConvert.DeserializeObject<T>(httpResponse.Content),
                    IsSuccess = true
                };
            }

            res.StatusCode = httpResponse.StatusCode;
            return res;
        }

        private RestResponse DoRequest(Method method, string resource, object requestData = null, bool json = false)
        {
            var req = new RestRequest {Method = method, Resource = resource};
            if (requestData != null &&
                (method == Method.POST
                 || method == Method.PUT
                 || method == Method.PATCH
                 || method == Method.DELETE))
            {
                if (json)
                {
                    req.AddJsonBody(requestData);
                }
                else
                {
                    req.AddHeader("ContentType", "application/x-www-form-urlencoded");
                    var dataDicrionary = requestData.ToStringDicrionary();
                    foreach (var param in dataDicrionary)
                    {
                        req.AddQueryParameter(param.Key, param.Value);
                    }
                }
            }

            req.AddHeader("api-expires", Expires.ToString());
            req.AddHeader("api-key", ApiKey);
            req.AddHeader("api-signature", CalculateSignature(method, resource, requestData));
            var httpResponse = _client.Execute(req);

            RestResponse res;
            if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                res = JsonConvert.DeserializeObject<RestResponse>(httpResponse.Content);
                res.IsSuccess = false;
            }
            else
            {
                res = new RestResponse
                {
                    Data = JsonConvert.DeserializeObject(httpResponse.Content),
                    IsSuccess = true
                };
            }

            res.StatusCode = httpResponse.StatusCode;
            return res;
        }

        private string CalculateSignature(Method method, string resource, object data = null)
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

            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(ApiSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(sb.ToString()));
                var sign = BitConverter.ToString(hash).Replace("-", "");
                return sign;
            }
        }
    }
}