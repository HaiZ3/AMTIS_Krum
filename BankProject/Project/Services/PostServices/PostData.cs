using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services.PostServices
{
    public class PostData
    {
        private readonly HashSet<string> allowedSessions = new HashSet<string> { "test", "sanity", "stress", "survival" };
        public PostData(string competitorId, string sessionType, string gitSha)
        {
            if (allowedSessions.Contains(competitorId))
            {
                throw new ArgumentException("Invalid session type!");
            }
            this.competitorId = competitorId;
            this.sessionType = sessionType;
            this.gitSha = gitSha;
        }

        public string competitorId { get; set; }
        public string sessionType { get; set; }
        public string gitSha { get; set; }
    }
}
