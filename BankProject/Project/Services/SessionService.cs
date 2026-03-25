using Project.Constants;
using Project.Models.SessionConstraints;
using Project.Services.PostServices;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public class SessionService : ISessionService
    {

        public SessionService()
        {

        }
        public async Task<SessionInfo?> GetSessionLimits()
        {
            var postData = new PostData(Values.competitorId, Values.sessionType, Values.gitSha);

            var json = JsonSerializer.Serialize(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Values.client.PostAsync(Values.baseUrl + "sessions", content);
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {

            }
            else
            {
                throw new Exception("Error: " + response.StatusCode);
            }
            SessionInfo? sessionInfo = JsonSerializer.Deserialize<SessionInfo>(responseContent);
            return sessionInfo;
        }
    }
}
