using Newtonsoft.Json;
using System.Collections.Generic;

namespace NadekoBot.Modules.Searches.Models
{
    public class BF3ApiModel
    {
        public static string imgURL { get; } = "http://www.team-cos.com/ezStatsBF3/stylesheets/images/";
        public string plat { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public string language { get; set; }
        public string country { get; set; }
        public string country_name { get; set; }
        public string country_img { get; set; }
        public int date_insert { get; set; }
        public int date_update { get; set; }
        public Dogtags dogtags { get; set; }
        public Stats stats { get; set; }
        public string status { get; set; }
        public class Dogtags
        {
            public Basic basic { get; set; }
            public Advanced advanced { get; set; }
            public class Basic
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string license { get; set; }
                public string image_l { get; set; }
                public string image_s { get; set; }
            }

            public class Advanced
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string license { get; set; }
                public string image_l { get; set; }
                public string image_s { get; set; }
            }
        }
        public class Stats
        {
            public int date_insert { get; set; }
            public int date_update { get; set; }
            public int date_check { get; set; }
            public string checkstate { get; set; }
            public Rank rank { get; set; }
            public List<Nextrank> nextranks { get; set; }
            public Scores scores { get; set; }
            public Global global { get; set; }
            public Coop coop { get; set; }
            public Teams teams { get; set; }
            public Vehcats vehcats { get; set; }
            public Kits kits { get; set; }
            public Specializations specializations { get; set; }
            public Gamemodes gamemodes { get; set; }
        }
        public class Rank
        {
            public int nr { get; set; }
            public string name { get; set; }
            public int score { get; set; }
            public string img_large { get; set; }
            public string img_medium { get; set; }
            public string img_small { get; set; }
            public string img_tiny { get; set; }
        }
        public class Nextrank
        {
            public int nr { get; set; }
            public string name { get; set; }
            public int score { get; set; }
            public string img_large { get; set; }
            public string img_medium { get; set; }
            public string img_small { get; set; }
            public string img_tiny { get; set; }
            public int left { get; set; }
        }
        public class Scores
        {
            public int score { get; set; }
            public int award { get; set; }
            public int assault { get; set; }
            public int bonus { get; set; }
            public int engineer { get; set; }
            public int general { get; set; }
            public int objective { get; set; }
            public int recon { get; set; }
            public int squad { get; set; }
            public int support { get; set; }
            public int team { get; set; }
            public int unlock { get; set; }
            public int vehicleaa { get; set; }
            public int vehicleah { get; set; }
            public int vehicleall { get; set; }
            public int vehicleifv { get; set; }
            public int vehiclejet { get; set; }
            public int vehiclembt { get; set; }
            public int vehiclesh { get; set; }
            public int vehiclelbt { get; set; }
            public int vehicleart { get; set; }
        }
        public class Global
        {
            public int kills { get; set; }
            public int deaths { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int shots { get; set; }
            public int hits { get; set; }
            public int headshots { get; set; }
            public double longesths { get; set; }
            public int time { get; set; }
            public int vehicletime { get; set; }
            public int vehiclekills { get; set; }
            public int revives { get; set; }
            public double killassists { get; set; }
            public int resupplies { get; set; }
            public double heals { get; set; }
            public int repairs { get; set; }
            public int rounds { get; set; }
            public double elo { get; set; }
            public int elo_games { get; set; }
            public int killstreakbonus { get; set; }
            public int vehicledestroyassist { get; set; }
            public int vehicledestroyed { get; set; }
            public int dogtags { get; set; }
            public int avengerkills { get; set; }
            public int saviorkills { get; set; }
            public int damagaassists { get; set; }
            public int suppression { get; set; }
            public int nemesisstreak { get; set; }
            public int nemesiskills { get; set; }
            public int mcomdest { get; set; }
            public int mcomdefkills { get; set; }
            public int flagcaps { get; set; }
            public int flagdef { get; set; }
            public double longesthandhs { get; set; }
            public double time_gunm { get; set; }
            public double time_scv { get; set; }
        }
        public class Coop
        {
            public int kills { get; set; }
            public int headshots { get; set; }
            public int mdrevives { get; set; }
            public int killassists { get; set; }
            public int spotassists { get; set; }
            public int vehicledestroyed { get; set; }
            public int avengerkills { get; set; }
            public int saviorkills { get; set; }
            public int score { get; set; }
            public int indscore { get; set; }
            public int rank { get; set; }
        }
        public class Teams
        {
            public us US { get; set; }
            public ru RU { get; set; }
            public class us
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int headshots { get; set; }
                public int shots { get; set; }
                public int hits { get; set; }
            }

            public class ru
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int headshots { get; set; }
                public int shots { get; set; }
                public int hits { get; set; }
            }
        }
        public class Vehcats
        {
            public Vehiclesh vehiclesh { get; set; }
            public Vehicleja vehicleja { get; set; }
            public Vehicleifv vehicleifv { get; set; }
            public Vehicleah vehicleah { get; set; }
            public Vehiclembt vehiclembt { get; set; }
            public Vehicleaa vehicleaa { get; set; }
            public Vehiclejf vehiclejf { get; set; }
            public Vehiclelbt vehiclelbt { get; set; }
            public Vehicleart vehicleart { get; set; }
            public class Vehicleart
            {
                public string name { get; set; }
                public int time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehiclelbt
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehiclejf
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehicleaa
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehiclembt
            {
                public string name { get; set; }
                public int time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehicleah
            {
                public string name { get; set; }
                public int time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehicleifv
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehicleja
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehiclesh
            {
                public string name { get; set; }
                public double time { get; set; }
                public int kills { get; set; }
                public int score { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
        }
        public class Kits
        {
            public Assault assault { get; set; }
            public Engineer engineer { get; set; }
            public Recon recon { get; set; }
            public Vehicle vehicle { get; set; }
            public Support support { get; set; }
            public General general { get; set; }
            public class Assault
            {
                public string name { get; set; }
                public string type { get; set; }
                public int score { get; set; }
                public int time { get; set; }
                public int timer { get; set; }
                public int timeu { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Engineer
            {
                public string name { get; set; }
                public string type { get; set; }
                public int score { get; set; }
                public int time { get; set; }
                public double timer { get; set; }
                public double timeu { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Recon
            {
                public string name { get; set; }
                public string type { get; set; }
                public int score { get; set; }
                public int time { get; set; }
                public int timer { get; set; }
                public int timeu { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Vehicle
            {
                public string name { get; set; }
                public string type { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class Support
            {
                public string name { get; set; }
                public string type { get; set; }
                public int score { get; set; }
                public int time { get; set; }
                public double timer { get; set; }
                public double timeu { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
            public class General
            {
                public string name { get; set; }
                public string type { get; set; }
                public string img { get; set; }
                public string img_bk { get; set; }
            }
        }
        public class Specializations
        {
            public Sprint sprint { get; set; }
            public Ammo ammo { get; set; }
            public Explresist explresist { get; set; }
            public Explosives explosives { get; set; }
            public Supprresist supprresist { get; set; }
            public Suppression suppression { get; set; }
            public Grenades grenades { get; set; }
            public Sprint2 sprint2 { get; set; }
            public Ammo2 ammo2 { get; set; }
            public Explresist2 explresist2 { get; set; }
            public Explosives2 explosives2 { get; set; }
            public Suppression2 suppression2 { get; set; }
            public Supprresist2 supprresist2 { get; set; }
            public Grenades2 grenades2 { get; set; }
            public class Sprint
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Ammo
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Explresist
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Explosives
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Supprresist
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Suppression
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Grenades
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Sprint2
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Ammo2
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Explresist2
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Explosives2
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Suppression2
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Supprresist2
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }

            public class Grenades2
            {
                public string name { get; set; }
                public string desc { get; set; }
                public string img { get; set; }
                public int curr { get; set; }
                public int needed { get; set; }
                public string nname { get; set; }
            }
        }
        public class Gamemodes
        {
            public Conquest conquest { get; set; }
            public Rush rush { get; set; }
            public Squadrush squadrush { get; set; }
            public Teamdm teamdm { get; set; }
            public Squaddm squaddm { get; set; }
            public class Conquest
            {
                public string name { get; set; }
                public int losses { get; set; }
                public int wins { get; set; }
            }

            public class Rush
            {
                public string name { get; set; }
                public int losses { get; set; }
                public int wins { get; set; }
            }

            public class Squadrush
            {
                public string name { get; set; }
                public int losses { get; set; }
                public int wins { get; set; }
            }

            public class Teamdm
            {
                public string name { get; set; }
                public int losses { get; set; }
                public int wins { get; set; }
            }

            public class Squaddm
            {
                public string name { get; set; }
                public int losses { get; set; }
                public int wins { get; set; }
            }
        }
    }
}
