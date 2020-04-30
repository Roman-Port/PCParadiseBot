using DSharpPlus.Entities;
using RomanPort.PCParadiseBot.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot
{
    public abstract class PCModule
    {
        public abstract Task OnInit();
        
        /// <summary>
        /// Binds to whenever a message is sent in some channel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public void BindToChannel(ulong id, DiscordFilterCommandCallback callback)
        {
            var b = new DiscordFilterChannel(id, callback);
            b.Bind(this);
        }

        /// <summary>
        /// Binds to whenever a command is sent
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="callback"></param>
        public void BindToCommand(string prefix, DiscordFilterCommandArgsCallback callback)
        {
            var b = new DiscordFilterCommand(prefix, callback);
            b.Bind(this);
        }

        /// <summary>
        /// Binds to whenever a command from a certain role is sent
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="callback"></param>
        public void BindToCommandRole(string prefix, ulong roleId, DiscordFilterCommandArgsCallback callback)
        {
            var b = new DiscordFilterCommandRole(prefix, roleId, callback);
            b.Bind(this);
        }

        /// <summary>
        /// Binds to a command, but only if sent by an ADMIN. For moderators, there is a separate binding
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="callback"></param>
        public void BindToCommandAdmin(string prefix, DiscordFilterCommandArgsCallback callback)
        {
            BindToCommandRole(prefix, PCStatics.enviornment.role_admin, callback);
        }

        /// <summary>
        /// Binds to a command, but only if sent by a MODERATOR
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="callback"></param>
        public void BindToCommandModerator(string prefix, DiscordFilterCommandArgsCallback callback)
        {
            BindToCommandRole(prefix, PCStatics.enviornment.role_moderator, callback);
        }

        /// <summary>
        /// Logs to the server logging channel
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task LogToServer(string title, string text, DiscordUser user)
        {
            //Build embed
            var embed = new DiscordEmbedBuilder();
            embed.Title = title;
            embed.Description = text;
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "MODULE " + GetType().FullName
            };
            if(user != null)
            {
                embed.Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = $"{user.Username}#{user.Discriminator} (ID {user.Id})",
                    IconUrl = user.AvatarUrl
                };
            }
            embed.Color = DiscordColor.Red;

            //Send
            await Program.discord.SendMessageAsync(await Program.discord.GetChannelAsync(PCStatics.enviornment.channel_logs), embed: embed);
        }
    }
}
