﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace NadekoBot.Services
{
    public interface IYoutubeService
    {
        Task<IEnumerable<string>> FindVideosByKeywordsAsync(string keywords, int count = 1);
        Task<IEnumerable<string>> FindPlaylistIdsByKeywordsAsync(string keywords, int count = 1);
        Task<IEnumerable<string>> FindRelatedVideosAsync(string url, int count = 1);
    }
}
