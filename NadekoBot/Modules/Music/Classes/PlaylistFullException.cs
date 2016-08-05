using System;

namespace NadekoBot.Modules.Music.Classes
{
    internal class PlaylistFullException : Exception
    {
        public PlaylistFullException(string message) : base(message)
        {
        }

        public PlaylistFullException() : base("Queue is full.")
        {
        }
    }
}