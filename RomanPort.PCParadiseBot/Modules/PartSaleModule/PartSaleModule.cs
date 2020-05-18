using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.PartSaleModule
{
    public class PartSaleModule : PCModule
    {
        const int FIVE_SECONDS = 5000;

        public override async Task OnInit()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await UpdateList();
                    await Task.Delay(FIVE_SECONDS);
                }
            });
        }

        public async Task UpdateList()
        {
            var salesChannel = await Program.discord.GetChannelAsync(PCStatics.enviornment.channel_sales);
            var lastMessage = await salesChannel.GetMessageAsync(salesChannel.LastMessageId);
            var sub = await Program.reddit.GetSub("buildapcsales");
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor(245, 84, 66))
            .WithTitle("Current Part Sales!");
            for (int i = 1; i < 6; i++)
            {
                await sub.posts.MoveNextAsync();
                builder.AddField($"Number {i}:", $"[{sub.posts.Current.name}]({sub.posts.Current.url})");
            }
            DiscordEmbed embed = builder.Build();
            Console.WriteLine(Program.discord.CurrentUser.Id);
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
