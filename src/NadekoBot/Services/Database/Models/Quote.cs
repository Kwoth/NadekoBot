using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class Quote : DbEntity
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
        [Required]
        public string Keyword { get; set; }
        [Required]
        public string AuthorName { get; set; }

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="AuthorId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("AuthorId")]
        public long _authorId { get; set; }

        [NotMapped]
        public ulong AuthorId
        {
            get { return (ulong) _authorId; }
            set { _authorId = (long) value; }
        }
        [Required]
        public string Text { get; set; }
    }
}
