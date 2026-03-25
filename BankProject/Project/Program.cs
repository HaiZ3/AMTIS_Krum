using BankingCompetition.Models;
using BankingCompetition.Services;
using BankingCompetition.Services;
using BankingCompetition.Utils;
using Project.Constants;
using Project.Models;
using Project.Models.SessionConstraints;
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
        TransactionService transactionService = new TransactionService(sessionInfo,Values.client);
        TransactionBatch transactionsToAudit = await transactionService.GetTransactionsForAudit(sessionInfo);
        
        //Process the transactions as to mark them as either approved or declined
        List<TransactionResult> transactionResults = transactionService.ProcessTransactions(transactionsToAudit.transactions);
        bool sendBatchSuccess = await transactionService.SendBatchResultsAsync(sessionInfo.sessionId, transactionsToAudit.transactionsBatchId, transactionResults);
        if(sendBatchSuccess)
        {
            Console.WriteLine("Batch send successfully");
        }
        else
        {
            Console.WriteLine("Proble with sending the batch");
        }
        ReportService reportService = new ReportService(Values.client, transactionService.TransactionsByClient);
        ReportConfiguration[] reportConfiguration = await reportService.GetReportConfigurationAsync(sessionInfo.sessionId);

        
        ;
    }
}
