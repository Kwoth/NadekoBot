using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Modules;
using NadekoBot.Modules.Permissions.Classes;
using Discord.Commands;
using NadekoBot.DataModels;
using Discord;
using NadekoBot.Classes;
using NadekoBot.Modules.Pokemon.Extensions;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace NadekoBot.Modules.Pokemon
{
    class PokemonModule : DiscordModule
    {
        public override string Prefix { get; } = NadekoBot.Config.CommandPrefixes.Pokemon;

        public ConcurrentDictionary<ulong, TrainerStats> UserStats = new ConcurrentDictionary<ulong, TrainerStats>();

        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                cgb.AddCheck(PermissionChecker.Instance);

                cgb.CreateCommand(Prefix + "active")
                .Description("Get the pokemon of someone|yourself")
                .Parameter("target", ParameterType.Optional)
                .Do(async e =>
                {
                    var target = e.Server.FindUsers(e.GetArg("target")).DefaultIfEmpty(null).FirstOrDefault() ?? e.User;

                    await e.Channel.SendMessage($"**{target.Mention}**:\n{ActivePokemon(target).pokemonString()}");
                    //foreach (var pkm in list)
                    //{
                    //    await e.Channel.SendMessage($"**{target.Mention}**:\n{pkm.pokemonString()}");
                    //}

                });

                cgb.CreateCommand(Prefix + "switch")
                .Description("Set your active pokemon uding the nickname")
                .Parameter("name", ParameterType.Unparsed)
                .Do(async e =>
                {
                    var list = PokemonList(e.User);
                    var toSet = list.Where(x => x.NickName == e.GetArg("name").Trim()).DefaultIfEmpty(null).FirstOrDefault();
                    if (toSet == null)
                    {
                        await e.Channel.SendMessage($"Could not find pokemon with name \"{e.GetArg("name").Trim()}\"");
                        return;
                    }
                    var trainerStats = UserStats.GetOrAdd(e.User.Id, new TrainerStats());
                    if (trainerStats.MovesMade > TrainerStats.MaxMoves)
                    {
                        await e.Channel.SendMessage($"{e.User.Mention} can't move!");
                        return;
                    }
                    if (SwitchPokemon(e.User, toSet))
                    {
                        trainerStats.MovesMade++;
                        UserStats.AddOrUpdate(e.User.Id, trainerStats, (s, t) => trainerStats);
                        await e.Channel.SendMessage($"Set active pokemon of {e.User.Mention} to {toSet.NickName} ");
                    } else
                    {
                        await e.Channel.SendMessage($"The pokemon to swtich to must have HP!");
                    }

                });

                cgb.CreateCommand(Prefix + "list")
                .Description("Gets a list of your pokemons (6) (active pokemon underlined)")
                .Do(async e =>
                {
                    var list = PokemonList(e.User);
                    string str = $"{e.User.Mention}'s pokemon are:\n";
                    foreach (var pkm in list)
                    {
                        if (pkm.IsActive)
                        {
                            str += $"__**{pkm.NickName}** : *{pkm.GetSpecies().name}*__\n";
                        } else
                        {
                            str += $"**{pkm.NickName}** : *{pkm.GetSpecies().name}*\n";
                        }

                    }
                    await e.Channel.SendMessage(str);
                });

                cgb.CreateCommand(Prefix + "heal")
                .Description($"Heal your given pokemon (by nickname) or the given target's active pokemon")
                .Parameter("args", ParameterType.Unparsed)
                .Do(async e =>
                {
                    var args = e.GetArg("args");
                    var target = e.Server.FindUsers(args).DefaultIfEmpty(null).FirstOrDefault() ?? e.User;

                    if (target == e.User)
                    {

                        var toHeal = PokemonList(target).Where(x => x.NickName == args.Trim()).DefaultIfEmpty(null).FirstOrDefault();
                        if (toHeal == null)
                        {
                            await e.Channel.SendMessage($"Could not find pokemon with name \"{e.GetArg("args").Trim()}\"");
                            return;
                        }

                        if (FlowersHandler.RemoveFlowers(target, "Healed pokemon", 1))
                        {
                            var hp = toHeal.HP;
                            toHeal.HP = toHeal.MaxHP;
                            await e.Channel.SendMessage($"{target.Mention} successfully healed {toHeal.NickName} for {toHeal.HP - hp} HP for a {NadekoBot.Config.CurrencySign}");
                            DbHandler.Instance.Save(toHeal);
                            //Heal your own userstats as well?


                        }
                        else
                        {
                            await e.Channel.SendMessage($"Could not pay {NadekoBot.Config.CurrencySign}, you're **{NadekoBot.Config.CurrencyName}**-less");
                        }
                        return;
                    }
                    var toHealn = ActivePokemon(target);
                    if (toHealn == null)
                    {
                        await e.Channel.SendMessage($"Could not get pokemon from {target.Name} correctly");
                        return;
                    }
                    if (FlowersHandler.RemoveFlowers(target, "Healed pokemon", 1))
                    {
                        var hp = toHealn.HP;
                        toHealn.HP = toHealn.MaxHP;
                        await e.Channel.SendMessage($"{target.Mention} successfully healed {toHealn.NickName} for {toHealn.HP - hp} HP for a {NadekoBot.Config.CurrencySign}");
                        DbHandler.Instance.Save(toHealn);
                    }
                    else
                    {
                        await e.Channel.SendMessage($"Could not pay {NadekoBot.Config.CurrencySign}, you're **{NadekoBot.Config.CurrencyName}**-less");
                    }
                    return;

                });

                cgb.CreateCommand(Prefix + "rename")
                .Alias(Prefix + "rn")
                .Description($"Rename your active pokemon")
                .Parameter("name", ParameterType.Unparsed)
                .Do(async e =>
                {
                    var newName = e.GetArg("name");
                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        await e.Channel.SendMessage("Name required");
                        return;
                    }
                    var activePkm = ActivePokemon(e.User);
                    activePkm.NickName = newName;
                    DbHandler.Instance.Save(activePkm);
                    await e.Channel.SendMessage($"Renamed active pokemon to {newName}");
                });


                cgb.CreateCommand(Prefix + "reset")
                .Description($"Resets your pokemon. CANNOT BE UNDONE\n**Usage**:{Prefix}reset true")
                .Parameter("true", ParameterType.Unparsed)
                .Do(async e =>
                {
                    bool willReset;
                    if (bool.TryParse(e.GetArg("true"), out willReset))
                    {
                        var db = DbHandler.Instance.GetAllRows<PokemonSprite>();
                        var row = db.Where(x => x.OwnerId == (long)e.User.Id);
                        //var toDelete = DbHandler.Instance.FindAll<PokemonSprite>(s => s.OwnerId == (long)e.User.Id);
                        foreach (var todel in row)
                        {
                            DbHandler.Instance.Delete<PokemonSprite>(todel.Id.Value);
                        }
                        await e.Channel.SendMessage("Your pokemon have been deleted.\n\nCruel.\n\n\nI have no words for this.");
                    } else
                    {
                        await e.Channel.SendMessage($"Use `{Prefix}reset true` to really kill all your pokemon :knife:");
                    }
                });

                cgb.CreateCommand(Prefix + "attack")
                .Description("Attacks given target with given move")
                .Parameter("args", ParameterType.Unparsed)
                .Do(async e =>
                {
                    Regex regex = new Regex(@"<@(?<id>\d+)>");
                    var args = e.GetArg("args");
                    if (!regex.IsMatch(args))
                    {
                        await e.Channel.SendMessage("Please specify target");
                        return;
                    }
                    Match m = regex.Match(args);
                    var id = ulong.Parse(m.Groups["id"].Value);
                    var target = e.Server.GetUser(id);
                    if (target == null)
                    {
                        await e.Channel.SendMessage("Please specify target");
                        return;
                    }
                    var moveString = args.Replace(m.Value, string.Empty).Replace("\"", "").Trim();
                    var attackerPokemon = ActivePokemon(e.User);
                    var species = attackerPokemon.GetSpecies();
                    if (!species.moves.Keys.Contains(moveString))
                    {
                        await e.Channel.SendMessage($"Cannot use \"{moveString}\", see `{Prefix}ML` for moves");
                        return;
                    }
                    var attackerStats = UserStats.GetOrAdd(e.User.Id, new TrainerStats());
                    var defenderStats = UserStats.GetOrAdd(target.Id, new TrainerStats());
                    if (attackerStats.MovesMade > TrainerStats.MaxMoves || attackerStats.LastAttacked.Contains(target.Id))
                    {
                        await e.Channel.SendMessage($"{e.User.Mention} can't move!");
                        return;
                    }
                    KeyValuePair<string, string> move = new KeyValuePair<string, string>(moveString, species.moves[moveString]);
                    var defenderPokemon = ActivePokemon(target);
                    PokemonAttack attack = new PokemonAttack(attackerPokemon, defenderPokemon, move);
                    var msg = attack.AttackString();
                    defenderPokemon.HP -= attack.Damage;

                    var HP = (defenderPokemon.HP < 0) ? 0 : defenderPokemon.HP;
                    msg += $"{defenderPokemon.NickName} has {HP} HP left!";
                    await e.Channel.SendMessage(msg);

                    if (defenderPokemon.HP <= 0)
                    {
                        defenderPokemon.HP = 0;
                        var str = $"{defenderPokemon.NickName} fainted!\n{attackerPokemon.NickName}'s owner {e.User.Mention} receives 1 {NadekoBot.Config.CurrencySign}\n";
                        var lvl = attackerPokemon.Level;
                        var extraXP = attackerPokemon.Reward(defenderPokemon);
                       
                        str += $"{attackerPokemon.NickName} gained {extraXP} from the battle\n";
                        if (attackerPokemon.Level > lvl) //levled up
                        {
                            str += $"**{attackerPokemon.NickName}** leveled up!\n**{attackerPokemon.NickName}** is now level **{attackerPokemon.Level}**";
                            //Check evostatus
                        }
                        var list = PokemonList(target).Where(s => (s.HP > 0 && s != defenderPokemon));
                        if (list.Any())
                        {
                            var toSet = list.FirstOrDefault();
                            SwitchPokemon(target, toSet);
                            str += $"\n{target.Mention}'s active pokemon set to **{toSet.NickName}**";
                        }
                        else
                        {
                            str += $"\n{target.Mention} has no pokemon left!";
                            //do something?
                        }
                        await e.Channel.SendMessage(str);
                        await FlowersHandler.AddFlowersAsync(e.User, "Victorious in pokemon", 1);
                    }
                    //Update stats, you shall
                    attackerStats.LastAttacked.Add(target.Id);
                    attackerStats.MovesMade++;
                    defenderStats.LastAttacked = new List<ulong>();
                    defenderStats.MovesMade = 0;
                    UserStats.AddOrUpdate(e.User.Id, x => attackerStats, (s, t) => attackerStats);
                    UserStats.AddOrUpdate(target.Id, x => defenderStats, (s, t) => defenderStats);
                
                    DbHandler.Instance.Save(defenderPokemon);
                    DbHandler.Instance.Save(attackerPokemon);
                });
                
            });

        }

        /// <summary>
        /// Sets the active pokemon of the given user to the given Sprite
        /// </summary>
        /// <param name="u"></param>
        /// <param name="newActive"></param>
        /// <returns></returns>
        bool SwitchPokemon(User u, PokemonSprite newActive)
        {
            var toUnset = PokemonList(u).Where(x => x.IsActive).FirstOrDefault();
            if (toUnset == null)
            {
                return false;
            }
            if (newActive.HP <= 0)
            {
                return false;
            }
            toUnset.IsActive = false;
            newActive.IsActive = true;

            DbHandler.Instance.Save(toUnset);
            DbHandler.Instance.Save(newActive);

            return true;
        }

        PokemonSprite ActivePokemon(User u)
        {
            var list = PokemonList(u);
            return list.Where(x => x.IsActive).FirstOrDefault();
        }

        List<PokemonSprite> PokemonList(User u)
        {
            var db = DbHandler.Instance.GetAllRows<PokemonSprite>();
            var row = db.Where(x => x.OwnerId == (long)u.Id);
            if (row.Count() >= 6)
            {
                return row.ToList();
            }
            else
            {
               
                var list = new List<PokemonSprite>();
                while (list.Count < 6)
                {
                    var pkm = GeneratePokemon(u);
                    if (!list.Where(x=>x.IsActive).Any())
                    {
                        pkm.IsActive = true;
                    }

                    list.Add(pkm);
                    DbHandler.Instance.Save(pkm);
                }
                //Set an active pokemon
                
                return list;
            }
        }

        private PokemonSprite GeneratePokemon(User u)
        {
            Random rng = new Random();
            var list = PokemonMain.Instance.pokemonClasses.Where(x => x.evolveLevel != -1).ToList();
            var speciesIndex = rng.Next(0,list.Count() - 1);
            
            var species = list[speciesIndex];
            
            PokemonSprite sprite = new PokemonSprite
            {
                SpeciesId = species.number,
                HP = species.baseStats["hp"],
                Level = 1,
                NickName = species.name,
                OwnerId = (long) u.Id,
                XP = 0,
                Attack = species.baseStats["attack"],
                Defense = species.baseStats["defense"],
                SpecialAttack = species.baseStats["special-attack"],
                SpecialDefense = species.baseStats["special-defense"],
                Speed = species.baseStats["speed"],
                MaxHP = species.baseStats["hp"]
            };

            while (sprite.Level < 4)
            {
                sprite.LevelUp();
            }
            sprite.XP = sprite.XPRequired();
            sprite.LevelUp();
            return sprite;
        }
    }
}
