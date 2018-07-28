using System.Collections.Generic;
using Newtonsoft.Json;

namespace NadekoBot.Core.Modules.Searches.Common
{
    //the following is for https://api.coinmarketcap.com/v2/listings/
    public class CryptoDataData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string website_slug { get; set; }
    }

    public class CryptoDataMetadata
    {
        public int timestamp { get; set; }
        public int num_cryptocurrencies { get; set; }
        public object error { get; set; }
    }

    public class CryptoData
    {
        public List<CryptoDataData> data { get; set; }
        public CryptoDataMetadata metadata { get; set; }
    }

    //the following is for https://api.coinmarketcap.com/v2/ticker/{ID}/
        
    public class USD
    {
        public decimal? price { get; set; }
        public decimal? volume_24h { get; set; }
        public decimal? market_cap { get; set; }
        public double? percent_change_1h { get; set; }
        public double? percent_change_24h { get; set; }
        public double? percent_change_7d { get; set; }
    }

    public class Quotes
    {
        public USD USD { get; set; }
    }

    public class CryptoCoinData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string website_slug { get; set; }
        public int rank { get; set; }
        public decimal? circulating_supply { get; set; }
        public decimal? total_supply { get; set; }
        public decimal? max_supply { get; set; }
        public Quotes quotes { get; set; }
        public int last_updated { get; set; }
    }

    public class CryptoCoinMetadata
    {
        public int timestamp { get; set; }
        public object error { get; set; }
    }

    public class CryptoCoin
    {
        public CryptoCoinData data { get; set; }
        public CryptoCoinMetadata metadata { get; set; }
    }
}
