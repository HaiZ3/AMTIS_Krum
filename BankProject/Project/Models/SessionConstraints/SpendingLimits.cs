using Project.Models.SessionConstraints.CardInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.SessionConstraints
{
    public class SpendingLimits
    {
        public decimal dailyClientLimit { get; set; }
        public decimal weeklyClientLimit { get; set; }
        public int allowedTransactionsPer10s { get; set; }
        public decimal interchangeFeePercentage { get; set; }
        public CardLimits cardLimits { get; set; }
    }
}
