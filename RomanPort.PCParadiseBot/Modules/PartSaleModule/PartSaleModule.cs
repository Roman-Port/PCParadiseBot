using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using RomanPort.PCParadiseBot.Addons.Reddit;

namespace RomanPort.PCParadiseBot.Modules.PartSaleModule
{
    /// <summary>
    /// WRITTEN BY: awesomearvinder
    /// </summary>
    public class PartSaleModule : PCModule
    {
        public RedditClient reddit;


        public List<RedditPost> previousPosts = null;

        public override async Task OnInit()
        {
            //Validate
            if (Program.config.part_sale_module_update_interval_seconds == 0)
                throw new Exception("PC Part Sale Module Update Interval is not set in the config file!");

            //Create the reddit client.
            reddit = await RedditClient.init(PCStatics.enviornment.reddit_secret, "PCParadiseBotv1");

            //Begin the loop to fetch the list
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await UpdateList();
                    }
                    catch (Exception ex)
                    {
                        await LogExceptionToServer($"Failed to update sales message! Message may have been deleted or failed network request.", ex);
                    }
                    await Task.Delay(Program.config.part_sale_module_update_interval_seconds * 1000);
                }
            });
        }

        public async Task UpdateList()
        {
            //fetch channel
            DiscordChannel salesChannel;
            salesChannel = await Program.discord.GetChannelAsync(PCStatics.enviornment.channel_sales);

            //Prepare Reddit client
            var sub = await reddit.GetSub("buildapcsales");

            //Create the embed
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor(245, 84, 66))
            .WithTitle("Current Part Sales!")
            .WithFooter("Powered by r/buildapcsales • Created by John Benber#9876", "https://www.redditinc.com/assets/images/site/reddit-logo.png")
            .WithTimestamp(DateTime.UtcNow);

            bool isSameAsPreviousPost = true;

            List<RedditPost> currentPosts = new List<RedditPost>();

            for (int i = 0; i < 8; i++)
            {
                //we don't care about stickies.
                if (!sub.posts.Current.stickied)
                {

                    builder.AddField($"Score {sub.posts.Current.score} (submitted by u/{sub.posts.Current.author}):", $"[{sub.posts.Current.name}]({sub.posts.Current.url})");
                    currentPosts.Add(sub.posts.Current);

                    //check if the previous posts and current posts match, if they don't - something changed.
                    if (previousPosts == null || sub.posts.Current.name != previousPosts[i].name)
                    {
                        isSameAsPreviousPost = false;
                    }

                }
                else
                    i--; //we still want 5 posts, so don't increment this time.
                await sub.posts.MoveNextAsync();
            }

            //Only post a new post if the content has changed in some meaningful way.
            if (isSameAsPreviousPost) return;

            //Delete last message if it exists.
            if (salesChannel.LastMessageId != 0)
            {
                var lastMessage = await salesChannel.GetMessageAsync(salesChannel.LastMessageId);
                await salesChannel.DeleteMessageAsync(lastMessage);
            }

            previousPosts = currentPosts;

            //Build and send message
            DiscordEmbed embed = builder.Build();
            await salesChannel.SendMessageAsync(content: "", embed: embed);
        }
    }
}
