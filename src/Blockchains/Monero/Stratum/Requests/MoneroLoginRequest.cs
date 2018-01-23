using Newtonsoft.Json;

namespace Hypepool.Monero.Stratum.Requests
{
    public class MoneroLoginRequest
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("pass")]
        public string Password { get; set; }

        [JsonProperty("agent")]
        public string UserAgent { get; set; }
    }
}
