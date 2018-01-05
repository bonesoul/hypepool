﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hypepool.Core.JsonRpc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcResponse : JsonRpcResponse<object>
    {
        public JsonRpcResponse()
        {
        }

        public JsonRpcResponse(object result, object id = null) : base(result, id)
        {
        }

        public JsonRpcResponse(JsonRpcException ex, object id = null, object result = null) : base(ex, id, result)
        {
        }
    }

    /// <summary>
    /// Represents a Json Rpc Response
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcResponse<T>
    {
        public JsonRpcResponse()
        {
        }

        public JsonRpcResponse(T result, object id = null)
        {
            Result = result;
            Id = id;
        }

        public JsonRpcResponse(JsonRpcException ex, object id, object result)
        {
            Error = ex;
            Id = id;

            if (result != null)
                Result = JToken.FromObject(result);
        }

        //[JsonProperty(PropertyName = "jsonrpc")]
        //public string JsonRpc => "2.0";

        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; set; }

        [JsonProperty(PropertyName = "error")]
        public JsonRpcException Error { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public object Id { get; set; }
    }
}
