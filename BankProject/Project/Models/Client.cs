using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Client
    {
        [JsonPropertyName("client_id")]
        public string clientId { get; set; }

        [JsonPropertyName("total_approved_count")]
        public int totalApprovedCount { get; set; }

        [JsonPropertyName("total_approved_amount")]
        public string totalApprovedAmount { get; set; }

        [JsonPropertyName("total_declined_count")]
        public int totalDeclinedCount { get; set; } 

        [JsonPropertyName("total_declined_amount")]
        public string totalDeclinedAmount { get; set; }

        [JsonPropertyName("total_earnings_amount")]
        public string totalEarningsAmount { get; set; }
    }
}