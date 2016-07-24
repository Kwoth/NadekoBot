namespace NadekoBot.DataModels
{
    class PlaylistSong : DataModels.IDataModel
    {
        /// <summary>
        /// Id of the playlist this song belongs to
        /// </summary>
        public int PlaylistId { get; set; }
        /// <summary>
        /// One of: Youtube, Soundcloud
        /// </summary>
        public string Provider { get; internal set; }
        public int ProviderType { get; internal set; }
        /// <summary>
        /// Title of the song
        /// </summary>
        public string Title { get; internal set; }
        /// <summary>
        /// Link to the song~
        /// </summary>
        public string Uri { get; internal set; }
        public string Query { get; internal set; }
    }
}
