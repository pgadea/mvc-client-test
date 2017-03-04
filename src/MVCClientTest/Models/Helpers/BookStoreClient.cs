using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MVCClientTest.Models.Helpers
{
    public class BookStoreClient
    {
        static HttpClient client;

        string url = "http://localhost:3795/api/";

        // Read from some configuration
        string bookStoreApiVersion = "5";
        public BookStoreClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if(!string.IsNullOrWhiteSpace(url))
            {
                client.DefaultRequestHeaders.Add("X-BookStoreAPI-Version", bookStoreApiVersion);
            }
        }

        public HttpClient Client { get { return client; } }
    }
}
