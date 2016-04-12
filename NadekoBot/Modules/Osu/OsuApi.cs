using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Osu
{
    public class OsuApi
    {
        private WebClient _client;
        private string _apiKey;

        private const string ApiUrl = "https://osu.ppy.sh/";
        private const string GetBeatmapsURL = ApiUrl + "/api/get_beatmaps";
        private const string GetUserURL = ApiUrl + "/api/get_user";
        private const string GetScoresURL = ApiUrl + "/api/get_scores";
        private const string GetUserBestURL = ApiUrl + "/api/get_user_best";
        private const string GetUserRecentURL = ApiUrl + "/api/get_user_recent";
        private const string GetMatchURL = ApiUrl + "/api/get_match";

        /// <summary>
        /// Constructs the api class with the given key
        /// </summary>
        /// <param name="apiKey">The osu!Api key given from https://osu.ppy.sh/p/api </param>
        public OsuApi(string apiKey)
        {
            _apiKey = apiKey;
            _client = new WebClient();
        }
        public User GetUser(string username, string mode)
        {
            var userList = GetResults<User>(GetUserURL + "?k=" + _apiKey + "&u=" + username + "&m=" + mode + "&type=string");
            return userList.Count > 0 ? userList[0] : null;
        }
        private List<T> GetResults<T>(string url)
        {
            var jsonResponse = _client.DownloadString(url);
            var listReturn = new List<T>();
            if (jsonResponse == "Please provide a valid API key.")
                throw new Exception("Invalid osu!Api key");
            var objectArray = JsonConvert.DeserializeObject<T[]>(jsonResponse);
            if (objectArray.Length < 1) return null;

            listReturn.AddRange(objectArray);
            return listReturn;
        }

        private T GetResult<T>(string url)
        {
            var jsonResponse = _client.DownloadString(url);
            if (jsonResponse == "Please provide a valid API key.")
                throw new Exception("Invalid osu!Api key");
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }
    }
}
