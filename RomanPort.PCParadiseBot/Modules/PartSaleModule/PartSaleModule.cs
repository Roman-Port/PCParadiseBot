using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using RomanPort.PCParadiseBot.Addons.Reddit;

namespace RomanPort.PCParadiseBot.Modules.PartSaleModule
{
    /// <summary>
    /// WRITTEN BY: awesomearvinder
    /// </summary>
    public class PartSaleModule : PCModule
    {
        const int UPDATE_INTERVAL = 60 * 1000;

        public RedditClient reddit;

        public DiscordChannel salesChannel;

        public override async Task OnInit()
        {
            //Create the reddit client.
            reddit = await RedditClient.init(PCStatics.enviornment.reddit_secret, "PCParadiseBotv1");

            //Fetch channel
            salesChannel = await Program.discord.GetChannelAsync(PCStatics.enviornment.channel_sales);

            //Begin the loop to fetch the list
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await UpdateList();
                        await Task.Delay(UPDATE_INTERVAL);
                    }
                    catch (Exception ex)
                    {
                        await LogToServer("Failed to Update", "Failed to update sales message! Message may have been deleted or failed network request.", null);
                    }
                }
            });
        }

        public async Task UpdateList()
        {
            //Prepare Reddit client
            var sub = await reddit.GetSub("buildapcsales");

            //Create the embed
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor(245, 84, 66))
            .WithTitle("Current Part Sales!")
            .WithFooter("Powered by r/buildapcsales • Created by John Benber#9876", "https://www.redditinc.com/assets/images/site/reddit-logo.png")
            .WithTimestamp(DateTime.UtcNow);
            for (int i = 1; i < 6; i++)
            {
                await sub.posts.MoveNextAsync();
                builder.AddField($"Score {sub.posts.Current.score} (submitted by u/{sub.posts.Current.author}):", $"[{sub.posts.Current.name}]({sub.posts.Current.url})");
            }

            //Build and send message
            DiscordEmbed embed = builder.Build();
            await salesChannel.SendMessageAsync(content: "", embed: embed);
        }
    }
}
