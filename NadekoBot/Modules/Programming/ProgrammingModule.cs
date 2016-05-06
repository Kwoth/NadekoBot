using Discord.Modules;
using Uni.Extensions;
using Uni.Modules.Permissions.Classes;
using Uni.Modules.Programming.Commands;

namespace Uni.Modules.Programming
{
    class ProgrammingModule : DiscordModule
    {
        public override string Prefix => Uni.Config.CommandPrefixes.Programming;

        public ProgrammingModule()
        {
            commands.Add(new HaskellRepl(this));
        }

        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                cgb.AddCheck(PermissionChecker.Instance);
                commands.ForEach(c => c.Init(cgb));
            });
        }
    }
}
