using NadekoBot.DataModels;
using System;

namespace NadekoBot.DataModels
{
    class Feed : IDataModel
    {
        public string Name { get; set; }
        public long ChannelId { get; set; }
        public DateTimeOffset lastUpdated { get; set; }
        public string Link { get; set; }
    }
}
