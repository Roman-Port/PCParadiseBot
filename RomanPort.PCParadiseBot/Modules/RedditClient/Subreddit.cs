using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace RomanPort.PCParadiseBot.Modules.RedditClient
{
    public class Subreddit
    {
#pragma warning disable 0169
        private HttpClient client;
#pragma warning restore 0169
        public string modhash;
        public string name;
        public IAsyncEnumerator<RedditPost> posts;
        public Subreddit(HttpClient client, string name, Queue<RedditPost> posts, string modhash, string after)
        {
            System.Console.WriteLine(after);
            this.modhash = modhash;
            this.name = name;
            this.posts = new Posts(client, posts, name, after);
        }
    }

    public class Posts : IAsyncEnumerator<RedditPost>
    {
        private HttpClient client;
        private string name;
        private string after;
        private Queue<RedditPost> posts;
        public async ValueTask<bool> MoveNextAsync()
        {
            if (posts.Count > 1)
            {
                posts.Dequeue();
                return true;
            }
            else
            {
                await goToNext();
                posts.Dequeue();
                return true;
            }
        }

        public async Task goToNext()
        {
            var response = await client.GetAsync($"https://oauth.reddit.com/r/{name}.json?after={after}");
            string json = await response.Content.ReadAsStringAsync();
            var jsonRedditPosts = JObject.Parse(json)["data"]["children"];
            after = (string)JObject.Parse(json)["data"]["after"];
            foreach (var post in jsonRedditPosts)
            {
                this.posts.Enqueue(post.ToObject<RedditPostBuilder>().build());
            }
        }

        public RedditPost Current
        {
            get
            {
                return posts.Peek();
            }
        }
        public Posts(HttpClient client, Queue<RedditPost> posts, string name, string after)
        {
            this.client = client;
            this.name = name;
            this.posts = posts;
            this.after = after;
        }
        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}
