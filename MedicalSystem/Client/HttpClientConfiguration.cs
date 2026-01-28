using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class HttpClientConfiguration
    {
        public static void ConfigureDefault(this HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost:7218/");
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("User-Agent", "WpfClient");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }
}
