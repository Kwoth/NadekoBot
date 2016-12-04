using System;
using System.Text.RegularExpressions;

namespace NadekoBot.Modules.Searches.Models
{
    public class OverwatchApiModel
    {

        //patch notes
        public OverwatchPatchNotes[] PatchNotes { get; set; }
        public OverwatchPagination[] Pagination { get; set; }

        //achievements
        public OverwatchAchievements[] Achievements { get; set; }
        public float totalNumberOfAchievements { get; set; }
        public float numberOfAchievementsCompleted { get; set; }
        public string finishedAchievements { get; set; }

        public class OverwatchPatchNotes
        {
            public bool Missing { get; set; } = false;
            public string program { get; set; }
            public string locale { get; set; }
            public string type { get; set; }
            public string patchVersion { get; set; }
            public string status { get; set; }
            public string detail { get; set; }
            public float buildNumber { get; set; }
            public float publish { get; set; }
            public float created { get; set; }
            public bool updated { get; set; }
            public string slug { get; set; }
            public string version { get; set; }
        }

        public class OverwatchPagination
        {
            public float totalEntries { get; set; }
            public float totalPages { get; set; }
            public float pageSize { get; set; }
            public float page { get; set; }
        }

        public class OverwatchAchievements
        {
            public string name { get; set; }
            public bool finished { get; set; }
            public string image { get; set; }
            public string description { get; set; }
        }

            internal static string StripHTML(string input)
        {
            var re = Regex.Replace(input, "<.*?>", String.Empty);
            re = Regex.Replace(re, "&#160;", $@" ");
            return re;
        }
    }
}