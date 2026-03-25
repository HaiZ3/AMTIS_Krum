using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constants
{
    public static class Values
    {
        public static readonly string baseUrl = "https://competition-engine-production.up.railway.app/";
        public static readonly string competitorId = "ea04a325-4362-11f0-bc60-0242ac130003";
        public static readonly string gitSha = "d1c7f55adcc9b56a5bfb5cb860fba1678a1c4385";
        public static readonly string sessionType = "test";
        public static readonly HttpClient client;
        static Values()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
        }
    }
}
