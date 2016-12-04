using System;
using System.Text.RegularExpressions;

namespace NadekoBot.Modules.Searches.Models
{
    public class OverwatchApiModel
    {
        public OverwatchPatchNotes[] PatchNotes { get; set; }

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
        internal static string StripHTML(string input)
        {
            var re = Regex.Replace(input, "<.*?>", String.Empty);
            re = Regex.Replace(re, "&#160;", $@" ");
            return re;
        }
    }
}