using BankingCompetition.Models;
using Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface IReportService
    {
        Task<ReportConfiguration> GetReportConfigurationAsync(string sessionId);
        List<Report> GenerateReports(ReportConfiguration config);
        Task<bool> SendReportsAsync(string sessionId, string competitorId, List<Report> reports);
    }
}
