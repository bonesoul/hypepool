#region license
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

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hypepool.Common.Daemon
{
    /// <summary>
    /// Daemon client interface.
    /// </summary>
    public interface IDaemonClient
    {
        void Initialize(string host, int port, string username, string password, string rpcLocation = "");

        /// <summary>
        /// Executes the request against configured daemon and returns the response.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task<DaemonResponse<TResponse>> ExecuteCommandAsync<TResponse>(string method,
            object payload = null) where TResponse : class;

        /// <summary>
        /// Executes the request against configured daemon and returns the response.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<DaemonResponse<JToken>> ExecuteCommandAsync(string method);
    }
}
