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
            public async Task SteamUser(IUserMessage umsg, string user, [Remainder] string option = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(option) || string.IsNullOrWhiteSpace(user))
                    return;

                if (option.Equals("profile",StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using (var http = new HttpClient())
                        {
                            var steamId = await QuerySteamId(user);
                            var model = await GetPlayerSummaries(steamId);
                            var profileStatus = model.Response.Players[0].personastate;
                            var embed = new EmbedBuilder()
                                .WithColor(NadekoBot.OkColor)
                                .WithTitle("Steam Profile")
                                .WithUrl(model.Response.Players[0].profileurl)
                                .WithAuthor(x => x.WithName(model.Response.Players[0].personaname)
                                    .WithUrl(model.Response.Players[0].profileurl))
                                .WithThumbnail(x => x.WithUrl(model.Response.Players[0].avatar))
                                .WithDescription("Profile Status: " + Enum.Parse(typeof(SteamApiModel.profilestate), profileStatus.ToString()));
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
                    return;
                } else if (option.Contains("friends"))
                {
                    try
                    {
                        string[] options = option.Split(' ');
                        var friend_num = 0;
                        try
                        {
                            if (!string.IsNullOrEmpty(options[1]))
                                friend_num = Int32.Parse(options[1].ToString()) - 1;
                        } catch
                        {
                            friend_num = 0;
                        }
                        using (var http = new HttpClient())
                        {
                            var steamId = await QuerySteamId(user);
                            var model = await GetPlayerFriends(steamId);
                            var ply_steamId = model.FriendsList.Friends[friend_num].steamid;
                            var plyModel = await GetPlayerSummaries(ply_steamId);
                            var profileStatus = plyModel.Response.Players[0].personastate;
                            var embed = new EmbedBuilder()
                                .WithColor(NadekoBot.OkColor)
                                .WithTitle($"Steam Friend - Friend #{friend_num+1}")
                                .WithUrl(plyModel.Response.Players[0].profileurl)
                                .WithAuthor(x => x.WithName(plyModel.Response.Players[0].personaname)
                                    .WithUrl(plyModel.Response.Players[0].profileurl))
                                .WithThumbnail(x => x.WithUrl(plyModel.Response.Players[0].avatar))
                                .WithDescription("Profile Status: " + Enum.Parse(typeof(SteamApiModel.profilestate), profileStatus.ToString()));
                            await channel.EmbedAsync(embed.Build());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        var steamId = await QuerySteamId(user);
                        Console.WriteLine($"{user} : {steamId}");
                        var embed = new EmbedBuilder()
                            .WithColor(NadekoBot.ErrorColor)
                            .WithTitle("Steam Profile")
                            .WithDescription("`Found no user with that id`");
                        await channel.EmbedAsync(embed.Build());
                    }
                }
                return;
            }

            public async Task<SteamApiModel> GetPlayerFriends(string steamid64)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        var steamUrl = await http.GetStringAsync($"http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={NadekoBot.Credentials.SteamApiKey}&steamid={steamid64}&relationship=friend");
                        var model = JsonConvert.DeserializeObject<SteamApiModel>(steamUrl);
                        return model;
                    }
                }
                catch
                {
                    return null;
                }
            }

            public async Task<SteamApiModel> GetPlayerSummaries(string steamid64)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        var steamUrl = await http.GetStringAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={NadekoBot.Credentials.SteamApiKey}&steamids={steamid64}");
                        var model = JsonConvert.DeserializeObject<SteamApiModel>(steamUrl);
                        return model;
                    }
                }
                catch
                {
                    return null;
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
