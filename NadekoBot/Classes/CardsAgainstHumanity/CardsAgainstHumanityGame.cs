using Discord;
using Discord.Commands;
using NadekoBot.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public User currentTzar { get; set; }
        public bool CzarDecision { get; set; }
        public bool allChosen { get; set; } 

        public int WinRequirement { get; } = 5;

        public CardsAgainstHumanityGame(CommandEventArgs e)
        {
            var allUsrs = e.Message.MentionedUsers.Union(new User[] { e.User });
            int i = 0;
            foreach (User u in allUsrs)
            {
                var s = new CAHStats(i++);

                Players.TryAdd(u, new CAHStats(i++));
            }
            server = e.Server;
            channel = e.Channel;
            Task.Run(StartGame);
        }

        private async Task StartGame()
        {
            var TzarIndex = 0;
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
        }

        private async Task End()
        {
            ShouldStopGame = true;
            await channel.SendMessage("**Cards Against Humanity game ended**\n" + getLeaderboard());
            CardsAgainstHumanityGame throwawayvalue;
            Commands.CardsAgainstHumanity.RunningCAHs.TryRemove(server.Id, out throwawayvalue);
        }

        public async Task StopGame()
        {
            if (!ShouldStopGame)
                await channel.SendMessage(":exclamation: CAH will stop after this round");
            ShouldStopGame = true;
        }

        public async void PotentialPlacement(object sender, MessageEventArgs e)
        {
            try
            {
                //Getting the messages we want
                if (e.Channel.IsPrivate) return; //Because the default message would show up as well
                if (e.Server != server) return; 
                if (!Players.Keys.Contains(e.User)) return; //Only track current players
                if (e.User == currentTzar && !CzarDecision) return;
                if (CzarDecision && e.User != currentTzar) return;


                //if the Tzar is making the decision
                if (CzarDecision)
                {
                    int choice;
                    if (!int.TryParse(e.Message.Text, out choice))
                        return;
                    //check whether it's out of range
                    if (choice > WhiteCardsAssortment.Count - 1) return;
                    var chosen = WhiteCardsAssortment[choice];                    
                    var winner = chosen.Key;
                    var message = $"{winner.Name} has won this round!";

                    //cahCancelSource.Cancel();
                    CzarDecision = false;
                    Players[winner].Points++;
                    GameActive = false;
                    await channel.SendMessage(message);
                    //Remove chosen card of winner
                    var cards = WhiteCardsAssortment[choice].Value;
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

                var msg = e.Message.Text;
                var cardSet = new HashSet<CAHWhiteCard>();
                CAHStats stat;
                if (!Players.TryGetValue(e.User, out stat)) return;

                if (currentBlackCard.WhiteCards == 1)
                {
                    int index;
                    if (!int.TryParse(msg, out index))
                        return;
                    cardSet.Add(stat.Hand[index]);
                }
                else {
                    var split = msg.Split(',');
                    foreach (var s in split)
                    {
                        int index;
                        if (!int.TryParse(s.Trim(), out index)) return;
                        cardSet.Add(stat.Hand[index]);
                    }
                }

                
               
                var newIndex = getRandomIndex();
                var kvp = new KeyValuePair<User, HashSet<CAHWhiteCard>>(e.User, cardSet);
                WhiteCardsAssortment.AddOrUpdate(newIndex, x=> kvp, (x,y) => kvp);
                //chosenWhiteCards.Add(e.User, stat.Hand[index]); //catch the outofRangeExceptions
                var unset = unSetPlayers();
                await channel.SendMessage($"Registered chosen card(s) for {e.User.Mention}\n{unset}");
                if (unset == String.Empty) //There are no players who haven't chosen yet
                {
                    allChosen = true;
                }
            } catch (IndexOutOfRangeException)
            {
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
            var missing = Players.Where(x => !dcit.ContainsKey(x.Key) && x.Key != currentTzar);
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
            int i = 0;
            foreach (var c in stats.Hand)
            {
                sb.AppendLine($"{i++}. {c.ToString()}");
            }
            return sb.ToString();
        }

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
