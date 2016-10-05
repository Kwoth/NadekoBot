using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NadekoBot.Services.Database.Models;
using NadekoBot.Services.Impl;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace NadekoBot.Services.Database.Impl
{
    public class NadekoPgsqlContext : NadekoContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var credentials = NadekoBot.Credentials ?? new BotCredentials();
            if (credentials.Db == null)
                throw new ArgumentNullException();
            optionsBuilder.UseNpgsql(credentials.Db.ConnectionString);
        }



        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Bot config
            modelBuilder.Entity<BotConfig>().Property(p => p.BufferSize).HasColumnType("numeric(20,0)");

            modelBuilder.Entity<BlacklistItem>().Property(p =>p.ItemId).HasColumnType("numeric(20,0)");

            //ClashWar
            {
                var model = modelBuilder.Entity<ClashWar>();
                model.Property(p => p.GuildId).HasColumnType("numeric(20,0)");
                model.Property(p => p.ChannelId).HasColumnType("numeric(20,0)");
            }

            //Currency
            modelBuilder.Entity<Currency>().Property(p => p.UserId).HasColumnType("numeric(20,0)");

            //Donator
            modelBuilder.Entity<Donator>().Property(p => p.UserId).HasColumnType("numeric(20,0)");

            //FollowedStream
            modelBuilder.Entity<FollowedStream>().Property(p => p.ChannelId).HasColumnType("numeric(20,0)");

            //GuildConfig
            {
                var model = modelBuilder.Entity<GuildConfig>();
                model.Property(p => p.AutoAssignRoleId).HasColumnType("numeric(20,0)");
                model.Property(p => p.ByeMessageChannelId).HasColumnType("numeric(20,0)");
                model.Property(p => p.GenerateCurrencyChannelId).HasColumnType("numeric(20,0)");
                model.Property(p => p.GreetMessageChannelId).HasColumnType("numeric(20,0)");
                model.Property(p => p.GuildId).HasColumnType("numeric(20,0)");
            }

            //IgnoredLogChannel
            modelBuilder.Entity<IgnoredLogChannel>().Property(p => p.ChannelId).HasColumnType("numeric(20,0)");

            //LogSetting
            {
                var model = modelBuilder.Entity<LogSetting>();
                model.Property(p => p.ChannelId).HasColumnType("numeric(20,0)");
                model.Property(p => p.UserPresenceChannelId).HasColumnType("numeric(20,0)");
                model.Property(p => p.VoicePresenceChannelId).HasColumnType("numeric(20,0)");
            }

            //Permission
            modelBuilder.Entity<Permission>().Property(p => p.PrimaryTargetId).HasColumnType("numeric(20,0)");

            //Quote
            {
                var model = modelBuilder.Entity<Quote>();
                model.Property(p => p.AuthorId).HasColumnType("numeric(20,0)");
                model.Property(p => p.GuildId).HasColumnType("numeric(20,0)");
            }

            //Reminder
            {
                var model = modelBuilder.Entity<Reminder>();
                model.Property(p => p.ChannelId).HasColumnType("numeric(20,0)");
                model.Property(p => p.ServerId).HasColumnType("numeric(20,0)");
                model.Property(p => p.UserId).HasColumnType("numeric(20,0)");
            }

            //Repeater
            {
                var model = modelBuilder.Entity<Repeater>();
                model.Property(p => p.ChannelId).HasColumnType("numeric(20,0)");
                model.Property(p => p.GuildId).HasColumnType("numeric(20,0)");
            }

            //SelfAssignableRole
            {
                var model = modelBuilder.Entity<SelfAssignedRole>();
                model.Property(p => p.GuildId).HasColumnType("numeric(20,0)");
                model.Property(p => p.RoleId).HasColumnType("numeric(20,0)");
            }

            //VoicePresenceChannel
            modelBuilder.Entity<IgnoredVoicePresenceChannel>().Property(p => p.ChannelId).HasColumnType("numeric(20,0)");
        }
        */
    }
}
