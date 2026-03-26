using BankingCompetition.Models;
using Project.Constants;
using Project.Models;
using Project.Models.SessionConstraints;
using System.Text;
using System.Text.Json;

namespace BankingCompetition.Services
{
    public class ReportService
    {
        private readonly List<Transaction> _transactions;
        private readonly HttpClient client = new HttpClient();

        public ReportService(List<Transaction> transactions)
        {
            _transactions = transactions;
        }


        public async Task<List<ReportConfiguration>> GetReportConfigurationAsync(string sessionId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "report-configuration");
            request.Headers.Add("Session-Id", sessionId);
            request.Headers.Add("Competitor-Id", Values.competitorId);

            HttpResponseMessage response = await Values.client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            List<ReportConfiguration> reportConfigurations = JsonSerializer.Deserialize<List<ReportConfiguration>>(json);
            return reportConfigurations;
        }

        public List<Report> GenerateReports(List<ReportConfiguration> configs, decimal bankFee,SessionInfo sessionInfo)
        {
            var reports = new List<Report>();
            decimal feeRate = (sessionInfo.spendingLimits.interchangeFeePercentage / 100.0m);

            foreach (var config in configs)
            {

                List<Transaction> periodTxs = _transactions.Where(tx =>
                    tx.timestamp >= config.FromTimestamp &&
                    tx.timestamp <= config.ToTimestamp &&
                    (config.ClientIds.Length == 0 || config.ClientIds.Contains(tx.client_id)) &&
                    tx.type == "authorization"
                ).ToList();

                List<Client> clientReports = new List<Client>();

                var grouped = periodTxs.GroupBy(x => x.client_id);

                foreach (var group in grouped)
                {
                    var approved = group.Where(x => x.status == "approved").ToList();
                    var declined = group.Where(x => x.status == "declined").ToList();

                    clientReports.Add(new Client
                    {
                        clientId = group.Key,
                        totalApprovedCount = approved.Count,
                        totalApprovedAmount = Math.Round(approved.Sum(x => x.amount),2),
                        totalDeclinedCount = declined.Count,
                        totalDeclinedAmount = Math.Round(declined.Sum(x => x.amount),2),
                        totalEarningsAmount = Math.Round(approved.Sum(x => x.amount) * feeRate, 2)
                    });
                }

                reports.Add(new Report
                {
                    id = config.Id,
                    fromTime = config.FromTimestamp,
                    toTime = config.ToTimestamp,
                    totalApprovedCount = clientReports.Sum(x => x.totalApprovedCount),
                    totalApprovedAmount = Math.Round(clientReports.Sum(x => x.totalApprovedAmount), 2),
                    totalDeclinedCount = clientReports.Sum(x => x.totalDeclinedCount),
                    totalDeclinedAmount = Math.Round(clientReports.Sum(x => x.totalDeclinedAmount), 2),
                    totalEarningsAmount = Math.Round(clientReports.Sum(x => x.totalEarningsAmount) * feeRate, 2),
                    clients = clientReports
                });
            }

            return reports;
        }


        public async Task<bool> SendReportsAsync(string sessionId, string competitorId, List<Report> reports)
        {
            var json = JsonSerializer.Serialize(reports);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, Values.baseUrl + "reports");
            request.Headers.Add("Session-Id", sessionId);
            request.Headers.Add("Competitor-Id", competitorId);
            request.Content = content;

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            return response.IsSuccessStatusCode;
        }
    }
}
