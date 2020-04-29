using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Entities
{
    public class DiscordFilterCommandRole : DiscordFilterCommand
    {
        public ulong id;
        
        public DiscordFilterCommandRole(string prefix, ulong roleId, DiscordFilterCommandArgsCallback callback) : base(prefix, callback)
        {
            id = roleId;
        }

        public override async Task<bool> IsAccepted(MessageCreateEventArgs e)
        {
            //Check before
            if (!await base.IsAccepted(e))
                return false;

            //Make sure that this is in the correct server
            if (e.Guild?.Id != PCStatics.enviornment.guild_id)
                return false;

            //Get member
            var member = await e.Guild.GetMemberAsync(e.Author.Id);
            if (member == null)
                return false;

            //Check roles
            return member.Roles.Where(x => x.Id == id).Count() > 0;
        }
    }
}
