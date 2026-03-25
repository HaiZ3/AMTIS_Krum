using System.Security.Cryptography;
using System.Text;

namespace BankingCompetition.Utils
{
    public static class SHA256
    {
        public static string GenerateSha256Hash(string json)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
                return Convert.ToHexString(hashedBytes);
            }
        }
    }
}
