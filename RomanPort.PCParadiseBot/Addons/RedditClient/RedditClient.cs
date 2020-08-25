using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RomanPort.PCParadiseBot.Addons.Reddit
{
    /// <summary>
    /// WRITTEN BY: awesomearvinder
    /// </summary>
    public class RedditClient
    {
        private HttpClient client;

        private RedditClient(HttpClient client)
        {
            this.client = client;
        }

        public static async Task<RedditClient> init(string secret, string userAgent)
        {
            //make a new Client. This isn't static so in the future for different authentication methods
            //you could have multiple RedditClient's with different users.
            HttpClient client = new HttpClient();
            await authenticate(client, secret, userAgent);
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await authenticate(client, secret, userAgent);
                    }
                    catch
                    {
                        continue;
                    }
                    await System.Threading.Tasks.Task.Delay(3000 * 1000);
                }
            });
            return new RedditClient(client);
        }
        private static async Task authenticate(HttpClient client, string secret, string userAgent)
        {
            //make a new Client. This isn't static so in the future for different authentication methods
            //you could have multiple RedditClient's with different users.
            string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(secret));
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", authToken);
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
            };
            var content = new FormUrlEncodedContent(values);
            Uri uri = new Uri("https://www.reddit.com/api/v1/access_token");
            var response = await client.PostAsync(uri, content);
            string json = await response.Content.ReadAsStringAsync();
            AuthenticationResponse authResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(json);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                    authResponse.access_token);
        }
        public async Task<Subreddit> GetSub(string name)
        {
            var response = await client.GetAsync($"https://oauth.reddit.com/r/{name}.json");
            SubredditBuilder builder = JsonConvert.DeserializeObject<SubredditBuilder>(await response.Content.ReadAsStringAsync());
            builder.name = name;
            builder.client = client;
            return builder.build();
        }
    }


    struct AuthenticationResponse
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public string scope;
    }
}
