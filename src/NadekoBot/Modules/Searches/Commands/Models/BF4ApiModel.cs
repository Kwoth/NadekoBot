using Newtonsoft.Json;
using System.Collections.Generic;

namespace NadekoBot.Modules.Searches.Models
{
    public class BF4ApiModel
    {
        public static string countryImgUrl { get; } = "https://raw.githubusercontent.com/emcrisostomo/flags/master/png/256/";
        public static string rankImgUrl { get; } = "https://raw.githubusercontent.com/Prophet731/BFAdminCP/master/public/images/games/";
        [JsonProperty("player")]
        public Player player { get; set; }

        [JsonProperty("stats")]
        public Stats stats { get; set; }

        [JsonProperty("dogtags")]
        public Dogtags dogtags { get; set; }

        public class Dogtags
        {

            [JsonProperty("advanced")]
            public Advanced advanced { get; set; }

            [JsonProperty("basic")]
            public Basic basic { get; set; }

            public class Advanced
            {

                [JsonProperty("id")]
                public int id { get; set; }

                [JsonProperty("image")]
                public string image { get; set; }

                [JsonProperty("name")]
                public string name { get; set; }

                [JsonProperty("desc")]
                public string desc { get; set; }

                [JsonProperty("license")]
                public object license { get; set; }

                [JsonProperty("category")]
                public string category { get; set; }

                [JsonProperty("img")]
                public string img { get; set; }
            }

            public class Basic
            {

                [JsonProperty("id")]
                public int id { get; set; }

                [JsonProperty("image")]
                public string image { get; set; }

                [JsonProperty("name")]
                public string name { get; set; }

                [JsonProperty("desc")]
                public string desc { get; set; }

                [JsonProperty("license")]
                public object license { get; set; }

                [JsonProperty("category")]
                public string category { get; set; }

                [JsonProperty("img")]
                public string img { get; set; }
            }
        }

        public class Stats
        {

            [JsonProperty("reset")]
            public Reset reset { get; set; }

            [JsonProperty("scores")]
            public Scores scores { get; set; }

            [JsonProperty("skill")]
            public int skill { get; set; }

            [JsonProperty("elo")]
            public int elo { get; set; }

            [JsonProperty("rank")]
            public int rank { get; set; }

            [JsonProperty("timePlayed")]
            public int timePlayed { get; set; }

            [JsonProperty("kills")]
            public int kills { get; set; }

            [JsonProperty("deaths")]
            public int deaths { get; set; }

            [JsonProperty("headshots")]
            public int headshots { get; set; }

            [JsonProperty("shotsFired")]
            public int shotsFired { get; set; }

            [JsonProperty("shotsHit")]
            public int shotsHit { get; set; }

            [JsonProperty("suppressionAssists")]
            public int suppressionAssists { get; set; }

            [JsonProperty("avengerKills")]
            public int avengerKills { get; set; }

            [JsonProperty("saviorKills")]
            public int saviorKills { get; set; }

            [JsonProperty("nemesisKills")]
            public int nemesisKills { get; set; }

            [JsonProperty("numRounds")]
            public int numRounds { get; set; }

            [JsonProperty("numLosses")]
            public int numLosses { get; set; }

            [JsonProperty("numWins")]
            public int numWins { get; set; }

            [JsonProperty("killStreakBonus")]
            public int killStreakBonus { get; set; }

            [JsonProperty("nemesisStreak")]
            public int nemesisStreak { get; set; }

            [JsonProperty("mcomDefendKills")]
            public int mcomDefendKills { get; set; }

            [JsonProperty("resupplies")]
            public int resupplies { get; set; }

            [JsonProperty("repairs")]
            public int repairs { get; set; }

            [JsonProperty("heals")]
            public int heals { get; set; }

            [JsonProperty("revives")]
            public int revives { get; set; }

            [JsonProperty("longestHeadshot")]
            public double longestHeadshot { get; set; }

            [JsonProperty("longestWinStreak")]
            public int longestWinStreak { get; set; }

            [JsonProperty("flagDefend")]
            public int flagDefend { get; set; }

            [JsonProperty("flagCaptures")]
            public int flagCaptures { get; set; }

            [JsonProperty("killAssists")]
            public int killAssists { get; set; }

            [JsonProperty("vehiclesDestroyed")]
            public int vehiclesDestroyed { get; set; }

            [JsonProperty("vehicleDamage")]
            public int vehicleDamage { get; set; }

            [JsonProperty("dogtagsTaken")]
            public int dogtagsTaken { get; set; }

            [JsonProperty("streak")]
            public int streak { get; set; }

            [JsonProperty("bestStreak")]
            public int bestStreak { get; set; }

            [JsonProperty("modes")]
            public List<Mode> modes { get; set; }

            [JsonProperty("kits")]
            public Kits kits { get; set; }

            [JsonProperty("extra")]
            public Extra extra { get; set; }

            public class Scores
            {

                [JsonProperty("score")]
                public int score { get; set; }

                [JsonProperty("award")]
                public int award { get; set; }

                [JsonProperty("bonus")]
                public int bonus { get; set; }

                [JsonProperty("unlock")]
                public int unlock { get; set; }

                [JsonProperty("vehicle")]
                public int vehicle { get; set; }

                [JsonProperty("team")]
                public int team { get; set; }

                [JsonProperty("objective")]
                public int objective { get; set; }

                [JsonProperty("squad")]
                public int squad { get; set; }

                [JsonProperty("general")]
                public int general { get; set; }

                [JsonProperty("totalScore")]
                public int totalScore { get; set; }

                [JsonProperty("rankScore")]
                public int rankScore { get; set; }

                [JsonProperty("combatScore")]
                public int combatScore { get; set; }
            }

            public class Mode
            {

                [JsonProperty("id")]
                public long id { get; set; }

                [JsonProperty("score")]
                public int score { get; set; }

                [JsonProperty("name")]
                public string name { get; set; }
            }

            public class Kits
            {

                [JsonProperty("assault")]
                public Assault assault { get; set; }

                [JsonProperty("engineer")]
                public Engineer engineer { get; set; }

                [JsonProperty("support")]
                public Support support { get; set; }

                [JsonProperty("recon")]
                public Recon recon { get; set; }

                [JsonProperty("commander")]
                public Commander commander { get; set; }
                public class Assault
                {

                    [JsonProperty("id")]
                    public int id { get; set; }

                    [JsonProperty("score")]
                    public int score { get; set; }

                    [JsonProperty("time")]
                    public int time { get; set; }

                    [JsonProperty("stars")]
                    public int stars { get; set; }

                    [JsonProperty("spm")]
                    public double spm { get; set; }

                    [JsonProperty("name")]
                    public string name { get; set; }
                }

                public class Engineer
                {

                    [JsonProperty("id")]
                    public int id { get; set; }

                    [JsonProperty("score")]
                    public int score { get; set; }

                    [JsonProperty("time")]
                    public int time { get; set; }

                    [JsonProperty("stars")]
                    public int stars { get; set; }

                    [JsonProperty("spm")]
                    public double spm { get; set; }

                    [JsonProperty("name")]
                    public string name { get; set; }
                }

                public class Support
                {

                    [JsonProperty("id")]
                    public int id { get; set; }

                    [JsonProperty("score")]
                    public int score { get; set; }

                    [JsonProperty("time")]
                    public int time { get; set; }

                    [JsonProperty("stars")]
                    public int stars { get; set; }

                    [JsonProperty("spm")]
                    public double spm { get; set; }

                    [JsonProperty("name")]
                    public string name { get; set; }
                }

                public class Recon
                {

                    [JsonProperty("id")]
                    public int id { get; set; }

                    [JsonProperty("score")]
                    public int score { get; set; }

                    [JsonProperty("time")]
                    public int time { get; set; }

                    [JsonProperty("stars")]
                    public int stars { get; set; }

                    [JsonProperty("spm")]
                    public double spm { get; set; }

                    [JsonProperty("name")]
                    public string name { get; set; }
                }

                public class Commander
                {

                    [JsonProperty("id")]
                    public int id { get; set; }

                    [JsonProperty("score")]
                    public int score { get; set; }

                    [JsonProperty("time")]
                    public int time { get; set; }

                    [JsonProperty("stars")]
                    public int stars { get; set; }

                    [JsonProperty("spm")]
                    public double spm { get; set; }

                    [JsonProperty("name")]
                    public string name { get; set; }
                }
            }

            public class Extra
            {

                [JsonProperty("kdr")]
                public double kdr { get; set; }

                [JsonProperty("wlr")]
                public double wlr { get; set; }

                [JsonProperty("spm")]
                public double spm { get; set; }

                [JsonProperty("gspm")]
                public double gspm { get; set; }

                [JsonProperty("kpm")]
                public double kpm { get; set; }

                [JsonProperty("sfpm")]
                public double sfpm { get; set; }

                [JsonProperty("hkp")]
                public double hkp { get; set; }

                [JsonProperty("khp")]
                public double khp { get; set; }

                [JsonProperty("accuracy")]
                public double accuracy { get; set; }

                [JsonProperty("roundsFinished")]
                public int roundsFinished { get; set; }

                [JsonProperty("vehicleTime")]
                public int vehicleTime { get; set; }

                [JsonProperty("vehicleKills")]
                public int vehicleKills { get; set; }

                [JsonProperty("weaponTime")]
                public int weaponTime { get; set; }

                [JsonProperty("weaponKills")]
                public int weaponKills { get; set; }

                [JsonProperty("unknownKills")]
                public int unknownKills { get; set; }

                [JsonProperty("weaTimeP")]
                public double weaTimeP { get; set; }

                [JsonProperty("weaKillsP")]
                public double weaKillsP { get; set; }

                [JsonProperty("weaKpm")]
                public double weaKpm { get; set; }

                [JsonProperty("vehTimeP")]
                public double vehTimeP { get; set; }

                [JsonProperty("vehKillsP")]
                public double vehKillsP { get; set; }

                [JsonProperty("vehKpm")]
                public double vehKpm { get; set; }

                [JsonProperty("ribbons")]
                public int ribbons { get; set; }

                [JsonProperty("ribbonsTotal")]
                public int ribbonsTotal { get; set; }

                [JsonProperty("ribbonsUnique")]
                public int ribbonsUnique { get; set; }

                [JsonProperty("medals")]
                public int medals { get; set; }

                [JsonProperty("medalsTotal")]
                public int medalsTotal { get; set; }

                [JsonProperty("medalsUnique")]
                public int medalsUnique { get; set; }

                [JsonProperty("assignments")]
                public int assignments { get; set; }

                [JsonProperty("assignmentsTotal")]
                public int assignmentsTotal { get; set; }

                [JsonProperty("ribpr")]
                public double ribpr { get; set; }
            }


            public class Reset
            {

                [JsonProperty("lastReset")]
                public int lastReset { get; set; }

                [JsonProperty("score")]
                public int score { get; set; }

                [JsonProperty("timePlayed")]
                public int timePlayed { get; set; }

                [JsonProperty("timePlayedSinceLastReset")]
                public int timePlayedSinceLastReset { get; set; }

                [JsonProperty("kills")]
                public int kills { get; set; }

                [JsonProperty("deaths")]
                public int deaths { get; set; }

                [JsonProperty("shotsFired")]
                public int shotsFired { get; set; }

                [JsonProperty("shotsHit")]
                public int shotsHit { get; set; }

                [JsonProperty("numLosses")]
                public int numLosses { get; set; }

                [JsonProperty("numWins")]
                public int numWins { get; set; }
            }
        }

        public class Player
        {

            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("game")]
            public string game { get; set; }

            [JsonProperty("plat")]
            public string plat { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("tag")]
            public string tag { get; set; }

            [JsonProperty("dateCheck")]
            public long dateCheck { get; set; }

            [JsonProperty("dateUpdate")]
            public long dateUpdate { get; set; }

            [JsonProperty("dateCreate")]
            public long dateCreate { get; set; }

            [JsonProperty("dateStreak")]
            public long dateStreak { get; set; }

            [JsonProperty("lastDay")]
            public string lastDay { get; set; }

            [JsonProperty("country")]
            public string country { get; set; }

            [JsonProperty("countryName")]
            public string countryName { get; set; }

            [JsonProperty("rank")]
            public Rank rank { get; set; }

            [JsonProperty("score")]
            public int score { get; set; }

            [JsonProperty("timePlayed")]
            public int timePlayed { get; set; }

            [JsonProperty("uId")]
            public string uId { get; set; }

            [JsonProperty("uName")]
            public string uName { get; set; }

            [JsonProperty("uGava")]
            public string uGava { get; set; }

            [JsonProperty("udCreate")]
            public long udCreate { get; set; }

            [JsonProperty("privacy")]
            public string privacy { get; set; }

            [JsonProperty("blPlayer")]
            public string blPlayer { get; set; }

            [JsonProperty("blUser")]
            public string blUser { get; set; }

            [JsonProperty("editable")]
            public bool editable { get; set; }

            [JsonProperty("viewable")]
            public bool viewable { get; set; }

            [JsonProperty("adminable")]
            public bool adminable { get; set; }

            [JsonProperty("linked")]
            public bool linked { get; set; }
            public class Rank
            {

                [JsonProperty("nr")]
                public int nr { get; set; }

                [JsonProperty("imgLarge")]
                public string imgLarge { get; set; }

                [JsonProperty("img")]
                public string img { get; set; }

                [JsonProperty("name")]
                public string name { get; set; }

                [JsonProperty("needed")]
                public int needed { get; set; }

                [JsonProperty("next")]
                public Next next { get; set; }
                public class Next
                {

                    [JsonProperty("nr")]
                    public int nr { get; set; }

                    [JsonProperty("imgLarge")]
                    public string imgLarge { get; set; }

                    [JsonProperty("img")]
                    public string img { get; set; }

                    [JsonProperty("name")]
                    public string name { get; set; }

                    [JsonProperty("needed")]
                    public int needed { get; set; }

                    [JsonProperty("curr")]
                    public int curr { get; set; }

                    [JsonProperty("relNeeded")]
                    public int relNeeded { get; set; }

                    [JsonProperty("relCurr")]
                    public int relCurr { get; set; }

                    [JsonProperty("relProg")]
                    public double relProg { get; set; }
                }
            }
        }

    }
}
