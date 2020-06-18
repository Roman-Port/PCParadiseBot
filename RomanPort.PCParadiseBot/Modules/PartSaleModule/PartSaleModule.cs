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
        public DiscordMessage salesMessage;

        public override async Task OnInit()
        {
            //Create the reddit client
            reddit = await RedditClient.init(PCStatics.enviornment.reddit_secret, "PCParadiseBotv1");

            //Fetch channel
            salesChannel = await Program.discord.GetChannelAsync(PCStatics.enviornment.channel_sales);

            //Attempt to get the last message to update. If it's our own, use it. If it's not, create one
            try
            {
                salesMessage = await salesChannel.GetMessageAsync(salesChannel.LastMessageId);
                if (salesMessage.Author.Id != Program.discord.CurrentUser.Id)
                    //We do not own the last message. Create a new one
                    salesMessage = null;
            }
            catch
            {
                salesMessage = null;
            }
            if (salesMessage == null)
                salesMessage = await salesChannel.SendMessageAsync("Loading, please wait...");

            //Begin the loop to fetch the list
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await UpdateList();
                    }
                    catch (Exception ex)
                    {
                        await LogToServer("Failed to Update", "Failed to update sales message! Message may have been deleted or failed network request.", null);
                    }
                    await Task.Delay(UPDATE_INTERVAL);
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
                if (!sub.posts.Current.stickied)
                    builder.AddField($"Score {sub.posts.Current.score} (submitted by u/{sub.posts.Current.author}):", $"[{sub.posts.Current.name}]({sub.posts.Current.url})");
                else
                    i--; //we still want 5 posts, so don't increment this time.
                await sub.posts.MoveNextAsync();
            }

            //Build and update message
            DiscordEmbed embed = builder.Build();
            await salesMessage.ModifyAsync(content: "", embed: embed);
        }
    }
}
