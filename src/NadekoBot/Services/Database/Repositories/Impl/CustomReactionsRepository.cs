using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NadekoBot.Services.Database.Models;
using System.Linq;

namespace NadekoBot.Services.Database.Repositories.Impl
{
    public class CustomReactionsRepository : Repository<CustomReaction>, ICustomReactionRepository
    {
        public CustomReactionsRepository(DbContext context) : base(context)
        {
        }

        public List<CustomReaction> GetList()
        {
            return _set.Include(x => x.Responses).ToList();
        }
    }
}