﻿#region license
// 
//      hypepool
//      https://github.com/bonesoul/hypepool
// 
//      Copyright (c) 2013 - 2018 Hüseyin Uslu
// 
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
// 
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
#endregion
using System;
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
using Serilog;

namespace Hypepool.Common.Daemon
{
    /// <summary>
    /// Daemon client.
    /// </summary>
    public class DaemonClient : IDaemonClient
    {
        private readonly ILogger _logger;

        private readonly string _rpcUrl;
        private HttpClient _httpClient;
        private AuthenticationHeaderValue _authenticationHeader;
        private string _username;
        private string _pasword;
        private int _requestCounter = 0;

        private readonly JsonSerializer _serializer;
        private readonly JsonSerializerSettings _serializerSettings;

        public DaemonClient(string host, int port, string username, string password, string rpcLocation = "")
        {
            _logger = Log.ForContext<DaemonClient>();

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            _serializer = new JsonSerializer
            {
                ContractResolver = _serializerSettings.ContractResolver
            };

            // build rpc url.
            _rpcUrl = $"http://{host}:{port}";
            if (!string.IsNullOrEmpty(rpcLocation))
                _rpcUrl += $"/{rpcLocation}";

            _username = username;
            _pasword = password;
        }

        public void Initialize()
        {
            // build authentication header if needed
            if (!string.IsNullOrEmpty(_username))
            {
                var auth = $"{_username}:{_pasword}";
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));
                _authenticationHeader = new AuthenticationHeaderValue("Basic", base64);
            }

            // create httpclient instance with credentals.
            _httpClient = new HttpClient(new HttpClientHandler
            {
                Credentials = new NetworkCredential(_username, _pasword),
                PreAuthenticate = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            });
        }

        /// <summary>
        /// Executes the request against configured daemon and returns the response.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<DaemonResponse<TResponse>> ExecuteCommandAsync<TResponse>(string method, object payload = null) where TResponse : class
        {
            var task = MakeRequestAsync(method, payload);

            try
            {
                await task;
            }
            catch (Exception e)
            {
                // TODO: ignored?
            }

            var result = MapDaemonResponse<TResponse>(task);
            return result;
        }

        /// <summary>
        /// Executes the request against configured daemon and returns the response.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public Task<DaemonResponse<JToken>> ExecuteCommandAsync(string method)
        {
            return ExecuteCommandAsync<JToken>(method);
        }

        /// <summary>
        /// Maps the response for the request.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        private DaemonResponse<TResponse> MapDaemonResponse<TResponse>(Task<JsonRpcResponse> task)
            where TResponse : class
        {
            var response = new DaemonResponse<TResponse>();

            // check tasks faults.
            if (task.IsFaulted)
                response.Error = new JsonRpcException(-999, task.Exception.Message, null, task.Exception.InnerExceptions.Count > 0 ? task.Exception.InnerException : task.Exception);
            else if (task.IsCanceled)
                response.Error = new JsonRpcException(-998, "Cancelled", null);
            else
            {
                Debug.Assert(task.IsCompletedSuccessfully);

                //_logger.Verbose($"<< [{task.Result?.Id:x8}] {task.Result?.Result}");

                if (task.Result?.Result is JToken token)
                    response.Response = token?.ToObject<TResponse>(_serializer);
                else
                    response.Response = (TResponse)task.Result?.Result;

                response.Error = task.Result?.Error; // set error if any.
            }

            return response;
        }

        /// <summary>
        /// Cooks the actual request.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
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

            //_logger.Verbose($">> [{rpcRequest.Id:x8}] {json}");

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
