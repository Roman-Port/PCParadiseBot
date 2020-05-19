using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using RomanPort.PCParadiseBot.Modules.ExampleModule;
using RomanPort.PCParadiseBot.Modules.PartSaleModule;
using RomanPort.PCParadiseBot.Modules.SetupsModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot
{
    class Program
    {
        public static DiscordClient discord;
        public static List<PCModule> modules;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            //Load config
            PCConfig config = JsonConvert.DeserializeObject<PCConfig>(File.ReadAllText("config.json"));
            PCStatics.enviornment = config.enviornments[config.enviornment_name];

            //Set up Discord
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = PCStatics.enviornment.access_token,
                TokenType = TokenType.Bot
            });

            //Add modules
            modules = new List<PCModule>();
            AddModules();

            //Connect
            await discord.ConnectAsync();

            //Enable modules
            foreach (var m in modules)
            {
                try
                {
                    await m.OnInit();
                }
                catch (Exception ex)
                {
                    LogModuleError(ex, "Error while initializing module", m, null);
                }
            }
            //Hang
            await Task.Delay(-1);
        }

        public static void AddModules()
        {
            modules.Add(new ExamplePCModule());
            modules.Add(new SetupsPCModule());
            modules.Add(new PartSaleModule());
        }

        /// <summary>
        /// Logs an error in a module. Context is not required, but if it is included an error will be sent in that channel.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="module"></param>
        /// <param name="context"></param>
        public static void LogModuleError(Exception ex, string title, PCModule module, DiscordChannel context)
        {
            Console.WriteLine($"Module {module.GetType().FullName} threw exception in {title}:\n    {ex.Message}\n    {ex.StackTrace}");
        }
    }
}
