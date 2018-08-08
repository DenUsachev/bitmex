using System;
using Connector.REST.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Connector.REST
{
    public static class HttpHelper
    {
        public static string CalculateSignature(Uri endpointUri, long expires, string secret, string method, string resource, object data = null)
        {
            var sb = new StringBuilder();
            sb
                .Append(method)
                .Append(endpointUri.LocalPath)
                .Append(resource)
                .Append(expires);

            if (data != null)
            {
                var requestString = JsonConvert.SerializeObject(data);
                sb.Append(requestString);
            }
            var tmp = sb.ToString();
            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(tmp));
                var sign = BitConverter.ToString(hash).Replace("-", "");
                return sign;
            }
        }

        public static RestResponse RawHttpRestQuery<T>(RestRequest request)
            where T : BaseRestObject
        {
            var result = new RestResponse();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(request.Url);
            webRequest.Method = request.Method;
            foreach (var hdr in request.Headers)
            {
                webRequest.Headers.Add(hdr.Key, hdr.Value);
            }
            try
            {
                if (request.Method == "POST" || request.Method == "PUT")
                {
                    webRequest.ContentType = request.IsJson ? "application/json" : "application/x-www-form-urlencoded";
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(request.Data, 0, request.Data.Length);
                    }
                }
                using (WebResponse webResponse = webRequest.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    var stringData = sr.ReadToEnd();
                    result.IsSuccess = true;
                    result.Data = JsonConvert.DeserializeObject<T>(stringData);
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        throw;

                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            var stringData = sr.ReadToEnd();
                            result = JsonConvert.DeserializeObject<RestResponse>(stringData);
                            result.IsSuccess = false;
                        }
                    }
                }
            }
            return result;
        }

        public static RestResponse RawHttpRestQuery(RestRequest request)
        {
            var result = new RestResponse();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(request.Url);
            webRequest.Method = request.Method;
            foreach (var hdr in request.Headers)
            {
                webRequest.Headers.Add(hdr.Key, hdr.Value.ToString());
            }
            try
            {
                webRequest.ContentType = request.IsJson ? "application/json" : "application/x-www-form-urlencoded";
                if (request.Data != null && request.Data.Length > 0)
                {
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(request.Data, 0, request.Data.Length);
                    }
                }
                using (WebResponse webResponse = webRequest.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    var stringData = sr.ReadToEnd();
                    result.Data = JsonConvert.DeserializeObject(stringData);
                    result.IsSuccess = true;
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        throw;

                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            var stringData = sr.ReadToEnd();
                            result = JsonConvert.DeserializeObject<RestResponse>(stringData);
                            result.IsSuccess = false;
                        }
                    }
                }
            }
            return result;
        }
    }
}
