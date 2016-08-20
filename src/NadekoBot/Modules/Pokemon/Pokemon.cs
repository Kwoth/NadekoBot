﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using NadekoBot.Services;
using NadekoBot.Attributes;
using System.Collections.Concurrent;
using NadekoBot.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Reflection;

namespace NadekoBot.Modules.Pokemon
{
    [Module(">", AppendSpace = false)]
    public class PokemonModule : DiscordModule
    {
        public static ConcurrentDictionary<long, PokemonServer> Dictionary;
        public static  List<PokemonType> PokemonTypes;
        public static  List<PokemonMove> PokemonMoves;
        public ConcurrentDictionary<PokemonPlayer, PokemonPlayer> DuelProposals;
        private Random _rand;
        public PokemonModule(ILocalization loc, CommandService cmds, IBotConfiguration config, IDiscordClient client) : base(loc, cmds, config, client)
        {
            var DataDir = Path.Combine(Directory.GetParent(typeof(NadekoBot).GetTypeInfo().Assembly.Location).FullName, "data");
            Dictionary = new ConcurrentDictionary<long, PokemonServer>();
            DuelProposals = new ConcurrentDictionary<PokemonPlayer, PokemonPlayer>();
            _rand = new Random();
            //TODO load Dictionary
            try
            {
                //TODO change this to loading from DB I guess?
                PokemonTypes = JsonConvert.DeserializeObject<List<PokemonType>>(File.ReadAllText(Path.Combine(DataDir, "pokemontypes.json")));
                PokemonMoves = JsonConvert.DeserializeObject<List<PokemonMove>>(File.ReadAllText(Path.Combine(DataDir, "pokemonmoves.json")));
            }
            catch (Exception e)
            {
                //TODO log exception, disable module or load default
                Console.WriteLine(e.Message);
            }
        }
        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Attack(IMessage msg, [Remainder] string attack)
        {

            var channel = msg.Channel as IGuildChannel;
            var guild = channel.Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            if (msg.MentionedUsers.Count == 1 && msg.MentionedUsers.FirstOrDefault().Id != (await _client.GetCurrentUserAsync()).Id)
            {
                var targetUser = msg.MentionedUsers.First();
                attack = attack.Replace(targetUser.Mention, "").Replace(targetUser.Mention.Insert(2, "!"), "").Trim();
                //now get both players
                var attacker = serverPlayers.GetOrAdd(msg.Author.Id, GenerateNewPlayer);
                var defender = serverPlayers.GetOrAdd(targetUser.Id, GenerateNewPlayer);
                //check if any of the two is fainted
                if (attacker.IsFainted())
                {
                    await msg.Reply(string.Format("{0} is fainted and can't move!", attacker));
                    return;
                }
                if (defender.IsFainted())
                {
                    await msg.Reply(string.Format("{0} has already fainted!", defender));
                    return;
                }
                //check if in duel
                if (!((attacker.Duelist == null && defender.Duelist == null) || (attacker.Duelist == defender)))
                {
                    await msg.Reply("One of the players is in a duel, and cannot engage other targets");
                    return;
                }
                //Check if already attacked
                if (attacker.Attacked.Contains(defender))
                {
                    await msg.Reply("Already attacked {0}");
                }
                //check if the move was a valid one :D
                if (!attacker.Moves.Any(kvp => kvp.Key.Name == attack))
                {
                    //TODO change this to moveslist + add prefix
                    await msg.Reply(string.Format("Move {0} not valid, use {1} to get your moves", attack, nameof(Attack)));
                    return;
                }
                var move = attacker.Moves.FirstOrDefault(m => m.Key.Name == attack);
                //check PP
                if (move.Value < 1)
                {
                    await msg.Reply(string.Format("Move {0} has no PP left, try using {1}", move.Key.Name, nameof(Attack)));
                    return;
                }
                //check agility for evasion~
                var diff = defender.Stats.Agility - attacker.Stats.Agility;
                //TODO mod this chance to make it purrfect add accuracy?

                var evaded = _rand.Next(1, 101) + diff > 85;
                if (evaded)
                {
                    await msg.Reply(string.Format("{0} evaded {1}'s {2}!", targetUser.Username, msg.Author.Username, attack));
                    return;
                }
                //execute move on target
                var damage = move.Key.CalculateDamage(attacker, defender);
                defender.Stats.Health -= damage;
                //todo add icon to things (toString method?)
                string message = string.Format("{0} attacked {1} with {2} for {3} HP!\n", attacker, defender, attack, damage);
                if (defender.IsFainted())
                {
                    message += string.Format("{0} has fainted!");
                    message += string.Format("{0} receives {1} for defeating {2}", attacker, attacker.Reward(), defender);
                    if (attacker.Duelist == defender)
                    {
                        message += "\nThe Duel has ended!";
                        attacker.Duelist = null;
                        defender.Duelist = null;
                        //todo decide whether we heal after duel
                    }
                }
                else
                {
                    message += string.Format("{0} has {1} HP left.", defender, defender.Stats.Health);
                }
                defender.Attacked.Remove(attacker);
                attacker.Attacked.Add(defender);
                //update Data
                var atkIndex = serverPlayers.IndexOf(attacker);
                var dfndIndex = serverPlayers.IndexOf(defender);
                serverPlayers[atkIndex] = attacker;
                serverPlayers[dfndIndex] = defender;
                var newServer = new PokemonServer() { Players = serverPlayers, ServerId = (long)guild.Id };
                Dictionary.TryUpdate((long)guild.Id, newServer, newServer);
            }
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Stats(IMessage msg, IGuildUser arg = null)
        {
            var guild = (msg.Channel as IGuildChannel).Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            StringBuilder builder = new StringBuilder();
            PokemonPlayer player = serverPlayers.GetOrAdd((arg ?? msg.Author).Id, GenerateNewPlayer);
            builder.AppendLine(string.Format("{0}'s Stats: ```xl", player));
            foreach (var p in player.Stats.GetType().GetProperties())
            {
                //TODO consider typing it out for localization purposes
                builder.AppendLine($"{p.Name} : {p.GetValue(player.Stats, null).ToString()}");
            }
            builder.AppendLine(string.Format("Type : {0}", player.PokemonType));
            if (player.Duelist != null)
                builder.AppendLine(string.Format("Duelist : {0}", player.Duelist));
            await msg.Reply(builder.AppendLine("```").ToString());
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Heal(IMessage msg, IGuildUser arg = null)
        {
            var guild = (msg.Channel as IGuildChannel).Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            var player = serverPlayers.GetOrAdd(arg?.Id ?? msg.Author.Id, GenerateNewPlayer);
            //TODO add pay
            
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task MovesList(IMessage msg, IGuildUser arg = null)
        {
            var channel = msg.Channel as IGuildChannel;
            var guild = channel.Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            StringBuilder builder = new StringBuilder();
            PokemonPlayer player = serverPlayers.GetOrAdd((arg ?? msg.Author).Id, GenerateNewPlayer);
            builder.AppendLine(string.Format("{0}'s moves: ```xl", player));
            foreach (var kvp in player.Moves)
            {
                var moveT = GetPokemonType(kvp.Key.MoveType);
                //todo should probably make this a table, but that's irritating
                builder.AppendLine(string.Format("{0}{1} | PP: {2}", kvp.Key.Name, moveT.Icon, kvp.Value));
            }
            builder.Append("```");
            await msg.Reply(builder.ToString());
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task StartDuel(IMessage msg, IGuildUser arg)
        {
            var guild = (msg.Channel as IGuildChannel).Guild;
            var serverPlayers = Dictionary.GetOrAdd((long)guild.Id, new PokemonServer() { ServerId = (long)guild.Id }).Players;
            PokemonPlayer player = serverPlayers.GetOrAdd(msg.Author.Id, GenerateNewPlayer);
            PokemonPlayer target = serverPlayers.GetOrAdd(arg.Id, GenerateNewPlayer);

        }


        public static PokemonPlayer GenerateNewPlayer(long id)
        {
            var _rand = new Random();
            var t = generateRandomType(id);
            var hp = _rand.Next(80, 120);
            return new PokemonPlayer()
            {
                UserId = id,
                Stats = new PokemonStats()
                {
                    Health = hp,
                    MaxHealth = hp,
                    Agility = _rand.Next(10, 50),
                    Strength = _rand.Next(80, 120)
                },
                PokemonType = t,
                Moves = generateRandomMoves(t, _rand),

            };
        }
        
        private static PokemonType generateRandomType(long id)
        {
            return PokemonTypes[(int)(id % PokemonTypes.Count)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focus">Moves will have at least 1 move of this type and will not have a move of a weakness of the type</param>
        /// <returns></returns>
        private static Dictionary<PokemonMove, int> generateRandomMoves(PokemonType focus, Random _rand)
        {
            var dict = new Dictionary<PokemonMove, int>();
            //2 steps: select a move with type Focus
            var selectRange = PokemonMoves.Where(m => m.MoveType == focus.Name).ToList();
            var selection = selectRange[_rand.Next(0, selectRange.Count)];
            dict.Add(selection, selection.PP);
            selectRange.AddRange(PokemonMoves.Where(m => !focus.Weaknesses.Contains(m.MoveType)));

            for (int i = 0; i < 3; i++)
            {
                selection = selectRange[_rand.Next(0, selectRange.Count)];
                if (dict.ContainsKey(selection)) i--;
                else dict.Add(selection, selection.PP);
            }
            return dict;
        }

        private PokemonType GetPokemonType(string name) => PokemonTypes.FirstOrDefault(p => p.Name == name);
    }

}