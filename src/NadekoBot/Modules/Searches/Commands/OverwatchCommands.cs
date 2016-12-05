using Discord;
using Discord.API;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using NadekoBot.Modules.Searches.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class OverwatchCommands
        {
            private Logger _log;

            public OverwatchCommands()
            {
                _log = LogManager.GetCurrentClassLogger();
            }

            //Your command
            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Overwatch(IUserMessage umsg, string region, [Remainder] string query = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(query))
                    return;
                var battletag = Regex.Replace(query, "#", "-", RegexOptions.IgnoreCase);
                try
                {
                    using (var http = new HttpClient())
                    {
                        var lootbox = await http.GetStringAsync($"https://api.lootbox.eu/pc/{region.ToLower()}/{battletag}/profile");
                        var model = JsonConvert.DeserializeObject<OverwatchApiModel>(lootbox);
                        await channel.SendMessageAsync($@"Username: {model.Data.username}
Level: {model.Data.level}
Quick Wins: {model.Data.Games.Quick.wins}
Current Competitive Wins: {model.Data.Games.Competitive.wins}
Current Competitive Loses: {model.Data.Games.Competitive.lost}
Competitive Playtime: {model.Data.Playtime.competitive}
Competitive Rank: {model.Data.Competitive.rank}").ConfigureAwait(false);

                    }
                }
                catch
                {
                    await channel.SendMessageAsync("`Found no user with the BattleTag.`").ConfigureAwait(false);
                }
            }
        }
       }
    }
