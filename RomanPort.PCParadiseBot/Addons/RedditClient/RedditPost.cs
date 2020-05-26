namespace RomanPort.PCParadiseBot.Addons.Reddit
{
    /// <summary>
    /// WRITTEN BY: awesomearvinder
    /// </summary>
    public class RedditPost
    {
#pragma warning disable 0649
        public string name
        {
            get;
            private set;
        }
        public int score
        {
            get;
            private set;
        }
        public string author
        {
            get;
            private set;
        }
        public string url
        {
            get;
            private set;
        }
        public RedditPost(string name, int score, string author, string url)
        {
            this.name = name;
            this.score = score;
            this.author = author;
            this.url = url;
        }
    }
}
