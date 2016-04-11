using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Collections.Concurrent;
using NadekoBot.Modules;
using NadekoBot.Classes.CardsAgainstHumanity;

namespace NadekoBot.Commands
{
    internal class CardsAgainstHumanity : DiscordCommand
    {
        public static ConcurrentDictionary<ulong, CardsAgainstHumanityGame> RunningCAHs = new ConcurrentDictionary<ulong, CardsAgainstHumanityGame>();
        internal override void Init(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand(Module.Prefix + "cah")
                .Description($"Starts a game of cards against humanity.\nuser is added to players list; other players must use {Module.Prefix}cahjoin to join the game")
                .Parameter("players", ParameterType.Unparsed)
                .Do(async e =>
                {
                    CardsAgainstHumanityGame cah;
                    if (!RunningCAHs.TryGetValue(e.Server.Id, out cah))
                    {
                        var cahGame = new CardsAgainstHumanityGame(e);
                        if (RunningCAHs.TryAdd(e.Server.Id, cahGame))
                            await e.Channel.SendMessage("**Cards Against Humanity started**");
                        else
                            await cahGame.StopGame();
                    }
                    else
                        await e.Channel.SendMessage("CAH game is already running on this server");
                });

            cgb.CreateCommand(Module.Prefix + "cahjoin")
                .Description("Join the active game of CAH")
                .Do(async e =>
                {
                    CardsAgainstHumanityGame cah;
                    if (!RunningCAHs.TryGetValue(e.Server.Id, out cah))
                    {
                        await e.Channel.SendMessage("No active game on this server");
                        return;
                    }
                    if (!await cah.JoinGame(e.User))
                    {
                        await e.Channel.SendMessage("Could not add Player to list.\n" +
                            "Possible explanations:\n" +
                            "Player already joined.\n" +
                            "Czar being chosen, try in a few seconds");
                    } else
                    {
                        await e.Channel.SendMessage($"{e.User.Mention} successfully joined CAH");
                    }
                });

            cgb.CreateCommand(Module.Prefix + "cahleave")
                .Description("Leaves the current game. **Bot owner can set target**")
                .Parameter("target", ParameterType.Optional)
                .Do(async e =>
                {
                    CardsAgainstHumanityGame cah;
                    if (!RunningCAHs.TryGetValue(e.Server.Id, out cah))
                    {
                        await e.Channel.SendMessage("No active game on this server");
                        return;
                    }
                    var usr = e.GetArg("target") ?? "";
                    if (NadekoBot.IsOwner(e.User.Id))
                    {
                        Discord.User target = e.Server.FindUsers(usr).FirstOrDefault() ?? e.User;
                        var left = cah.LeaveGame(target);
                        if (!cah.LeaveGame(target))
                        {
                            await e.Channel.SendMessage($"Could not let {target.Mention} leave game");
                        } else
                        {
                            await e.Channel.SendMessage($"{target.Mention} successfully left game");
                        }
                    }
                    else
                    {
                        if (!cah.LeaveGame(e.User))
                        {
                            await e.Channel.SendMessage($"Could not get {e.User.Mention} to leave game");
                        }
                        else
                        {
                            await e.Channel.SendMessage($"{e.User.Mention} successfully left game");
                        }
                    }
                   
                });

            cgb.CreateCommand(Module.Prefix + "cahstart")
                .Description("Starts game of Cards Against Humanity")
                .Do(async e =>
                {
                    CardsAgainstHumanityGame cah;
                    if (!RunningCAHs.TryGetValue(e.Server.Id, out cah))
                    {
                        await e.Channel.SendMessage("No active game on this server");
                        return;
                    }
                    if (cah.Players.Count < 2)
                    {
                        await e.Channel.SendMessage("Can't start without more than one player");
                    }
                    cah.StartUp = false;
                    
                });

            cgb.CreateCommand(Module.Prefix + "cahnext")
                .Description("Skips to the next phase of the round.")
                .Do(async e =>
                {
                    CardsAgainstHumanityGame cah;
                    if (!RunningCAHs.TryGetValue(e.Server.Id, out cah))
                    {
                        await e.Channel.SendMessage("No active game on this server");
                        return;
                    }
                    if (cah.SkipToNext())
                    {
                        await e.Channel.SendMessage("skipped succesfully");

                     } else
                    {
                        await e.Channel.SendMessage($"Could not skip. try `{Module.Prefix}cahstart");
                    }

                });

            cgb.CreateCommand(Module.Prefix + "cahq")
                .Description("Quits current CAH game after current round")
                .Do(async e =>
                {
                    CardsAgainstHumanityGame cah;
                    if (RunningCAHs.TryGetValue(e.Server.Id, out cah))
                        await cah.StopGame();
                    else
                        await e.Channel.SendMessage("NO CAH is running on this server");
                });
        }

        public CardsAgainstHumanity(DiscordModule module) : base(module) { }
    }
}
