using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.SetupsModule
{
    public class SetupsPCModule : PCModule
    {
        public override async Task OnInit()
        {
            BindToChannel(705487040560496760, OnMsgInSetupsSent);
        }

        public Dictionary<ulong, DateTime> lastSentMessage = new Dictionary<ulong, DateTime>(); //Lazy man way of saving the last time a user sent a message that was OK

        public Regex linkRegex = new Regex(@"https?:\/\/(www\.)?(?!.*pcpartpicker\.com)([-a-zA-Z0-9@:%._\+~#=]+)\.[a-z]{2,4}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

        public const string TITLE_FORMATTING = "YOUR MESSAGE WAS NOT FORMATTED CORRECTLY AND WILL BE REMOVED";
        public const string TITLE_USER = "YOU ARE NOT YET ELIGIBLE AND YOUR MESSAGE WILL BE REMOVED";
        public const string TITLE_TIMEOUT = "YOU POSTING TOO QUICKLY AND YOUR MESSAGE WILL BE REMOVED";

        public const int REQUIREMENT_DAYS = 3;
        public const int REQUIREMENT_WORDS = 10;
        public const int REQUIREMENT_TIMEOUT_MINUTES = 30;

        public const string GUIDE_LINK = "https://discordapp.com/channels/246414844851519490/260165399536992257/705647305176645723";

        private async Task OnMsgInSetupsSent(MessageCreateEventArgs e)
        {
            //If this is CorpBot, delete it (jank)
            if (e.Author.Id == 225748203151163393)
            {
                await e.Message.DeleteAsync("DIE, FELLOW BOT!!!!!!!!!!");
                return;
            }

            //Make sure this wasn't a bot
            if (e.Author.IsBot)
                return;

            //Get how long this user has been in the server
            var member = await e.Guild.GetMemberAsync(e.Author.Id);
            TimeSpan timeInServer = DateTime.UtcNow - member.JoinedAt.UtcDateTime;

            //Allow admins and mods to bypass this
            if (e.Message.Content.StartsWith("-") && member.Roles.Where(x => x.Id == PCStatics.enviornment.role_admin || x.Id == PCStatics.enviornment.role_moderator).Count() > 0)
                return;

            //Check if this message contains the start "description:"
            if (!e.Message.Content.ToLower().StartsWith("description"))
            {
                await OnMsgNoPass(e, TITLE_FORMATTING, $"Your message should describe what you've posted. Begin your message with \"description\".", "NO_DESCRIPTION");
                return;
            }

            //Check if the message contains an attachment
            if (e.Message.Attachments.Count == 0)
            {
                await OnMsgNoPass(e, TITLE_FORMATTING, "Your message must contain a picture of your setup!", "NO_ATTACHMENT");
                return;
            }

            //Check if the message contains some number of words
            if (e.Message.Content.Split(' ').Length < REQUIREMENT_WORDS)
            {
                await OnMsgNoPass(e, TITLE_FORMATTING, "Your description must contain at least a few words. Be descriptive and tell us about your setup!", "NOT_ENOUGH_WORDS");
                return;
            }

            //Check if the user has been on the server longer than a few days
            if (timeInServer.TotalDays < REQUIREMENT_DAYS)
            {
                TimeSpan timeRequirementRemaining = new TimeSpan(REQUIREMENT_DAYS, 0, 0, 0) - timeInServer;
                await OnMsgNoPass(e, TITLE_USER, $"To post, you need to be a member of this server for at least {REQUIREMENT_DAYS} days. Please wait {timeRequirementRemaining.Days} days, {timeRequirementRemaining.Hours} hours and try again.", "NEW_USER");
                return;
            }

            //Check if this user has timed out
            /*if(CheckIfUserTimedOut(e.Author.Id))
            {
                TimeSpan tss = DateTime.UtcNow - lastSentMessage[e.Author.Id];
                await OnMsgNoPass(e, TITLE_TIMEOUT, $"Sorry, you may only post every {REQUIREMENT_TIMEOUT_MINUTES} seconds. Please wait {Math.Round(tss.TotalMinutes)} and try again.", "TIMED_OUT_USER");
                return;
            }*/

            //Check if we have a link
            if (linkRegex.IsMatch(e.Message.Content))
            {
                await OnMsgNoPass(e, TITLE_FORMATTING, $"You cannot have links other than pcpartpicker in your description. Please remove any links and post again.", "HAS_LINKS");
                return;
            }

            //Update or add this
            if (lastSentMessage.ContainsKey(e.Author.Id))
                lastSentMessage[e.Author.Id] = DateTime.UtcNow;
            else
                lastSentMessage.Add(e.Author.Id, DateTime.UtcNow);

            //Add initial reactions
            await e.Message.CreateReactionAsync(DSharpPlus.Entities.DiscordEmoji.FromGuildEmote(Program.discord, 422591814340706314));
            await e.Message.CreateReactionAsync(DSharpPlus.Entities.DiscordEmoji.FromGuildEmote(Program.discord, 422593649977589770));
        }

        private bool CheckIfUserTimedOut(ulong id)
        {
            //Check if we have information on them
            if (!lastSentMessage.ContainsKey(id))
                return false;

            //Check if it's within bounds
            TimeSpan tss = DateTime.UtcNow - lastSentMessage[id];
            return tss.TotalMinutes < REQUIREMENT_TIMEOUT_MINUTES;
        }

        private async Task OnMsgNoPass(MessageCreateEventArgs e, string title, string reason, string loggingReason)
        {
            //Send the message as a normal message so we can @mention the user
            string content = $"**{title}**\n{reason}\n\n__For help, view the guide at <{GUIDE_LINK}>__\n{e.Author.Mention}";
            var msg = await e.Message.RespondAsync(content);

            //Delete the message the user sent
            await e.Message.DeleteAsync("Setup was not formatted correctly and was removed.");

            //Log
            await LogToServer("User Failed to Send Setup", $"User failed with reason {loggingReason}\n\n```{e.Message.Content}```", e.Author);

            //Wait 10s and remove the alert
            await Task.Delay(8 * 1000);

            //Remove
            await msg.DeleteAsync("Cleaning up my message");
        }
    }
}
