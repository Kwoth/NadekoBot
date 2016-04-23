using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Pokemon
{
    class TrainerStats
    {

        public static int MaxMoves { get; } = 5;
        /// <summary>
        /// Amount of moves made since last time attacked
        /// </summary>
        public int MovesMade { get; set; } = 0;
        /// <summary>
        /// Last people attacked
        /// </summary>
        public List<ulong> LastAttacked { get; set; } = new List<ulong>();

    }
}
