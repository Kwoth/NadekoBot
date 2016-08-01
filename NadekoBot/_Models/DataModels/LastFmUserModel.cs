using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.DataModels
{
	internal class LastFmUser : IDataModel
	{
		public long DiscordUserId { get; set; }

		public string LastFmUsername { get; set; }
	}
}
