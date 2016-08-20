﻿using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Classes;
using NadekoBot.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Gambling.Commands
{
    public partial class Gambling
    {
        [Group]
        public class AnimalRacing
        {
            public static ConcurrentDictionary<ulong, AnimalRace> AnimalRaces = new ConcurrentDictionary<ulong, AnimalRace>();

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            public async Task Race(IMessage imsg)
            {
                var channel = imsg.Channel as ITextChannel;

                var ar = new AnimalRace(channel.Guild.Id, channel);

                if (ar.Fail)
                    await channel.SendMessageAsync("🏁 `Failed starting a race. Another race is probably running.`");
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            public async Task JoinRace(IMessage imsg, int amount = 0)
            {
                var channel = imsg.Channel as ITextChannel;

                if (amount < 0)
                    amount = 0;

                //todo DB
                //var userFlowers = Gambling.GetUserFlowers(imsg.Author.Id);

                //if (userFlowers < amount)
                //{
                //    await channel.SendMessageAsync($"{imsg.Author.Mention} You don't have enough {NadekoBot.Config.CurrencyName}s. You only have {userFlowers}{NadekoBot.Config.CurrencySign}.").ConfigureAwait(false);
                //    return;
                //}

                //if (amount > 0)
                //    await FlowersHandler.RemoveFlowers(imsg.Author, "BetRace", (int)amount, true).ConfigureAwait(false);

                AnimalRace ar;
                if (!AnimalRaces.TryGetValue(channel.Guild.Id, out ar))
                {
                    await channel.SendMessageAsync("No race exists on this server");
                    return;
                }
                await ar.JoinRace(imsg.Author as IGuildUser, amount);
            }

            public class AnimalRace
            {

                private ConcurrentQueue<string> animals = new ConcurrentQueue<string>(NadekoBot.Config.RaceAnimals.Shuffle());

                public bool Fail { get; internal set; }

                public List<Participant> participants = new List<Participant>();
                private ulong serverId;
                private int messagesSinceGameStarted = 0;

                public ITextChannel raceChannel { get; set; }
                public bool Started { get; private set; } = false;

                public AnimalRace(ulong serverId, ITextChannel ch)
                {
                    this.serverId = serverId;
                    this.raceChannel = ch;
                    if (!AnimalRaces.TryAdd(serverId, this))
                    {
                        Fail = true;
                        return;
                    }
                    var cancelSource = new CancellationTokenSource();
                    var token = cancelSource.Token;
                    var fullgame = CheckForFullGameAsync(token);
                    Task.Run(async () =>
                    {
                        try
                        {
                            //todo Commmand prefixes from config
                            await raceChannel.SendMessageAsync($"🏁`Race is starting in 20 seconds or when the room is full. Type $jr to join the race.`");
                            var t = await Task.WhenAny(Task.Delay(20000, token), fullgame);
                            Started = true;
                            cancelSource.Cancel();
                            if (t == fullgame)
                            {
                                await raceChannel.SendMessageAsync("🏁`Race full, starting right now!`");
                            }
                            else if (participants.Count > 1)
                            {
                                await raceChannel.SendMessageAsync("🏁`Game starting with " + participants.Count + " participants.`");
                            }
                            else
                            {
                                await raceChannel.SendMessageAsync("🏁`Race failed to start since there was not enough participants.`");
                                var p = participants.FirstOrDefault();
                                //todo DB
                                //if (p != null)
                                //    await FlowersHandler.AddFlowersAsync(p.User, "BetRace", p.AmountBet, true).ConfigureAwait(false);
                                End();
                                return;
                            }
                            await Task.Run(StartRace);
                            End();
                        }
                        catch { }
                    });
                }

                private void End()
                {
                    AnimalRace throwaway;
                    AnimalRaces.TryRemove(serverId, out throwaway);
                }

                private async Task StartRace()
                {
                    var rng = new Random();
                    Participant winner = null;
                    IMessage msg = null;
                    int place = 1;
                    try
                    {
                        NadekoBot.Client.MessageReceived += Client_MessageReceived;

                        while (!participants.All(p => p.Total >= 60))
                        {
                            //update the state
                            participants.ForEach(p =>
                            {

                                p.Total += 1 + rng.Next(0, 10);
                                if (p.Total > 60)
                                {
                                    p.Total = 60;
                                    if (winner == null)
                                    {
                                        winner = p;
                                    }
                                    if (p.Place == 0)
                                        p.Place = place++;
                                }
                            });


                            //draw the state

                            var text = $@"|🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🔚|
{String.Join("\n", participants.Select(p => $"{(int)(p.Total / 60f * 100),-2}%|{p.ToString()}"))}
|🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🏁🔚|";
                            if (msg == null || messagesSinceGameStarted >= 10) // also resend the message if channel was spammed
                            {
                                if (msg != null)
                                    try { await msg.DeleteAsync(); } catch { }
                                msg = await raceChannel.SendMessageAsync(text).ConfigureAwait(false);
                                messagesSinceGameStarted = 0;
                            }
                            else
                                await msg.ModifyAsync(m => m.Content = text).ConfigureAwait(false);

                            await Task.Delay(2500);
                        }
                    }
                    finally
                    {
                        NadekoBot.Client.MessageReceived -= Client_MessageReceived;
                    }

                    if (winner.AmountBet > 0)
                    {
                        var wonAmount = winner.AmountBet * (participants.Count - 1);
                        //todo DB
                        //await FlowersHandler.AddFlowersAsync(winner.User, "Won a Race", wonAmount).ConfigureAwait(false);
                        await raceChannel.SendMessageAsync($"🏁 {winner.User.Mention} as {winner.Animal} **Won the race and {wonAmount}{NadekoBot.Config.Currency.Sign}!**").ConfigureAwait(false);
                    }
                    else
                    {
                        await raceChannel.SendMessageAsync($"🏁 {winner.User.Mention} as {winner.Animal} **Won the race!**");
                    }

                }

                private async Task Client_MessageReceived(IMessage imsg)
                {
                    if (await imsg.IsAuthor(NadekoBot.Client) || !(imsg.Channel is ITextChannel) || imsg.Channel != raceChannel)
                        return;
                    messagesSinceGameStarted++;
                }

                private async Task CheckForFullGameAsync(CancellationToken cancelToken)
                {
                    while (animals.Count > 0)
                    {
                        await Task.Delay(100, cancelToken);
                    }
                }

                public async Task<bool> JoinRace(IGuildUser u, int amount = 0)
                {
                    var animal = "";
                    if (!animals.TryDequeue(out animal))
                    {
                        await raceChannel.SendMessageAsync($"{u.Mention} `There is no running race on this server.`");
                        return false;
                    }
                    var p = new Participant(u, animal, amount);
                    if (participants.Contains(p))
                    {
                        await raceChannel.SendMessageAsync($"{u.Mention} `You already joined this race.`");
                        return false;
                    }
                    if (Started)
                    {
                        await raceChannel.SendMessageAsync($"{u.Mention} `Race is already started`");
                        return false;
                    }
                    participants.Add(p);
                    await raceChannel.SendMessageAsync($"{u.Mention} **joined the race as a {p.Animal}" + (amount > 0 ? $" and bet {amount} {(amount == 1? NadekoBot.Config.Currency.Name: NadekoBot.Config.Currency.PluralName)}!**" : "**"));
                    return true;
                }
            }

            public class Participant
            {
                public IGuildUser User { get; set; }
                public string Animal { get; set; }
                public int AmountBet { get; set; }

                public float Coeff { get; set; }
                public int Total { get; set; }

                public int Place { get; set; } = 0;

                public Participant(IGuildUser u, string a, int amount)
                {
                    this.User = u;
                    this.Animal = a;
                    this.AmountBet = amount;
                }

                public override int GetHashCode()
                {
                    return User.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    var p = obj as Participant;
                    return p == null ?
                        false :
                        p.User == User;
                }

                public override string ToString()
                {
                    var str = new string('‣', Total) + Animal;
                    if (Place == 0)
                        return str;
                    if (Place == 1)
                    {
                        return str + "🏆";
                    }
                    else if (Place == 2)
                    {
                        return str + "`2nd`";
                    }
                    else if (Place == 3)
                    {
                        return str + "`3rd`";
                    }
                    else
                    {
                        return str + $"`{Place}th`";
                    }

                }
            }
        }
    }
}