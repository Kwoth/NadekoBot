﻿using Discord.Commands;
using Discord.Modules;
using Uni.Classes.Help.Commands;
using Uni.Extensions;
using Uni.Modules.Permissions.Classes;
using System.Linq;

namespace Uni.Modules.Help
{
    internal class HelpModule : DiscordModule
    {

        public HelpModule()
        {
            commands.Add(new HelpCommand(this));
        }

        public override string Prefix { get; } = Uni.Config.CommandPrefixes.Help;

        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                cgb.AddCheck(PermissionChecker.Instance);
                commands.ForEach(com => com.Init(cgb));

                cgb.CreateCommand(Prefix + "modules")
                    .Alias(".modules")
                    .Description("List all bot modules.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("`List of modules:` \n• " + string.Join("\n• ", Uni.Client.GetService<ModuleService>().Modules.Select(m => m.Name)))
                                       .ConfigureAwait(false);
                    });

                cgb.CreateCommand(Prefix + "commands")
                    .Alias(".commands")
                    .Description("List all of the bot's commands from a certain module.")
                    .Parameter("module", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var cmds = Uni.Client.GetService<CommandService>().AllCommands
                                                    .Where(c => c.Category.ToLower() == e.GetArg("module").Trim().ToLower());
                        var cmdsArray = cmds as Command[] ?? cmds.ToArray();
                        if (!cmdsArray.Any())
                        {
                            await e.Channel.SendMessage("That module does not exist.").ConfigureAwait(false);
                            return;
                        }
                        await e.Channel.SendMessage("`List of commands:` \n• " + string.Join("\n• ", cmdsArray.Select(c => c.Text)))
                                       .ConfigureAwait(false);
                    });
            });
        }
    }
}
