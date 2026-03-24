using Project.Models.SessionConstraints;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface ISessionService
    {
        public Task<SessionInfo?> GetSessionLimits();
    }
}
