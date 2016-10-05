using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class ClashWar : DbEntity
    {
        public enum DestroyStars
        {
            One, Two, Three
        }
        public enum StateOfWar
        {
            Started, Ended, Created
        }

        public string EnemyClan { get; set; }
        public int Size { get; set; }
        public StateOfWar WarState { get; set; } = StateOfWar.Created;
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="GuildId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("GuildId")]
        public long _guildId { get; set; }

        [NotMapped]
        public ulong GuildId
        {
            get { return (ulong) _guildId; }
            set { _guildId = (long) value; }
        }

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="ChannelId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("ChannelId")]
        public long _channelId { get; set; }

        [NotMapped]
        public ulong ChannelId
        {
            get { return (ulong) _channelId; }
            set { _channelId = (long) value; }
        }

        [NotMapped]
        public ITextChannel Channel { get; set; }

        public List<ClashCaller> Bases { get; set; }
    }
}
