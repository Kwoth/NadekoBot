using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Modules;
using Discord.Commands;
using NadekoBot.Modules.Permissions.Classes;
using Discord;

namespace NadekoBot.Modules.CustomReactions
{
    class CustomReactionsModule : DiscordModule
    {
        public override string Prefix { get; } = "";

        public override void Install(ModuleManager manager)
        {

            manager.CreateCommands("",cgb =>
            {

                cgb.AddCheck(PermissionChecker.Instance);

                foreach (var command in NadekoBot.Config.CustomReactions)
                {
                    var commandName = command.Key.Replace("%mention%", NadekoBot.BotMention);

                    var c = cgb.CreateCommand(commandName);
                    c.Description($"Custom reaction.\n**Usage**:{command.Key}");
                    c.Parameter("args", ParameterType.Unparsed);
                    c.Do(async e =>
                    {
                        Random range = new Random();
                        var ownerMentioned = e.Message.MentionedUsers.Where(x =>/* x != e.User &&*/ NadekoBot.IsOwner(x.Id));
                        var ownerReactions = command.Value.Where(x => x.Contains("%owner%")).ToList();
                        string str;

                        if (ownerMentioned.Any() && ownerReactions.Any())
                        {
                            str = ownerReactions[range.Next(0, ownerReactions.Count)];
                            str = str.Replace("%owner%", ownerMentioned.FirstOrDefault().Mention);
                        }
                        else if (ownerReactions.Any())
                        {
                            var others = command.Value.Except(ownerReactions).ToList();
                            str = others[range.Next(0, others.Count())];
                        }
                        else
                        {
                            str = command.Value[range.Next(0, command.Value.Count())];
                        }

                        str = str.Replace("%user%", e.User.Mention);
                        str = str.Replace("%rng%", "" + range.Next());
                        if (str.Contains("%target%"))
                        {
                            var args = e.GetArg("args");
                            if (string.IsNullOrWhiteSpace(args)) args = string.Empty;
                            str = str.Replace("%target%", e.GetArg("args"));
                        }

                        await e.Channel.SendMessage(str).ConfigureAwait(false);
                    });
                }
               
            });
        }
    }
}
