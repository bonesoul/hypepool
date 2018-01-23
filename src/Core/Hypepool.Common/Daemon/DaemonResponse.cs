using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.JsonRpc;

namespace Hypepool.Common.Daemon
{
    public class DaemonResponse<T>
    {
        public JsonRpcException Error { get; set; }

        public T Response { get; set; }
    }
}
