using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class MusicPlaylist : DbEntity
    {
        public string Name { get; set; }
        public string Author { get; set; }
        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="AuthorId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("AuthorId")]
        public long _authorId { get; set; }

        [NotMapped]
        public ulong AuthorId
        {
            get { return (ulong)_authorId; }
            set { _authorId = (long)value; }
        }
        public List<PlaylistSong> Songs { get; set; }
    }
}
