using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using NadekoBot.Modules.Searches.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class SteamCommands
        {
            private Logger _log;

            public SteamCommands()
            {
                _log = LogManager.GetCurrentClassLogger();
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task SteamUser(IUserMessage umsg, [Remainder] string query = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(query))
                    return;
                try
                {
                    using (var http = new HttpClient())
                    {
                        var steamId = await QuerySteamId(query);
                        var steamUrl = await http.GetStringAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={NadekoBot.Credentials.SteamApiKey}&steamids={steamId}");
                        var model = JsonConvert.DeserializeObject<SteamApiModel>(steamUrl);
                        var profileStatus = model.Response.Players[0].personastate;
                        var embed = new EmbedBuilder()
                            .WithColor(NadekoBot.OkColor)
                            .WithTitle("Steam Profile")
                            .WithUrl(model.Response.Players[0].profileurl)
                            .WithAuthor(x => x.WithName(model.Response.Players[0].personaname)
                                .WithUrl(model.Response.Players[0].profileurl))
                            .WithThumbnail(x => x.WithUrl(model.Response.Players[0].avatar))
                            .WithDescription("Profile Status: " + Enum.Parse(typeof(SteamApiModel.profilestate),profileStatus.ToString()));
                        await channel.EmbedAsync(embed.Build());
                    }
                }
                catch
                {
                        var embed = new EmbedBuilder()
                            .WithColor(NadekoBot.ErrorColor)
                            .WithTitle("Steam Profile")
                            .WithDescription("`Found no user with that id`");
                        await channel.EmbedAsync(embed.Build());
                }
            }

            public async Task<string> QuerySteamId(string user)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        var steamUrl = await http.GetStringAsync($"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={NadekoBot.Credentials.SteamApiKey}&vanityurl={user}");
                        var model = JsonConvert.DeserializeObject<SteamApiModel>(steamUrl);
                        if (string.IsNullOrEmpty(model.Response.steamid))
                            return null;
                        else 
                            return model.Response.steamid;
                    }
                }
                catch
                {
                    return null;
                }
            }

        }
    }
}
