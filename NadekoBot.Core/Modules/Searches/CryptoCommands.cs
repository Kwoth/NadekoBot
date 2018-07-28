using AngleSharp;
using AngleSharp.Dom.Html;
using Discord;
using Discord.Commands;
using NadekoBot.Extensions;
using NadekoBot.Modules.Searches.Services;
using NadekoBot.Core.Modules.Searches.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using NadekoBot.Common.Attributes;

namespace NadekoBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class CryptoCommands : NadekoSubmodule<CryptoService>
        {
            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Crypto(string name)
            {
                name = name?.ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(name))
                    return;
                var cryptos = await _service.CryptoData().ConfigureAwait(false);
                var crypto = cryptos.data
                    ?.FirstOrDefault(x => x.id.ToString() == name || x.name.ToUpperInvariant() == name
                        || x.symbol.ToUpperInvariant() == name || x.website_slug.ToUpperInvariant() == name);

                (CryptoDataData Elem, int Distance)? nearest = null;
                if (crypto == null)
                {
                    nearest = cryptos.data.Select(x => (x, Distance: x.name.ToUpperInvariant().LevenshteinDistance(name)))
                        .OrderBy(x => x.Distance)
                        .Where(x => x.Distance <= 2)
                        .FirstOrDefault();

                    crypto = nearest?.Elem;
                }

                if (crypto == null)
                {
                    await ReplyErrorLocalized("crypto_not_found").ConfigureAwait(false);
                    return;
                }

                if (nearest != null)
                {
                    var embed = new EmbedBuilder()
                            .WithTitle(GetText("crypto_not_found"))
                            .WithDescription(GetText("did_you_mean", Format.Bold($"{crypto.name} ({crypto.symbol})")));

                    if (!await PromptUserConfirmAsync(embed).ConfigureAwait(false))
                        return;
                }

                var coin = await _service.CryptoCoin(crypto.id).ConfigureAwait(false);

                var data = coin.data;

                await Context.Channel.EmbedAsync(new EmbedBuilder()
                    .WithOkColor()
                    .WithTitle($"{data.name} ({data.symbol})")
                    .WithThumbnailUrl($"https://files.coinmarketcap.com/static/img/coins/32x32/{data.id}.png")
                    .AddField(GetText("market_cap"), $"${data.quotes.USD.market_cap:n0}", true)
                    .AddField(GetText("price"), $"${data.quotes.USD.price}", true)
                    .AddField(GetText("volume_24h"), $"${data.quotes.USD.volume_24h:n0}", true)
                    .AddField(GetText("change_7d_24h"), $"{data.quotes.USD.percent_change_7d}% / {data.quotes.USD.percent_change_24h}%", true)).ConfigureAwait(false);
            }
        }
    }
}