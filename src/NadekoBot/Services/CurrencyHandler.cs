﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using NadekoBot.Services.Database;
using NadekoBot.Extensions;
using NadekoBot.Modules.Gambling;

namespace NadekoBot.Services
{
    public static class CurrencyHandler
    {
        public static async Task<bool> RemoveCurrencyAsync(IGuildUser author, string reason, long amount, bool sendMessage)
        {
            var success = await RemoveCurrencyAsync(author.Id, reason, amount);

            if (success && sendMessage)
                try { await author.SendMessageAsync($"`You lost:` {amount} {Gambling.CurrencySign}\n`Reason:` {reason}").ConfigureAwait(false); } catch { }

            return success;
        }

        public static async Task<bool> RemoveCurrencyAsync(ulong authorId, string reason, long amount)
        {
            if (amount < 0)
                throw new ArgumentNullException(nameof(amount));


            using (var uow = DbHandler.UnitOfWork())
            {
                var success = uow.Currency.TryUpdateState(authorId, -amount);
                if (!success)
                    return false;
                await uow.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }

        public static async Task AddCurrencyAsync(IGuildUser author, string reason, long amount, bool sendMessage)
        {
            await AddCurrencyAsync(author.Id, reason, amount);

            if (sendMessage)
                try { await author.SendMessageAsync($"`You received:` {amount} {Gambling.CurrencySign}\n`Reason:` {reason}").ConfigureAwait(false); } catch { }
        }

        public static async Task AddCurrencyAsync(ulong receiverId, string reason, long amount)
        {
            if (amount < 0)
                throw new ArgumentNullException(nameof(amount));


            using (var uow = DbHandler.UnitOfWork())
            {
                uow.Currency.TryUpdateState(receiverId, amount);
                await uow.CompleteAsync();
            }
        }
    }
}
