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
        public static readonly string gitSha = "1c5dc3da430d78ed09fb57a38109d8636d14f3eb";
        public static readonly string sessionType = "test";
        public static HttpClient client = new HttpClient();
    }
}
