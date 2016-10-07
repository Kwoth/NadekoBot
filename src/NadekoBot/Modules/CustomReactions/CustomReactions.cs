using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using NadekoBot.Modules.Permissions;
using NadekoBot.Services;
using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.Modules.CustomReactions
{
    [NadekoModule("Custom reactions", ".")]
    public class CustomReactions : DiscordModule
    {
        public static HashSet<CustomReaction> GlobalReactions { get; private set; }
        public static HashSet<CustomReaction> ServerReactions { get; private set; }
        private ReactionFormatProvider _formatProvider = new ReactionFormatProvider();
        private NadekoRandom _random = new NadekoRandom();

        public CustomReactions(ILocalization loc, CommandService cmds, ShardedDiscordClient client) : base(loc, cmds, client)
        {
            using (var uow = DbHandler.UnitOfWork())
            {
                var crs = uow.CustomReactions.GetList();
                //init regex fro those that have it
                foreach (var cr in crs.Where(x => x.IsRegex))
                {
                    cr.Regex = new Regex(cr.Trigger, RegexOptions.Compiled);
                }
                var comparer = new CustomComparer();
                ServerReactions = new HashSet<CustomReaction>(/*crs.Where(x=>x.ServerId.HasValue),*/ comparer);
                GlobalReactions = new HashSet<CustomReaction>(/*crs.Where(x => !x.ServerId.HasValue),*/ comparer);
                if (crs.Any(x => x.ServerId.HasValue))
                    ServerReactions.UnionWith(crs.Where(x => x.ServerId.HasValue));
                if (crs.Any(x => !x.ServerId.HasValue))
                    GlobalReactions.UnionWith(crs.Where(x => !x.ServerId.HasValue));
            }
            client.MessageReceived += OnMessageReceived;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        private async Task OnMessageReceived(IMessage arg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (arg is IUserMessage && arg.Author.Id != NadekoBot.Client.GetCurrentUser().Id)
            {
                var throwaway = Task.Run(() => CustomReactionChecker((IUserMessage)arg));
            }
        }

        private async Task CustomReactionChecker(IUserMessage msg)
        {
            foreach (var reaction in GlobalReactions)
            {
                if (reaction.IsRegex)
                {
                    Match m;
                    if ((m = reaction.Regex.Match(msg.Content)).Success)
                    {
                        _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
                        await React(msg, reaction, m);
                    }
                }
                else if (msg.Content.StartsWith(reaction.Trigger))
                {
                    _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
                    await React(msg, reaction);
                }
            }
            var guildId = (long)(msg.Channel as IGuildChannel).Guild.Id;
            //same for serverReactions
            foreach (var reaction in ServerReactions.Where(s => s.ServerId == guildId))
            {
                if (reaction.IsRegex)
                {
                    Match m;
                    if ((m = reaction.Regex.Match(msg.Content)).Success)
                    {
                        _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
                        await React(msg, reaction, m);
                    }
                }
                else if (msg.Content.StartsWith(reaction.Trigger))
                {
                    _log.Info($@"Custom reaction: ""{msg.Content}"" matched ""{reaction.Trigger}""");
                    await React(msg, reaction);
                }
            }
        }

        private async Task<IMessage[]> React(IUserMessage msg, CustomReaction reaction, Match m = null)
        {
            var guild = (msg.Channel as ITextChannel)?.Guild;
            try
            {
                bool verbose = false;
                Permission rootPerm = null;
                string permRole = "";
                if (guild != null)
                {
                    using (var uow = DbHandler.UnitOfWork())
                    {
                        var config = uow.GuildConfigs.PermissionsFor(guild.Id);
                        verbose = config.VerbosePermissions;
                        rootPerm = config.RootPermission;
                        permRole = config.PermissionRole.Trim().ToLowerInvariant();
                    }
                    int index;
                    if (!rootPerm.AsEnumerable().CheckPermissions(msg, reaction.Id.ToString(), "CustomReactions", out index))
                    {
                        var returnMsg = $"Permission number #{index + 1} **{rootPerm.GetAt(index).GetCommand()}** is preventing this action.";
                    }
                    else
                        return (await msg.ReplyLong(string.Format(_formatProvider, reaction.Responses[_random.Next(0, reaction.Responses.Count - 1)].Text, msg, m, _random)));
                }
            }
            catch (Exception ex)
            {
                _log.Warn(ex, "Error in RegexCommandHandler");
                if (ex.InnerException != null)
                    _log.Warn(ex.InnerException, "Inner Exception of the error in RegexCommandHandler");
            }
            return null;

        }
        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task AddCustReact(IUserMessage msg, [Remainder] CustomReaction reaction)
        {
            //ascertain that only the owner makes a global reaction
            if (!NadekoBot.Credentials.IsOwner(msg.Author) && reaction.ServerId == null)
                reaction.ServerId = (long)(msg.Channel as IGuildChannel).Guild.Id;
            using (var uow = DbHandler.UnitOfWork())
            {
                uow.CustomReactions.Add(reaction);
                await uow.CompleteAsync();
            }
            if (reaction.ServerId == null)
            {
                //global reaction
                GlobalReactions.Add(reaction);
            }
            else
            {
                //server reaction
                ServerReactions.Add(reaction);
            }

            _log.Info("Added new custom reaction: " + reaction.ToString());
            await msg.ReplyLong("Added reaction: " + reaction.ToString());
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task AddCustReact(IUserMessage msg, int id, [Remainder] string reaction)
        {
            var item = ServerReactions.FirstOrDefault(x => x.Id == id) ?? GlobalReactions.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                await msg.Reply("Could not find reaction with id " + id);
                return;
            }
            item.Responses.Add(new Response { Text = reaction });
            using (var uow = DbHandler.UnitOfWork())
            {
                uow.CustomReactions.Update(item);
                await uow.CompleteAsync();
            }
            if (item.ServerId.HasValue)
            {
                ServerReactions.RemoveWhere(x => x.Id == item.Id);
                ServerReactions.Add(item);
            }
            else
            {
                GlobalReactions.RemoveWhere(x => x.Id == item.Id);
                GlobalReactions.Add(item);
            }
            await msg.ReplyLong(string.Format("Updated reaction {0} to ```xl\n{1}```", id, item));
        }

        public int PageSize { get; } = 5;

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task ListCustReact(IUserMessage msg, string pageOrId = "page", [Remainder] int number = 0)
        {
            int page = -1;
            if (pageOrId == "id")
            {
                var item = ServerReactions.FirstOrDefault(x => x.Id == number) ?? GlobalReactions.FirstOrDefault(x => x.Id == number);

                if (item == null)
                {
                    await msg.Reply("Reaction not found");
                    return;
                }
                await msg.ReplyLong("Found reaction: ```xl\n" + item.ToString() + "```");
                return;
            }
            page--;
            //Page of the thing

            List<CustomReaction> crs;
            crs = new List<CustomReaction>();
            crs.AddRange(GlobalReactions);
            crs.AddRange(ServerReactions);
            var max = (int)System.Math.Ceiling((double)crs.Count / PageSize);
            page = page < 0 ? 0 : page;
            page = page > max - 1 ? max - 1 : page;
            var items = crs.Skip(PageSize * page).Take(PageSize);
            StringBuilder sb = new StringBuilder($"Custom reactions on page {page + 1}/{max}:\n");
            foreach (var item in items)
            {
                sb.AppendLine("```json\n" + item.ToStringWithoutResponses() + "```");
            }
            await msg.ReplyLong(sb.ToString());
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        //inputting this perm for now
        [RequirePermission(GuildPermission.ManageGuild)]
        public async Task DelCustReact(IUserMessage msg, [Remainder] int id)
        {
            var item = ServerReactions.FirstOrDefault(x => x.Id == id) ?? GlobalReactions.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                await msg.Reply("Could not find reaction with id " + id);
                return;
            }

            if (item.ServerId == null && !NadekoBot.Credentials.IsOwner(msg.Author))
            {
                await msg.Reply("Only the bot owner can delete global reactions, you should disable using the permissions system if wanted");
                return;
            }
            if (!item.ServerId.HasValue)
            {
                GlobalReactions.Remove(item);
            }
            else
            {
                ServerReactions.Remove(item);
            }
            using (var uow = DbHandler.UnitOfWork())
            {
                //does this work if the ID is not known?
                uow.CustomReactions.Remove(item);
                await uow.CompleteAsync();
            }
            await msg.ReplyLong("Successfully removed custom reaction: " + item.ToString());
        }
    }
}