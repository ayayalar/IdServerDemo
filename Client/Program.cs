using System;
using System.Net.Http;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync("https://localhost:5000").Result;

            if (!disco.IsError)
            {
                var token = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "apiClient",
                    ClientSecret = "secret",
                    Scope = "api1"
                }).Result;

                if (!token.IsError)
                {
                    Console.WriteLine(token.Json);

                    client = new HttpClient();
                    client.SetBearerToken(token.AccessToken);

                    var response = client.GetAsync("https://localhost:5001/api/identity").Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                    }
                    else
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(JArray.Parse(content));
                    }

                }
            }

            Console.ReadLine();
        }
    }
}
