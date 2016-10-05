﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Configuration.Internal;

namespace NadekoBot.Services.Database.Models
{
    public class BotConfig : DbEntity
    {
        public HashSet<BlacklistItem> Blacklist { get; set; }

        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="BufferSize"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("BufferSize")]
        public long _bufferSize { get; set; } = 4000000;

        [NotMapped]
        public ulong BufferSize
        {
            get { return (ulong) _bufferSize; }
            set { _bufferSize = (long) value; }
        }

        public bool DontJoinServers { get; set; } = false;
        public bool ForwardMessages { get; set; } = true;
        public bool ForwardToAllOwners { get; set; } = true;

        public float CurrencyGenerationChance { get; set; } = 0.02f;
        public int CurrencyGenerationCooldown { get; set; } = 10;

        public List<ModulePrefix> ModulePrefixes { get; set; } = new List<ModulePrefix>();

        public List<PlayingStatus> RotatingStatusMessages { get; set; } = new List<PlayingStatus>();

        public bool RotatingStatuses { get; set; } = false;
        public string RemindMessageFormat { get; set; } = "❗⏰**I've been told to remind you to '%message%' now by %user%.**⏰❗";


        public string CurrencySign { get; set; } = "🌸";
        public string CurrencyName { get; set; } = "Nadeko Flower";
        public string CurrencyPluralName { get; set; } = "Nadeko Flowers";

        public List<EightBallResponse> EightBallResponses { get; set; } = new List<EightBallResponse>();
        public List<RaceAnimal> RaceAnimals { get; set; } = new List<RaceAnimal>();
    }

    public class PlayingStatus :DbEntity
    {
        public string Status { get; set; }
    }

    public class BlacklistItem : DbEntity
    {
        /// <summary>
        /// <strong>DO NOT USE IT DIRECTLY</strong>, pls use <see cref="ItemId"/>.
        /// It's used internally by EF
        /// </summary>
        [Column("ItemId")]
        public long _itemId { get; set; }

        [NotMapped]
        public ulong ItemId
        {
            get { return (ulong) _itemId; }
            set { _itemId = (long) value; }
        }

        public BlacklistType Type { get; set; }

        public enum BlacklistType
        {
            Server,
            Channel,
            User
        }
    }

    public class EightBallResponse : DbEntity
    {
        public string Text { get; set; }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EightBallResponse))
                return base.Equals(obj);

            return ((EightBallResponse)obj).Text == Text;
        }
    }

    public class RaceAnimal : DbEntity
    {
        public string Icon { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return Icon.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RaceAnimal))
                return base.Equals(obj);

            return ((RaceAnimal)obj).Icon == Icon;
        }
    }
    
    public class ModulePrefix : DbEntity
    {
        public string ModuleName { get; set; }
        public string Prefix { get; set; }

        public int BotConfigId { get; set; } = 1;
        public BotConfig BotConfig { get; set; }

        public override int GetHashCode()
        {
            return ModuleName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is ModulePrefix))
                return base.Equals(obj);

            return ((ModulePrefix)obj).ModuleName == ModuleName;
        }
    }
}
