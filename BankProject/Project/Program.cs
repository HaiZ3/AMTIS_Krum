using BankingCompetition.Models;
using BankingCompetition.Services;
using BankingCompetition.Services;
using BankingCompetition.Utils;
using Project.Constants;
using Project.Models;
using Project.Models.SessionConstraints;
using Project.Services.PostRequests;
using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{

    static async Task Main()
    {
        //Initialize the session and get the session limits.
        SessionService sessionService = new SessionService();
        SessionInfo? sessionInfo = await sessionService.GetSessionLimits();

        //Initialize the transaction service and get the transactions to audit.
        TransactionService transactionService = new TransactionService(sessionInfo);
        TransactionBatch transactionsToAudit = await transactionService.GetTransactionsForAudit(sessionInfo);

        var transactions = await transactionService.ProcessTransactions(transactionsToAudit.transactions);

    }
}
