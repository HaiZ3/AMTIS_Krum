using BankingCompetition.Services;
using Project.Constants;
using Project.Models;
using Project.Models.SessionConstraints;

class Program
{

    static async Task Main()
    {
        //Initialize the session and get the session limits.
        SessionService sessionService = new SessionService();
        SessionInfo? sessionInfo = await sessionService.GetSessionLimits();

        //Initialize the transaction service and get the transactions to audit.
        TransactionService transactionService = new TransactionService(sessionInfo, Values.client);
        while (true)
        {
            TransactionBatch batch = await transactionService.GetTransactionsForAudit(sessionInfo.sessionId);

            if (batch == null || batch.transactions == null || batch.transactions.Length == 0)
            {
                Console.WriteLine("All Transactions are processed!");
                break;
            }
            //Process the transactions as to mark them as either approved or declined
            List<TransactionResult> transactionResults = transactionService.ProcessTransactions(batch.transactions);
            bool sendBatchSuccess = await transactionService.SendBatchResultsAsync(sessionInfo.sessionId, batch.transactionsBatchId, transactionResults);
        }

        ReportService reportService = new ReportService(transactionService.TransactionsByClient);
        List<ReportConfiguration> reportConfiguration = await reportService.GetReportConfigurationAsync(sessionInfo.sessionId);
        
        if(reportConfiguration is null)
        {
            Console.WriteLine("kur");
        }
    }
}
