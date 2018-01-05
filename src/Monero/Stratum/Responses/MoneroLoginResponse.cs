using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Hypepool.Monero.Stratum.Responses
{
    public class MoneroJobParams
    {
        [JsonProperty("job_id")]
        public string JobId { get; set; }

        public string Blob { get; set; }

        public string Target { get; set; }
    }

    public class MoneroLoginResponse : MoneroResponseBase
    {
        public string Id { get; set; }
        public MoneroJobParams Job { get; set; }
    }
}
