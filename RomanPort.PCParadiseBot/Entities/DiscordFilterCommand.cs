using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Entities
{
    public class DiscordFilterCommand : IDiscordFilter
    {
        public string prefix;
        public DiscordFilterCommandArgsCallback callback;

        public DiscordFilterCommand(string prefix, DiscordFilterCommandArgsCallback callback)
        {
            this.prefix = prefix;
            this.callback = callback;
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
