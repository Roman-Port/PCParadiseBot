using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Entities
{
    public abstract class IDiscordFilterBasic : IDiscordFilter
    {
        public DiscordFilterCommandCallback eventCallback;

        public IDiscordFilterBasic(DiscordFilterCommandCallback callback)
        {
            this.eventCallback = callback;
        }

        public override async Task OnAccepted(MessageCreateEventArgs e)
        {
            await eventCallback(e);
        }
    }

    public delegate Task DiscordFilterCommandCallback(MessageCreateEventArgs e);
}
