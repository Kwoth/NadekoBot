using System;
using System.Linq;
using Discord.Modules;
using NadekoBot.Commands;
using Newtonsoft.Json.Linq;
using System.IO;
using Discord.Commands;
using NadekoBot.Extensions;
using System.Collections;
using System.Collections.Generic;

namespace NadekoBot.Modules
{
    internal class Games : DiscordModule
    {
        private readonly Random rng = new Random();

        public Games()
        {
            commands.Add(new Trivia(this));
            commands.Add(new SpeedTyping(this));
            commands.Add(new PollCommand(this));
            //commands.Add(new BetrayGame(this));
        }

        public override string Prefix { get; } = NadekoBot.Config.CommandPrefixes.Games;

        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {

                cgb.AddCheck(Classes.Permissions.PermissionChecker.Instance);

                commands.ForEach(cmd => cmd.Init(cgb));

                cgb.CreateCommand(Prefix + "choose")
                  .Description("Chooses a thing from a list of things\n**Usage**: >choose Get up;Sleep;Sleep more")
                  .Parameter("list", Discord.Commands.ParameterType.Unparsed)
                  .Do(async e =>
                  {
                      var arg = e.GetArg("list");
                      if (string.IsNullOrWhiteSpace(arg))
                          return;
                      var list = arg.Split(';');
                      if (list.Count() < 2)
                          return;
                      await e.Channel.SendMessage(list[rng.Next(0, list.Length)]);
                  });

                cgb.CreateCommand(Prefix + "8ball")
                    .Description("Ask the 8ball a yes/no question.")
                    .Parameter("question", Discord.Commands.ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var question = e.GetArg("question");
                        if (string.IsNullOrWhiteSpace(question))
                            return;
                        try
                        {
                            await e.Channel.SendMessage(
                                $":question: **Question**: `{question}` \n🎱 **8Ball Answers**: `{NadekoBot.Config._8BallResponses[rng.Next(0, NadekoBot.Config._8BallResponses.Length)]}`");
                        }
                        catch { }
                    });

                cgb.CreateCommand(Prefix + "attack")
                    .Description("Attack a person. Supported attacks: 'splash', 'strike', 'burn', 'surge'.\n**Usage**: >attack strike @User")
                    .Parameter("attack_type", Discord.Commands.ParameterType.Required)
                    .Parameter("target", Discord.Commands.ParameterType.Required)
                    .Do(async e =>
                    {
                        if (stats.ContainsKey(e.User.Id))
                        {
                            //If the one attacking has already fainted, they shouldn't be able to move
                            if (stats[e.User.Id] < 0)
                            {
                                await e.Channel.Send($"{e.User.Name} has fainted and is unable to move!");
                                return;
                            }
                        }
                        var usr = e.Server.FindUsers(e.GetArg("target")).FirstOrDefault();
                        if (usr == null)
                        {
                            await e.Channel.SendMessage("No such person.");
                            return;
                        }
                        var usrType = GetType(usr.Id);
                        var response = "";
                        var dmg = GetDamage(usrType, GetType(e.User.Id), e.GetArg("attack_type").ToLowerInvariant());
                        response = e.GetArg("attack_type") + (e.GetArg("attack_type") == "splash" ? "es " : "s ") + $"{usr.Mention}{GetImage(usrType)} for {dmg}\n";
                        if (!stats.ContainsKey(usr.Id))
                        {
                            stats.Add(usr.Id, BASEHEALTH - dmg);
                        }
                        else
                        {
                            stats[usr.Id] -= dmg;
                        }
                        if (dmg >= 65)
                        {
                            response += "It's super effective!";
                        }
                        else if (dmg <= 35)
                        {
                            response += "Ineffective!";
                        }
                        if (stats[usr.Id] > 0)
                        {
                            response += $"{usr.Name} has {stats[usr.Id]} Health left!";
                        }
                        else
                        {
                            response += $"{usr.Name} has fainted!";
                        }
                        await e.Channel.SendMessage($"{ e.User.Mention }{GetImage(GetType(e.User.Id))} {response}");
                    });

                cgb.CreateCommand(Prefix + "heal")
                .Description("Heals someone. Revives those that fainted. \n**Usage**:>revive @someone")
                .Parameter("target", ParameterType.Required)
                .Do(async e =>
                {
                    //Should this cost NadekoFlowers?

                    var usr = e.Server.FindUsers(e.GetArg("target")).FirstOrDefault();
                    if (usr == null)
                    {
                        await e.Channel.SendMessage("No such person.");
                        return;
                    }
                    if (stats.ContainsKey(usr.Id))
                    {
                        var HP = stats[usr.Id];
                        if (HP == BASEHEALTH)
                        {
                            await e.Channel.SendMessage($"{usr.Name} already has full HP!");
                            return;
                        }
                        stats[usr.Id] = BASEHEALTH;
                        if (HP < 0)
                        {
                            //Could heal only for half HP?
                            await e.Channel.SendMessage($"{e.User.Name} revived {usr.Name}");
                            return;
                        }

                        await e.Channel.SendMessage($"{e.User.Name} healed {usr.Name} for {BASEHEALTH - HP} HP");
                        return;
                    }
                });

                cgb.CreateCommand(Prefix + "poketype")
                    .Parameter("target", Discord.Commands.ParameterType.Required)
                    .Description("Gets the users element type. Use this to do more damage with strike!")
                    .Do(async e =>
                    {
                        var usr = e.Server.FindUsers(e.GetArg("target")).FirstOrDefault();
                        if (usr == null)
                        {
                            await e.Channel.SendMessage("No such person.");
                            return;
                        }
                        var t = GetType(usr.Id);
                        await e.Channel.SendMessage($"{usr.Name}'s type is {GetImage(t)} {t}");
                    });
                cgb.CreateCommand(Prefix + "rps")
                    .Description("Play a game of rocket paperclip scissors with nadeko.\n**Usage**: >rps scissors")
                    .Parameter("input", ParameterType.Required)
                    .Do(async e =>
                    {
                        var input = e.GetArg("input").Trim();
                        int pick;
                        switch (input)
                        {
                            case "r":
                            case "rock":
                            case "rocket":
                                pick = 0;
                                break;
                            case "p":
                            case "paper":
                            case "paperclip":
                                pick = 1;
                                break;
                            case "scissors":
                            case "s":
                                pick = 2;
                                break;
                            default:
                                return;
                        }
                        var nadekoPick = new Random().Next(0, 3);
                        var msg = "";
                        if (pick == nadekoPick)
                            msg = $"It's a draw! Both picked :{GetRPSPick(pick)}:";
                        else if ((pick == 0 && nadekoPick == 1) ||
                                 (pick == 1 && nadekoPick == 2) ||
                                 (pick == 2 && nadekoPick == 0))
                            msg = $"{NadekoBot.BotMention} won! :{GetRPSPick(nadekoPick)}: beats :{GetRPSPick(pick)}:";
                        else
                            msg = $"{e.User.Mention} won! :{GetRPSPick(pick)}: beats :{GetRPSPick(nadekoPick)}:";

                        await e.Channel.SendMessage(msg);
                    });

                cgb.CreateCommand(Prefix + "linux")
                    .Description("Prints a customizable Linux interjection")
                    .Parameter("gnu", ParameterType.Required)
                    .Parameter("linux", ParameterType.Required)
                    .Do(async e =>
                    {
                        var guhnoo = e.Args[0];
                        var loonix = e.Args[1];

                        await e.Channel.SendMessage(
$@"
I'd just like to interject for moment. What you're refering to as {loonix}, is in fact, {guhnoo}/{loonix}, or as I've recently taken to calling it, {guhnoo} plus {loonix}. {loonix} is not an operating system unto itself, but rather another free component of a fully functioning {guhnoo} system made useful by the {guhnoo} corelibs, shell utilities and vital system components comprising a full OS as defined by POSIX.

Many computer users run a modified version of the {guhnoo} system every day, without realizing it. Through a peculiar turn of events, the version of {guhnoo} which is widely used today is often called {loonix}, and many of its users are not aware that it is basically the {guhnoo} system, developed by the {guhnoo} Project.

There really is a {loonix}, and these people are using it, but it is just a part of the system they use. {loonix} is the kernel: the program in the system that allocates the machine's resources to the other programs that you run. The kernel is an essential part of an operating system, but useless by itself; it can only function in the context of a complete operating system. {loonix} is normally used in combination with the {guhnoo} operating system: the whole system is basically {guhnoo} with {loonix} added, or {guhnoo}/{loonix}. All the so-called {loonix} distributions are really distributions of {guhnoo}/{loonix}.
");
                    });
            });
        }
        /*

            🌿 or 🍃 or 🌱 Grass
⚡ Electric
❄ Ice
☁ Fly
🔥 Fire
💧 or 💦 Water
⭕ Normal
🐛 Insect
🌟 or 💫 or ✨ Fairy
    */
        //NORMAL, FIRE, WATER, ELECTRIC, GRASS, ICE, FIGHTING, POISON, GROUND, FLYING, PSYCHIC, BUG, ROCK, GHOST, DRAGON, DARK, STEEL
        private string GetImage(PokeType t)
        {
            switch (t)
            {

                case PokeType.FIRE:
                    return "🔥";
                case PokeType.WATER:
                    return "💦";
                case PokeType.ELECTRIC:
                    return "⚡️";
                case PokeType.GRASS:
                    return "🌿";
                case PokeType.ICE:
                    return "❄";
                case PokeType.FIGHTING: //1
                    return "⚡️";
                case PokeType.POISON:
                    return "☠";
                case PokeType.GROUND: //1
                    return "⚡️";
                case PokeType.FLYING:
                    return "☁";
                case PokeType.PSYCHIC:
                    return "💫";
                case PokeType.BUG:
                    return "🐛";
                case PokeType.ROCK: //1
                    return "⚡️";
                case PokeType.GHOST:
                    return "👻";
                case PokeType.DRAGON:
                    return "🐉";
                case PokeType.DARK:
                    return "🕶";
                case PokeType.STEEL:
                    return "🔩";
                default: //Normal type
                    return "⭕️";
            }
        }

        private int GetDamage(PokeType targetType, PokeType userType, string v)
        {
            var rng = new Random();
            int damage = rng.Next(0, 100);
            //Default magnification
            double magnifier = 1;
            if (moves.ContainsKey(v))
            {
                //Get the PokeType of the move
                var moveType = moves[v];

                var specialTypes = pTypes[moveType];
                if (specialTypes.ContainsKey(targetType))
                {
                    //Change magnification if the type is special (as in, super effective or weak)
                    magnifier = specialTypes[targetType];
                }
                //If the move is used by same-type user
                if (moveType == userType)
                {
                    magnifier = magnifier * 1.5;
                }
            }
            else magnifier = 0.6;
            damage = (int)((double)damage * magnifier);
            return damage;
        }

        private PokeType GetType(ulong id)
        {

            if (setTypes.ContainsKey(id))
            {
                return setTypes[id];
            }

            var remainder = id % 16;
            return (PokeType)remainder;
        }

        private enum PokeType
        {
            NORMAL, FIRE, WATER, ELECTRIC, GRASS, ICE, FIGHTING, POISON, GROUND, FLYING, PSYCHIC, BUG, ROCK, GHOST, DRAGON, DARK, STEEL
        }

        private static Dictionary<string, PokeType> moves = new Dictionary<string, PokeType>()
        {
            {"splash", PokeType.WATER },
            {"burn", PokeType.FIRE },
            {"flame", PokeType.FIRE },
            {"freeze", PokeType.ICE },
            {"strike", PokeType.GRASS },
            {"surge", PokeType.ELECTRIC },
            {"electrocute", PokeType.ELECTRIC }

        };
        //This should actually be saved in a DataModel, but I'm not good at that
        private Dictionary<ulong, PokeType> setTypes = new Dictionary<ulong, PokeType>()
        {
            {113760353979990024, PokeType.FIRE},

            {131474815298174976, PokeType.DRAGON},
            {144807035337179136, PokeType.DARK }
        };

        //For now only HP
        private Dictionary<ulong, int> stats = new Dictionary<ulong, int>();

        //The weaknesses and strengths of attacks
        private static Dictionary<PokeType, Dictionary<PokeType, double>> pTypes = new Dictionary<PokeType, Dictionary<PokeType, double>>()
        {
            {PokeType.NORMAL, new Dictionary<PokeType, double>()
            {
                {PokeType.ROCK,  0.5},
                {PokeType.GHOST, 0 },
                {PokeType.STEEL, 0.5 }
            }
            },
            {PokeType.FIRE, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  0.5},
                {PokeType.WATER, 0.5},
                {PokeType.GRASS, 2},
                {PokeType.ICE, 2},
                {PokeType.BUG, 2},
                {PokeType.ROCK, 0.5},
                {PokeType.DRAGON, 0.5},
                {PokeType.STEEL, 2}
            }
            },
            {PokeType.WATER, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  2},
                {PokeType.WATER, 0.5 },
                {PokeType.GRASS, 0.5 },
                {PokeType.GROUND, 2 },
                {PokeType.ROCK, 2 },
                {PokeType.DRAGON, 0.5 }
            }
            },
            {PokeType.ELECTRIC, new Dictionary<PokeType, double>()
            {
                {PokeType.WATER, 2 },
                {PokeType.ELECTRIC, 0.5 },
                {PokeType.GRASS, 2 },
                {PokeType.GROUND, 0 },
                {PokeType.FLYING, 2 },
                {PokeType.DRAGON, 0.5 }
            }
            },
            {PokeType.GRASS, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  0.5},
                {PokeType.WATER, 0.5 },
                {PokeType.GRASS, 2 },
                {PokeType.ICE, 2 },
                {PokeType.BUG, 2 },
                {PokeType.ROCK, 0.5 },
                {PokeType.DRAGON, 0.5 },
                {PokeType.STEEL, 2}
            }
            },
            {PokeType.ICE, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  0.5},
                {PokeType.WATER, 0.5 },
                {PokeType.GRASS, 2 },
                {PokeType.ICE, 0.5},
                {PokeType.GROUND, 2 },
                {PokeType.FLYING, 2 },
                {PokeType.DRAGON, 2 },
                {PokeType.STEEL, 0.5}
            }
            },
            {PokeType.FIGHTING, new Dictionary<PokeType, double>()
            {
                {PokeType.NORMAL,  2},
                {PokeType.ICE, 2 },
                {PokeType.POISON, 0.5},
                {PokeType.FLYING, 0.5 },
                {PokeType.PSYCHIC, 0.5 },
                {PokeType.BUG, 0.5 },
                {PokeType.ROCK, 2 },
                {PokeType.GHOST, 0},
                {PokeType.DARK, 2 },
                {PokeType.STEEL, 2 }
            }
            },
            {PokeType.POISON, new Dictionary<PokeType, double>()
            {
                {PokeType.GRASS,  2},
                {PokeType.POISON, 0.5 },
                {PokeType.GROUND, 0.5 },
                {PokeType.ROCK, 0.5 },
                {PokeType.GHOST, 0.5 },
                {PokeType.STEEL, 0}
            }
            },
            {PokeType.GROUND, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  2},
                {PokeType.ELECTRIC, 2 },
                {PokeType.GRASS, 0.5},
                {PokeType.POISON, 0.5},
                {PokeType.FLYING, 0 },
                {PokeType.BUG, 0.5 },
                {PokeType.ROCK, 2 },
                {PokeType.STEEL, 2}
            }
            },
            {PokeType.FLYING, new Dictionary<PokeType, double>()
            {
                {PokeType.ELECTRIC,  0.5},
                {PokeType.GRASS, 2 },
                {PokeType.FIGHTING, 2 },
                {PokeType.BUG, 2 },
                {PokeType.ROCK, 0.5 },
                {PokeType.STEEL, 0.5}
            }
            },
            {PokeType.PSYCHIC, new Dictionary<PokeType, double>()
            {
                {PokeType.FIGHTING,  2},
                {PokeType.POISON, 2 },
                {PokeType.PSYCHIC, 0.5 },
                {PokeType.DARK, 0 },
                {PokeType.STEEL, 0.5 }
            }
            },
            {PokeType.BUG, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  0.5},
                {PokeType.GRASS, 2 },
                {PokeType.FIGHTING, 0.5 },
                {PokeType.POISON, 0.5 },
                {PokeType.FLYING, 0.5 },
                {PokeType.PSYCHIC, 2 },
                {PokeType.ROCK, 0.5},
                {PokeType.DARK, 2 },
                {PokeType.STEEL,0.5 }
            }
            },
            {PokeType.ROCK, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  2},
                {PokeType.ICE, 2 },
                {PokeType.FIGHTING, 0.5 },
                {PokeType.GROUND, 0.5 },
                {PokeType.FLYING, 2 },
                {PokeType.BUG, 2 },
                {PokeType.STEEL, 0.5}
            }
            },
            {PokeType.GHOST, new Dictionary<PokeType, double>()
            {
                {PokeType.NORMAL,  0},
                {PokeType.PSYCHIC, 2 },
                {PokeType.GHOST, 2 },
                {PokeType.DARK, 0.5 },
                {PokeType.STEEL, 0.5}
            }
            },
            {PokeType.DRAGON, new Dictionary<PokeType, double>()
            {
                {PokeType.DRAGON, 2 },
                {PokeType.STEEL, 0.5}
            }
            },
            {PokeType.DARK, new Dictionary<PokeType, double>()
            {
                {PokeType.FIGHTING,  0.5},
                {PokeType.PSYCHIC, 2 },
                {PokeType.GHOST, 2 },
                {PokeType.DARK, 0.5 },
                {PokeType.STEEL, 0.5}
            }
            },
            {PokeType.STEEL, new Dictionary<PokeType, double>()
            {
                {PokeType.FIRE,  0.5},
                {PokeType.WATER, 0.5 },
                {PokeType.ELECTRIC, 0.5 },
                {PokeType.ICE, 2 },
                {PokeType.ROCK, 2 },
                {PokeType.STEEL, 0.5}
            }
            },




        };
        private readonly int BASEHEALTH = 500;

        private string GetRPSPick(int i)
        {
            if (i == 0)
                return "rocket";
            else if (i == 1)
                return "paperclip";
            else
                return "scissors";
        }
    }
}
