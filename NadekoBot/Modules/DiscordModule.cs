using Discord.Modules;
using NadekoBot.Classes;
using System.Collections.Generic;

namespace NadekoBot.Modules
{
    public abstract class DiscordModule : IModule
    {
        protected readonly HashSet<DiscordCommand> commands = new HashSet<DiscordCommand>();

        public abstract string Prefix { get; }

        public abstract void Install(ModuleManager manager);
    }
}