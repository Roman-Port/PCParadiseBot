using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.EmojiBanModule
{
    public class EmojiBanPCModule : PCModule
    {
        public override async Task OnInit()
        {
            //Load list
            if (File.Exists("banned_emoji.txt"))
                bannedWords = File.ReadAllLines("banned_emoji.txt").ToList();
            else
                bannedWords = new List<string>();

            //Bind
            BindToCommandAdmin("Ban Emoji", "Bans an emoji by it's ID", PCStatics.enviornment.command_prefix + "banemoji", OnBanCommand);
            BindToChannel(246414844851519490, OnMessage);
        }

        public List<string> bannedWords = new List<string>();

        private async Task OnBanCommand(MessageCreateEventArgs e, string content, string[] args)
        {
            bannedWords.Add(content);
            File.WriteAllLines("banned_emoji.txt", bannedWords.ToArray());
        }

        private async Task OnMessage(MessageCreateEventArgs e)
        {
            //Detect
            bool banned = false;
            foreach (var b in bannedWords)
                banned = banned || e.Message.Content.Contains(b);

            //Remove
            if (banned)
                await e.Message.DeleteAsync("Message contained banned emoji");
        }
    }
}
