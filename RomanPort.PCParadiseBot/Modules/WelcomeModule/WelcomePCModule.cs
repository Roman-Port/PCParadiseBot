using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.WelcomeModule
{
    public class WelcomePCModule : PCModule
    {
        DiscordChannel general;

        public override async Task OnInit()
        {
            general = await Program.discord.GetChannelAsync(PCStatics.enviornment.channel_general);
            BindToChannel(672617188003151941, OnMessage);
        }

        private async Task OnMessage(MessageCreateEventArgs e)
        {
            //Make sure it contains rock and is in this server
            if (e.Message.Content.ToLower().Contains("rock") && e.Guild != null)
            {
                //Get guild user
                var member = await e.Guild.GetMemberAsync(e.Author.Id);

                //Give role
                await member.GrantRoleAsync(e.Guild.GetRole(672613900042240001), "Passed verification in #welcome");

                //Delete message
                await e.Message.DeleteAsync();

                //if the user's display name isn't ascii (a charecter's value is over 127) mention them.
                if (member.DisplayName.Any(c => c >= 127))
                {
                    await general.SendMessageAsync($"{member.Mention} I've noticed you have non-ASCII characters in your name. We'd appreciate it if you stuck with ASCII charecters for easy pinging. Thanks!");
                }
            }
        }
    }
}
