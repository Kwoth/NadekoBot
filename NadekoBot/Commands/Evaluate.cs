using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Commands;
using NadekoBot.Extensions;
using NadekoBot.Modules;
using Mathos.Parser;
using System.Text.RegularExpressions;

namespace NadekoBot.Commands
{
    internal class Evaluate : DiscordCommand
    {
        private CustomParser parser = new CustomParser();
        public Func<CommandEventArgs, Task> DoFunc() => async e => {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            
            string expression = e.GetArg("Expression");
            sw.Start();
            string answer = evaluate(expression);
            sw.Stop();
            
            if (answer == null)
            {
                await e.Channel.SendMessage($"Expression {expression} failed to evaluate");
                return;
            }
            await e.Channel.SendMessage($"result: {answer}\n ticks: {sw.ElapsedTicks}\n milliseconds: {sw.ElapsedMilliseconds}");
           
        };

        private string evaluate(string expression)
        {
            
            expression = Regex.Replace(expression, @"\s+", "");
            expression = Regex.Replace(expression, @"\d+\!", new MatchEvaluator(FactorialString));
            try
            {
                string result = parser.Parse(expression).ToString();
                return result;
            } catch(System.OverflowException e)
            {
                return $"Overflow error on {expression}";
            } catch(System.FormatException e)
            {
                return $"\"{expression}\" was not formatted correctly";
            }

            
            
        }

        class CustomParser : MathParser
        {
            public CustomParser() : base()
            {
                Console.WriteLine("Customizing here");
                this.OperatorAction.Add("!", (x, y) => factorial(x));
                this.OperatorAction.Add("_", (x, y) => 10);
            }
        }

        internal override void Init(CommandGroupBuilder cgb)
        {       


            cgb.CreateCommand(">eval")
                .Description("Evaluates expression given")
                .Parameter("Expression", ParameterType.Unparsed)
                .Do(DoFunc());
        }

        public Evaluate(DiscordModule module) : base(module) {

        }

        static decimal factorial(decimal x)
        {
            Console.WriteLine("pass");
            if (x == 1 || x == 0)
            {
               
                return 1;
            }
            else
            {
                return x * factorial(x - 1);
            }
        }

        static string FactorialString(Match m)
        {
            Console.WriteLine(m.Value);
            return m.Value + "0";
        }
    }
}
