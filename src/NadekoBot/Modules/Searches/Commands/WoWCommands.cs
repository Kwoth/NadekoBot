using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Modules.Searches.Models;
using NadekoBot.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Discord.API;
using System.Text;
using System.Threading.Tasks;
using NadekoBot.Extensions;
using System.Globalization;

namespace NadekoBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class WoWCommands
        {
            private Logger _log;
            private static List<WoWJoke> wowJoke = new List<WoWJoke>();

            public WoWCommands()
            {
                _log = LogManager.GetCurrentClassLogger();
                if (File.Exists("data/wowjokes.json"))
                {
                    wowJoke = JsonConvert.DeserializeObject<List<WoWJoke>>(File.ReadAllText("data/wowjokes.json"));
                }
                else
                    _log.Warn("data/wowjokes.json is missing. WOW Jokes are not loaded.");
            }

            private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
            {
                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                DateTime time = startDate.AddMilliseconds(unixTimeStamp);
                return time;
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task WoWStatus(IUserMessage umsg, string region, [Remainder] string realmNum = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(realmNum))
                {
                    await channel.SendMessageAsync("Please enter a region (e.g., us, eu, kr, tw, cn, sea), followed by a a realm number (0-x).").ConfigureAwait(false);
                }
                var realm = GetWoWRealmStatus(region, int.Parse(realmNum));
                var status = await realm.ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(status))
                    await channel.SendMessageAsync("Something went wrong ;(.").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync(status).ConfigureAwait(false);
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task WoWChar(IUserMessage umsg, string region, string realmName, [Remainder] string characterName = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(realmName) || string.IsNullOrWhiteSpace(characterName))
                {
                    await channel.SendMessageAsync("Please enter a region (e.g., us, eu, kr, tw, cn, sea), realm name (e.g., medivh), followed by a character name (e.g., Lisiano)").ConfigureAwait(false);
                }

                Embed status = await GetCharacter(region, realmName, characterName).ConfigureAwait(false);
                await channel.EmbedAsync(status).ConfigureAwait(false);
            }

            public static async Task<string> GetWoWRealmStatus(string region, int realmNum)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        var reqString = $"https://{region.ToLower()}.api.battle.net/wow/realm/status?apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                        var items = obj["realms"] as JArray;

                        string statusCheck = (bool)items[realmNum]["status"] ? "(✔)GOOD" : "(X)BAD";

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"`---Connected Realms---`");
                        foreach (var item in items[realmNum]["connected_realms"].ToArray())
                        {
                            sb.AppendLine($"〔:rosette: {item.ToString().ToUpper()}〕");
                        }
                        sb.Append($"`----------------------`");

                        var response = $@"```css
[☕ {region.ToUpper()}-WoW (Realm {realmNum}): {items[realmNum]["name"].ToString()} 〘Status: {statusCheck}〙]
Server Type: [{items[realmNum]["type"].ToString().ToUpper()}]
Battle Group: [{items[realmNum]["battlegroup"].ToString()}]
Population: [{items[realmNum]["population"].ToString().ToUpper()}]
Timezone: [{items[realmNum]["timezone"].ToString()}]
```
{sb.ToString()}";
                        return response;
                    }
                }
                catch
                {
                    return "Something went wrong ;(";
                }
            }

            public static string ToUpperFirstLetter(string source)
            {
                if (string.IsNullOrEmpty(source))
                    return string.Empty;
                char[] letters = source.ToCharArray();
                letters[0] = char.ToUpper(letters[0]);
                return new string(letters);
            }

            public static async Task<Embed> GetCharacter(string region, string realm, string characterName)
            {
                var characterString = "";
                var classesString = "";
                var racesString = "";
                var titleChar = ToUpperFirstLetter(characterName);
                var embed = new EmbedBuilder();
                long lastModified = 0;
                try
                {
                    using (var http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.Clear();
                        //Character Data
                        characterString = $"https://{region.ToLower()}.api.battle.net/wow/character/{realm.ToLower()}/{characterName.ToLower()}?locale=en_US&apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var characterObject = JObject.Parse(await http.GetStringAsync(characterString).ConfigureAwait(false));

                        //Classes Data
                        classesString = $"https://{region.ToLower()}.api.battle.net/wow/data/character/classes?locale=en_US&apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var classesObject = JObject.Parse(await http.GetStringAsync(classesString).ConfigureAwait(false));

                        //Races Data
                        racesString = $"https://{region.ToLower()}.api.battle.net/wow/data/character/races?locale=en_US&apikey={NadekoBot.Credentials.BlizzardApiKey}";
                        var racesObject = JObject.Parse(await http.GetStringAsync(racesString).ConfigureAwait(false));

                        var classesData = classesObject["classes"] as JArray;
                        var racesData = racesObject["races"] as JArray;

                        float charClassNum = (float)characterObject["class"];
                        float charRaceNum = (float)characterObject["race"];
                        string charThumbnail = $"https://render-api-{region.ToLower()}.worldofwarcraft.com/static-render/{region.ToLower()}/" + characterObject["thumbnail"].ToString();
                        lastModified = (long)characterObject["lastModified"];
                        DateTime time = UnixTimeStampToDateTime(lastModified);
                        string battleGroup = characterObject["battlegroup"].ToString();
                        string charClass = "";
                        string charClass_powerType = "";
                        string charRace = "";
                        string charRace_side = "";

                        foreach (var clss in classesData)
                        {
                            if ((int)clss["id"] == (int)charClassNum)
                            {
                                charClass = clss["name"].ToString();
                                charClass_powerType = clss["powerType"].ToString().ToUpper();
                            }
                        }
                        foreach (var clsx in racesData)
                        {
                            if ((int)clsx["id"] == (int)charClassNum)
                            {
                                charRace = clsx["name"].ToString();
                                charRace_side = clsx["side"].ToString().ToUpper();
                            }
                        }

                        string charGender = (bool)characterObject["gender"] ? "FEMALE" : "MALE";
                        float charLvl = (float)characterObject["level"];
                        float achievementPoints = (float)characterObject["achievementPoints"];
                        float totalHonorableKills = (float)characterObject["totalHonorableKills"];

                        var joke = wowJoke[new NadekoRandom().Next(0, wowJoke.Count())].ToString();
                        embed
                            .WithAuthor(ex => ex.WithName("Name: " + titleChar).WithUrl($"http://{region.ToLower()}.battle.net/wow/en/character/{realm.ToLower()}/{characterName}/statistic"))
                            .WithTitle("World of Warcraft - Character")
                            .WithDescription(joke)
                            //.WithThumbnail(tb => tb.WithUrl(charThumbnail))
                            .AddField(fb => fb.WithName("**🗺 __Realm__**").WithValue($"{ToUpperFirstLetter(realm)}").WithIsInline(true))
                            .AddField(fb => fb.WithName("**💁 __Class__**").WithValue($"{charClass}").WithIsInline(true))
                            .AddField(fb => fb.WithName("**📄 __Race__**").WithValue($"{charRace} / {charRace_side}").WithIsInline(true))
                            .AddField(fb => fb.WithName("**🆙 __Level__**").WithValue($"{(int)charLvl}").WithIsInline(true))
                            .AddField(fb => fb.WithName("**🚻 __Gender__**").WithValue($"{charGender}").WithIsInline(true))
                            .AddField(fb => fb.WithName("**💯 __Achievement Points__**").WithValue($"{achievementPoints}").WithIsInline(true))
                            .AddField(fb => fb.WithName("**🆗 __Honorable Kills__**").WithValue($"{totalHonorableKills}").WithIsInline(false))
                            .WithFooter(foot => foot.WithText($"**Last Modified (24hr)**: {time}"))
                            .WithColor(NadekoBot.OkColor);
                        return embed.Build();
                    }
                }
                catch { return null; }
            }
        }
    }
}