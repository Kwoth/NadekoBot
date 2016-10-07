using NadekoBot.Services.Database.Models;
using System.Collections.Generic;

namespace NadekoBot.Services.Database.Repositories
{
    public interface ICustomReactionRepository : IRepository<CustomReaction>
    {
        List<CustomReaction> GetList();
    }
}