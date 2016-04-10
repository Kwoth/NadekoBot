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
                .Description("Starts a game of cards against humanity, mention all players in this message")
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
