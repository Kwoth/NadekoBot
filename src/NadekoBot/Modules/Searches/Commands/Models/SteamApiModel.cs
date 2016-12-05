using System.Collections.Generic;

namespace NadekoBot.Modules.Searches.Models
{
    public class SteamApiModel
    {
        //Search User
        public SteamQueryUser Response { get; set; }
        public class SteamQueryUser
        {
            public string steamid { get; set; } = "";
            public string message { get; set; } = "";
            public int success { get; set; }

            public SteamPlayers[] Players { get; set; }
            public class SteamPlayers
            {
                public string steamid { get; set; }
                public int communityvisibilitystate { get; set; }
                public int profilestate { get; set; }
                public string personaname { get; set; }
                public ulong lastlogoff { get; set; }
                public string profileurl { get; set; }
                public string avatar { get; set; }
                public string avatarmedium { get; set; }
                public string avatarfull { get; set; }
                public int personastate { get; set; }
                public string primaryclanid { get; set; }
                public ulong timecreated { get; set; }
                public int personastateflags { get; set; }
                public string loccountrycode { get; set; }
            }
        }

        //Player Friends List
        public SteamPlayerFriends FriendsList { get; set; }
        public class SteamPlayerFriends
        {
            public GetFriends[] Friends { get; set; }
            public class GetFriends
            {
                public string steamid { get; set; } //64 bit Steam ID to return friends list
                public string relationship { get; set; } //Relationship filter. Possibles values: all, friend.
                public ulong friend_since { get; set; } //Unix timestamp
            }
        }

        //Player Achievements
        public SteamPlayerStats PlayerStats { get; set; }
        public class SteamPlayerStats
        {
            public string steamID { get; set; }
            public string gameName { get; set; }
            public SteamPlayerAchievements[] Achievements { get; set; }
            public class SteamPlayerAchievements
            {
                string apiname { get; set; }
                int achieved { get; set; } //1 true - 0 false
            }
        }

        //User Persona Enum
        public enum profilestate
        {
            Offline = 0,
            Online = 1,
            Busy = 2,
            Away = 3,
            Snooze = 4,
            Private = 0
        }
    }
}
