using System;

namespace NadekoBot.Services
{
    [Serializable]
    internal class UnknowDatabaseProviderException : Exception
    {
        public UnknowDatabaseProviderException()
        {
        }

        public UnknowDatabaseProviderException(string message) : base(message)
        {
        }

        public UnknowDatabaseProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}