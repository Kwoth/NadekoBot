using NadekoBot.Core.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NadekoBot.Core.Modules.Searches.Common;
using System.Linq;
using RateLimiter;

namespace NadekoBot.Modules.Searches.Services
{
    public class CryptoService : INService
    {
        private readonly Logger _log;
        private readonly IDataCache _cache;
        private readonly IHttpClientFactory _httpFactory;

        private readonly TimeLimiter ratelimit;

        private readonly SemaphoreSlim _cryptoLock = new SemaphoreSlim(1, 1);

        private string CryptoCoinData;

        private int coinid;


        public CryptoService(IDataCache cache, IHttpClientFactory httpFactory)
        {
            _log = LogManager.GetCurrentClassLogger();
            _cache = cache;
            _httpFactory = httpFactory;
            ratelimit = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(2));
        }

        public async Task<CryptoData> CryptoData()
        {
            string data;
            var r = _cache.Redis.GetDatabase();
            data = await r.StringGetAsync("crypto_data").ConfigureAwait(false);

            if (data == null)
            {
                using (var http = _httpFactory.CreateClient())
                {
                    data = await http.GetStringAsync(new Uri("https://api.coinmarketcap.com/v2/listings/"))
                        .ConfigureAwait(false);
                }
                await r.StringSetAsync("crypto_data", data, TimeSpan.FromHours(1)).ConfigureAwait(false);
            }
            

            return JsonConvert.DeserializeObject<CryptoData>(data);
        }

        public async Task<CryptoCoin> CryptoCoin(int id)
        {
            string key = "crypto_coin" + id;
            var r = _cache.Redis.GetDatabase();
            string coin = await r.StringGetAsync(key).ConfigureAwait(false);

            if (coin == null)
            {
                await _cryptoLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    coinid = id;
                    await ratelimit.Perform(SetCoinData);
                    coin = CryptoCoinData;
                }
                finally
                {
                    _cryptoLock.Release();
                }
                await r.StringSetAsync(key, coin, TimeSpan.FromMinutes(30)).ConfigureAwait(false);
            }

            return JsonConvert.DeserializeObject<CryptoCoin>(coin);
        }

        private async Task SetCoinData()
        {
            using (var http = _httpFactory.CreateClient())
            {
                CryptoCoinData = await http.GetStringAsync(
                    new Uri($"https://api.coinmarketcap.com/v2/ticker/{coinid}/"))
                    .ConfigureAwait(false);
            }
        }

    }
}