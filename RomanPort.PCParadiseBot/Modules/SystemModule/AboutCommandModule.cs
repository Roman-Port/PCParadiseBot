using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using RomanPort.PCParadiseBot.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.SystemModule
{
    public class AboutCommandModule : PCModule
    {
        public override async Task OnInit()
        {
            BindToCommand("About Command", "Shows info about the bot.", PCStatics.enviornment.command_prefix + "about", OnAboutCommand);
        }

        public async Task OnAboutCommand(MessageCreateEventArgs e, string content, string[] args)
        {
            //Make base embed
            var embed = new DiscordEmbedBuilder();
            embed.Color = Program.STANDARD_EMBED_COLOR;
            embed.Title = "RobotPort Info";
            embed.AddField("Bot Enviornment", Program.config.server_name + " - " + Program.config.enviornment_name);
            embed.AddField("Bot Version", Program.BOT_VERSION_MAJOR + "." + Program.BOT_VERSION_MINOR);
            embed.AddField("Bot Author", "RomanPort#0001 - Written for PC Paradise");
            embed.AddField("Bot Contributors", "PC Part Sales Module: John Benber#8013");
            embed.AddField("Open Source", "https://github.com/Roman-Port/PCParadiseBot");

            //Send
            await e.Message.RespondAsync(embed: embed);
        }
    }
}
