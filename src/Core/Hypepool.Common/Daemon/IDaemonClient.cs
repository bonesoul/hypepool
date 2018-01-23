using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hypepool.Common.Daemon
{
    public interface IDaemonClient
    {
        void Initialize(string host, int port, string username, string password, string rpcLocation = "");

        Task<DaemonResponse<TResponse>> ExecuteCommandAsync<TResponse>(string method,
            object payload = null) where TResponse : class;
    }
}
