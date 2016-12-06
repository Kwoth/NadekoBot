using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using System.Linq;
using NadekoBot.Services;
using NadekoBot.Services.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using NLog;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Concurrent;
using static NadekoBot.Modules.Gambling.Gambling;

namespace NadekoBot.Modules.Pokemon
{

    [NadekoModule("Pokemon", ">")]
    public partial class Pokemon : DiscordModule
    {
        private static List<PokemonType> PokemonTypes = new List<PokemonType>();
        private static ConcurrentDictionary<ulong, PokeStats> Stats = new ConcurrentDictionary<ulong, PokeStats>();
        
        public const string PokemonTypesFile = "data/pokemon_types.json";

        private Logger _pokelog { get; }

        public Pokemon(ILocalization loc, CommandService cmds, ShardedDiscordClient client) : base(loc, cmds, client)
        {
            _pokelog = LogManager.GetCurrentClassLogger();
            if (File.Exists(PokemonTypesFile))
            {
                PokemonTypes = JsonConvert.DeserializeObject<List<PokemonType>>(File.ReadAllText(PokemonTypesFile));
            }
            else
            {
                _pokelog.Warn(PokemonTypesFile + " is missing. Pokemon types not loaded.");
            }
        }


        private int GetDamage(PokemonType usertype, PokemonType targetType)
        {
            var rng = new Random();
            int damage = rng.Next(40, 60);
            foreach (PokemonMultiplier Multiplier in usertype.Multipliers)
            {
                if (Multiplier.Type == targetType.Name)
                {
                    var multiplier = Multiplier.Multiplication;
                    damage = (int)(damage * multiplier);
                }
            }

            return damage;
        }
            

        private PokemonType GetPokeType(ulong id)
        {

            Dictionary<ulong, string> setTypes;
            using (var uow = DbHandler.UnitOfWork())
            {
                setTypes = uow.PokeGame.GetAll().ToDictionary(x => x.UserId, y => y.type);
            }

            if (setTypes.ContainsKey(id))
            {
                return StringToPokemonType(setTypes[id]);
            }
            int count = PokemonTypes.Count;

            int remainder = Math.Abs((int)(id % (ulong)count));

            return PokemonTypes[remainder];
        }



        private PokemonType StringToPokemonType(string v)
        {
            var str = v?.ToUpperInvariant();
            var list = PokemonTypes;
            foreach (PokemonType p in list)
            {
                if (str == p.Name)
                {
                    return p;
                }
            }
            return null;
        }


        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Attack(IUserMessage umsg, string move, IGuildUser targetUser = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            IGuildUser user = (IGuildUser)umsg.Author;

            if (string.IsNullOrWhiteSpace(move)) {
                return;
            }

            if (targetUser == null)
            {
                await channel.SendMessageAsync("No such person.").ConfigureAwait(false);
                return;
            }
            else if (targetUser == user)
            {
                await channel.SendMessageAsync("You can't attack yourself.").ConfigureAwait(false);
                return;
            }

                   
            // Checking stats first, then move
            //Set up the userstats
            PokeStats userStats;
            userStats = Stats.GetOrAdd(user.Id, new PokeStats());

            //Check if able to move
            //User not able if HP < 0, has made more than 4 attacks
            if (userStats.Hp < 0)
            {
                await channel.SendMessageAsync($"{user.Mention} has fainted and was not able to move!").ConfigureAwait(false);
                return;
            }
            if (userStats.MovesMade >= 5)
            {
                await channel.SendMessageAsync($"{user.Mention} has used too many moves in a row and was not able to move!").ConfigureAwait(false);
                return;
            }
            if (userStats.LastAttacked.Contains(targetUser.Id))
            {
                await channel.SendMessageAsync($"{user.Mention} can't attack again without retaliation!").ConfigureAwait(false);
                return;
            }
            //get target stats
            PokeStats targetStats;
            targetStats = Stats.GetOrAdd(targetUser.Id, new PokeStats());

            //If target's HP is below 0, no use attacking
            if (targetStats.Hp <= 0)
            {
                await channel.SendMessageAsync($"{targetUser.Mention} has already fainted!").ConfigureAwait(false);
                return;
            }

            //Check whether move can be used
            PokemonType userType = GetPokeType(user.Id);

            var enabledMoves = userType.Moves;
            if (!enabledMoves.Contains(move.ToLowerInvariant()))
            {
                await channel.SendMessageAsync($"{user.Mention} is not able to use **{move}**. Type {NadekoBot.ModulePrefixes[typeof(Pokemon).Name]}ml to see moves").ConfigureAwait(false);
                return;
            }

            //get target type
            PokemonType targetType = GetPokeType(targetUser.Id);
            //generate damage
            int damage = GetDamage(userType, targetType);
            //apply damage to target
            targetStats.Hp -= damage;

            var response = $"{user.Mention} used **{move}**{userType.Icon} on {targetUser.Mention}{targetType.Icon} for **{damage}** damage";

            //Damage type
            if (damage < 40)
            {
                response += "\nIt's not effective..";
            }
            else if (damage > 60)
            {
                response += "\nIt's super effective!";
            }
            else
            {
                response += "\nIt's somewhat effective";
            }

            //check fainted

            if (targetStats.Hp <= 0)
            {
                response += $"\n**{targetUser.Mention}** has fainted!";
            }
            else
            {
                response += $"\n**{targetUser.Mention}** has {targetStats.Hp} HP remaining";
            }

            //update other stats
            userStats.LastAttacked.Add(targetUser.Id);
            userStats.MovesMade++;
            targetStats.MovesMade = 0;
            if (targetStats.LastAttacked.Contains(user.Id))
            {
                targetStats.LastAttacked.Remove(user.Id);
            }

            //update dictionary
            //This can stay the same right?
            Stats[user.Id] = userStats;
            Stats[targetUser.Id] = targetStats;

            await channel.SendMessageAsync(response).ConfigureAwait(false);
        }


        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Movelist(IUserMessage umsg)
        {
            var channel = (ITextChannel)umsg.Channel;
            IGuildUser user = (IGuildUser)umsg.Author;

            var userType = GetPokeType(user.Id);
            var movesList = userType.Moves;
            var str = $"**Moves for `{userType.Name}` type.**";
            foreach (string m in movesList)
            {
                str += $"\n{userType.Icon}{m}";
            }
            await channel.SendMessageAsync(str).ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Heal(IUserMessage umsg, IGuildUser targetUser = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            IGuildUser user = (IGuildUser)umsg.Author;

            if (targetUser == null) {
                await channel.SendMessageAsync("No such person.").ConfigureAwait(false);
                return;
            }

            if (Stats.ContainsKey(targetUser.Id))
            {
                var targetStats = Stats[targetUser.Id];
                if (targetStats.Hp == targetStats.MaxHp)
                {
                    await channel.SendMessageAsync($"{targetUser.Mention} already has full HP!").ConfigureAwait(false);
                    return;
                }
                //Payment~
                var amount = 1;

                var target = (targetUser.Id == user.Id) ? "yourself" : targetUser.Mention;
                if (amount > 0)
                {
                        if (!await CurrencyHandler.RemoveCurrencyAsync(user, $"Poke-Heal {target}", amount, true).ConfigureAwait(false))
                        {
                            try { await channel.SendMessageAsync($"{user.Mention} You don't have enough {CurrencyName}s.").ConfigureAwait(false); } catch { }
                            return;
                        }
                }

                //healing
                targetStats.Hp = targetStats.MaxHp;
                if (targetStats.Hp < 0)
                {
                    //Could heal only for half HP?
                    Stats[targetUser.Id].Hp = (targetStats.MaxHp / 2);
                    if (target == "yourself")
                    {
                        await channel.SendMessageAsync($"You revived yourself with one {CurrencySign}").ConfigureAwait(false);
                    }
                    else
                    {
                        await channel.SendMessageAsync($"{user.Mention} revived {targetUser.Mention} with one {CurrencySign}").ConfigureAwait(false);
                    }
                   return;
                }
                await channel.SendMessageAsync($"{user.Mention} healed {targetUser.Mention} with one {CurrencySign}").ConfigureAwait(false);
                return;
            }
            else
            {
                await channel.SendMessageAsync($"{targetUser.Mention} already has full HP!").ConfigureAwait(false);
            }
        }


        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Type(IUserMessage umsg, IGuildUser targetUser = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            IGuildUser user = (IGuildUser)umsg.Author;

            if (targetUser == null)
            {
                return;
            }

            var pType = GetPokeType(targetUser.Id);
            await channel.SendMessageAsync($"Type of {targetUser.Mention} is **{pType.Name.ToLowerInvariant()}**{pType.Icon}").ConfigureAwait(false);

        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Settype(IUserMessage umsg, [Remainder] string typeTargeted = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            IGuildUser user = (IGuildUser)umsg.Author;

            var targetType = StringToPokemonType(typeTargeted);
            if (targetType == null)
            {
                var pokemonTypeName = PokemonTypes.Select(t => $"{t.Name,-10}").ToArray();
                var pokemonTypeIcon = PokemonTypes.Select(t => $"{t.Icon}").ToArray();
                var embed = new EmbedBuilder()
                                .WithTitle("**__Available Types__**")
                                .WithDescription("These are the available types you can set by typing `>settype TYPE`")
                                .AddField(x=>x.WithName(pokemonTypeName[0]).WithValue(pokemonTypeIcon[0]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[1]).WithValue(pokemonTypeIcon[1]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[2]).WithValue(pokemonTypeIcon[2]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[3]).WithValue(pokemonTypeIcon[3]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[4]).WithValue(pokemonTypeIcon[4]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[5]).WithValue(pokemonTypeIcon[5]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[6]).WithValue(pokemonTypeIcon[6]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[7]).WithValue(pokemonTypeIcon[7]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[8]).WithValue(pokemonTypeIcon[8]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[9]).WithValue(pokemonTypeIcon[9]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[10]).WithValue(pokemonTypeIcon[10]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[11]).WithValue(pokemonTypeIcon[11]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[12]).WithValue(pokemonTypeIcon[12]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[13]).WithValue(pokemonTypeIcon[13]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[14]).WithValue(pokemonTypeIcon[14]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[15]).WithValue(pokemonTypeIcon[15]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[16]).WithValue(pokemonTypeIcon[16]).WithIsInline(true))
                                .AddField(x => x.WithName(pokemonTypeName[17]).WithValue(pokemonTypeIcon[17]).WithIsInline(true))
                                .WithColor(NadekoBot.OkColor);
                await channel.EmbedAsync(embed.Build());
                return;
            }
            if (targetType == GetPokeType(user.Id))
            {
                await channel.SendMessageAsync($"Your type is already {targetType.Name.ToLowerInvariant()}{targetType.Icon}").ConfigureAwait(false);
                return;
            }

            //Payment~
            var amount = 1;
            if (amount > 0)
            {
                if (!await CurrencyHandler.RemoveCurrencyAsync(user, $"{user.Mention} change type to {typeTargeted}", amount, true).ConfigureAwait(false))
                {
                    try { await channel.SendMessageAsync($"{user.Mention} You don't have enough {CurrencyName}s.").ConfigureAwait(false); } catch { }
                    return;
                }
            }

            //Actually changing the type here
            Dictionary<ulong, string> setTypes;

            using (var uow = DbHandler.UnitOfWork())
            {
                var pokeUsers = uow.PokeGame.GetAll();
                setTypes = pokeUsers.ToDictionary(x => x.UserId, y => y.type);
                var pt = new UserPokeTypes
                {
                    UserId = user.Id,
                    type = targetType.Name,
                };
                if (!setTypes.ContainsKey(user.Id))
                {
                    //create user in db
                    uow.PokeGame.Add(pt);
                }
                else
                {
                    //update user in db
                    var pokeUserCmd = pokeUsers.Where(p => p.UserId == user.Id).FirstOrDefault();
                    pokeUserCmd.type = targetType.Name;
                    uow.PokeGame.Update(pokeUserCmd);
                }
                await uow.CompleteAsync();
            }

            //Now for the response
            await channel.SendMessageAsync($"Set type of {user.Mention} to {typeTargeted}{targetType.Icon} for a {CurrencySign}").ConfigureAwait(false);
        }

    }
}




