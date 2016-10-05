using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class GuildConfig : DbEntity
    {
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
        public bool DeleteMessageOnCommand { get; set; }

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="AutoAssignRoleId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("AutoAssignRoleId")]
        public long _autoAssignRoleId { get; set; }

        [NotMapped]
        public ulong AutoAssignRoleId
        {
            get { return (ulong) _autoAssignRoleId; }
            set { _autoAssignRoleId = (long) value; }
        }

        //greet stuff
        public bool AutoDeleteGreetMessages { get; set; }
        public bool AutoDeleteByeMessages { get; set; }
        public int AutoDeleteGreetMessagesTimer { get; set; } = 30;

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="GreetMessageChannelId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("GreetMessageChannelId")]
        public long _greetMessageChannelId { get; set; }

        [NotMapped]
        public ulong GreetMessageChannelId
        {
            get { return (ulong) _greetMessageChannelId; }
            set { _greetMessageChannelId = (long) value; }
        }

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="ByeMessageChannelId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("ByeMessageChannelId")]
        public long _byeMessageChannelId { get; set; }

        [NotMapped]
        public ulong ByeMessageChannelId
        {
            get { return (ulong) _byeMessageChannelId; }
            set { _byeMessageChannelId = (long) value; }
        }

        public bool SendDmGreetMessage { get; set; }
        public string DmGreetMessageText { get; set; } = "Welcome to the %server% server, %user%!";

        public bool SendChannelGreetMessage { get; set; }
        public string ChannelGreetMessageText { get; set; } = "Welcome to the %server% server, %user%!";

        public bool SendChannelByeMessage { get; set; }
        public string ChannelByeMessageText { get; set; } = "%user% has left!";

        public LogSetting LogSetting { get; set; } = new LogSetting();

        //self assignable roles
        public bool ExclusiveSelfAssignedRoles { get; set; }
        public bool AutoDeleteSelfAssignedRoleMessages { get; set; }
        public float DefaultMusicVolume { get; set; } = 1.0f;
        public bool VoicePlusTextEnabled { get; set; }

        //stream notifications
        public List<FollowedStream> FollowedStreams { get; set; } = new List<FollowedStream>();

        //currencyGeneration
        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="GenerateCurrencyChannelId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("GenerateCurrencyChannelId")]
        public long? _generateCurrencyChannelId { get; set; }

        [NotMapped]
        public ulong? GenerateCurrencyChannelId
        {
            get { return _generateCurrencyChannelId.HasValue ? (ulong) _generateCurrencyChannelId : (ulong?) null; }
            set { _generateCurrencyChannelId = (long) value; }
        }

        //permissions
        public Permission RootPermission { get; set; }
        public bool VerbosePermissions { get; set; }
        public string PermissionRole { get; set; } = "Nadeko";

        //filtering
        public bool FilterInvites { get; set; }
        public HashSet<FilterChannelId> FilterInvitesChannelIds { get; set; }

        public bool FilterWords { get; set; }
        public HashSet<FilteredWord> FilteredWords { get; set; }
        public HashSet<FilterChannelId> FilterWordsChannelIds { get; set; }
    }

    public class FilterChannelId :DbEntity
    {
        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="ChannelId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("ChannelId")]
        public long _channelId { get; set; }

        [NotMapped]
        public ulong ChannelId
        {
            get { return (ulong)_channelId; }
            set { _channelId = (long)value; }
        }
    }

    public class FilteredWord :DbEntity
    {
        public string Word { get; set; }
    }
}
