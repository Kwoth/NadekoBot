using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using NadekoBot.Services;
using NadekoBot.Services.Database;
using NadekoBot.Services.Database.Models;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class LanguageCommands
        {
            private Logger _log;
            public LanguageCommands()
            {
                _log = LogManager.GetCurrentClassLogger();
            }

            [NadekoCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task SetLanguage(IUserMessage umsg, [Remainder] string text = null)
            {
                var channel = (ITextChannel)umsg.Channel;

                if (string.IsNullOrWhiteSpace(text))
                {
                    string languageText;
                    using (var uow = DbHandler.UnitOfWork())
                    {
                        languageText = uow.GuildConfigs.For(channel.Guild.Id, set => set).Language;
                    }
                    await channel.SendMessageAsync("ℹ️ Current **Language**: `" + languageText?.SanitizeMentions() + "`");
                    return;
                }

                var sendGreetEnabled = SetLanguage(channel.Guild.Id, ref text);

                await channel.SendMessageAsync("🆗 New language **set**.").ConfigureAwait(false);
            }

            public static string SetLanguage(ulong guildId, ref string message)
            {
                message = message?.SanitizeMentions();

                if (string.IsNullOrWhiteSpace(message))
                    throw new ArgumentNullException(nameof(message));

                using (var uow = DbHandler.UnitOfWork())
                {
                    var conf = uow.GuildConfigs.For(guildId, set => set);
                    conf.Language = message;

                    uow.Complete();
                }
                return message;
            }
        }
    }
}