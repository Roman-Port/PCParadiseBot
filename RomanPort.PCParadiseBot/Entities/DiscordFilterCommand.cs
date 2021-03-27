using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Entities
{
    public class DiscordFilterCommand : IDiscordFilter
    {
        public string prefix;
        public DiscordFilterCommandArgsCallback callback;

        public string name;
        public string help;

        public DiscordFilterCommand(string name, string help, string prefix, DiscordFilterCommandArgsCallback callback)
        {
            this.prefix = prefix;
            this.callback = callback;
            this.name = name;
            this.help = help;
        }

        /// <summary>
        /// Used to check if a user is allowed to use this command when viewing the help menu
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public virtual bool IsUserAllowed(DiscordMember m)
        {
            return true;
        }
        
        public override async Task<bool> IsAccepted(MessageCreateEventArgs e)
        {
            return e.Message.Content.ToLower().StartsWith(prefix);
        }

        public override async Task OnAccepted(MessageCreateEventArgs e)
        {
            //Split args
            string[] args = e.Message.Content.Split(' ');

            //Get the content (excluding the prefix)
            string content = e.Message.Content.Substring(prefix.Length).TrimStart(' ');

            //Trim the first element of args
            string[] argsNext = new string[args.Length - 1];
            Array.Copy(args, 1, argsNext, 0, args.Length - 1);

            //Call next
            await callback(e, content, argsNext);
        }
    }

    public delegate Task DiscordFilterCommandArgsCallback(MessageCreateEventArgs e, string content, string[] args);
}
