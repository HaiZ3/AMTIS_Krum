using BankingCompetition.Models;
using Project.Constants;
using Project.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            var request = new HttpRequestMessage(HttpMethod.Get,"report-configuration");
            request.Headers.Add("Session-Id", sessionId);
            request.Headers.Add("Competitor-Id", Values.competitorId);

            HttpResponseMessage response = await Values.client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ReportConfiguration>>(json);
        }

        public List<Report> GenerateReports(ReportConfiguration config)
        {
            return null;
            //var reports = new List<Report>();

            //foreach (var config in configs)
            //{
            //    var transactionsInPeriod = _transactions.FindAll(tx =>
            //        tx.timestamp >= config.FromTimestamp &&
            //        tx.timestamp <= config.ToTimestamp);

            //    if (config.ClientIds != null && config.ClientIds.Count > 0)
            //    {
            //        transactionsInPeriod = transactionsInPeriod.FindAll(tx =>config.ClientIds.Contains(tx.client_id));
            //    }

            //    var groupedByClient = new Dictionary<string, List<Transaction>>();
            //    foreach (var tx in transactionsInPeriod)
            //    {
            //        if (!groupedByClient.ContainsKey(tx.client_id))
            //        {
            //            groupedByClient[tx.client_id] = new List<Transaction>();
            //        }

            //        groupedByClient[tx.client_id].Add(tx);
            //    }

            //    int totalApprovedCount = 0;
            //    decimal totalApprovedAmount = 0m;
            //    int totalDeclinedCount = 0;
            //    decimal totalDeclinedAmount = 0m;
            //    decimal totalEarningsAmount = 0m;

            //    var clientReports = new List<ClientReport>();

            //    foreach (var clientGroup in groupedByClient)
            //    {
            //        var clientId = clientGroup.Key;
            //        var clientTransactions = clientGroup.Value;

            //        int approvedCount = 0;
            //        decimal approvedAmount = 0m;
            //        int declinedCount = 0;
            //        decimal declinedAmount = 0m;
            //        decimal earningsAmount = 0m;

            //        foreach (var tx in clientTransactions)
            //        {
            //            string status = "approved";

            //            if (status == "approved")
            //            {
            //                approvedCount++;
            //                approvedAmount += tx.amount;
            //                earningsAmount += tx.amount * 0.003m; 
            //            }
            //            else if (status == "declined")
            //            {
            //                declinedCount++;
            //                declinedAmount += tx.amount;
            //            }
            //        }

            //        totalApprovedCount += approvedCount;
            //        totalApprovedAmount += approvedAmount;
            //        totalDeclinedCount += declinedCount;
            //        totalDeclinedAmount += declinedAmount;
            //        totalEarningsAmount += earningsAmount;

            //        clientReports.Add(new ClientReport
            //        {
            //            clientId = clientId,
            //            totalApprovedCount = approvedCount,
            //            totalApprovedAmount = approvedAmount,
            //            totalDecliendCount = declinedCount,
            //            totalDeclinedAmount = declinedAmount,
            //            totalEarningsAmount = earningsAmount
            //        });
            //    }

            //    reports.Add(new Report
            //    {
            //        id = config.Id,
            //        fromTime = config.FromTimestamp,
            //        toTime = config.ToTimestamp,
            //        totalApprovedCount = totalApprovedCount,
            //        totalApprovedAmount = totalApprovedAmount,
            //        totalDeclinedCount = totalDeclinedCount,
            //        totalDeclinedAmount = totalDeclinedAmount,
            //        totalEarningsAmount = totalEarningsAmount,
            //        clients = clientReports
            //    });
            //}

            //return reports;
        }


        public async Task<bool> SendReportsAsync(string sessionId, string competitorId, List<Report> reports)
        {
            var json = JsonSerializer.Serialize(reports);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, "reports");
            request.Headers.Add("Session-Id", sessionId);
            request.Headers.Add("Competitor-Id", competitorId);
            request.Content = content;

            var response = await Values.client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
