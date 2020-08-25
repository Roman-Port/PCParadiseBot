using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using RomanPort.PCParadiseBot.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.SystemModule
{
    public class HelpCommandModule : PCModule
    {
        public override async Task OnInit()
        {
            BindToCommand("Help Command", "Shows this help menu.", PCStatics.enviornment.command_prefix + "help", OnHelpCommand);
        }

        public static List<DiscordFilterCommand> commands = new List<DiscordFilterCommand>(); //When binding to a command, add it to this. A little bit janky

        public async Task OnHelpCommand(MessageCreateEventArgs e, string content, string[] args)
        {
            //Get Discord member
            if (e.Guild == null)
                return;
            var member = await e.Guild.GetMemberAsync(e.Author.Id);
            
            //Make base embed
            var embed = new DiscordEmbedBuilder();
            embed.Color = Program.STANDARD_EMBED_COLOR;
            embed.Title = "RobotPort Help";
            embed.Description = "Here are a list of commands you can use. Users with more roles may be able to see other commands. [Open Source](https://github.com/Roman-Port/PCParadiseBot)";
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "Created by RomanPort#0001"
            };

            //Get commands
            foreach(var c in commands)
            {
                if(c.IsUserAllowed(member))
                    embed.AddField(c.prefix, c.help, false);
            }

            //Send
            await e.Message.RespondAsync(embed: embed);
        }
    }
}
