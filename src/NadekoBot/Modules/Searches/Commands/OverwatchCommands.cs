﻿using Discord;
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
                        
                        var rankimg = $"{model.Data.Competitive.rank_img}";
                        var rank = $"{model.Data.Competitive.rank}";
                        if (string.IsNullOrWhiteSpace(rank))
                        {
                            var embed = new EmbedBuilder()
                                .WithAuthor(eau => eau.WithName($"{model.Data.username}")
                                .WithUrl($"https://www.overbuff.com/players/pc/{battletag}")
                                .WithIconUrl($"{model.Data.avatar}"))
                                .WithThumbnail(th => th.WithUrl("https://cdn.discordapp.com/attachments/155726317222887425/255653487512256512/YZ4w2ey.png"))
                                .AddField(fb => fb.WithName("**Level**").WithValue($"{model.Data.level}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Quick Wins**").WithValue($"{model.Data.Games.Quick.wins}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Current Competitive Wins**").WithValue($"{model.Data.Games.Competitive.wins}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Current Competitive Loses**").WithValue($"{model.Data.Games.Competitive.lost}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Current Competitive Played**").WithValue($"{model.Data.Games.Competitive.played}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Competitive Rank**").WithValue("0").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Competitive Playtime**").WithValue($"{model.Data.Playtime.competitive}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Quick Playtime**").WithValue($"{model.Data.Playtime.quick}").WithIsInline(true))
                                .WithColor(NadekoBot.OkColor);
                            await channel.EmbedAsync(embed.Build()).ConfigureAwait(false);
                        }
                        else
                        {
                            var embed = new EmbedBuilder()
                                .WithAuthor(eau => eau.WithName($"{model.Data.username}")
                                .WithUrl($"https://www.overbuff.com/players/pc/{battletag}")
                                .WithIconUrl($"{model.Data.avatar}"))
                                .WithThumbnail(th => th.WithUrl(rankimg))
                                .AddField(fb => fb.WithName("**Level**").WithValue($"{model.Data.level}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Quick Wins**").WithValue($"{model.Data.Games.Quick.wins}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Current Competitive Wins**").WithValue($"{model.Data.Games.Competitive.wins}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Current Competitive Loses**").WithValue($"{model.Data.Games.Competitive.lost}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Current Competitive Played**").WithValue($"{model.Data.Games.Competitive.played}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Competitive Rank**").WithValue(rank).WithIsInline(true))
                                .AddField(fb => fb.WithName("**Competitive Playtime**").WithValue($"{model.Data.Playtime.competitive}").WithIsInline(true))
                                .AddField(fb => fb.WithName("**Quick Playtime**").WithValue($"{model.Data.Playtime.quick}").WithIsInline(true))
                                .WithColor(NadekoBot.OkColor);
                            await channel.EmbedAsync(embed.Build()).ConfigureAwait(false);
                            return;
                        }
                    }
                }
                catch
                {
                    var embed = new EmbedBuilder()
                        .AddField(fb => fb.WithName("**Found no user!**").WithValue("Please check the **Region** and **BattleTag** before trying again.").WithIsInline(true))
                        .WithColor(NadekoBot.ErrorColor);
                    await channel.EmbedAsync(embed.Build()).ConfigureAwait(false);
                }
            }
        }
       }
    }
