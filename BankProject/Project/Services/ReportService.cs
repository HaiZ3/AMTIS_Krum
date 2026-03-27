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
                report.totalApprovedCount = validTransactions
                    .Where(x => x.status == "approved")
                    .Count();

                decimal totalApprovedAmount = validTransactions
                    .Where(x => x.status == "approved")
                    .Sum(x => x.amount);

                report.totalDeclinedCount = validTransactions
                    .Where(x => x.status == "declined")
                    .Count();

                decimal totalDeclinedAmount = validTransactions
                    .Where(x => x.status == "declined")
                    .Sum(x => x.amount);
                report.totalDeclinedAmount = totalDeclinedAmount;

                report.totalEarningsAmount = ((totalApprovedAmount - totalDeclinedAmount) * (bankFee / 100m));
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
                        client.totalApprovedCount = transactionsForTheClient
                            .Where(x => x.status == "approved")
                            .Count();

                        decimal clientApprovedAmount = transactionsForTheClient
                            .Where(x => x.status == "approved")
                            .Sum(x => x.amount);
                        client.totalApprovedAmount = clientApprovedAmount;

                        client.totalDeclinedCount = transactionsForTheClient
                            .Where(x => x.status == "declined")
                            .Count();

                        decimal clientDeclinedAmount = transactionsForTheClient
                            .Where(x => x.status == "declined")
                            .Sum(x => x.amount);
                        client.totalDeclinedAmount = clientDeclinedAmount;

                        client.totalEarningsAmount = ((clientApprovedAmount - clientDeclinedAmount) * (bankFee / 100.0m));
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

            var response = await client.SendAsync(request);
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
