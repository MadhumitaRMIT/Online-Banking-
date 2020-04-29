using System;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Assignment2.Web
{
    public class BankApi
    {
        private const string ApiBaseUri = "https://localhost:44387/";

        public HttpClient InitializeClient()
        {
            /*
            var client = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            */

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56074/");

            return client;
        }
    }
}
