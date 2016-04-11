using Discord;
using Discord.Commands;
using NadekoBot.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace NadekoBot.Classes.CardsAgainstHumanity
{
    class CardsAgainstHumanityGame
    {
        private readonly object _templock = new object();

        private Server server;
        private Channel channel;

        private int RoundDurationMilliseconds { get; } = 40000;
        private int DecideDurationMilliseconds { get; } = 30000;

        private CancellationTokenSource cahCancelSource { get; set; }

        public CAHBlackCard currentBlackCard { get; private set; }
        public HashSet<CAHBlackCard> oldBlackCards { get; set; } = new HashSet<CAHBlackCard>();

        public HashSet<CAHWhiteCard> oldWhiteCards { get; set; } = new HashSet<CAHWhiteCard>();

        //public Dictionary<User, CAHWhiteCard> chosenWhiteCards { get; set; } = new Dictionary<User, CAHWhiteCard>();

        public ConcurrentDictionary<int, KeyValuePair<User, HashSet<CAHWhiteCard>>> WhiteCardsAssortment { get; set; } = new ConcurrentDictionary<int, KeyValuePair<User, HashSet<CAHWhiteCard>>>();

        public ConcurrentDictionary<User, CAHStats> Players { get; } = new ConcurrentDictionary<User, CAHStats>();

        public bool GameActive { get; private set; } = false;
        public bool ShouldStopGame { get; private set; }
        public User currentCzar { get; set; }
        public bool CzarDecision { get; set; }
        public bool allChosen { get; set; } 
        public bool StartUp { get; set; }

        /// <summary>
        /// Allow more players to join the game
        /// </summary>
        public bool allowChangePlayers { get; set; }


        public int WinRequirement { get; } = 5;

        public CardsAgainstHumanityGame(CommandEventArgs e)
        {
            #region oldSettingPlayers
            var allUsrs = e.Message.MentionedUsers.Union(new User[] { e.User });
            int i = 0;
            foreach (User u in allUsrs)
            {
                var s = new CAHStats(i++);

                Players.TryAdd(u, new CAHStats(i++));
            }
            #endregion
            StartUp = true;
            server = e.Server;
            channel = e.Channel;
            allowChangePlayers = true;
            Players.TryAdd(e.User, new CAHStats(0));
            Task.Run(StartGame);
        }

        private async Task StartGame()
        {
            var CzarIndex = 0;
            //Three Phases:
            //Allow users to join 
            Message msg = null;
            while (StartUp)
            {
                if (msg != null)
                {
                    await msg.Delete();
                }
                msg = await channel.SendMessage("New Players may join using >cahjoin.\n Use >cahstart to start");
                await Task.Delay(10000);
            }
            
            while (!ShouldStopGame)
            {
                allowChangePlayers = false;
                //Set the CzarIndex correctly
                if (CzarIndex >= Players.Count)
                    CzarIndex = 0;
                //Set the current Czar
                currentCzar = Players.Keys.ToList()[CzarIndex++];

                //Get a new Black card
                currentBlackCard = CAHBlackCardPool.Instance.GetRandomBlackCard(oldBlackCards);
                if (currentBlackCard == null)
                {
                    await channel.SendMessage("No more black cards found");
                    await End();
                    return;
                }
                oldBlackCards.Add(currentBlackCard);

                //Start the new round
                await channel.SendMessage($"**New Round**:\n" +
                    $"Current Card Czar = {currentCzar.Mention}\n" +
                    $"Black Card:\n" +
                    $"{currentBlackCard.ToString()}\n" +
                    $"**SET YOUR CHOICE BY SENDING INDEX OF CARD TO CHAT: a number from 1-{CAHStats.HandSize}**\n"+
                    "If multiple cards are asked, seperate them with commas: `1,2`");

                //send the players their cards
                foreach (var player in Players.Where(x => x.Key != currentCzar))
                {
                    await player.Key.SendMessage(filled(player.Key));
                }
                allowChangePlayers = true;
                //receive messages
                NadekoBot.Client.MessageReceived += PotentialPlacement;

                //Allow the players to play
                GameActive = true;
                CzarDecision = false;
                allChosen = false;

                while (!allChosen)
                {
                    await Task.Delay(5000);
                }

                await sendCzarChoice();

                CzarDecision = true;
                while (CzarDecision)
                {
                    //check every 5 seconds whether the Czar has chosen
                    await Task.Delay(5000);
                }
                GameActive = false;

                NadekoBot.Client.MessageReceived -= PotentialPlacement;
                WhiteCardsAssortment.Clear();
                await Task.Delay(2000);

            }
            await End();
            #region Previousway
            /*
            while (!ShouldStopGame)
            {
                
                //reset the cancellation source
                cahCancelSource = new CancellationTokenSource();
                var token =cahCancelSource.Token;
                //Load black card
                currentBlackCard = CAHBlackCardPool.Instance.GetRandomBlackCard(oldBlackCards);
                if (currentBlackCard == null)
                {
                    await channel.SendMessage($":exclamation: Failed to load black card");
                    await End();
                    return;
                }
                //add current to exclusion list
                oldBlackCards.Add(currentBlackCard);
                
                
                currentTzar = Players.ToList()[TzarIndex++].Key; //should I sort the list?

                await channel.SendMessage($"**New Round**:\n"+
                    $"Current Card Czar = {currentTzar.Mention}\n"+
                    $"Black Card:\n"+
                    $"{currentBlackCard.ToString()}\n"+
                    //$"You have {RoundDurationMilliseconds/1000} seconds to set your choice.\n" +
                    $"**SET YOUR CHOICE BY SENDING INDEX OF CARD TO CHAT**");

                foreach (var player in Players.Where(x=>x.Key != currentTzar))
                {
                    await player.Key.SendMessage(filled(player.Key));
                }

                //receive messages
                NadekoBot.Client.MessageReceived += PotentialPlacement;

                //allow sending number
                GameActive = true;
                CzarDecision = false;
                allChosen = false;
                try
                {

                    while (!allChosen)
                    {
                        //check every 5 seconds whether everyone has chosen
                        await Task.Delay(5000); 
                    }
                    // await Task.Delay(RoundDurationMilliseconds, token);
                    // var response = $":clock2: Time's up! time for the Tzar to decide is {DecideDurationMilliseconds/1000} seconds:";
                    var response = $"Everyone has chosen! Time for the Tzar to decide:";
                    //int i = 0;
                    response += $"**{currentBlackCard}**:\n";
                    foreach (var kvp in WhiteCardsAssortment.OrderBy(x=>x.Key)) 
                    {
                        var str = "";
                        foreach (var t in kvp.Value.Value)
                        {
                            str += t.ToString() + ", ";
                        }
                        response += $"\n{kvp.Key}. **{str}**";
                    }
                    if (response.Length >= 2000)
                    {
                        var split = response.Split('\n').ToList();
                        var splitIndex = split.Count() / 2;
                        var partOne = string.Join("\n", split.GetRange(0, splitIndex));
                        var partTwo = string.Join("\n", split.GetRange(splitIndex, splitIndex));
                        await channel.SendMessage(partOne);
                        await channel.SendMessage(partTwo);
                    }
                    else
                    {
                        await channel.Send(response);
                    }
                    
                    CzarDecision = true;
                    //await Task.Delay(DecideDurationMilliseconds, token);
                    while (CzarDecision)
                    {
                        //check every 5 seconds whether the Czar has chosen
                        await Task.Delay(5000);
                    }
                    
                } catch (TaskCanceledException)
                {
                    //Console.WriteLine("CAH canceled");
                }

                GameActive = false;
                if (!cahCancelSource.IsCancellationRequested)
                {
                    string s = $"Time's up! Since Czar couldn't decide, no one gets a point";
                    await channel.SendMessage(s);
                }
                NadekoBot.Client.MessageReceived -= PotentialPlacement;
                WhiteCardsAssortment.Clear();
                await Task.Delay(2000);
            }
            await End();
            */
            #endregion

        }

        private async Task sendCzarChoice()
        {
            var response = $"Everyone has chosen! Time for {currentCzar.Mention} to decide:\n";
            //int i = 0;
            response += $"{currentBlackCard.ToString()}:\n";
            foreach (var kvp in WhiteCardsAssortment.OrderBy(x => x.Key))
            {
                var str = "";
                foreach (var t in kvp.Value.Value)
                {
                    str += t.ToString() + ", ";
                }
                response += $"\n{kvp.Key + 1}. {str}"; //!!!
            }

            //a check for long responses
            if (response.Length >= 2000)
            {
                var split = response.Split('\n').ToList();
                var splitIndex = split.Count() / 2;
                var partOne = string.Join("\n", split.GetRange(0, splitIndex));
                var partTwo = string.Join("\n", split.GetRange(splitIndex, splitIndex));
                await channel.SendMessage(partOne);
                await channel.SendMessage(partTwo);
            }
            else
            {
                await channel.Send(response);
            }
        }

        /// <summary>
        /// Ends the game
        /// </summary>
        /// <returns></returns>
        private async Task End()
        {
            ShouldStopGame = true;
            await channel.SendMessage("**Cards Against Humanity game ended**\n" + getLeaderboard());
            CardsAgainstHumanityGame throwawayvalue;
            Commands.CardsAgainstHumanity.RunningCAHs.TryRemove(server.Id, out throwawayvalue);
        }

        /// <summary>
        /// Sets the game to stop after the current round
        /// </summary>
        /// <returns></returns>
        public async Task StopGame()
        {
            if (!ShouldStopGame)
                await channel.SendMessage(":exclamation: CAH will stop after this round");
            ShouldStopGame = true;
        }
        
        /// <summary>
        /// Join the game: adds given user to Players
        /// </summary>
        /// <param name="u">User to add to Players</param>
        /// <returns></returns>
        public async Task<bool> JoinGame(User u)
        {
            if (!allowChangePlayers)
            {
                return false;
            }

            if (!Players.TryAdd(u, new CAHStats(Players.Count)))
                return false;
            if (!StartUp)
            {
                await u.SendMessage(filled(u));
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void PotentialPlacement(object sender, MessageEventArgs e)
        {
            try
            {
                #region checks
                //Getting the messages we want
                if (e.Channel.IsPrivate) return; //Because the default message would show up as well
                if (e.Server != server) return; 
                if (!Players.Keys.Contains(e.User)) return; //Only track current players
                if (e.User == currentCzar && !CzarDecision) return;
                if (CzarDecision && e.User != currentCzar) return;
                #endregion

                #region CzarDecision
                //if the Tzar is making the decision
                if (CzarDecision)
                {
                    int choice;
                    if (!int.TryParse(e.Message.Text, out choice))
                        return;
                    choice--;
                    //check whether it's out of range
                    if (choice > WhiteCardsAssortment.Count - 1) return;
                    var chosen = WhiteCardsAssortment[choice];                    
                    var winner = chosen.Key;
                    var message = $"{winner.Name} has won this round with:\n";
                    var cards = WhiteCardsAssortment[choice].Value;
                    if (currentBlackCard.CardContents.Contains("\\_\\_\\_"))
                    {
                        var str = currentBlackCard.ToString();
                        var cardsArray = cards.ToArray();
                        for (int i=0; i < cards.Count(); i++)
                        {
                            var c = cardsArray[0];
                            str = str.Replace("\\_\\_\\_", c.CardContents);
                        }
                        message += str;
                    } else
                    {
                        message += currentBlackCard.ToString();
                        cards.ToList().ForEach(x => message += x.CardContents + "\n" );
                    }
                    //cahCancelSource.Cancel();
                    CzarDecision = false;
                    Players[winner].Points++;
                    GameActive = false;
                    await channel.SendMessage(message);
                    //Remove chosen card of winner
                    
                    foreach (var card in cards)
                    {
                        Players[winner].Hand.Remove(card);
                    }
                    //Is it a final win?
                    if (Players[winner].Points != WinRequirement) return;
                    ShouldStopGame = true;
                    await channel.Send($"We have a winner! Its {winner.Mention}\n{getLeaderboard()}"); //add leaderboard

                    await FlowersHandler.AddFlowersAsync(winner, "Won CAH", 2);
                    return;
                }
                #endregion

                #region regularPlayers
                var msg = e.Message.Text;
                var cardSet = new HashSet<CAHWhiteCard>();
                CAHStats stat;
                if (!Players.TryGetValue(e.User, out stat)) return;

                

                if (currentBlackCard.WhiteCards == 1)
                {
                    int index;
                    if (!int.TryParse(msg.Trim(), out index))
                        return;
                    index--; //!!!
                
                    cardSet.Add(stat.Hand[index]);
                }
                else {
                    if (!Regex.IsMatch(msg, @"\d")) return;
                    var matches = Regex.Matches(msg, @"\d+");
                    if (matches.Count < currentBlackCard.WhiteCards) 
                    {
                        await channel.SendMessage($"Black card need {currentBlackCard.WhiteCards} cards, seperated from each other.");
                        return;
                    }
                   
                   
                    foreach (Match m  in matches)
                    {
                        var s = m.Value;
                        int index;
                        if (!int.TryParse(s, out index)) return;
                        index--; //!!!
                        cardSet.Add(stat.Hand[index]);
                    }
                }

                
                
                var newIndex = getRandomIndex();
                var kvp = new KeyValuePair<User, HashSet<CAHWhiteCard>>(e.User, cardSet);
                var containing = false;
                WhiteCardsAssortment.Values.ForEach(x =>
                {
                    if (x.Key == e.User) {
                        containing = true;
                        var toUpdate = WhiteCardsAssortment.Where(y => y.Value.Equals(x)).First();
                        WhiteCardsAssortment.TryUpdate(toUpdate.Key, kvp, toUpdate.Value);
                    }
                });
                if (!containing)
                {
                    WhiteCardsAssortment.TryAdd(newIndex, kvp);
                }
                
                //chosenWhiteCards.Add(e.User, stat.Hand[index]); //catch the outofRangeExceptions
                var unset = unSetPlayers();
                await channel.SendMessage($"Registered chosen card(s) for {e.User.Mention}\n{unset}");
                if (unset == String.Empty) //There are no players who haven't chosen yet
                {
                    allChosen = true;
                }
                #endregion
            }
            catch (IndexOutOfRangeException)
            {
                //This shouldn't happen anymore
                await channel.SendMessage("Index given was out of range");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string getLeaderboard()
        {
            var sb = new StringBuilder();
            sb.Append("**Leaderboard:**\n-----------\n");

            foreach (var item in Players.OrderByDescending(x => x.Value.Points))
            {
                sb.AppendLine($"**{item.Key.Name}** has {item.Value.Points} points".ToString().SnPl(item.Value.Points));
            }
            return sb.ToString();
        }

        public string unSetPlayers()
        {
            var sb = new StringBuilder();
            //Could do this smarter I guess
            var dcit = WhiteCardsAssortment.Values.ToDictionary(x=> x.Key);
            var missing = Players.Where(x => !dcit.ContainsKey(x.Key) && x.Key != currentCzar);
            if (missing.Any())
            {
                sb.AppendLine("Still missing cards from players:");
                foreach (var p in missing)
                {
                    sb.AppendLine($"**{p.Key.Name}**");
                }
            }
            return sb.ToString();
        }

        public bool LeaveGame(User u)
        {
            if (!allowChangePlayers)
            {
                return false;
            }
            if (u == currentCzar)
            {
                return false;
            }
            CAHStats throwawayValue;
            return Players.TryRemove(u, out throwawayValue);
        }

        public bool SkipToNext()
        {
            if (GameActive)
            {
                if (CzarDecision)
                {
                    CzarDecision = false;
                    return true;
                }
                else
                {
                    allChosen = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a full hand of cards for the player
        /// </summary>
        /// <param name="player">Player to get the cards from</param>
        /// <returns></returns>
        public string filled(User player)
        {
            CAHStats stats;
            Players.TryGetValue(player, out stats);
            while (stats.Hand.Count < CAHStats.HandSize)
            {
                stats.Hand.Add(CAHWhiteCardPool.Instance.GetRandomWhiteCard(oldWhiteCards));
            }
            var sb = new StringBuilder();
            sb.Append("You have the following cards:\n");
            int i = 1; //!!!
            foreach (var c in stats.Hand)
            {
                sb.AppendLine($"{i++}. {c.ToString()}");
            }
            return sb.ToString();
        }
        /// <summary>
        /// A random index between the limits
        /// </summary>
        /// <returns>Random index</returns>
        private int getRandomIndex()
        {
            var random = new Random();
            var rand = random.Next(0, Players.Count - 1);

            while (WhiteCardsAssortment.ContainsKey(rand))
            {
                rand = random.Next(0, Players.Count - 1);
            }

            return rand;
        }

    }

    public class CAHStats
    {
        //public User Player { get; set; }
        public int Points { get; set; } = 0;
        public int Index { get; set; }
        public static int HandSize { get; } = 12;
        public List<CAHWhiteCard> Hand { get; set; } = new List<CAHWhiteCard>();

        public CAHStats( int index)
        {
            //Player = u;
            Index = index;
        }
    }
}
