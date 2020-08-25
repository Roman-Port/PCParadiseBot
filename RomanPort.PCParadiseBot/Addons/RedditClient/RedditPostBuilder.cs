using Newtonsoft.Json;

namespace RomanPort.PCParadiseBot.Addons.Reddit
{
    /// <summary>
    /// WRITTEN BY: awesomearvinder
    /// </summary>
    public class RedditPostBuilder
    {
        public class data
        {
            public string subreddit;
            public string permalink;
            [JsonProperty("title")]
            public string name;
            public int score;
            public string author;
            public string url;
            public bool stickied;
        }
        [JsonPropertyAttribute("data")]
        public data internal_data;
        public RedditPost build()
        {
            return new RedditPost(internal_data.name, internal_data.score, internal_data.author, internal_data.url, internal_data.stickied);
        }
    }
}
