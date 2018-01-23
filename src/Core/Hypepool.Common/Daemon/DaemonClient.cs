using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hypepool.Common.JsonRpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hypepool.Common.Daemon
{
    public class DaemonClient : IDaemonClient
    {
        private string _rpcUrl;
        private Int32 _requestCounter = 0;
        private HttpClient _httpClient;
        private AuthenticationHeaderValue _authenticationHeader;

        private readonly JsonSerializer _serializer;
        private readonly JsonSerializerSettings _serializerSettings;

        public DaemonClient()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            _serializer = new JsonSerializer
            {
                ContractResolver = _serializerSettings.ContractResolver
            };
        }

        public void Initialize(string host, int port, string username, string password, string rpcLocation = "")
        {
            // build rpc url.
            _rpcUrl = $"http://{host}:{port}";
            if (!string.IsNullOrEmpty(rpcLocation))
                _rpcUrl += $"/{rpcLocation}";

            // build authentication header if needed
            if (!string.IsNullOrEmpty(username))
            {
                var auth = $"{username}:{password}";
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));
                _authenticationHeader = new AuthenticationHeaderValue("Basic", base64);
            }

            // create httpclient instance with credentals.
                _httpClient = new HttpClient(new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
                PreAuthenticate = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            });
        }

        public async Task<DaemonResponse<TResponse>> ExecuteCommandAsync<TResponse>(string method,
            object payload = null) where TResponse : class
        {
            var task = MakeRequestAsync(method, payload);

            try
            {
                await task;
            }

            catch (Exception e)
            {
                // ignored
            }

            var result = MapDaemonResponse<TResponse>(task);
            return result;
        }

        private DaemonResponse<TResponse> MapDaemonResponse<TResponse>(Task<JsonRpcResponse> task)
            where TResponse : class
        {
            var response = new DaemonResponse<TResponse>();

            // check tasks faults.
            if (task.IsFaulted)
                response.Error = new JsonRpcException(-999, task.Exception.Message,
                    task.Exception.InnerExceptions.Count > 0 ? task.Exception.InnerException : task.Exception);
            else if (task.IsCanceled)
                response.Error = new JsonRpcException(-998, "Cancelled", null);
            else
            {
                Debug.Assert(task.IsCompletedSuccessfully);

                if (task.Result?.Result is JToken token)
                    response.Response = token?.ToObject<TResponse>(_serializer);
                else
                    response.Response = (TResponse)task.Result?.Result;

                response.Error = task.Result?.Error; // set error if any.
            }

            return response;
        }


        private async Task<JsonRpcResponse> MakeRequestAsync(string method, object payload)
        {
            var rpcRequestId = _requestCounter++;

            // build rpc request
            var rpcRequest = new JsonRpcRequest<object>(method, payload, rpcRequestId);

            // cook the http request
            var request = new HttpRequestMessage(HttpMethod.Post, _rpcUrl);
            var json = JsonConvert.SerializeObject(rpcRequest, _serializerSettings);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Authorization = _authenticationHeader;

            // send the request.

            using (var response = await _httpClient.SendAsync(request))
            {
                // check if succeded
                if (!response.IsSuccessStatusCode)
                    throw new DaemonException(response.StatusCode, response.ReasonPhrase);

                // deserialize the response
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        using (var jreader = new JsonTextReader(reader))
                        {
                            var result = _serializer.Deserialize<JsonRpcResponse>(jreader);
                            return result;
                        }
                    }
                }
            }
        }
    }
}
