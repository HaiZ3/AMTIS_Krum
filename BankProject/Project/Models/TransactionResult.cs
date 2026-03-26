using BankingCompetition.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class TransactionResult
    {
        public string transaction_id { get; set; }
        public string status { get; set; }
        public TransactionResult(Transaction transaction)
        {
            this.transaction_id = transaction.transaction_id;
            this.status = transaction.status;
        }
    }
}
