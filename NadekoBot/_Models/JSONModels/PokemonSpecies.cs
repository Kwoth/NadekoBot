using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.JSONModels
{
    class PokemonSpecies
    {
        /// <summary>
        /// Number in the index
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// The name
        /// </summary>
        public string name { get; set; }

        public int baseExperience { get; set; }

        /// <summary>
        /// Contains following keys:
        /// attack,
        /// defense,
        /// special attack,
        /// special defense,
        /// hp,
        /// speed
        /// </summary>
        public Dictionary<string, int> baseStats { get; set; }

        /// <summary>
        /// Level at which it evolves
        /// </summary>
        public int evolveLevel { get; set; }
        
        /// <summary>
        /// Pokemon it evolves to
        /// </summary>
        public string evolveTo { get; set; }
        /// <summary>
        /// Pokemon Types
        /// </summary>
        public string[] types { get; set; }
        /// <summary>
        /// The set of moves it has; 4 random ones taken from the API
        /// </summary>
        public Dictionary<string, string> moves { get; set; }
        /// <summary>
        /// link to the image
        /// </summary>
        public string imageLink { get; set; }
    }
}
