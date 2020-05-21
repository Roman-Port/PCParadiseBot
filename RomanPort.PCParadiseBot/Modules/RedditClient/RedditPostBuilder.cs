using Newtonsoft.Json;

namespace RomanPort.PCParadiseBot.Modules.Reddit
{
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
        }
        [JsonPropertyAttribute("data")]
        public data internal_data = new data();
        public RedditPost build()
        {
            return new RedditPost(internal_data.name, internal_data.score, internal_data.author, internal_data.url);
        }
    }
}
