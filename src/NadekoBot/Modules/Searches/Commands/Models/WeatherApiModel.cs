using Newtonsoft.Json;
using System.Collections.Generic;

namespace NadekoBot.Modules.Searches.Models
{
    public class WeatherApiModel
    {
        public Weather weather { get; set; }
        public class Weather
        {
            public string target { get; set; }
            public string sunrise { get; set; }
            public string sunset { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string centigrade { get; set; }
            public string fahrenheit { get; set; }
            public string feelscentigrade { get; set; }
            public string feelsfahrenheit { get; set; }
            public string condition { get; set; }
            public string winddir { get; set; }
            public string humidity { get; set; }
            public string windspeedm { get; set; }
            public string windspeedk { get; set; }
        }
    }
}