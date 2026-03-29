using BankingCompetition.Models;
using Project.Constants;
using Project.Models;
using System.Text;
using System.Text.Json;

namespace BankingCompetition.Services
{
    public class ReportService
    {
        private readonly List<Transaction> _transactions;

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

        public List<Report> GenerateReports(List<ReportConfiguration> configs, decimal bankFee)
        {
            List<Report> reports = new List<Report>();
            foreach (var config in configs)
            {
                Transaction[] validTransactions = _transactions
                    .Where(x => x.timestamp >= config.FromTimestamp
                    && x.timestamp < config.ToTimestamp)
                    .ToArray();
                Report report = new Report();
                report.id = config.Id;
                report.fromTime = config.FromTimestamp;
                report.toTime = config.ToTimestamp;

                Transaction[] approvedTransactions = validTransactions
                    .Where(x => x.status == "approved")
                    .ToArray();

                report.totalApprovedCount = approvedTransactions.Length;

                decimal totalApprovedAmount = approvedTransactions
                    .Sum(x => x.amount);

                report.totalApprovedAmount = Math.Round(totalApprovedAmount, 2);

                Transaction[] declinedTransactions = validTransactions
                    .Where(x => x.status == "declined")
                    .ToArray();

                report.totalDeclinedCount = declinedTransactions.Length;

                decimal totalDeclinedAmount = declinedTransactions
                    .Sum(x => Math.Abs(x.amount));

                report.totalDeclinedAmount = Math.Round(totalDeclinedAmount, 2);

                report.totalEarningsAmount = Math.Round(totalApprovedAmount * (bankFee / 100m), 2);

                if (config.ClientIds is not null)
                {
                    Client[] clients = new Client[config.ClientIds.Length];
                    int i = 0;
                    foreach (var currentClient in config.ClientIds)
                    {
                        Client client = new();
                        Transaction[] transactionsForTheClient = validTransactions
                            .Where(x => x.client_id == currentClient)
                            .ToArray();
                        client.clientId = currentClient;

                        Transaction[] approvedClientTransactions = transactionsForTheClient
                            .Where(x => x.status == "approved")
                            .ToArray();

                        client.totalApprovedCount = approvedClientTransactions.Length;

                        decimal clientApprovedAmount = approvedClientTransactions
                            .Sum(x => x.amount);

                        client.totalApprovedAmount = Math.Round(clientApprovedAmount, 2);

                        Transaction[] clientDeclinedTransactions = transactionsForTheClient
                            .Where(x => x.status == "declined")
                            .ToArray();

                        client.totalDeclinedCount = clientDeclinedTransactions.Length;

                        decimal clientDeclinedAmount = clientDeclinedTransactions
                            .Sum(x => Math.Abs(x.amount));

                        client.totalDeclinedAmount = Math.Round(clientDeclinedAmount, 2);

                        client.totalEarningsAmount = Math.Round(clientApprovedAmount * (bankFee / 100.0m), 2);

                        clients[i] = client;
                        i++;
                    }
                    report.clients = clients;
                }
                else
                {
                    report.clients = Array.Empty<Client>();
                }
                reports.Add(report);
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

            var response = await Values.client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            return response.IsSuccessStatusCode;
        }
    }
}
