using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Entities
{
    public abstract class IDiscordFilter
    {
        public abstract Task<bool> IsAccepted(MessageCreateEventArgs e);

        public abstract Task OnAccepted(MessageCreateEventArgs e);

        public PCModule module;

        public void Bind(PCModule module)
        {
            this.module = module;
            Program.discord.MessageCreated += Discord_MessageCreated;
        }

        private async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            try
            {
                //Check if supported
                if (await IsAccepted(e))
                {
                    await OnAccepted(e);
                }
            }
            catch (Exception ex)
            {
                Program.LogModuleError(ex, "Exception thrown while executing command", module, e.Channel);
            }
        }
    }
}
