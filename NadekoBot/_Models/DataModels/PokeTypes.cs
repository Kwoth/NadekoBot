namespace NadekoBot.DataModels
{
    internal class UserPokeTypes : IDataModel
    {
        public long UserId { get; set; }
        public string type { get; set; }
    }
}