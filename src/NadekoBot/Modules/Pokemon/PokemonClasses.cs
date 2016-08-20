using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace NadekoBot.Modules.Pokemon
{
    public class PokemonServer
    {
        public long ServerId { get; set; }
        public List<PokemonPlayer> Players { get; set; } = new List<PokemonPlayer>();
        //TODO add some extras like disabled players etc

    }
    //TODO make this class serializable
    public class PokemonPlayer
    {
        public long UserId { get; set; }
        public List<PokemonPlayer> Attacked { get; set; } = new List<PokemonPlayer>();
        public PokemonStats Stats { get; set; }
        public PokemonPlayer Duelist { get; set; }
        public Dictionary<PokemonMove, int> Moves { get; set; } = new Dictionary<PokemonMove, int>();
        //public List<PokemonMove> Moves { get; set; } = new List<PokemonMove>();
        public PokemonType PokemonType { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            var pl = obj as PokemonPlayer;
            //this could allow 'equal' players from different servers
            return pl != null && pl.UserId == UserId;
        }

        public override int GetHashCode() => UserId.GetHashCode();
    }

    public class PokemonStats
    {
        //todo consider having a unique name
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        
    }

    public class PokemonMove
    {
        public string Name { get; set; }
        public string MoveType { get; set; }
        public short PP { get; set; }
        public int Power { get; set; }
    }

    public class PokemonType
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<string> Weaknesses { get; set; } = new List<string>();
        public List<string> Strenghts { get; set; } = new List<string>();
    }

    public static class PokemonExtensions
    {
        public static PokemonPlayer GetOrAdd(this List<PokemonPlayer> list, ulong playerId, System.Func<long, PokemonPlayer> generator)
        {
            var player = list.FirstOrDefault(u => u.UserId == (long)playerId);
            if (player != null) return player;
            player = generator.Invoke((long)playerId);
            list.Add(player);
            return player;
        }
        public static PokemonType ToPokemonType(this string name)
        {
            throw new System.NotImplementedException();
        }

        public static int CalculateDamage(this PokemonMove move, PokemonPlayer attacker, PokemonPlayer defender)
        {
            //TODO make this a nice formula~
            double mod = 1;
            if (defender.PokemonType.Weaknesses.Any(x => x == move.MoveType)) mod = 2;
            else if (defender.PokemonType.Strenghts.Any(x => x == move.MoveType)) mod = 0.5;
            var rand = new Random();
            //TODO factor in strength?
            return (int)Math.Round(rand.Next(80, 120) * mod, 0);
        }
        public static bool IsFainted(this PokemonPlayer player) => player.Stats.Health <= 0;

        public static string Reward(this PokemonPlayer attacker)
        {
            var random = new Random().Next(1, 101) / 10;
            string reward = "yet to fill";
            switch (random) //TODO think of some nice rewards like an increase in agility
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                default:
                    break;
            }
            return reward;
        }
    }

}