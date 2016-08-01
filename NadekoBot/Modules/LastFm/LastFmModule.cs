using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Modules;
using NadekoBot.Extensions;
using NadekoBot.Modules.LastFm.Commands;
using NadekoBot.Modules.Permissions.Classes;

namespace NadekoBot.Modules.LastFm
{
	internal class LastFmModule : DiscordModule
	{
		public LastFmModule()
		{
			commands.Add(new LastFmCommands(this));
		}

		public override string Prefix => NadekoBot.Config.CommandPrefixes.LastFm;

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
