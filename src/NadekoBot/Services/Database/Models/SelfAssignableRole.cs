﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class SelfAssignedRole : DbEntity
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
            get { return Convert.ToUInt64(_guildId); }
            set { _guildId = Convert.ToInt64(value); }
        }

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="RoleId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("RoleId")]
        public long _roleId { get; set; }

        [NotMapped]
        public ulong RoleId
        {
            get { return Convert.ToUInt64(_roleId); }
            set { _roleId = Convert.ToInt64(value); }
        }
    }
}
