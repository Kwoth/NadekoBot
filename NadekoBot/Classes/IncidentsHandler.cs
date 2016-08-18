using NadekoBot.DataModels;
using System;

namespace NadekoBot.Classes
{
    internal static class IncidentsHandler
    {
        public static void Add(ulong serverId, ulong channelId, string text)
        {
            NadekoBot.WriteInColor($"INCIDENT: {text}", ConsoleColor.Red);
            var incident = new Incident
            {
                ChannelId = (long)channelId,
                ServerId = (long)serverId,
                Text = text,
                Read = false
            };

            DbHandler.Instance.Connection.Insert(incident, typeof(Incident));
        }
    }
}