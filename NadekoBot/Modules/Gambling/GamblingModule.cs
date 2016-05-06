using Discord;
using Discord.Commands;
using Discord.Modules;
using Uni.Classes;
using Uni.Extensions;
using Uni.Modules.Permissions.Classes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Uni.Modules.Gambling
{
    internal class GamblingModule : DiscordModule
    {

        public GamblingModule()
        {
            commands.Add(new DrawCommand(this));
            commands.Add(new FlipCoinCommand(this));
            commands.Add(new DiceRollCommand(this));
        }

        public override string Prefix { get; } = Uni.Config.CommandPrefixes.Gambling;

        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                cgb.AddCheck(PermissionChecker.Instance);

                commands.ForEach(com => com.Init(cgb));

                cgb.CreateCommand(Prefix + "raffle")
                    .Description("Prints a name and ID of a random user from the online list from the (optional) role.")
                    .Parameter("role", ParameterType.Optional)
                    .Do(RaffleFunc());

                cgb.CreateCommand(Prefix + "$$")
                    .Description(string.Format("Check how much {0}s you have.", Uni.Config.CurrencyName))
                    .Do(NadekoFlowerCheckFunc());

                cgb.CreateCommand(Prefix + "give")
                    .Description(string.Format("Give someone a certain amount of {0}s", Uni.Config.CurrencyName))
                    .Parameter("amount", ParameterType.Required)
                    .Parameter("receiver", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var amountStr = e.GetArg("amount")?.Trim();
                        long amount;
                        if (!long.TryParse(amountStr, out amount) || amount < 0)
                            return;

                        var mentionedUser = e.Message.MentionedUsers.FirstOrDefault(u =>
                                                            u.Id != Uni.Client.CurrentUser.Id &&
                                                            u.Id != e.User.Id);
                        if (mentionedUser == null)
                            return;

                        var userFlowers = GetUserFlowers(e.User.Id);

                        if (userFlowers < amount)
                        {
                            await e.Channel.SendMessage($"{e.User.Mention} You don't have enough {Uni.Config.CurrencyName}s. You have only {userFlowers}{Uni.Config.CurrencySign}.").ConfigureAwait(false);
                            return;
                        }

                        FlowersHandler.RemoveFlowers(e.User, "Gift", (int)amount);
                        await FlowersHandler.AddFlowersAsync(mentionedUser, "Gift", (int)amount).ConfigureAwait(false);

                        await e.Channel.SendMessage($"{e.User.Mention} successfully sent {amount} {Uni.Config.CurrencyName}s to {mentionedUser.Mention}!").ConfigureAwait(false);

                    });

                cgb.CreateCommand(Prefix + "award")
                    .Description("Gives someone a certain amount of flowers. **Owner only!**")
                    .AddCheck(SimpleCheckers.OwnerOnly())
                    .Parameter("amount", ParameterType.Required)
                    .Parameter("receiver", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var amountStr = e.GetArg("amount")?.Trim();
                        long amount;
                        if (!long.TryParse(amountStr, out amount) || amount < 0)
                            return;

                        var mentionedUser = e.Message.MentionedUsers.FirstOrDefault(u =>
                                                            u.Id != Uni.Client.CurrentUser.Id);
                        if (mentionedUser == null)
                            return;

                        await FlowersHandler.AddFlowersAsync(mentionedUser, $"Awarded by bot owner. ({e.User.Name}/{e.User.Id})", (int)amount).ConfigureAwait(false);

                        await e.Channel.SendMessage($"{e.User.Mention} successfully awarded {amount} {Uni.Config.CurrencyName}s to {mentionedUser.Mention}!").ConfigureAwait(false);
                    });

                cgb.CreateCommand(Prefix + "take")
                    .Description("Takes a certain amount of flowers from someone. **Owner only!**")
                    .AddCheck(SimpleCheckers.OwnerOnly())
                    .Parameter("amount", ParameterType.Required)
                    .Parameter("rektperson", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var amountStr = e.GetArg("amount")?.Trim();
                        long amount;
                        if (!long.TryParse(amountStr, out amount) || amount < 0)
                            return;

                        var mentionedUser = e.Message.MentionedUsers.FirstOrDefault(u =>
                                                            u.Id != Uni.Client.CurrentUser.Id);
                        if (mentionedUser == null)
                            return;

                        FlowersHandler.RemoveFlowers(mentionedUser, $"Taken by bot owner.({e.User.Name}/{e.User.Id})", (int)amount);

                        await e.Channel.SendMessage($"{e.User.Mention} successfully took {amount} {Uni.Config.CurrencyName}s from {mentionedUser.Mention}!").ConfigureAwait(false);
                    });
            });
        }

        private static Func<CommandEventArgs, Task> NadekoFlowerCheckFunc()
        {
            return async e =>
            {
                var pts = GetUserFlowers(e.User.Id);
                var str = $"`You have {pts} {Uni.Config.CurrencyName}s".SnPl((int)pts) + "`\n";
                for (var i = 0; i < pts; i++)
                {
                    str += Uni.Config.CurrencySign;
                }
                await e.Channel.SendMessage(str).ConfigureAwait(false);
            };
        }

        private static long GetUserFlowers(ulong userId) =>
            Classes.DbHandler.Instance.GetStateByUserId((long)userId)?.Value ?? 0;

        private static Func<CommandEventArgs, Task> RaffleFunc()
        {
            return async e =>
            {
                var arg = string.IsNullOrWhiteSpace(e.GetArg("role")) ? "@everyone" : e.GetArg("role");
                var role = e.Server.FindRoles(arg).FirstOrDefault();
                if (role == null)
                {
                    await e.Channel.SendMessage("💢 Role not found.").ConfigureAwait(false);
                    return;
                }
                var members = role.Members.Where(u => u.Status == UserStatus.Online); // only online
                var membersArray = members as User[] ?? members.ToArray();
                var usr = membersArray[new Random().Next(0, membersArray.Length)];
                await e.Channel.SendMessage($"**Raffled user:** {usr.Name} (id: {usr.Id})").ConfigureAwait(false);
            };
        }
    }
}
