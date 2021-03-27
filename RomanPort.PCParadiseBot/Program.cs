using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using RomanPort.PCParadiseBot.Modules.ExampleModule;
using RomanPort.PCParadiseBot.Modules.SystemModule;
using RomanPort.PCParadiseBot.Modules.SetupsModule;
using RomanPort.PCParadiseBot.Modules.WelcomeModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RomanPort.PCParadiseBot.Modules.EmojiBanModule;

namespace RomanPort.PCParadiseBot
{
    class Program
    {
        public static PCConfig config;
        public static DiscordClient discord;
        public static List<PCModule> modules;
        public const int BOT_VERSION_MAJOR = 2;
        public const int BOT_VERSION_MINOR = 2;

        public static readonly DiscordColor STANDARD_EMBED_COLOR = new DiscordColor(56, 130, 220);

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            //Load config
            config = JsonConvert.DeserializeObject<PCConfig>(File.ReadAllText("config.json"));
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
            Console.WriteLine("Modules added.");

            //Connect
            await discord.ConnectAsync();
            Console.WriteLine("Connected to Discord.");

            //Enable modules
            foreach (var m in modules)
            {
                try
                {
                    await m.OnInit();
                } catch (Exception ex)
                {
                    await LogModuleError(ex, "Error while initializing module", m, null);
                }
            }
            Console.WriteLine("Modules loaded.");
            Console.WriteLine("Startup finished.");

            //Connect
            await discord.ConnectAsync();

            //Hang
            await Task.Delay(-1);
        }

        public static void AddModules()
        {
            modules.Add(new HelpCommandModule());
            modules.Add(new AboutCommandModule());
            modules.Add(new SetupsPCModule());
            modules.Add(new WelcomePCModule());
            modules.Add(new EmojiBanPCModule());
        }

        /// <summary>
        /// Logs an error in a module. Context is not required, but if it is included an error will be sent in that channel.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="module"></param>
        /// <param name="context"></param>
        public static async Task LogModuleError(Exception ex, string title, PCModule module, DiscordChannel context)
        {
            Console.WriteLine($"Module {module.GetType().FullName} threw exception in {title}:\n    {ex.Message}\n    {ex.StackTrace}");
        }
    }
}
