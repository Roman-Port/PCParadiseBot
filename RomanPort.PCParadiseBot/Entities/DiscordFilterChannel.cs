using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Entities
{
    public class DiscordFilterChannel : IDiscordFilterBasic
    {
        public ulong id;

        public DiscordFilterChannel(ulong id, DiscordFilterCommandCallback callback) : base(callback)
        {
            this.id = id;
        }
        
        public override async Task<bool> IsAccepted(MessageCreateEventArgs e)
        {
            return e.Channel.Id == id;
        }
    }
}
