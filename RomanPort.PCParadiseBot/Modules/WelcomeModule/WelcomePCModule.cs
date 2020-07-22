using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.WelcomeModule
{
    public class WelcomePCModule : PCModule
    {
        public override async Task OnInit()
        {
            BindToChannel(672617188003151941, OnMessage);
        }

        public const int FAILED_PROMPT_REMOVED_SECONDS = 8; //Number of seconds to wait before removing the prompt when a user fails
        public const string FAILED_PROMPT_TEMPLATE = "Hey {@MENTION}, you failed to verify yourself. Please reread the rules to learn how to access all of the channels on this server. Thanks!"; //The template to send. Replaces "{@MENTION}" with a mention to the user

        private async Task OnMessage(MessageCreateEventArgs e)
        {
            //Make sure it contains rock and is in this server
            if(e.Message.Content.ToLower().Contains("rock") && e.Guild != null)
            {
                //Get guild user
                var member = await e.Guild.GetMemberAsync(e.Author.Id);

                //Give role
                await member.GrantRoleAsync(e.Guild.GetRole(672613900042240001), "Passed verification in #welcome");

                //Delete message
                await e.Message.DeleteAsync();
            } else if (e.Guild != null)
            {
                //Fetch member so we can check the roles
                var member = await e.Guild.GetMemberAsync(e.Author.Id);

                //Check if this member is an admin
                bool bypassEnabled = member.Roles.Where(x => x.Id == PCStatics.enviornment.role_moderator).Count() > 0;

                //If the message begins with "-" and the user is an admin, allow them to bypass the deletion
                if (e.Message.Content.StartsWith("-") && bypassEnabled)
                    return;

                //Notify the user that they've made a mistake
                var alertMsg = await e.Message.RespondAsync(FAILED_PROMPT_TEMPLATE.Replace("{@MENTION}", e.Author.Mention));

                //Delete their message
                await e.Message.DeleteAsync("Failed to pass verification in #welcome");

                //Send log message
                await LogToServer("User failed verification in #welcome", $"```{e.Message.Content.Replace("```", @"\`\`\`")}```", e.Author);

                //Set a delay to remove the message
                await Task.Delay(FAILED_PROMPT_REMOVED_SECONDS * 1000);

                //Remove
                await alertMsg.DeleteAsync("Cleaning up my message");
            }
        }
    }
}
