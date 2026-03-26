using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Report
    {
        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("from_timestamp")]
        public DateTime fromTime { get; set; }

        [JsonPropertyName("to_timestamp")]
        public DateTime toTime { get; set; }

        [JsonPropertyName("total_approved_count")]
        public int totalApprovedCount { get; set; }

        [JsonPropertyName("total_approved_amount")]
        public decimal totalApprovedAmount { get; set; }

        [JsonPropertyName("total_declined_count")]
        public int totalDeclinedCount { get; set; }

        [JsonPropertyName("total_declined_amount")]
        public decimal totalDeclinedAmount { get; set; }

        [JsonPropertyName("total_earnings_amount")]
        public decimal totalEarningsAmount { get; set; }

        [JsonPropertyName("clients")]
        public List<Client> clients { get; set; }
    }
}
