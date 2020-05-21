using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using RomanPort.PCParadiseBot.Modules.Reddit;
namespace RomanPort.PCParadiseBot.Modules.PartSaleModule
{
    public class PartSaleModule : PCModule
    {
        const int UPDATE_INTERVAL = 60 * 1000;

        public static RedditClient reddit;
        public override async Task OnInit()
        {
            reddit = await RedditClient.init(PCStatics.enviornment.reddit_secret, "PCParadiseBotv1");
            await Task.Run(async () =>
            {
                while (true)
                {
                    await UpdateList();
                    await Task.Delay(UPDATE_INTERVAL);
                }
            });
        }

        public async Task UpdateList()
        {
            var salesChannel = await Program.discord.GetChannelAsync(PCStatics.enviornment.channel_sales);
            var lastMessage = await salesChannel.GetMessageAsync(salesChannel.LastMessageId);
            var sub = await reddit.GetSub("buildapcsales");
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor(245, 84, 66))
            .WithTitle("Current Part Sales!")
            .WithFooter("Powered by r/buildapcsales • Created by John Benber#9876", "https://www.redditinc.com/assets/images/site/reddit-logo.png")
            .WithTimestamp(DateTime.UtcNow);
            for (int i = 1; i < 6; i++)
            {
                await sub.posts.MoveNextAsync();
                builder.AddField($"Number {i}:", $"[{sub.posts.Current.name}]({sub.posts.Current.url})");
            }
            DiscordEmbed embed = builder.Build();
            if (lastMessage.Author.Id == Program.discord.CurrentUser.Id)
            {
                await lastMessage.ModifyAsync(default(Optional<string>), new Optional<DiscordEmbed>(embed));
            }
            else
            {
                await salesChannel.SendMessageAsync("", false, embed);
            }
        }
    }
}
