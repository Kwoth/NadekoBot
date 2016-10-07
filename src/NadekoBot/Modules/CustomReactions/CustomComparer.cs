using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Modules.CustomReactions
{
    public class CustomComparer : IEqualityComparer<CustomReaction>
    {
        public CustomComparer() { }
        public bool Equals(CustomReaction x, CustomReaction y)
        {
            return GetHashCode(x).Equals(GetHashCode(y));
        }

        public int GetHashCode(CustomReaction obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
