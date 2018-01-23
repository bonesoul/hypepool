using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Hypepool.Common.Daemon
{
    public class DaemonException : Exception
    {
        public HttpStatusCode Code { get; set; }

        public DaemonException(string msg) 
            : base(msg)
        {
        }

        public DaemonException(HttpStatusCode code, string msg)
            : base(msg)
        {
            Code = code;
        }
    }
}
