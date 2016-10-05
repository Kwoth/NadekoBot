using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class Currency : DbEntity
    {
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

        public long Amount { get; set; }
    }
}
