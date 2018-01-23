using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hypepool.Common.JsonRpc
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcRequest<T>
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc => "2.0";

        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty("params")]
        public object Params { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public object Id { get; set; }

        public JsonRpcRequest(string method, T parameters, object id)
        {
            Method = method;
            Params = parameters;
            Id = id;
        }

        public TParam ParamsAs<TParam>() where TParam : class
        {
            if (Params is JToken)
                return ((JToken)Params)?.ToObject<TParam>();

            return (TParam)Params;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcRequest : JsonRpcRequest<object>
    {
        public JsonRpcRequest(string method, object parameters, object id) 
            : base(method, parameters, id)
        {
        }
    }
}
