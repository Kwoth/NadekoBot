using Discord.Commands;
using NadekoBot.Classes;
using NadekoBot.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;

namespace NadekoBot.Modules.Gambling
{
    internal class DiceRollCommand : DiscordCommand
    {

        public DiceRollCommand(DiscordModule module) : base(module) { }


        internal override void Init(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand(Module.Prefix + "roll")
                .Description("Rolls 0-100. If you supply a number [x] it rolls up to 30 normal dice." +
                             " If you split 2 numbers with letter d (xdy) it will roll x dice from 1 to y.\n**Usage**: $roll or $roll 7 or $roll 3d5")
                .Parameter("num", ParameterType.Optional)
                .Do(RollFunc());
            cgb.CreateCommand(Module.Prefix + "nroll")
                .Description("Rolls in a given range.\n**Usage**: `$nroll 5` (rolls 0-5) or `$nroll 5-15`")
                .Parameter("range", ParameterType.Required)
                .Do(NRollFunc());
        }

        private static double Evaluate(string expression)
        {
            DataTable dataTable = new DataTable();
            DataColumn column = new DataColumn("Eval", typeof(double), expression);
            dataTable.Columns.Add(column);
            dataTable.Rows.Add(new object[1] { (object)0 });
            return (double)dataTable.Rows[0]["Eval"];
        }

        private Image GetDice(int num) => num != 10
                                          ? Properties.Resources.ResourceManager.GetObject("_" + num) as Image
                                          : new[]
                                            {
                                              (Properties.Resources.ResourceManager.GetObject("_" + 1) as Image),
                                              (Properties.Resources.ResourceManager.GetObject("_" + 0) as Image),
                                            }.Merge();


        Regex dndRegex = new Regex(@"(?<n1>\d+)d(?<n2>\d+)", RegexOptions.Compiled);
        private Func<CommandEventArgs, Task> RollFunc()
        {
            var r = new Random();
            return async e =>
            {
                var arg = e.Args[0]?.Trim();
                if (string.IsNullOrWhiteSpace(arg))
                {
                    var gen = r.Next(0, 101);

                    var num1 = gen / 10;
                    var num2 = gen % 10;

                    var imageStream = new Image[2] { GetDice(num1), GetDice(num2) }.Merge().ToStream(ImageFormat.Png);

                    await e.Channel.SendFile("dice.png", imageStream).ConfigureAwait(false);
                    return;
                }
                if (arg.IndexOf('d') != -1 && arg.IndexOf('d') != 0)
                {
                    try { 
                        string rollExpression = "";
                        int prevSubstring = 0;
                        foreach (Match match in dndRegex.Matches(arg))
                        {
                            int newSubstring = arg.IndexOf(match.ToString(), StringComparison.Ordinal);
                            string preInfo = arg.Substring(prevSubstring, newSubstring - prevSubstring);
                            prevSubstring = match.ToString().Length + newSubstring;
                            int n1 = 0;
                            int n2 = 0;
                            string computedRolls = "";
                            if (int.TryParse(match.Groups["n1"].ToString(), out n1) &&
                                int.TryParse(match.Groups["n2"].ToString(), out n2) &&
                                n1 <= 50 && n2 <= 100000 && n1 > 0 && n2 > 0)
                            {
                                var arr = new int[n1];
                                for (int i = 0; i < n1; i++)
                                {
                                    arr[i] += r.Next(1, n2 + 1);
                                }
                                int elemCnt = 0;
                                computedRolls = "(" +string.Join("+", arr.OrderBy(x => x).Select(x => elemCnt++ % 2 == 0 ? $"{x}" : x.ToString())) + ")";
                            }
                            rollExpression += preInfo + computedRolls;
                        }
                        rollExpression += arg.Substring(prevSubstring, arg.Length - prevSubstring);
                        rollExpression = rollExpression.Replace(" ", string.Empty);
                        double answer = Evaluate(rollExpression);
                        await e.Channel.SendMessage($"`Rolled {rollExpression}`\n`Result:` {answer}" ).ConfigureAwait(false);
                        return;
                    }
                    catch (Exception ex)
                    {
                        await e.Channel.SendMessage("`" + ex.Message + "`").ConfigureAwait(false);
                    }
                }
                try
                {
                    var num = int.Parse(e.Args[0]);
                    if (num < 1) num = 1;
                    if (num > 30)
                    {
                        await e.Channel.SendMessage("You can roll up to 30 dice at a time.").ConfigureAwait(false);
                        num = 30;
                    }
                    var dices = new List<Image>(num);
                    var values = new List<int>(num);
                    for (var i = 0; i < num; i++)
                    {
                        var randomNumber = r.Next(1, 7);
                        var toInsert = dices.Count;
                        if (randomNumber == 6 || dices.Count == 0)
                            toInsert = 0;
                        else if (randomNumber != 1)
                            for (var j = 0; j < dices.Count; j++)
                            {
                                if (values[j] < randomNumber)
                                {
                                    toInsert = j;
                                    break;
                                }
                            }
                        dices.Insert(toInsert, GetDice(randomNumber));
                        values.Insert(toInsert, randomNumber);
                    }

                    var bitmap = dices.Merge();
                    await e.Channel.SendMessage(values.Count + " Dice rolled. Total: **" + values.Sum() + "** Average: **" + (values.Sum() / (1.0f * values.Count)).ToString("N2") + "**").ConfigureAwait(false);
                    await e.Channel.SendFile("dice.png", bitmap.ToStream(ImageFormat.Png)).ConfigureAwait(false);
                }
                catch
                {
                    await e.Channel.SendMessage("Please enter a number of dice to roll.").ConfigureAwait(false);
                }
            };
        }


        private Func<CommandEventArgs, Task> NRollFunc() =>
            async e =>
            {
                try
                {
                    int rolled;
                    if (e.GetArg("range").Contains("-"))
                    {
                        var arr = e.GetArg("range").Split('-')
                                                 .Take(2)
                                                 .Select(int.Parse)
                                                 .ToArray();
                        if (arr[0] > arr[1])
                            throw new ArgumentException("First argument should be bigger than the second one.");
                        rolled = new Random().Next(arr[0], arr[1] + 1);
                    }
                    else
                    {
                        rolled = new Random().Next(0, int.Parse(e.GetArg("range")) + 1);
                    }

                    await e.Channel.SendMessage($"{e.User.Mention} rolled **{rolled}**.").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await e.Channel.SendMessage($":anger: {ex.Message}").ConfigureAwait(false);
                }
            };
    }
}