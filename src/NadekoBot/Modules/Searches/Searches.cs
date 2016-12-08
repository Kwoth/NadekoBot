﻿using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using NadekoBot.Services;
using System.Threading.Tasks;
using NadekoBot.Attributes;
using System.Text.RegularExpressions;
using System.Net;
using NadekoBot.Modules.Searches.Models;
using System.Collections.Generic;
using ImageSharp;
using NadekoBot.Extensions;
using System.IO;
using NadekoBot.Modules.Searches.Commands.OMDB;

namespace NadekoBot.Modules.Searches
{
    [NadekoModule("Searches", "~")]
    public partial class Searches : DiscordModule
    {
        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Weather(IUserMessage umsg, string city, string country)
        {
            var channel = (ITextChannel)umsg.Channel;
            city = city.Replace(" ", "");
            country = city.Replace(" ", "");
            string response;
            using (var http = new HttpClient())
                response = await http.GetStringAsync($"http://api.ninetales.us/nadekobot/weather/?city={city}&country={country}").ConfigureAwait(false);

            var obj = JsonConvert.DeserializeObject<WeatherApiModel>(response);
            var model = obj.weather;
            var embed = new EmbedBuilder()
                .AddField(fb => fb.WithName("🌍 **Location**").WithValue($"{model.target}").WithIsInline(true))
                .AddField(fb => fb.WithName("📏 **Lat,Long**").WithValue($"{model.latitude}, {model.longitude}").WithIsInline(true))
                .AddField(fb => fb.WithName("☁ **Condition**").WithValue($"{model.condition}").WithIsInline(true))
                .AddField(fb => fb.WithName("😓 **Humidity**").WithValue($"{model.humidity}%").WithIsInline(true))
                .AddField(fb => fb.WithName("💨 **Wind Speed**").WithValue($"{model.windspeedk}km/h ({model.windspeedm}mph)").WithIsInline(true))
                .AddField(fb => fb.WithName("🌡 **Temperature**").WithValue($"{model.centigrade}°C ({model.fahrenheit}°F)").WithIsInline(true))
                .AddField(fb => fb.WithName("🔆 **Feels like**").WithValue($"{model.feelscentigrade}°C ({model.feelsfahrenheit}°F)").WithIsInline(true))
                .AddField(fb => fb.WithName("🌄 **Sunrise**").WithValue($"{model.sunrise}").WithIsInline(true))
                .AddField(fb => fb.WithName("🌇 **Sunset**").WithValue($"{model.sunset}").WithIsInline(true))
                .WithColor(NadekoBot.OkColor);
            await channel.EmbedAsync(embed.Build()).ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Youtube(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            if (!(await ValidateQuery(channel, query).ConfigureAwait(false))) return;
            var result = (await NadekoBot.Google.GetVideosByKeywordsAsync(query, 1)).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(result))
            {
                await channel.SendMessageAsync("No results found for that query.");
                return;
            }
            await channel.SendMessageAsync(result).ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Imdb(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (!(await ValidateQuery(channel, query).ConfigureAwait(false))) return;
            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);

            var movie = await OmdbProvider.FindMovie(query);
            if (movie == null)
            {
                await channel.SendMessageAsync("Failed to find that movie.").ConfigureAwait(false);
                return;
            }
            await channel.SendMessageAsync(movie.ToString()).ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task RandomCat(IUserMessage umsg)
        {
            var channel = (ITextChannel)umsg.Channel;
            using (var http = new HttpClient())
            {
                await channel.SendMessageAsync(JObject.Parse(
                                await http.GetStringAsync("http://www.random.cat/meow").ConfigureAwait(false))["file"].ToString())
                                    .ConfigureAwait(false);
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task RandomDog(IUserMessage umsg)
        {
            var channel = (ITextChannel)umsg.Channel;
            using (var http = new HttpClient())
            {
                await channel.SendMessageAsync("http://random.dog/" + await http.GetStringAsync("http://random.dog/woof").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task I(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(query))
                return;
            try
            {
                using (var http = new HttpClient())
                {
                    var reqString = $"https://www.googleapis.com/customsearch/v1?q={Uri.EscapeDataString(query)}&cx=018084019232060951019%3Ahs5piey28-e&num=1&searchType=image&fields=items%2Flink&key={NadekoBot.Credentials.GoogleApiKey}";
                    var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                    await channel.SendMessageAsync(obj["items"][0]["link"].ToString()).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.Message.Contains("403 (Forbidden)"))
                {
                    await channel.SendMessageAsync("Daily limit reached!");
                }
                else
                {
                    await channel.SendMessageAsync("Something went wrong.");
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Ir(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(query))
                return;
            try
            {
                using (var http = new HttpClient())
                {
                    var rng = new NadekoRandom();
                    var reqString = $"https://www.googleapis.com/customsearch/v1?q={Uri.EscapeDataString(query)}&cx=018084019232060951019%3Ahs5piey28-e&num=1&searchType=image&start={ rng.Next(1, 50) }&fields=items%2Flink&key={NadekoBot.Credentials.GoogleApiKey}";
                    var obj = JObject.Parse(await http.GetStringAsync(reqString).ConfigureAwait(false));
                    var items = obj["items"] as JArray;
                    await channel.SendMessageAsync(items[0]["link"].ToString()).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.Message.Contains("403 (Forbidden)"))
                {
                    await channel.SendMessageAsync("Daily limit reached!");
                }
                else
                {
                    await channel.SendMessageAsync("Something went wrong.");
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Lmgtfy(IUserMessage umsg, [Remainder] string ffs = null)
        {
            var channel = (ITextChannel)umsg.Channel;


            if (string.IsNullOrWhiteSpace(ffs))
                return;

            await channel.SendMessageAsync(await NadekoBot.Google.ShortenUrl($"<http://lmgtfy.com/?q={ Uri.EscapeUriString(ffs) }>"))
                           .ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Shorten(IUserMessage msg, [Remainder] string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
                return;

            await msg.Channel.SendMessageAsync(await NadekoBot.Google.ShortenUrl(arg).ConfigureAwait(false));
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Google(IUserMessage umsg, [Remainder] string terms = null)
        {
            var channel = (ITextChannel)umsg.Channel;


            terms = terms?.Trim();
            if (string.IsNullOrWhiteSpace(terms))
                return;
            await channel.SendMessageAsync($"https://google.com/search?q={ WebUtility.UrlEncode(terms).Replace(' ', '+') }")
                           .ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task MagicTheGathering(IUserMessage umsg, [Remainder] string name = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            var arg = name;
            if (string.IsNullOrWhiteSpace(arg))
            {
                await channel.SendMessageAsync("💢 `Please enter a card name to search for.`").ConfigureAwait(false);
                return;
            }

            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            string response = "";
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                response = await http.GetStringAsync($"https://api.deckbrew.com/mtg/cards?name={Uri.EscapeUriString(arg)}")
                                        .ConfigureAwait(false);
                try
                {
                    var items = JArray.Parse(response).Shuffle().ToList();
                    if (items == null)
                        throw new KeyNotFoundException("Cannot find a card by that name");
                    var msg = $@"```css
[☕ Magic The Gathering]: {items[0]["name"].ToString()}
[Store URL]: {await NadekoBot.Google.ShortenUrl(items[0]["store_url"].ToString())}
[Cost]: {items[0]["cost"].ToString()}
[Description]: {items[0]["text"].ToString()}
```
{items[0]["editions"][0]["image_url"].ToString()}";
                    await channel.SendMessageAsync(msg).ConfigureAwait(false);
                }
                catch
                {
                    await channel.SendMessageAsync($"💢 Error could not find the card {arg}").ConfigureAwait(false);
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Hearthstone(IUserMessage umsg, [Remainder] string name = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            var arg = name;
            if (string.IsNullOrWhiteSpace(arg))
            {
                await channel.SendMessageAsync("💢 `Please enter a card name to search for.`").ConfigureAwait(false);
                return;
            }

            if (string.IsNullOrWhiteSpace(NadekoBot.Credentials.MashapeKey))
            {
                await channel.SendMessageAsync("💢 `Bot owner didn't specify MashapeApiKey. You can't use this functionality.`").ConfigureAwait(false);
                return;
            }

            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            string response = "";
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("X-Mashape-Key", NadekoBot.Credentials.MashapeKey);
                response = await http.GetStringAsync($"https://omgvamp-hearthstone-v1.p.mashape.com/cards/search/{Uri.EscapeUriString(arg)}")
                                        .ConfigureAwait(false);
                try
                {
                    var items = JArray.Parse(response).Shuffle().ToList();
                    var images = new List<Image>();
                    if (items == null)
                        throw new KeyNotFoundException("Cannot find a card by that name");
                    foreach (var item in items.Where(item => item.HasValues && item["img"] != null).Take(4))
                    {
                        using (var sr = await http.GetStreamAsync(item["img"].ToString()))
                        {
                            var imgStream = new MemoryStream();
                            await sr.CopyToAsync(imgStream);
                            imgStream.Position = 0;
                            images.Add(new Image(imgStream));
                        }
                    }
                    string msg = null;
                    if (items.Count > 4)
                    {
                        msg = "⚠ Found over 4 images. Showing random 4.";
                    }
                    var ms = new MemoryStream();
                    images.Merge().SaveAsPng(ms);
                    ms.Position = 0;
                    await channel.SendFileAsync(ms, arg + ".png", msg).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await channel.SendMessageAsync($"💢 Error {ex.Message}").ConfigureAwait(false);
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Yodify(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(NadekoBot.Credentials.MashapeKey))
            {
                await channel.SendMessageAsync("💢 `Bot owner didn't specify MashapeApiKey. You can't use this functionality.`").ConfigureAwait(false);
                return;
            }

            var arg = query;
            if (string.IsNullOrWhiteSpace(arg))
            {
                await channel.SendMessageAsync("💢 `Please enter a sentence.`").ConfigureAwait(false);
                return;
            }
            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("X-Mashape-Key", NadekoBot.Credentials.MashapeKey);
                http.DefaultRequestHeaders.Add("Accept", "text/plain");
                var res = await http.GetStringAsync($"https://yoda.p.mashape.com/yoda?sentence={Uri.EscapeUriString(arg)}").ConfigureAwait(false);
                try
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("Young Padawan")
                        .WithUrl("http://www.yodaspeak.co.uk/")
                        .WithAuthor(au => au.WithName("Yoda").WithIconUrl("http://www.yodaspeak.co.uk/yoda-small1.gif"))
                        .WithDescription("Seek advice, you must!")
                        .WithThumbnail(th => th.WithUrl("http://i.imgur.com/62Uh4u6.jpg"))
                        .AddField(fb => fb.WithName($"🌍 **{umsg.Author.Username}**").WithValue($"{res.ToString()}").WithIsInline(false))
                        .WithColor(NadekoBot.OkColor);
                    await channel.EmbedAsync(embed.Build()).ConfigureAwait(false);
                }
                catch
                {
                    await channel.SendMessageAsync("💢 Failed to yodify your sentence.").ConfigureAwait(false);
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task UrbanDict(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(NadekoBot.Credentials.MashapeKey))
            {
                await channel.SendMessageAsync("💢 `Bot owner didn't specify MashapeApiKey. You can't use this functionality.`").ConfigureAwait(false);
                return;
            }

            var arg = query;
            if (string.IsNullOrWhiteSpace(arg))
            {
                await channel.SendMessageAsync("💢 `Please enter a search term.`").ConfigureAwait(false);
                return;
            }
            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("Accept", "application/json");
                var res = await http.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={Uri.EscapeUriString(arg)}").ConfigureAwait(false);
                try
                {
                    var items = JObject.Parse(res);
                    var sb = new StringBuilder();
                    sb.AppendLine($"`Term:` {items["list"][0]["word"].ToString()}");
                    sb.AppendLine($"`Definition:` {items["list"][0]["definition"].ToString()}");
                    sb.Append($"`Link:` <{await NadekoBot.Google.ShortenUrl(items["list"][0]["permalink"].ToString()).ConfigureAwait(false)}>");
                    await channel.SendMessageAsync(sb.ToString());
                }
                catch
                {
                    await channel.SendMessageAsync("💢 Failed finding a definition for that term.").ConfigureAwait(false);
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Hashtag(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            var arg = query;
            if (string.IsNullOrWhiteSpace(arg))
            {
                await channel.SendMessageAsync("💢 `Please enter a search term.`").ConfigureAwait(false);
                return;
            }
            if (string.IsNullOrWhiteSpace(NadekoBot.Credentials.MashapeKey))
            {
                await channel.SendMessageAsync("💢 `Bot owner didn't specify MashapeApiKey. You can't use this functionality.`").ConfigureAwait(false);
                return;
            }

            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            string res = "";
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("X-Mashape-Key", NadekoBot.Credentials.MashapeKey);
                res = await http.GetStringAsync($"https://tagdef.p.mashape.com/one.{Uri.EscapeUriString(arg)}.json").ConfigureAwait(false);
            }

            try
            {
                var items = JObject.Parse(res);
                var str = $@"`Hashtag:` {items["defs"]["def"]["hashtag"].ToString()}
`Definition:` {items["defs"]["def"]["text"].ToString()}
`Link:` <{await NadekoBot.Google.ShortenUrl(items["defs"]["def"]["uri"].ToString()).ConfigureAwait(false)}>";
                await channel.SendMessageAsync(str);
            }
            catch
            {
                await channel.SendMessageAsync("💢 Failed finding a definition for that tag.").ConfigureAwait(false);
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Catfact(IUserMessage umsg)
        {
            var channel = (ITextChannel)umsg.Channel;
            using (var http = new HttpClient())
            {
                var response = await http.GetStringAsync("http://catfacts-api.appspot.com/api/facts").ConfigureAwait(false);
                if (response == null)
                    return;
                await channel.SendMessageAsync($"🐈 `{JObject.Parse(response)["facts"][0].ToString()}`").ConfigureAwait(false);
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Revav(IUserMessage umsg, [Remainder] IUser usr = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (usr == null)
                usr = umsg.Author;
            await channel.SendMessageAsync($"https://images.google.com/searchbyimage?image_url={usr.AvatarUrl}").ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Revimg(IUserMessage umsg, [Remainder] string imageLink = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            imageLink = imageLink?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(imageLink))
                return;
            await channel.SendMessageAsync($"https://images.google.com/searchbyimage?image_url={imageLink}").ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Safebooru(IUserMessage umsg, [Remainder] string tag = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            tag = tag?.Trim() ?? "";
            var link = await GetSafebooruImageLink(tag).ConfigureAwait(false);
            if (link == null)
                await channel.SendMessageAsync("`No results.`");
            else
                await channel.SendMessageAsync(link).ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Wiki(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            query = query?.Trim();
            if (string.IsNullOrWhiteSpace(query))
                return;
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync("https://en.wikipedia.org//w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles=" + Uri.EscapeDataString(query));
                var data = JsonConvert.DeserializeObject<WikipediaApiModel>(result);
                if (data.Query.Pages[0].Missing)
                    await channel.SendMessageAsync("`That page could not be found.`");
                else
                    await channel.SendMessageAsync(data.Query.Pages[0].FullUrl);
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Color(IUserMessage umsg, [Remainder] string color = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            color = color?.Trim().Replace("#", "");
            if (string.IsNullOrWhiteSpace((string)color))
                return;
            var img = new Image(50, 50);

            var red = Convert.ToInt32(color.Substring(0, 2), 16);
            var green = Convert.ToInt32(color.Substring(2, 2), 16);
            var blue = Convert.ToInt32(color.Substring(4, 2), 16);

            img.BackgroundColor(new ImageSharp.Color(color));

            await channel.SendFileAsync(img.ToStream(), $"{color}.png");
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Videocall(IUserMessage umsg, [Remainder] string arg = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            try
            {
                var allUsrs = umsg.MentionedUsers.Append(umsg.Author);
                var allUsrsArray = allUsrs.ToArray();
                var str = allUsrsArray.Aggregate("http://appear.in/", (current, usr) => current + Uri.EscapeUriString(usr.Username[0].ToString()));
                str += new NadekoRandom().Next();
                foreach (var usr in allUsrsArray)
                {
                    await (await (usr as IGuildUser).CreateDMChannelAsync()).SendMessageAsync(str).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Avatar(IUserMessage umsg, [Remainder] string mention = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            var usr = umsg.MentionedUsers.FirstOrDefault();
            if (usr == null)
            {
                await channel.SendMessageAsync("Invalid user specified.").ConfigureAwait(false);
                return;
            }
            await channel.SendMessageAsync(await NadekoBot.Google.ShortenUrl(usr.AvatarUrl).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public static async Task<string> GetSafebooruImageLink(string tag)
        {
            var rng = new NadekoRandom();
            var url =
            $"http://safebooru.org/index.php?page=dapi&s=post&q=index&limit=100&tags={tag.Replace(" ", "_")}";
            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(url).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "file_url=\"(?<url>.*?)\"");
                if (matches.Count == 0)
                    return null;
                var match = matches[rng.Next(0, matches.Count)];
                return "http:" + matches[rng.Next(0, matches.Count)].Groups["url"].Value;
            }
        }
        
        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Wikia(IUserMessage umsg, string target, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            if (string.IsNullOrWhiteSpace(target) || string.IsNullOrWhiteSpace(query))
            {
                await channel.SendMessageAsync("💢 Please enter a target wikia, followed by search query.").ConfigureAwait(false);
                return;
            }
            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                try
                {
                    var res = await http.GetStringAsync($"http://www.{Uri.EscapeUriString(target)}.wikia.com/api/v1/Search/List?query={Uri.EscapeUriString(query)}&limit=25&minArticleQuality=10&batch=1&namespaces=0%2C14").ConfigureAwait(false);
                    var items = JObject.Parse(res);
                    var found = items["items"][0];
                    var response = $@"`Title:` {found["title"].ToString()}
`Quality:` {found["quality"]}
`URL:` {await NadekoBot.Google.ShortenUrl(found["url"].ToString()).ConfigureAwait(false)}";
                    await channel.SendMessageAsync(response);
                }
                catch
                {
                    await channel.SendMessageAsync($"💢 Failed finding `{query}`.").ConfigureAwait(false);
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task MCPing(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            var arg = query;
            if (string.IsNullOrWhiteSpace(arg))
            {
                await channel.SendMessageAsync("💢 Please enter a `ip:port`.").ConfigureAwait(false);
                return;
            }
            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                string ip = arg.Split(':')[0];
                string port = arg.Split(':')[1];
                var res = await http.GetStringAsync($"https://api.minetools.eu/ping/{Uri.EscapeUriString(ip)}/{Uri.EscapeUriString(port)}").ConfigureAwait(false);
                try
                {
                    var items = JObject.Parse(res);
                    var sb = new StringBuilder();
                    int ping = (int)Math.Ceiling(Double.Parse(items["latency"].ToString()));
                    sb.AppendLine($"`Server:` {arg}");
                    sb.AppendLine($"`Version:` {items["version"]["name"].ToString()} / Protocol {items["version"]["protocol"].ToString()}");
                    sb.AppendLine($"`Description:` {items["description"].ToString()}");
                    sb.AppendLine($"`Online Players:` {items["players"]["online"].ToString()}/{items["players"]["max"].ToString()}");
                    sb.Append($"`Latency:` {ping}");
                    await channel.SendMessageAsync(sb.ToString());
                }
                catch
                {
                    await channel.SendMessageAsync($"💢 Failed finding `{arg}`.").ConfigureAwait(false);
                }
            }
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task MCQ(IUserMessage umsg, [Remainder] string query = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            var arg = query;
            if (string.IsNullOrWhiteSpace(arg))
            {
                await channel.SendMessageAsync("💢 Please enter a `ip:port`.").ConfigureAwait(false);
                return;
            }
            await umsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Clear();
                try
                {
                    string ip = arg.Split(':')[0];
                    string port = arg.Split(':')[1];
                    var res = await http.GetStringAsync($"https://api.minetools.eu/query/{Uri.EscapeUriString(ip)}/{Uri.EscapeUriString(port)}").ConfigureAwait(false);
                    var items = JObject.Parse(res);
                    var sb = new StringBuilder();
                    sb.AppendLine($"`Server:` {arg.ToString()} 〘Status: {items["status"]}〙");
                    sb.AppendLine($"`Player List (First 5):`");
                    foreach (var item in items["Playerlist"].Take(5))
                    {
                        sb.AppendLine($"〔:rosette: {item}〕");
                    }
                    sb.AppendLine($"`Online Players:` {items["Players"]} / {items["MaxPlayers"]}");
                    sb.AppendLine($"`Plugins:` {items["Plugins"]}");
                    sb.Append($"`Version:` {items["Version"]}");
                    await channel.SendMessageAsync(sb.ToString());
                }
                catch
                {
                    await channel.SendMessageAsync($"💢 Failed finding server `{arg}`.").ConfigureAwait(false);
                }
            }
        }

        public static async Task<bool> ValidateQuery(ITextChannel ch, string query)
        {
            if (!string.IsNullOrEmpty(query.Trim())) return true;
            await ch.SendMessageAsync("Please specify search parameters.").ConfigureAwait(false);
            return false;
        }
    }
}
