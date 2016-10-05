using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class Reminder : DbEntity
    {
        public DateTime When { get; set; }
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
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="UserId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("UserId")]
        public long _userId { get; set; }

        [NotMapped]
        public ulong UserId
        {
            get { return (ulong) _userId; }
            set { _userId = (long) value; }
        }
        public string Message { get; set; }
        public bool IsPrivate { get; set; }
    }
}
