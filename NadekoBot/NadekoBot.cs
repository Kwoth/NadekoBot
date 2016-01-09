using Discord;
using System;
using System.IO;
using Newtonsoft.Json;
using Parse;
using Discord.Commands;
using NadekoBot.Modules;
using Discord.Modules;
using Discord.Legacy;
using Discord.Audio;

namespace NadekoBot
{
    class NadekoBot
    {
        public static DiscordClient client;
       // public static StatsCollector stats_collector;
        public static string botMention;
        public static string GoogleAPIKey;
        public static ulong OwnerID;
        public static string password;

        static void Main()
        {
			//Change Console Window settings.
			Console.Title = "NadekoBot v0.5.0";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
			
            //load credentials from credentials.json
            Credentials c;
            try
            {
                c = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText("credentials.json"));
                botMention = c.BotMention;
                GoogleAPIKey = c.GoogleAPIKey;
                OwnerID = c.OwnerID;
                password = c.Password;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load stuff from credentials.json, RTFM");
                Console.ReadKey();
                return;
            }

            client = new DiscordClient();
            
            //init parse
            if (c.ParseKey != null && c.ParseID != null)
                ParseClient.Initialize(c.ParseID,c.ParseKey);

            //create new discord client
            

            //create a command service
            var commandService = new CommandService(new CommandServiceConfig
            {
                CommandChar = null,
                HelpMode = HelpMode.Disable
            });

            //monitor commands for logging
            //stats_collector = new StatsCollector(commandService);

            //add command service
            var commands = client.Services.Add<CommandService>(commandService);
            
            //create module service
            var modules = client.Services.Add(new ModuleService());

            //add audio service
            var audio = client.Services.Add<AudioService>(new AudioService(new AudioServiceConfig() {
                Channels = 2,
                EnableEncryption = false
            }));

            //install modules
            modules.Add(new Administration(), "Administration", ModuleFilter.None);
            modules.Add(new Conversations(), "Conversations", ModuleFilter.None);
            modules.Add(new Gambling(), "Gambling", ModuleFilter.None);
            modules.Add(new Games(), "Games", ModuleFilter.None);
            modules.Add(new Music(), "Music", ModuleFilter.None);
            modules.Add(new Searches(), "Searches", ModuleFilter.None);

            //run the bot
            client.Run(async () =>
            {
                await client.Connect(c.Username, c.Password);
                Console.WriteLine("Logged in as " + client.CurrentUser.Name + "\nBot id=" + client.CurrentUser.Id + "\nDiscord.Net v" + DiscordConfig.LibVersion + "\n-------------------------\n");
            });
            Console.WriteLine("Exiting...");
            Console.ReadKey();
        }
    }
}