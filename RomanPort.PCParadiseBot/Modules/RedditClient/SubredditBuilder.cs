using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace RomanPort.PCParadiseBot.Modules.RedditClient
{
    class SubredditBuilder
    {
        public HttpClient client;
        public string name;
        //WHY THE FUCK is everything nested underneath data, reddit?
        public class data
        {

            [JsonProperty("children")]
            public List<RedditPostBuilder> postBuilders;
            public string modhash;
            public string after;
            public string before;
        }
        [JsonPropertyAttribute("data")]
        public data internal_data;
        public Subreddit build()
        {
            if (internal_data.modhash == null || name == null || internal_data.postBuilders == null || client == null)
            {
                throw new ArgumentNullException("Got null argument when building Subreddit");
            }
            Queue<RedditPost> posts = new Queue<RedditPost>();
            foreach (var postBuilder in internal_data.postBuilders)
            {
                posts.Enqueue(postBuilder.build());
            }
            return new Subreddit(client, name, posts, internal_data.modhash, internal_data.after);
        }
    }
}
