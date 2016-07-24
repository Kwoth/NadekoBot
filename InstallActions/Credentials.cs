using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallActions
{
    public class Credentials
    {
        public string Token { get; set; } = "";
        public string ClientId { get; set; } = "170254782546575360";
        public ulong BotId { get; set; } = 1231231231231;
        public ulong[] OwnerIds { get; set; } = { 123123123123, 5675675679845 };
        public string GoogleAPIKey { get; set; } = "";
        public string SoundCloudClientID { get; set; } = "";
        public string MashapeKey { get; set; } = "";
        public string LOLAPIKey { get; set; } = "";
        public string TrelloAppKey { get; set; } = "";
        public string CarbonKey { get; set; } = "";
        public string OsuAPIKey { get; set; } = "";
    }
}
