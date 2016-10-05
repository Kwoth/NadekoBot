using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NadekoBot.Services.Impl;

namespace NadekoBot.Services.Database.Impl
{
    public class NadekoSqlServerContext : NadekoContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var credentials = NadekoBot.Credentials ?? new BotCredentials();
            if (credentials.Db == null)
                throw new ArgumentNullException();
            optionsBuilder.UseSqlServer(credentials.Db.ConnectionString);
        }
    }
}
