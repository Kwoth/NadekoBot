﻿//using Discord;
//using Discord.Commands;
//using NadekoBot.Classes;
//using NadekoBot.DataModels;
//using NadekoBot.Extensions;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;

////todo DB
////todo Rewrite?
//namespace NadekoBot.Modules.Games.Commands
//{
//    public static class SentencesProvider
//    {
//        internal static string GetRandomSentence()
//        {
//            var data = DbHandler.Instance.GetAllRows<TypingArticle>();
//            try
//            {
//                return data.ToList()[new Random().Next(0, data.Count())].Text;
//            }
//            catch
//            {
//                return "Failed retrieving data from parse. Owner didn't add any articles to type using `typeadd`.";
//            }
//        }
//    }

//    public class TypingGame
//    {
//        public const float WORD_VALUE = 4.5f;
//        private readonly Channel channel;
//        public string CurrentSentence;
//        public bool IsActive;
//        private readonly Stopwatch sw;
//        private readonly List<ulong> finishedUserIds;

//        public TypingGame(Channel channel)
//        {
//            this.channel = channel;
//            IsActive = false;
//            sw = new Stopwatch();
//            finishedUserIds = new List<ulong>();
//        }

//        public Channel Channell { get; internal set; }

//        internal async Task<bool> Stop()
//        {
//            if (!IsActive) return false;
//            NadekoBot.Client.MessageReceived -= AnswerReceived;
//            finishedUserIds.Clear();
//            IsActive = false;
//            sw.Stop();
//            sw.Reset();
//            await channel.Send("Typing contest stopped").ConfigureAwait(false);
//            return true;
//        }

//        internal async Task Start()
//        {
//            while (true)
//            {
//                if (IsActive) return; // can't start running game
//                IsActive = true;
//                CurrentSentence = SentencesProvider.GetRandomSentence();
//                var i = (int)(CurrentSentence.Length / WORD_VALUE * 1.7f);
//                await channel.SendMessageAsync($":clock2: Next contest will last for {i} seconds. Type the bolded text as fast as you can.").ConfigureAwait(false);


//                var msg = await channel.SendMessageAsync("Starting new typing contest in **3**...").ConfigureAwait(false);
//                await Task.Delay(1000).ConfigureAwait(false);
//                await msg.Edit("Starting new typing contest in **2**...").ConfigureAwait(false);
//                await Task.Delay(1000).ConfigureAwait(false);
//                await msg.Edit("Starting new typing contest in **1**...").ConfigureAwait(false);
//                await Task.Delay(1000).ConfigureAwait(false);
//                await msg.Edit($":book:**{CurrentSentence.Replace(" ", " \x200B")}**:book:").ConfigureAwait(false);
//                sw.Start();
//                HandleAnswers();

//                while (i > 0)
//                {
//                    await Task.Delay(1000).ConfigureAwait(false);
//                    i--;
//                    if (!IsActive)
//                        return;
//                }

//                await Stop().ConfigureAwait(false);
//            }
//        }

//        private void HandleAnswers()
//        {
//            NadekoBot.Client.MessageReceived += AnswerReceived;
//        }

//        private async void AnswerReceived(object sender, MessageEventArgs e)
//        {
//            try
//            {
//                if (e.Channel == null || e.Channel.Id != channel.Id || imsg.Author.Id == NadekoBot.Client.CurrentUser.Id) return;

//                var guess = e.Message.RawText;

//                var distance = CurrentSentence.LevenshteinDistance(guess);
//                var decision = Judge(distance, guess.Length);
//                if (decision && !finishedUserIds.Contains(imsg.Author.Id))
//                {
//                    finishedUserIds.Add(imsg.Author.Id);
//                    await channel.Send($"{imsg.Author.Mention} finished in **{sw.Elapsed.Seconds}** seconds with { distance } errors, **{ CurrentSentence.Length / WORD_VALUE / sw.Elapsed.Seconds * 60 }** WPM!").ConfigureAwait(false);
//                    if (finishedUserIds.Count % 2 == 0)
//                    {
//                        await channel.SendMessageAsync($":exclamation: `A lot of people finished, here is the text for those still typing:`\n\n:book:**{CurrentSentence}**:book:").ConfigureAwait(false);
//                    }
//                }
//            }
//            catch { }
//        }

//        private bool Judge(int errors, int textLength) => errors <= textLength / 25;

//    }

//    internal class SpeedTyping : DiscordCommand
//    {

//        public static ConcurrentDictionary<ulong, TypingGame> RunningContests;

//        public SpeedTyping(DiscordModule module) : base(module)
//        {
//            RunningContests = new ConcurrentDictionary<ulong, TypingGame>();
//        }

//        public Func<CommandEventArgs, Task> DoFunc() =>
//            async e =>
//            {
//                var game = RunningContests.GetOrAdd(imsg.Author.Server.Id, id => new TypingGame(e.Channel));

//                if (game.IsActive)
//                {
//                    await channel.SendMessageAsync(
//                            $"Contest already running in " +
//                            $"{game.Channell.Mention} channel.")
//                                .ConfigureAwait(false);
//                }
//                else
//                {
//                    await game.Start().ConfigureAwait(false);
//                }
//            };

//        private Func<CommandEventArgs, Task> QuitFunc() =>
//            async e =>
//            {
//                TypingGame game;
//                if (RunningContests.TryRemove(imsg.Author.Server.Id, out game))
//                {
//                    await game.Stop().ConfigureAwait(false);
//                    return;
//                }
//                await channel.SendMessageAsync("No contest to stop on this channel.").ConfigureAwait(false);
//            };

//        internal override void Init(CommandGroupBuilder cgb)
//        {
//            cgb.CreateCommand(Module.Prefix + "typestart")
//                .Description($"Starts a typing contest. | `{Prefix}typestart`")
//                .Do(DoFunc());

//            cgb.CreateCommand(Module.Prefix + "typestop")
//                .Description($"Stops a typing contest on the current channel. | `{Prefix}typestop`")
//                .Do(QuitFunc());

//            cgb.CreateCommand(Module.Prefix + "typeadd")
//                .Description($"Adds a new article to the typing contest. Owner only. | `{Prefix}typeadd wordswords`")
//                .Parameter("text", ParameterType.Unparsed)
//                .Do(async e =>
//                {
//                    if (!NadekoBot.IsOwner(imsg.Author.Id) || string.IsNullOrWhiteSpace(e.GetArg("text"))) return;

//                    DbHandler.Instance.Connection.Insert(new TypingArticle
//                    {
//                        Text = e.GetArg("text"),
//                        DateAdded = DateTime.Now
//                    });

//                    await channel.SendMessageAsync("Added new article for typing game.").ConfigureAwait(false);
//                });
//        }
//    }
//}
