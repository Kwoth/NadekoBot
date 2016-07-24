using NadekoBot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using NadekoBot.Modules.Music.Classes;
using NadekoBot.DataModels;
using System.Text.RegularExpressions;
using NadekoBot.Extensions;

namespace NadekoBot.Modules.Music
{
    class PlaylistCommands : DiscordCommand
    {
        public PlaylistCommands(DiscordModule module) : base(module)
        {
        }

        internal override void Init(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand(Prefix + "save")
                    .Description($"Saves a playlist under a certain name. Name must be no longer than 20 characters and mustn't contain dashes. | `{Prefix}save classical1`")
                    .Parameter("name", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var name = e.GetArg("name")?.Trim();

                        if (string.IsNullOrWhiteSpace(name) ||
                            name.Length > 20 ||
                            name.Contains("-"))
                            return;

                        MusicPlayer musicPlayer;
                        if (!MusicModule.MusicPlayers.TryGetValue(e.Server, out musicPlayer))
                            return;

                        //to avoid concurrency issues
                        var currentPlaylist = new List<Song>(musicPlayer.Playlist);
                        var curSong = musicPlayer.CurrentSong;
                        if (curSong != null)
                            currentPlaylist.Insert(0, curSong);

                        if (!currentPlaylist.Any())
                            return;
                        var playlist = new MusicPlaylist
                        {
                            CreatorId = (long)e.User.Id,
                            CreatorName = e.User.Name,
                            Name = name.ToLowerInvariant(),
                        };
                        DbHandler.Instance.Save(playlist);
                        try
                        {
                            var songInfos = currentPlaylist.Select(s => new DataModels.PlaylistSong
                            {
                                PlaylistId = playlist.Id.Value,
                                Provider = s.SongInfo.Provider,
                                ProviderType = (int)s.SongInfo.ProviderType,
                                Title = s.SongInfo.Title,
                                Uri = s.SongInfo.Uri,
                                Query = s.SongInfo.Query
                            }).ToList();
                       
                        DbHandler.Instance.SaveAll(songInfos);
                        await e.Channel.SendMessage($"🎵 `Saved playlist as {name}-{playlist.Id}`").ConfigureAwait(false);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    });

            cgb.CreateCommand(Prefix + "load")
                .Description($"Loads a playlist using the given id | `{Prefix}load classical-1`")
                .Parameter("name", ParameterType.Unparsed)
                .Do(async e =>
                {
                    var voiceCh = e.User.VoiceChannel;
                    var textCh = e.Channel;
                    if (voiceCh == null || voiceCh.Server != textCh.Server)
                    {
                        await textCh.SendMessage("💢 You need to be in a voice channel on this server.\n If you are already in a voice channel, try rejoining.").ConfigureAwait(false);
                        return;
                    }
                    var name = e.GetArg("name")?.Trim().ToLowerInvariant();

                    if (string.IsNullOrWhiteSpace(name))
                        return;
                    var idMatch = Regex.Match(name, @"([^-]{1,20}-)?(?<id>\d+)");
                    if (!idMatch.Success)
                    {
                        await e.Channel.SendMessage("could not find id in message.");
                        return;
                    }
                    int playlistNumber;
                    if (!int.TryParse(idMatch.Groups["id"].Value, out playlistNumber)) return;

                    var playlist = DbHandler.Instance.FindOne<MusicPlaylist>(
                        p => p.Id == playlistNumber);

                    if (playlist == null)
                    {
                        await e.Channel.SendMessage("Can't find playlist under that name.").ConfigureAwait(false);
                        return;
                    }

                    var psis = DbHandler.Instance.FindAll<PlaylistSong>(psi =>
                        psi.PlaylistId == playlist.Id);

                    await e.Channel.SendMessage($"`Attempting to load {psis.Count()} songs`").ConfigureAwait(false);
                    foreach (var si in psis)
                    {
                        try
                        {
                            await MusicModule.QueueSong(e.User, textCh, voiceCh, si.Query, true, (MusicType)si.ProviderType).ConfigureAwait(false);
                        }
                        catch (PlaylistFullException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed QueueSong in load playlist. {ex}");
                        }
                    }
                });

            cgb.CreateCommand(Prefix + "playlists")
                .Alias(Prefix + "pls")
                .Description($"Lists all playlists. Paginated. 20 per page. Default page is 0. |`{Prefix}pls 1`")
                .Parameter("num", ParameterType.Optional)
                .Do(async e =>
                {
                    int num = 0;
                    int.TryParse(e.GetArg("num"), out num);
                    if (num < 0)
                        return;
                    var result = DbHandler.Instance.GetPlaylistData(num);
                    if (result.Count == 0)
                        await e.Channel.SendMessage($"`No saved playlists found on page {num}`").ConfigureAwait(false);
                    else
                    {
                        //await  e.Channel.SendMessage(
                        //     $"```js\n--- List of saved playlists ---\n\n" +
                        //     string.Join("\n", result.Select(r => $"'{r.Name}-{r.Id}' by {r.Creator} ({r.SongCnt} songs)")) + $"\n\n        --- Page {num} ---```").ConfigureAwait(false);

                        string message = result.Aggregate(new StringBuilder($@"```xl
--- List of saved playlists ---
---------- Page {num} -----------
┏━━━━━┳━━━━━━━━━━━━━━━━━━━━━┳━━━━━━━━━━━━━━━━━━━━━━┳━━━━━━━┓
┃  Id |         Name        |        Creator       | Count |
"), (cur, cs) => cur.AppendLine($"┣━━━━━╋━━━━━━━━━━━━━━━━━━━━━╋━━━━━━━━━━━━━━━━━━━━━━╋━━━━━━━┫\n┃ {cs.Id, -3} ┃ {cs.Name.TrimTo(20, true),-20}┃{cs.Creator.TrimTo(21), -21} | {cs.SongCnt, -5} |")).ToString() + @"┗━━━━━┻━━━━━━━━━━━━━━━━━━━━━┻━━━━━━━━━━━━━━━━━━━━━━┻━━━━━━━┛```";
                        await e.Channel.SendMessage(message);

                    }
                });

            cgb.CreateCommand(Prefix + "port")
                .Description($"TEMPORARY COMMAND TO PORT OVER PLAYLISTS FROM OLDER VERSIONS.\n `{Prefix}port animu-5`")
                .Parameter("name", ParameterType.Unparsed)
                .Do(async e =>
                {
                    var name = e.GetArg("pl")?.Trim().ToLowerInvariant();

                    if (string.IsNullOrWhiteSpace(name))
                        return;
                    var idMatch = Regex.Match(name, @"([^-]{1,20}-)?(?<id>\d+)");
                    if (!idMatch.Success)
                    {
                        await e.Channel.SendMessage("could not find id in message");
                        return;
                    }
                    int id = int.Parse(idMatch.Groups["id"].Value);
                    var playlist = DbHandler.Instance.FindOne<MusicPlaylist>(x => x.Id == id);
                    if (playlist == null)
                    {
                        await e.Channel.SendMessage("Could not find playlist");
                        return;
                    }
                    var psis = DbHandler.Instance.FindAll<PlaylistSongInfo>(x => x.PlaylistId == id);
                    if (psis.Count == 0)
                    {
                        await e.Channel.SendMessage("Already ported likely");
                        return;
                    }
                    List<PlaylistSong> list = new List<PlaylistSong>();
                    foreach (var psi in psis)
                    {
                        var songInfo = DbHandler.Instance.FindOne<DataModels.SongInfo>(si => si.Id == psi.SongInfoId);
                        list.Add(new PlaylistSong()
                        {
                            PlaylistId = id,
                            Provider = songInfo.Provider,
                            ProviderType = songInfo.ProviderType,
                            Query = songInfo.Query,
                            Title = songInfo.Title,
                            Uri = songInfo.Uri
                        });
                    }
                    DbHandler.Instance.SaveAll(list);
                    await e.Channel.SendMessage("OK 👌");
                });

            cgb.CreateCommand(Prefix + "deleteplaylist")
                .Alias(Prefix + "delpls")
                .Description($"Deletes a saved playlist. Only if you made it or if you are the bot owner. | `{Prefix}delpls animu-5`")
                .Parameter("pl", ParameterType.Required)
                .Do(async e =>
                {
                    var name = e.GetArg("pl")?.Trim().ToLowerInvariant();

                    if (string.IsNullOrWhiteSpace(name))
                        return;
                    var idMatch = Regex.Match(name, @"([^-]{1,20}-)?(?<id>\d+)");
                    if (!idMatch.Success)
                    {
                        await e.Channel.SendMessage("could not find id in message D:");
                        return;
                    }
                    
                    var plnum = int.Parse(idMatch.Groups["id"].Value);
                    if (NadekoBot.IsOwner(e.User.Id))
                        DbHandler.Instance.Delete<MusicPlaylist>(plnum);
                    else
                        DbHandler.Instance.DeleteWhere<MusicPlaylist>(mp => mp.Id == plnum && (long)e.User.Id == mp.CreatorId);
                    await e.Channel.SendMessage("`Ok.` :ok:").ConfigureAwait(false);
                });

        }
    }
}
