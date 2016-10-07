using Discord.Commands;
using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Permissions
{
    public class PermissionCommand
    {
        public bool IsCommand { get; set; }
        public CustomReaction CustomReaction { get; set; } = null;
        public Command Command { get; set; } = null;
        public string Text
        {
            get
            {
                return IsCommand ? Command.Text : CustomReaction.Id.ToString();
            }
        }
    }
}
