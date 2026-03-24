using BankingCompetition.Models;
using BankingCompetition.Utils;
using Project.Constants;
using Project.Models;
using Project.Models.SessionConstraints;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public class TransactionService
    {
        private readonly HttpClient _client;

        private Dictionary<string, List<Transaction>> _transactionsByCard = new();
        private Dictionary<string, List<Transaction>> _transactionsByClient = new();
        private List<TransactionResult> _allTransactionsResults = new();
        private SessionInfo SessionInfo { get; }
        public TransactionService(SessionInfo sessionInfo)
        {
            SessionInfo = sessionInfo;
        }

        public async Task<List<TransactionResult>> ProcessTransactions(Transaction[] transactions)
        {
            foreach (Transaction transaction in transactions)
            {
                TransactionResult result = new TransactionResult();
                result.transaction_id = transaction.transaction_id;
                if(transaction.amount < 0)
                {
                    result.status = "declined";
                    _allTransactionsResults.Add(result);
                    continue;
                }
                if (transaction.type == "authorization")
                {
                    if (_transactionsByCard.ContainsKey(transaction.card_id) == false)
                    {
                        _transactionsByCard.Add(transaction.card_id, new List<Transaction>() { });
                    }
                    if (_transactionsByClient.ContainsKey(transaction.client_id) == false)
                    {
                        _transactionsByClient.Add(transaction.client_id, new List<Transaction>() { });
                    }

                    //Get all the transactions by card for the day
                    List<Transaction> transactionsByCardForTheDay = _transactionsByCard[transaction.card_id]
                        .Where(x =>
                        x.timestamp > x.timestamp.Date
                        && x.timestamp < x.timestamp.Date.AddDays(1))
                        .ToList();

                    decimal sumForTheDayByCard = transactionsByCardForTheDay.Sum(x => x.amount);
                   
                    //Check for the type of card
                    if (transaction.card_type == "standard")
                    {
                        //Check if we are over the limit(Standard)
                        if (sumForTheDayByCard + transaction.amount > SessionInfo.spendingLimits.cardLimits.standard.dailyLimit)
                        {
                            result.status = "declined";
                            _allTransactionsResults.Add(result);
                            continue;
                        }
                    }
                    else
                    {
                        //Check if we are over the limit(Premium)
                        if (sumForTheDayByCard + transaction.amount > SessionInfo.spendingLimits.cardLimits.premium.dailyLimit)
                        {
                            result.status = "declined";
                            _allTransactionsResults.Add(result);
                            continue;
                        }
                    }

                    DateTime transactionDate = transaction.timestamp.Date;
                    //Get all the transactions by card for the week
                    List<Transaction> transactionsByCardForTheWeek = _transactionsByCard[transaction.card_id]
                        .Where(x => 
                        x.timestamp >= transactionDate.Date.AddDays(-(int)(transactionDate.DayOfWeek + 6) % 7))
                        .ToList();

                    decimal sumForTheWeekByCard = transactionsByCardForTheWeek.Sum(x => x.amount);

                    //Check for the type of card
                    if (transaction.card_type == "standard")
                    {
                        //Check if we are over the limit(Standard)
                        if (sumForTheWeekByCard + transaction.amount > SessionInfo.spendingLimits.cardLimits.standard.weeklyLimit)
                        {
                            result.status = "declined";
                            _allTransactionsResults.Add(result);
                            continue;
                        }
                    }
                    else
                    {
                        //Check if we are over the limit(Premium)
                        if(sumForTheWeekByCard + transaction.amount > SessionInfo.spendingLimits.cardLimits.premium.weeklyLimit)
                        {
                            result.status = "declined";
                            _allTransactionsResults.Add(result);
                            continue;
                        }
                    }

                    //Get all the transactions by client for the week
                    List<Transaction> transactionsByClientForTheDay = _transactionsByClient[transaction.client_id]
                        .Where(x =>
                        x.timestamp > x.timestamp.Date
                        && x.timestamp < x.timestamp.Date.AddDays(1))
                        .ToList();

                    decimal sumForTheDayByClient = transactionsByCardForTheDay.Sum(x => x.amount);

                    //Check if the client is over the daily limit
                    if(sumForTheDayByClient + transaction.amount > SessionInfo.spendingLimits.dailyClientLimit)
                    {
                        result.status = "declined";
                        _allTransactionsResults.Add(result);
                        continue;
                    }

                    List<Transaction> transactionsByClientForTheWeek = _transactionsByClient[transaction.client_id]
                        .Where(x =>
                        x.timestamp >= transactionDate.Date.AddDays(-(int)(transactionDate.DayOfWeek + 6) % 7))
                        .ToList();

                    decimal sumForTheWeekByClient = transactionsByClientForTheWeek.Sum(x => x.amount);

                    //Check if the client is over the weekly limit
                    if (sumForTheWeekByClient + transaction.amount > SessionInfo.spendingLimits.weeklyClientLimit)
                    {
                        result.status = "declined";
                        _allTransactionsResults.Add(result);
                        continue;
                    }

                     int numberOfTransactionsIn10Seconds = _transactionsByClient[transaction.client_id]
                        .Where(x => x.timestamp <= transaction.timestamp
                        && x.timestamp >= transaction.timestamp.AddSeconds(-10))
                        .Count();

                    //Check if there were more transactions than the allowed amount
                    if(numberOfTransactionsIn10Seconds == SessionInfo.spendingLimits.allowedTransactionsPer10s)
                    {
                        result.status = "declined";
                        _allTransactionsResults.Add(result);
                        continue;
                    }

                    _transactionsByCard[transaction.card_id].Add(transaction);
                    _transactionsByClient[transaction.client_id].Add(transaction);
                    result.status = "approved";
                    _allTransactionsResults.Add(result);
                }
            }
            return _allTransactionsResults;
        }
        public async Task<TransactionBatch?> GetTransactionsForAudit(SessionInfo sessionInfo)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Values.baseUrl + "transaction-batches");
            request.Headers.Add("Session-Id", sessionInfo.sessionId);
            request.Headers.Add("Competitor-Id", Values.competitorId);

            var response = await Values.client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            TransactionBatch transactionsToAudit = JsonSerializer.Deserialize<TransactionBatch>(responseContent);
            return transactionsToAudit;
        }

        public async Task<bool> SendBatchResultsAsync(string sessionId, string competitorId, string batchId, List<TransactionResult> results, string resultsHash)
        {
            var json = JsonSerializer.Serialize(results);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, $"transaction-batches/{batchId}");
            request.Headers.Add("Session-Id", sessionId);
            request.Headers.Add("Competitor-Id", competitorId);
            request.Headers.Add("Results-Hash", resultsHash);
            request.Content = content;

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
