
using NadekoBot.Classes.JSONModels;
using NadekoBot.DataModels;
using NadekoBot.JSONModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Pokemon
{
    class PokemonMain
    {
        public static PokemonMain Instance { get; } = new PokemonMain();

        public List<PokemonSpecies> pokemonClasses { get; set; }

        private readonly string path = "data/pokemonlist.json";

        private PokemonMain()
        {
            Reload();
        }

        public void Reload()
        {

            try
            {
                pokemonClasses = JsonConvert.DeserializeObject<List<PokemonSpecies>>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing {path}: {ex.Message}");
            }

        }

    }
}
namespace NadekoBot.Modules.Pokemon.Extensions
{
    static class PokemonExtension
    {
        /// <summary>
        /// Species of Sprite
        /// </summary>
        /// <param name="pkm"></param>
        /// <returns></returns>
        public static PokemonSpecies GetSpecies(this PokemonSprite pkm)
        {
            return PokemonMain.Instance.pokemonClasses.Where(x => x.number == pkm.SpeciesId).DefaultIfEmpty(null).First();
        }

        public static string PokemonString(this PokemonSprite pkm)
        {
            var species = pkm.GetSpecies();
            var str = $"**Name**: {pkm.NickName}\n" +
                $"**Species**: {species.name}\n{ImageUrl(species.imageLink, pkm.IsShiny)}\n" +
                $"**HP**: {pkm.HP}/{pkm.MaxHP}\n" +
                $"**Level**: {pkm.Level}\n" +
                $"**XP**: {pkm.XP}/{pkm.XPRequired()}\n"+
                $"**Stats**\n" +
                $"**Attack:** {pkm.Attack}\n" +
                $"**Defense:** {pkm.Defense}\n" +
                $"**Speed:** {pkm.Speed}\n" +
                "**Moves**:\n";
            foreach (var move in species.moves)
            {
                str += $"**{move.Key}** *{move.Value}*\n";
            }
            return str;
        }

        private static string ImageUrl(string normalUrl, bool shiny)
        {
            if (shiny)
                return normalUrl.Insert(40, "shiny/");
            else return normalUrl;
        }

        public static int XPRequired(this PokemonSprite pkm)
        {
            //Using fast (http://bulbapedia.bulbagarden.net/wiki/Experience)
            return (int)Math.Floor((4 * Math.Pow(pkm.Level, 3)) / 5);
        }

        public static int Reward(this PokemonSprite pkm, PokemonSprite defeated)
        {
            var reward = CalcXPReward(pkm, defeated);
            pkm.XP += reward;
            if (pkm.XP > pkm.XPRequired())
            {
                pkm.LevelUp();
            }
            return reward;
        }


        

        private static int CalcXPReward(PokemonSprite winner, PokemonSprite loser)
        {
            var a = 1;
            var b = loser.GetSpecies().baseExperience;
            var L = loser.Level;
            var s = 1;
            var L_p = winner.Level;
            var t = 1;
            //Give them all a lucky egg
            var e = 1.5;
            var p = 1;
            var result = (((a * b * L) / (5 * s)) * (Math.Pow(2 * L + 10, 2.5) / Math.Pow(L + L_p + 10, 2.5)) + 1) * t * e * p;
            return (int)Math.Ceiling(result);
        }

        public static List<PokemonType> GetPokemonTypes(this PokemonSpecies spe)
        {
            var list = new List<PokemonType>();
            foreach (var typeString in spe.types)
            {
                var t = typeString.ToUpperInvariant();
                list.Add(NadekoBot.Config.PokemonTypes.Where(x => x.Name == t).FirstOrDefault());
            }
            return list;
        }

        public static PokemonType StringToPokemonType(this string s)
        {
            var str = s.ToUpperInvariant();
            return NadekoBot.Config.PokemonTypes.Where(x => x.Name == str).DefaultIfEmpty(null).FirstOrDefault();

        }

        /// <summary>
        /// levels up the pokemon, along with all the accompanying changes; including evolution
        /// </summary>
        /// <param name="pkm"></param>
        /// <returns></returns>
        public static void LevelUp(this PokemonSprite pkm)
        {
            Random rng = new Random();
            var species = pkm.GetSpecies();
            var baseStats = species.baseStats;
            pkm.Level += 1;

            //Up them stats
            pkm.MaxHP = (int)Math.Ceiling((((baseStats["hp"] + rng.Next(0, 12)) + (Math.Sqrt((655535 / 100) * pkm.Level) / 4) * pkm.Level) / 100 + pkm.Level + 10));
            pkm.Attack = CalcStat(baseStats["attack"], pkm.Level);
            pkm.Defense = CalcStat(baseStats["defense"], pkm.Level);
            pkm.SpecialAttack = CalcStat(baseStats["special-attack"], pkm.Level);
            pkm.SpecialDefense = CalcStat(baseStats["special-defense"], pkm.Level);
            var newHp = pkm.HP + (pkm.MaxHP / 10);
            pkm.HP = (newHp > pkm.MaxHP) ? pkm.MaxHP : newHp;
            pkm.Speed = CalcStat(baseStats["speed"], pkm.Level);

            //Will it evolve!?
            var evolveLevel = species.evolveLevel;
            if (evolveLevel > 0)
            {
                if (evolveLevel == pkm.Level)
                {
                    //*GASP* IT'S GONNA EVOLVE
                    //Play an animation?
                    int newSpecies = int.Parse(species.evolveTo);
                    pkm.SpeciesId = newSpecies;
                }
            }


        }

        private static int CalcStat(int _base, int level)
        {
            Random rng = new Random();
            var m = (((_base + rng.Next(0, 12)) * 2 + (Math.Sqrt((655535 / 100) * level) / 4)) * level / 100) + level + 5;
            return (int)Math.Ceiling(m);
        }


    }
}
