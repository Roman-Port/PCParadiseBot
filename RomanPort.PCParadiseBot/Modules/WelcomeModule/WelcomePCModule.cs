using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.WelcomeModule
{
    public class WelcomePCModule : PCModule
    {
        public override async Task OnInit()
        {
            BindToChannel(672617188003151941, OnMessage);
        }

        private async Task OnMessage(MessageCreateEventArgs e)
        {
            //Make sure it contains rock and is in this server
            if(e.Message.Content.ToLower().Contains("rock") && e.Guild != null)
            {
                //Get guild user
                var member = await e.Guild.GetMemberAsync(e.Author.Id);

                //Give role
                await member.GrantRoleAsync(e.Guild.GetRole(672613900042240001), "Passed verification in #welcome");

                //Delete message
                await e.Message.DeleteAsync();
            }
        }
    }
}
