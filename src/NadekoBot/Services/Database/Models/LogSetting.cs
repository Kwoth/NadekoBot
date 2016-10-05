using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class LogSetting : DbEntity
    {
        public bool IsLogging { get; set; }
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
        public HashSet<IgnoredLogChannel> IgnoredChannels { get; set; }

        public bool MessageUpdated { get; set; } = true;
        public bool MessageDeleted { get; set; } = true;

        public bool UserJoined { get; set; } = true;
        public bool UserLeft { get; set; } = true;
        public bool UserBanned { get; set; } = true;
        public bool UserUnbanned { get; set; } = true;
        public bool UserUpdated { get; set; } = true;

        public bool ChannelCreated { get; set; } = true;
        public bool ChannelDestroyed { get; set; } = true;
        public bool ChannelUpdated { get; set; } = true;

        //userpresence
        public bool LogUserPresence { get; set; } = false;

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="UserPresenceChannelId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("UserPresenceChannelId")]
        public long _userPresenceChannelId { get; set; }

        [NotMapped]
        public ulong UserPresenceChannelId
        {
            get { return (ulong) _userPresenceChannelId; }
            set { _userPresenceChannelId = (long) value; }
        }

        //voicepresence
        public bool LogVoicePresence { get; set; } = false;

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="VoicePresenceChannelId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("VoicePresenceChannelId")]
        public long _voicePresenceChannelId { get; set; }

        [NotMapped]
        public ulong VoicePresenceChannelId
        {
            get { return (ulong) _voicePresenceChannelId; }
            set { _voicePresenceChannelId = (long) value; }
        }
        public HashSet<IgnoredVoicePresenceChannel> IgnoredVoicePresenceChannelIds { get; set; }

    }
}
