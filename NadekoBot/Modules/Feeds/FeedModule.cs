using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Modules;
using NadekoBot.Extensions;
using Discord.Commands;
using Discord;
using System.Net.Http;
using System.Xml.Linq;
using Discord.Net;
using System.Net;
using NadekoBot.Classes;
using NadekoBot.Modules.Permissions.Classes;
using NadekoBot.DataModels;

namespace NadekoBot.Modules.Feeds
{
    class FeedModule : DiscordModule
    {

        public FeedModule() { }
        public override string Prefix { get; } = NadekoBot.Config.CommandPrefixes.Feeds;
        private bool isRunning;



        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands(Prefix, cgb =>
            {
                cgb.AddCheck(PermissionChecker.Instance);

                commands.ForEach(com => com.Init(cgb));

                cgb.CreateCommand("list")
                .Description("Lists all feeds for this channel")
                .Do(async e =>
                {
                    var list = DbHandler.Instance.GetAllRows<Feed>();

                    var channelList = list.Where(x => x.ChannelId == (long)e.Channel.Id);
                    if (channelList.Any())
                    {
                        var str = "Your linked feeds are:\n";
                        foreach (var item in channelList.OrderBy(x => x.lastUpdated))
                        {
                            str += $"{item.Name} from <{item.Link}>\n";
                        }
                        await e.Channel.SendMessage(str);
                        return;
                    }
                    await e.Channel.SendMessage("No feeds for this channel");
                });

                cgb.CreateCommand("add")
                .Description("Add rss feed to list of feeds to check for this channel")
                .Parameter("url", ParameterType.Required)
                .Do(async e =>
                {
                    var url = e.GetArg("url");
                    Uri uriResult;
                    bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (!result)
                    {
                        await e.Channel.SendMessage("link must be valid url");
                        return;
                    }

                    //Check if already existing
                    var list = DbHandler.Instance.GetAllRows<Feed>().Where(x => x.ChannelId == (long)e.Channel.Id);
                    if (list.Where(x => x.Link == url).Any())
                    {
                        await e.Channel.SendMessage($"Channel already has feed from {url} set up");
                        return;
                    }
                    string feedName = await GetFeedName(url);
                    if (feedName != null)
                    {
                        DbHandler.Instance.InsertData(new Feed()
                        {
                            Name = feedName,
                            ChannelId = (long)e.Channel.Id,
                            Link = url,
                            lastUpdated = DateTimeOffset.UtcNow
                        });
                        await e.Channel.SendMessage($"Successfully added feed {feedName} to channel");
                    }
                    else
                    {
                        await e.Channel.SendMessage("Unkown feed type");
                    }


                });

                cgb.CreateCommand("remove")
                .Description($"remove the feed with the corresponding url \n**Usage**: {Prefix} remove <feed link>")
                .Parameter("url", ParameterType.Required)
                .Do(async e =>
                {
                    var url = e.GetArg("url");
                    var list = DbHandler.Instance.GetAllRows<Feed>().Where(x => x.ChannelId == (long)e.Channel.Id);
                    if (list.Where(x => x.Link == url).Any())
                    {
                        var toRemove = list.First();
                        int id = toRemove.Id ?? 0;
                        DbHandler.Instance.Delete<Feed>(id);

                        await e.Channel.SendMessage($"Feed from {url} successfully removed");
                    }
                    else
                    {
                        await e.Channel.SendMessage($"Channel does not have feed from {url} set up");
                        return;
                    }

                });

                NadekoBot.Client.Ready += (s, e) =>
                {
                    if (!isRunning)
                    {
                        Task.Run(Run);
                        isRunning = true;
                    }
                };
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="feedName"></param>
        /// <returns></returns>
        private async Task<string> GetFeedName(string url)
        {

            var content = await GetContent(HttpMethod.Get, url);
            var doc = XDocument.Load(await content.ReadAsStreamAsync());
            var rssNode = doc.Element("rss");
            var atomNode = doc.Element("{http://www.w3.org/2005/Atom}feed");
            if (rssNode != null)
            {
                string title = rssNode.Element("channel").Element("title")?.Value;
                string description = rssNode.Element("channel").Element("description")?.Value;
                var toReturn = $"**{title}**";
                if (description != null)
                {
                    toReturn += $"\n-*{description}*";
                }
                return toReturn;
            }
            else if (atomNode != null)
            {
                var channel = atomNode.Element("feed");
                var title = channel.Element("title")?.Value;
                var subtitle = channel.Element("subtitle")?.Value;
                var toReturn = $"**{title}**";
                if (subtitle != null)
                {
                    toReturn += $"\n-*{subtitle}*";
                }
                return toReturn;
            }
            else return null;
        }



        public async Task Run()
        {
            var cancelToken = NadekoBot.Client.CancelToken;
            try
            {
                while (!NadekoBot.Client.CancelToken.IsCancellationRequested)
                {
                    //Console.WriteLine("Updating feeds");
                    var feedsList = DbHandler.Instance.GetAllRows<Feed>();
                    if (feedsList.Any())
                    {
                        foreach (var feed in feedsList)
                        {
                            try
                            {

                                var channel = NadekoBot.Client.GetChannel((ulong)feed.ChannelId);
                                if (channel != null && channel.Server.CurrentUser.GetPermissions(channel).SendMessages)
                                {
                                    var content = await GetContent(HttpMethod.Get, feed.Link);
                                    var doc = XDocument.Load(await content.ReadAsStreamAsync());
                                    var rssNode = doc.Element("rss");
                                    var atomNode = doc.Element("{http://www.w3.org/2005/Atom}feed");

                                    IEnumerable<Article> articles;
                                    if (rssNode != null)
                                    {
                                        articles = rssNode
                                            .Element("channel")
                                            .Elements("item")
                                            .Select(x => new Article
                                            {
                                                Title = x.Element("title")?.Value,
                                                Link = x.Element("link")?.Value,
                                                PublishedAt = DateTimeOffset.Parse(x.Element("pubDate").Value)
                                            });
                                    }
                                    else if (atomNode != null)
                                    {
                                        articles = atomNode
                                            .Elements("{http://www.w3.org/2005/Atom}entry")
                                            .Select(x => new Article
                                            {
                                                Title = x.Element("{http://www.w3.org/2005/Atom}title")?.Value,
                                                Link = x.Element("{http://www.w3.org/2005/Atom}link")?.Attribute("href")?.Value,
                                                Description = x.Element("{http://www.w3.org/2005/atom}description")?.Value,
                                                PublishedAt = DateTimeOffset.Parse(x.Element("{http://www.w3.org/2005/Atom}published").Value)
                                            });
                                    }
                                    else
                                        throw new InvalidOperationException("Unknown feed type.");

                                    articles = articles
                                        .Where(x => x.PublishedAt > feed.lastUpdated)
                                        .OrderBy(x => x.PublishedAt)
                                        .ToArray();

                                    foreach (var article in articles)
                                    {
                                        NadekoBot.Client.Log.Info("Feed", $"New article: {article.Title}");
                                        if (article.Link != null)
                                        {
                                            try
                                            {
                                                string str = $"Feed update from {Format.Escape(feed.Name)}:\n" +
                                                    $"Title: {Format.Bold(article.Title)}\n" +
                                                    $"link: {Format.Escape(article.Link)}\n" +
                                                    $"Content: {article.Description}";
                                                await channel.SendMessage(str);
                                            }
                                            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.Forbidden) { }
                                        }

                                        if (article.PublishedAt > feed.lastUpdated)
                                        {
                                            feed.lastUpdated = article.PublishedAt;
                                            DbHandler.Instance.Save(feed);
                                            //await _settings.Save(settings.Key, settings.Value);
                                        }
                                        await Task.Delay(2000);
                                    };
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    await Task.Delay(1000 * 300, cancelToken); //Wait 5 minutes between updates

                }
            }
            catch (Exception ex) when (!(ex is TaskCanceledException))
            {
                NadekoBot.Client.Log.Error("Feed", ex);
            }
        }

        static HttpClient httpClient = null;
        private static void InitClient()
        {
            httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                UseCookies = false,
                //PreAuthenticate = false
            });
            httpClient.DefaultRequestHeaders.Add("accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");
            //httpClient.DefaultRequestHeaders.Add("user-agent", NadekoBot.Client.Config.UserAgent);
        }

        private async Task<HttpContent> GetContent(HttpMethod method, string url)
        {
            if (httpClient == null)
            {
                InitClient();
            }
            HttpRequestMessage msg = new HttpRequestMessage(method, url);
            //RogueException's version is so unclear in its function here
            //string json = JsonConvert.SerializeObject()
            //    msg.Content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await httpClient.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode)
                throw new HttpException(response.StatusCode);
            return response.Content;
        }
    }

    //public class AllFeed
    //{
    //    public ulong ChannelId { get; set; }
    //    public ulong ServerId { get; set; }
    //    public DateTimeOffset lastUpdate { get; set; } = DateTimeOffset.UtcNow;
    //    public string url { get; set; }

    //}

    public class Article
    {
        public string Title;
        public string Link;
        public DateTimeOffset PublishedAt;
        public string Description;
    }

    //public class Setting
    //{
    //    public class Feed
    //    {
    //        public ulong ChannelId { get; set; }
    //        public DateTimeOffset lastUpdate { get; set; } = DateTimeOffset.UtcNow;
    //    }

    //    public ConcurrentDictionary<string, Feed> Feeds = new ConcurrentDictionary<string, Feed>();
    //    public bool AddFeed(string url, ulong channelId)
    //        => Feeds.TryAdd(url, new Feed { ChannelId = channelId });
    //    public bool RemoveFeed(string url)
    //    {
    //        Feed ignored;
    //        return Feeds.TryRemove(url, out ignored);
    //    }
    //}
}
