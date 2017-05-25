using Discord;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using NadekoBot.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

//todo drawing
namespace NadekoBot.Modules.Searches
{
    public partial class Searches
    {
        [NadekoCommand, Usage, Description, Aliases]
            public async Task EthPrice()
            {
                using (var http = new HttpClient())
				{
					var response = await http.GetStringAsync("https://api.coinbase.com/v2/prices/ETH-USD/spot").ConfigureAwait(false);
					if (response == null)
						return;

					var fact = JObject.Parse(response)["data"]["amount"].ToString();
					await Context.Channel.SendConfirmAsync("$" + fact+"/ETH").ConfigureAwait(false);
				}
            }
			
		[NadekoCommand, Usage, Description, Aliases]
            public async Task BtcPrice()
            {
                using (var http = new HttpClient())
				{
					var response = await http.GetStringAsync("https://api.coinbase.com/v2/prices/BTC-USD/spot").ConfigureAwait(false);
					if (response == null)
						return;

					var fact = JObject.Parse(response)["data"]["amount"].ToString();
					await Context.Channel.SendConfirmAsync("$" + fact+"/BTC").ConfigureAwait(false);
				}
            }
    }
}
