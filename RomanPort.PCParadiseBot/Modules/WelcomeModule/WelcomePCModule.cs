using DSharpPlus;
using DSharpPlus.Entities;
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
            BindToCommandModerator("Lock", "Locks the server to new members. Only for use during raids. Moderator only.", PCStatics.enviornment.command_prefix + "lock", OnLockCommand);
            BindToCommandModerator("Unlock", "Unlocks the server to new members. Moderator only.", PCStatics.enviornment.command_prefix + "unlock", OnUnlockCommand);
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
            } else if (e.Guild != null && !e.Author.IsBot)
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

        public const ulong LOCKED_CHANNEL_ID = 673588744363966464;
        public const ulong EMERGENCY_ROLE = 735575461245223013; //This role is given to moderators and allows them to manage roles while the server is locked. It serves no other purpose

        private async Task OnLockCommand(MessageCreateEventArgs e, string content, string[] args)
        {
            //Get required entites
            var lockedChannel = e.Guild.GetChannel(LOCKED_CHANNEL_ID);
            var welcomeChannel = e.Guild.GetChannel(PCStatics.enviornment.channel_welcome);

            //Create a locked message to send in #locked
            await lockedChannel.SendMessageAsync(CreateLockedMessage());

            //Allow access to that channel and block access to #welcome
            await SetAccessChannelPermission(lockedChannel, Permissions.AccessChannels, Permissions.EmbedLinks | Permissions.AttachFiles | Permissions.AddReactions | Permissions.SendMessages);
            await SetAccessChannelPermission(welcomeChannel, Permissions.None, Permissions.AccessChannels);

            //Give moderators access to permissions
            await e.Guild.UpdateRoleAsync(e.Guild.GetRole(EMERGENCY_ROLE), permissions: Permissions.ManageRoles, reason: "Granting permission because the server has been locked.");

            //Log
            await LogToServer("Server Locked", $"Server was locked.", e.Author);

            //Reply
            await e.Message.RespondAsync("Server was locked successfully. Remember to unlock the server to allow new members to access it.");
        }

        private async Task SetAccessChannelPermission(DiscordChannel targetChannel, Permissions allowedPermissions, Permissions disallowedPermissions)
        {
            foreach (var c in targetChannel.PermissionOverwrites)
            {
                if (c.Id == targetChannel.Guild.EveryoneRole.Id) //@everyone role
                {
                    await targetChannel.UpdateOverwriteAsync(c, allowedPermissions, disallowedPermissions, "Updated server lock status.");
                    return;
                }
            }
            throw new Exception("Could not find @everyone role.");
        }

        private string CreateLockedMessage()
        {
            DateTime refTime = DateTime.UtcNow.AddHours(-6);
            return $"__**Welcome to PC Paradise!**__\n\nUnfortunately, the Discord server is currently receiving attacks. As a result, we cannot allow new members into the server. **This should only last up to 30 minutes.** The server was locked at {refTime.ToLongTimeString()} {refTime.ToShortDateString()} CST.\n\nTo join, wait until the server will be unlocked shortly, or DM one of the staff to gain access now. DO NOT interact with any direct messages you receive from members of this server during this time, as they are likely scams targeted at our members.\n\nThank you for your cooperation.";
        }

        private async Task OnUnlockCommand(MessageCreateEventArgs e, string content, string[] args)
        {
            //Get required entites
            var lockedChannel = e.Guild.GetChannel(LOCKED_CHANNEL_ID);
            var welcomeChannel = e.Guild.GetChannel(PCStatics.enviornment.channel_welcome);

            //Allow access to #welcome and disable #locked
            await SetAccessChannelPermission(welcomeChannel, Permissions.AccessChannels | Permissions.SendMessages, Permissions.EmbedLinks | Permissions.AttachFiles | Permissions.AddReactions);
            await SetAccessChannelPermission(lockedChannel, Permissions.None, Permissions.AccessChannels);

            //Reset moderator emergency permissions
            await e.Guild.UpdateRoleAsync(e.Guild.GetRole(EMERGENCY_ROLE), permissions: Permissions.None, reason: "Server has been unlocked.");

            //Clear all messages in locked. This cleans up the bot's messages
            var lockedMessages = await lockedChannel.GetMessagesAsync(50);
            await lockedChannel.DeleteMessagesAsync(lockedMessages, "Clearing #locked, as the server has been unlocked.");

            //Log
            await LogToServer("Server Locked", $"Server was unlocked.", e.Author);

            //Reply
            await e.Message.RespondAsync("Server was unlocked successfully!");
        }
    }
}
