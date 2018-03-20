using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using NadekoBot.Common.Collections;
using NadekoBot.Common.ModuleBehaviors;
using NadekoBot.Core.Services;
using NadekoBot.Core.Services.Impl;
using NadekoBot.Core.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace NadekoBot.Modules.Permissions.Services
{
    public class GlobalWhitelistService : INService
    {
        private readonly DbService _db;
        private readonly DiscordSocketClient _client;
        private readonly NadekoStrings _strs;

        private string enabledText = Format.Code("✅");
        private string disabledText = Format.Code("❌");
        public readonly int numPerPage = 5;

        public enum FieldType {
            A = 0, ALL = A, EVERYTHING = A, GENERAL = A, GEN = A, G = A,
            CMD = 1, COMMAND = CMD, COMMANDS = CMD, CMDS = CMD,
            MOD = 2, MODULE = MOD, MODULES = MOD, MODS = MOD, MDL = MOD, MDLS = MOD,
            S = 3, SRVR = S, SERVER = S, SERVERS = S, SRVRS = S,
            C = 4, CHNL = C, CHANNEL = C, CHANNELS = C, CHNLS = C,
            U = 5, USR = U, USER = U, USERS = U, USRS = U,
            R = 6, ROLE = R, ROLES = R,
            UB = 7, UNBLOCK = UB, UNBLOCKED = UB,
            M = 8, MEM = M, MEMBER = M, MEMBERS = M
        };

        public readonly string[] FT_Strings = {
            "GENERAL", 		// FieldType 0, GWLType 0
            "COMMAND",		// FieldType 1, UnblockedType 0
            "MODULE", 		// FieldType 2, UnblockedType 1
            "SERVER", 		// FieldType 3, ItemType 0
            "CHANNEL",		// FieldType 4, ItemType 1
            "USER",			// FieldType 5, ItemType 2
            "ROLE", 		// FieldType 6, ItemType 3, GWLType 2
            "UNBLOCKED",	// FieldType 7
            "MEMBER"		// FieldType 8, GWLType 1
        };

        public GlobalWhitelistService(DiscordSocketClient client, DbService db, NadekoStrings strings)
        {
            _db = db;
            _client = client;
            _strs = strings;
        }

        #region General Whitelist Actions

        public bool CreateWhitelist(string listName, GWLType type)
        {
            using (var uow = _db.UnitOfWork)
            {
                uow._context.Database.ExecuteSqlCommand(
                    "INSERT INTO GWLSet ('ListName', 'Type') VALUES (@p0,@p1);",
                    listName,
                    type);

                uow.Complete();
            }

            return true;
        }

        /// <summary>Assumes the provided listName came from a valid GWLSet object.</summary>
        public bool RenameWhitelist(string oldName, string listName)
        {
            using (var uow = _db.UnitOfWork)
            {
                GWLSet group = uow._context.Set<GWLSet>()
                    .Where(g => g.ListName.Equals(oldName))
                    .SingleOrDefault();

                if (group == null) return false;

                group.ListName = listName;
                uow.Complete();
            }
            return true;
        }

        /// <summary>Assumes the provided listName came from a valid GWLSet object.</summary>
        public bool SetEnabledStatus(string listName, bool status)
        {
            using (var uow = _db.UnitOfWork)
            {
                GWLSet group = uow._context.Set<GWLSet>()
                    .Where(g => g.ListName.Equals(listName))
                    .SingleOrDefault();

                if (group == null) return false;

                group.IsEnabled = status;
                uow.Complete();
            }
            return true;
        }

        /// <summary>Assumes the provided listName came from a valid GWLSet object.</summary>
        public bool DeleteWhitelist(string listName)
        {
            using (var uow = _db.UnitOfWork)
            {
                // Delete the whitelist record and all relation records
                uow._context.Set<GWLSet>().Remove(
                    uow._context.Set<GWLSet>()
                    .Where( x => x.ListName.Equals(listName) ).FirstOrDefault()
                );
                uow.Complete();
            }
            return true;
        }

        #endregion General Whitelist Actions

        #region Add/Remove

        public bool AddMemberToGroup(ulong[] items, GWLItemType type, GWLSet group, out ulong[] successList)
        {
            successList = null;

            using (var uow = _db.UnitOfWork)
            {
                // For each non-existing member, add it to the database
                // Fetch all member names already in the database
                ulong[] curItems = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(type))
                    .Select(x => x.ItemId)
                    .ToArray();
                // Focus only on given names that aren't yet in the database, while simultaneously removing dupes
                var excludedItems = items.Except(curItems);
                if (excludedItems.Count() > 0) {
                    for (int i=0; i<excludedItems.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "INSERT INTO GWLItem ('ItemId', 'Type') VALUES (@p0,@p1);",
                            excludedItems.ElementAt(i),
                            (int)type);
                    }
                    uow.Complete();
                }

                // For each non-existing relationship, add it to the database
                // Fetch all member IDs existing in DB with given type and name in list
                int[] curIDs = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(type))
                    .Where(x => items.Contains(x.ItemId))
                    .Select(x => x.Id)
                    .ToArray();
                // Fetch all member IDs already related to group
                int[] curRel = uow._context.Set<GWLItemSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Select(x => x.ItemPK)
                    .ToArray();
                // Focus only on given IDs that aren't yet related to group (automatically removes dupes)
                var excludedIDs = curIDs.Except(curRel);
                if (excludedIDs.Count() > 0) {
                    for (int i=0; i<excludedIDs.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "INSERT INTO GWLItemSet ('ListPK', 'ItemPK') VALUES (@p0,@p1);",
                            group.Id,
                            excludedIDs.ElementAt(i));
                    }
                    uow.Complete();
                }

                // Return list of all newly added relationships
                successList = uow._context.Set<GWLItem>()
                    .Where(x => excludedIDs.Contains(x.Id))
                    .Select(x => x.ItemId)
                    .OrderBy(id => id)
                    .ToArray();

                uow.Complete();
                if (successList.Count() > 0) return true;
                return false;
            }
        }

        public bool RemoveMemberFromGroup(ulong[] items, GWLItemType type, GWLSet group, out ulong[] successList)
        {
            successList = null;

            using (var uow = _db.UnitOfWork)
            {
                // For each non-existing relationship, add it to the database
                // Fetch all member IDs existing in DB with given type and name in list
                int[] curIDs = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(type))
                    .Where(x => items.Contains(x.ItemId))
                    .Select(x => x.Id)
                    .ToArray();
                // Fetch all member IDs related to the given group BEFORE delete
                int[] relIDs = uow._context.Set<GWLItemSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Where(x => curIDs.Contains(x.ItemPK))
                    .Select(x => x.ItemPK)
                    .ToArray();

                // Delete all existing IDs where type and name matches (above lines ensure no dupes)
                if (curIDs.Count() > 0) {
                    for (int i=0; i<curIDs.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "DELETE FROM GWLItemSet WHERE ListPK = @p0 AND ItemPK = @p1;",
                            group.Id,
                            curIDs[i]);
                    }
                    uow.Complete();
                }
                // Fetch all member IDs related to the given group AFTER delete
                int[] relIDsRemain = uow._context.Set<GWLItemSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Where(x => curIDs.Contains(x.ItemPK))
                    .Select(x => x.ItemPK)
                    .ToArray();

                var deletedIDs = relIDs.Except(relIDsRemain);

                // Return list of all deleted relationships
                successList = uow._context.Set<GWLItem>()
                    .Where(x => deletedIDs.Contains(x.Id))
                    .Select(x => x.ItemId)
                    .OrderBy(id => id)
                    .ToArray();

                uow.Complete();
                if (successList.Count() > 0) return true;
                return false;
            }
        }

        public bool AddRoleToGroup(ulong serverID, ulong[] items, GWLSet group, out ulong[] successList)
        {
            GWLItemType type = GWLItemType.Role;
            successList = null;

            using (var uow = _db.UnitOfWork)
            {
                // For each non-existing member, add it to the database
                // Fetch all member names already in the database
                ulong[] curItems = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(type))
                    .Where(x => x.RoleServerId.Equals(serverID))
                    .Select(x => x.ItemId)
                    .ToArray();
                // Focus only on given names that aren't yet in the database, while simultaneously removing dupes
                var excludedItems = items.Except(curItems);
                if (excludedItems.Count() > 0) {
                    for (int i=0; i<excludedItems.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "INSERT INTO GWLItem ('ItemId', 'Type', 'RoleServerId') VALUES (@p0,@p1,@p2);",
                            excludedItems.ElementAt(i),
                            (int)type,
                            serverID);
                    }
                    uow.Complete();
                }

                // For each non-existing relationship, add it to the database
                // Fetch all member IDs existing in DB with given type and name in list
                int[] curIDs = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(type))
                    .Where(x => x.RoleServerId.Equals(serverID))
                    .Where(x => items.Contains(x.ItemId))
                    .Select(x => x.Id)
                    .ToArray();
                // Fetch all member IDs already related to group
                int[] curRel = uow._context.Set<GWLItemSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Select(x => x.ItemPK)
                    .ToArray();
                // Focus only on given IDs that aren't yet related to group (automatically removes dupes)
                var excludedIDs = curIDs.Except(curRel);
                if (excludedIDs.Count() > 0) {
                    for (int i=0; i<excludedIDs.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "INSERT INTO GWLItemSet ('ListPK', 'ItemPK') VALUES (@p0,@p1);",
                            group.Id,
                            excludedIDs.ElementAt(i));
                    }
                    uow.Complete();
                }

                // Return list of all newly added relationships
                successList = uow._context.Set<GWLItem>()
                    .Where(x => x.RoleServerId.Equals(serverID))
                    .Where(x => excludedIDs.Contains(x.Id))
                    .Select(x => x.ItemId)
                    .OrderBy(id => id)
                    .ToArray();

                uow.Complete();
                if (successList.Count() > 0) return true;
                return false;
            }
        }

        public bool RemoveRoleFromGroup(ulong serverID, ulong[] items, GWLSet group, out ulong[] successList)
        {
            GWLItemType type = GWLItemType.Role;
            successList = null;

            using (var uow = _db.UnitOfWork)
            {
                // For each non-existing relationship, add it to the database
                // Fetch all member IDs existing in DB with given type and name in list
                int[] curIDs = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(type))
                    .Where(x => x.RoleServerId.Equals(serverID))
                    .Where(x => items.Contains(x.ItemId))
                    .Select(x => x.Id)
                    .ToArray();
                // Fetch all member IDs related to the given group BEFORE delete
                int[] relIDs = uow._context.Set<GWLItemSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Where(x => curIDs.Contains(x.ItemPK))
                    .Select(x => x.ItemPK)
                    .ToArray();

                // Delete all existing IDs where type and name matches (above lines ensure no dupes)
                if (curIDs.Count() > 0) {
                    for (int i=0; i<curIDs.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "DELETE FROM GWLItemSet WHERE ListPK = @p0 AND ItemPK = @p1;",
                            group.Id,
                            curIDs[i]);
                    }
                    uow.Complete();
                }
                // Fetch all member IDs related to the given group AFTER delete
                int[] relIDsRemain = uow._context.Set<GWLItemSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Where(x => curIDs.Contains(x.ItemPK))
                    .Select(x => x.ItemPK)
                    .ToArray();

                var deletedIDs = relIDs.Except(relIDsRemain);

                // Return list of all deleted relationships
                successList = uow._context.Set<GWLItem>()
                    .Where(x => x.RoleServerId.Equals(serverID))
                    .Where(x => deletedIDs.Contains(x.Id))
                    .Select(x => x.ItemId)
                    .OrderBy(id => id)
                    .ToArray();

                uow.Complete();
                if (successList.Count() > 0) return true;
                return false;
            }
        }

        public bool AddUnblockedToGroup(string[] names, UnblockedType type, GWLSet group, out string[] successList)
        {
            successList = null;

            using (var uow = _db.UnitOfWork)
            {
                // Get the necessary botconfig ID
                var bc = uow.BotConfig.GetOrCreate();
                var bcField = (type.Equals(UnblockedType.Module)) ? "BotConfigId1" : "BotConfigId";

                // For each non-existing ub, add it to the database
                // Fetch all ub names already in the database
                string[] curNames = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(type))
                    .Select(x => x.Name)
                    .ToArray();
                // Focus only on given names that aren't yet in the database (and auto remove dupes)
                var excludedNames = names.Except(curNames);
                if (excludedNames.Count() > 0) {
                    for (int i=0; i<excludedNames.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            $"INSERT INTO UnblockedCmdOrMdl ('{bcField}', 'Name', 'Type') VALUES (@p0,@p1,@p2);",
                            bc.Id,
                            excludedNames.ElementAt(i),
                            (int)type);
                    }
                    uow.Complete();
                }

                // For each non-existing relationship, add it to the database
                // Fetch all ub IDs existing in DB with given type and name in list
                int[] curIDs = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(type))
                    .Where(x => names.Contains(x.Name))
                    .Select(x => x.Id)
                    .ToArray();
                // Fetch all ub IDs already related to group
                int[] curRel = uow._context.Set<GlobalUnblockedSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Select(x => x.UnblockedPK)
                    .ToArray();
                // Focus only on given IDs that aren't yet related to group (and auto remove dupes)
                var excludedIDs = curIDs.Except(curRel);
                if (excludedIDs.Count() > 0) {
                    for (int i=0; i<excludedIDs.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "INSERT INTO GlobalUnblockedSet ('ListPK', 'UnblockedPK') VALUES (@p0,@p1);",
                            group.Id,
                            excludedIDs.ElementAt(i));
                    }
                    uow.Complete();
                }

                // Return list of all newly added relationships
                successList = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => excludedIDs.Contains(x.Id))
                    .Select(x => x.Name)
                    .OrderBy(n => n)
                    .ToArray();

                uow.Complete();
                if (successList.Count() > 0) return true;
                return false;
            }
        }

        public bool RemoveUnblockedFromGroup(string[] names, UnblockedType type, GWLSet group, out string[] successList)
        {
            successList = null;

            using (var uow = _db.UnitOfWork)
            {
                // For each non-existing relationship, add it to the database
                // Fetch all ub IDs existing in DB with given type and name in list
                int[] curIDs = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(type))
                    .Where(x => names.Contains(x.Name))
                    .Select(x => x.Id)
                    .ToArray();
                // Fetch all ub IDs related to the given group BEFORE delete
                int[] relIDs = uow._context.Set<GlobalUnblockedSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Where(x => curIDs.Contains(x.UnblockedPK))
                    .Select(x => x.UnblockedPK)
                    .ToArray();

                // Delete all existing IDs where type and name matches (above lines ensure no dupes)
                if (curIDs.Count() > 0) {
                    for (int i=0; i<curIDs.Count(); i++) {
                        uow._context.Database.ExecuteSqlCommand(
                            "DELETE FROM GlobalUnblockedSet WHERE ListPK = @p0 AND UnblockedPK = @p1;",
                            group.Id,
                            curIDs[i]);
                    }
                    uow.Complete();
                }
                // Fetch all ub IDs related to the given group AFTER delete
                int[] relIDsRemain = uow._context.Set<GlobalUnblockedSet>()
                    .Where(x => x.ListPK.Equals(group.Id))
                    .Where(x => curIDs.Contains(x.UnblockedPK))
                    .Select(x => x.UnblockedPK)
                    .ToArray();

                var deletedIDs = relIDs.Except(relIDsRemain);

                // Return list of all deleted relationships
                successList = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => deletedIDs.Contains(x.Id))
                    .Select(x => x.Name)
                    .OrderBy(n => n)
                    .ToArray();

                uow.Complete();
                if (successList.Count() > 0) return true;
                return false;
            }
        }

        #endregion Add/Remove

        #region Clear

        public bool ClearAll(GWLSet group)
        {
            return ClearMembers(group) && ClearUnblocked(group);
        }

        public bool ClearMembers(GWLSet group)
        {
            int result;
            using (var uow = _db.UnitOfWork)
            {
                string sql = "DELETE FROM GWLItemSet WHERE GWLItemSet.ListPK = @p0;";
                result = uow._context.Database.ExecuteSqlCommand(sql, group.Id);
                uow.Complete();
            }
            return true;
        }

        public bool ClearMembers(GWLSet group, GWLItemType type)
        {
            int result;
            using (var uow = _db.UnitOfWork)
            {
                string sql = @"DELETE FROM GWLItemSet WHERE GWLItemSet.ListPK = @p0 AND GWLItemSet.ItemPK IN
                    (SELECT Id FROM GWLItem WHERE GWLItem.Type = @p1);";
                result = uow._context.Database.ExecuteSqlCommand(sql, group.Id, type);
                uow.Complete();
            }
            return true;
        }

        public bool ClearUnblocked(GWLSet group)
        {
            int result;
            using (var uow = _db.UnitOfWork)
            {
                string sql = "DELETE FROM GlobalUnblockedSet WHERE GlobalUnblockedSet.ListPK = @p0;";
                result = uow._context.Database.ExecuteSqlCommand(sql, group.Id);
                uow.Complete();
            }
            return true;
        }
        public bool ClearUnblocked(GWLSet group, UnblockedType type)
        {
            int result;
            using (var uow = _db.UnitOfWork)
            {
                string sql = @"DELETE FROM GlobalUnblockedSet WHERE GlobalUnblockedSet.ListPK = @p0 AND GlobalUnblockedSet.UnblockedPK IN
                    (SELECT Id FROM UnblockedCmdOrMdl WHERE UnblockedCmdOrMdl.Type = @p1);";
                result = uow._context.Database.ExecuteSqlCommand(sql, group.Id, type);
                uow.Complete();
            }
            return true;
        }

        #endregion Clear

        #region Purge

        public bool PurgeMember(GWLItemType type, ulong id)
        {
            using (var uow = _db.UnitOfWork)
            {
                uow._context.Set<GWLItem>().Remove(
                    uow._context.Set<GWLItem>()
                    .Where( x => x.Type.Equals(type) )
                    .Where( x => x.RoleServerId.Equals(0) )
                    .Where( x => x.ItemId.Equals(id) )
                    .FirstOrDefault()
                );
                uow.Complete();
            }
            return true;
        }

        public bool PurgeRole(ulong sid, ulong id)
        {
            using (var uow = _db.UnitOfWork)
            {
                uow._context.Set<GWLItem>().Remove(
                    uow._context.Set<GWLItem>()
                    .Where( x => x.Type.Equals(GWLItemType.Role) )
                    .Where( x => x.RoleServerId.Equals(sid) )
                    .Where( x => x.ItemId.Equals(id) )
                    .FirstOrDefault()
                );
                uow.Complete();
            }
            return true;
        }

        public bool PurgeUnblocked(UnblockedType type, string name)
        {
            using (var uow = _db.UnitOfWork)
            {
                // Delete the unblockedcmd record and all relation records
                uow._context.Set<UnblockedCmdOrMdl>().Remove(
                    uow._context.Set<UnblockedCmdOrMdl>()
                    .Where( x => x.Type.Equals(type) )
                    .Where( x => x.Name.Equals(name) )
                    .FirstOrDefault()
                );
                uow.Complete();
            }
            return true;
        }

        #endregion Purge

        #region IsInGroup

        public bool IsMemberInGroup(ulong id, GWLItemType type, GWLSet group)
        {
            var result = true;

            using (var uow = _db.UnitOfWork)
            {
                var temp = uow._context.Set<GWLItem>()
                    .Where( x => x.Type.Equals(type) )
                    .Where( x => x.RoleServerId.Equals(0) )
                    .Where( x => x.ItemId.Equals(id) )
                    .Join(
                        uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i,gi) => new {
                            i.ItemId,
                            gi.ListPK
                        })
                    // Don't need to verify GWLType since the caller should've checked IsCompat
                    .Where( y => y.ListPK.Equals(group.Id) )
                    .FirstOrDefault();

                uow.Complete();

                if (temp != null) {
                  result = true;
                } else {
                  result = false;
                }
            }
            return result;
        }

        public bool IsUserRoleInGroup(ulong uid, GWLSet group)
        {
            var result = true;

            using (var uow = _db.UnitOfWork)
            {
                var temp = group.GWLItemSets
                    .Join( uow._context.Set<GWLItem>(),
                        x => x.ItemPK, i => i.Id,
                        (x, i) => i)
                    .Where( i => i.Type.Equals(GWLItemType.Role) )
                    .Where( i => GuildRoleHasUser(i.RoleServerId, i.ItemId, uid) )
                    .FirstOrDefault();

                uow.Complete();

                if (temp != null) {
                  result = true;
                } else {
                  result = false;
                }
            }
            return result;
        }

        public bool IsRoleInGroup(ulong sid, ulong id, GWLSet group)
        {
            var result = true;

            using (var uow = _db.UnitOfWork)
            {
                var temp = uow._context.Set<GWLItem>()
                    .Where( x => x.Type.Equals(GWLItemType.Role) )
                    .Where( x => x.RoleServerId.Equals(sid) )
                    .Where( x => x.ItemId.Equals(id) )
                    .Join(
                        uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i,gi) => new {
                            i.ItemId,
                            gi.ListPK
                        })
                    .Where( y => y.ListPK.Equals(group.Id) )
                    .FirstOrDefault();

                uow.Complete();

                if (temp != null) {
                  result = true;
                } else {
                  result = false;
                }
            }
            return result;
        }

        public bool IsUnblockedInGroup(string name, UnblockedType type, GWLSet group)
        {
            var result = true;

            using (var uow = _db.UnitOfWork)
            {
                var temp = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where( x => x.Type.Equals(type) )
                    .Where( x => x.Name.Equals(name) )
                    .Join(
                        uow._context.Set<GlobalUnblockedSet>(),
                        u => u.Id, gu => gu.UnblockedPK,
                        (u,gu) => new {
                            u.Name,
                            gu.ListPK
                        })
                    .Where( y => y.ListPK.Equals(group.Id) )
                    .FirstOrDefault();

                uow.Complete();

                if (temp != null) {
                  result = true;
                } else {
                  result = false;
                }
            }
            return result;
        }

        #endregion IsInGroup

        #region CheckUnblocked

        public bool CheckIfUnblockedForAll(string ubName, UnblockedType ubType, int page, out string[] lists, out int count)
        {
            lists = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(ubType))
                    .Where(x => x.Name.Equals(ubName))
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.General)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g);

                uow.Complete();

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip > count) numSkip = numPerPage * ((count-1)/numPerPage);

                lists = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();
            }
            return true;
        }

        public bool CheckIfUnblockedForAll(string mdlName, string cmdName, int page, out string[] lists, out int count)
        {
            lists = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x =>
                        (x.Type.Equals(UnblockedType.Command) && x.Name.Equals(cmdName)) ||
                        (x.Type.Equals(UnblockedType.Module) && x.Name.Equals(mdlName)) )
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.General)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g)
                    .GroupBy(g => g.Id).Select(set => set.FirstOrDefault()).Where(g => g != null);

                uow.Complete();

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip > count) numSkip = numPerPage * ((count-1)/numPerPage);

                lists = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();
            }
            return true;
        }

        public bool CheckIfUnblockedForMember(string ubName, UnblockedType ubType, ulong memID, GWLItemType memType, int page, out string[] lists, out int count)
        {
            lists = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(ubType))
                    .Where(x => x.Name.Equals(ubName))
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Member)),
                        //.Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g
                        )
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => new {
                            g,
                            gi.ItemPK
                        })
                    .Join(uow._context.Set<GWLItem>()
                        .Where(x => x.Type.Equals(memType))
                        .Where(x => x.RoleServerId.Equals(0))
                        .Where(x => x.ItemId.Equals(memID)),
                        gi => gi.ItemPK, i => i.Id,
                        (gi, i) => gi.g);

                uow.Complete();

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip > count) numSkip = numPerPage * ((count-1)/numPerPage);

                lists = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();
            }
            return true;
        }

        public bool CheckIfUnblockedForMember(string mdlName, string cmdName, ulong memID, GWLItemType memType, int page, out string[] lists, out int count)
        {
            lists = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x =>
                        (x.Type.Equals(UnblockedType.Command) && x.Name.Equals(cmdName)) ||
                        (x.Type.Equals(UnblockedType.Module) && x.Name.Equals(mdlName)) )
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Member)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g
                        )
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => new {
                            g,
                            gi.ItemPK
                        })
                    .Join(uow._context.Set<GWLItem>()
                        .Where(x => x.Type.Equals(memType))
                        .Where(x => x.RoleServerId.Equals(0))
                        .Where(x => x.ItemId.Equals(memID)),
                        gi => gi.ItemPK, i => i.Id,
                        (gi, i) => gi.g)
                    .GroupBy(g => g.Id).Select(set => set.FirstOrDefault()).Where(g => g != null);

                uow.Complete();

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip > count) numSkip = numPerPage * ((count-1)/numPerPage);

                lists = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();
            }
            return true;
        }

        public bool CheckIfUnblockedForUserRole(string ubName, UnblockedType ubType, ulong uid, int page, out string[] names, out int count)
        {
            names = null; count = 0;

            using (var uow = _db.UnitOfWork)
            {
                // Find all roles linked to the given Unblocked data
                var roles = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(ubType))
                    .Where(x => x.Name.Equals(ubName))
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    // Role GWLSets
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g)
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => gi)
                    // Roles
                    .Join(uow._context.Set<GWLItem>()
                        .Where(i => i.Type.Equals(GWLItemType.Role)),
                        rel => rel.ItemPK, i => i.Id,
                        (rel, i) => i)
                    // Remove duplicate roles
                    .GroupBy(r => r.Id).Select(set => set.FirstOrDefault()).Where(r => r != null)
                    // Filter down to only those with the given user
                    .Where(i => GuildRoleHasUser(i.RoleServerId, i.ItemId, uid));

                // Find all groups linked to the filtered roles
                var groups = roles
                    .Join(uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i, gi) => gi.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        pk => pk, g => g.Id,
                        (pk, g) => g)
                    .GroupBy(g => g.Id).Select(set => set.FirstOrDefault()).Where(g => g != null);

                uow.Complete();

                if (groups != null)	count = groups.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = groups
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? $"{enabledText} {g.ListName}" : $"{disabledText} {g.ListName}" )
                    .ToArray();
            }
            return true;
        }

        public bool CheckIfUnblockedForUserRole(string mdlName, string cmdName, ulong uid, int page, out string[] names, out int count)
        {
            names = null; count = 0;
            using (var uow = _db.UnitOfWork)
            {
                // Find all roles linked to the given Unblocked data
                var roles = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x =>
                        (x.Type.Equals(UnblockedType.Command) && x.Name.Equals(cmdName)) ||
                        (x.Type.Equals(UnblockedType.Module) && x.Name.Equals(mdlName)) )
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    // Role GWLSets
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g)
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => gi)
                    // Roles
                    .Join(uow._context.Set<GWLItem>()
                        .Where(i => i.Type.Equals(GWLItemType.Role)),
                        rel => rel.ItemPK, i => i.Id,
                        (rel, i) => i)
                    // Remove duplicate roles
                    .GroupBy(r => r.Id).Select(set => set.FirstOrDefault()).Where(r => r != null)
                    // Filter down to only those with the given user
                    .Where(i => GuildRoleHasUser(i.RoleServerId, i.ItemId, uid));

                // Find all groups linked to the filtered roles
                var groups = roles
                    .Join(uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i, gi) => gi.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        pk => pk, g => g.Id,
                        (pk, g) => g)
                    .GroupBy(g => g.Id).Select(set => set.FirstOrDefault()).Where(g => g != null);

                uow.Complete();

                if (groups != null)	count = groups.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = groups
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? $"{enabledText} {g.ListName}" : $"{disabledText} {g.ListName}" )
                    .ToArray();
            }
            return true;
        }

        public bool CheckIfUnblockedForRole(string ubName, UnblockedType ubType, ulong sid, ulong id, int page, out string[] lists, out int count)
        {
            lists = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(ubType))
                    .Where(x => x.Name.Equals(ubName))
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role)),
                        //.Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g
                        )
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => new {
                            g,
                            gi.ItemPK
                        })
                    .Join(uow._context.Set<GWLItem>()
                        .Where(x => x.Type.Equals(GWLItemType.Role))
                        .Where(x => x.RoleServerId.Equals(sid))
                        .Where(x => x.ItemId.Equals(id)),
                        gi => gi.ItemPK, i => i.Id,
                        (gi, i) => gi.g)
                    .GroupBy(g => g.Id).Select(set => set.FirstOrDefault()).Where(g => g != null);

                uow.Complete();

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip > count) numSkip = numPerPage * ((count-1)/numPerPage);

                lists = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();
            }
            return true;
        }

        public bool CheckIfUnblockedForRole(string mdlName, string cmdName, ulong sid, ulong id, int page, out string[] lists, out int count)
        {
            lists = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x =>
                        (x.Type.Equals(UnblockedType.Command) && x.Name.Equals(cmdName)) ||
                        (x.Type.Equals(UnblockedType.Module) && x.Name.Equals(mdlName)) )
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role)),
                        // .Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g
                        )
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => new {
                            g,
                            gi.ItemPK
                        })
                    .Join(uow._context.Set<GWLItem>()
                        .Where(x => x.Type.Equals(GWLItemType.Role))
                        .Where(x => x.RoleServerId.Equals(sid))
                        .Where(x => x.ItemId.Equals(id)),
                        gi => gi.ItemPK, i => i.Id,
                        (gi, i) => gi.g)
                    .GroupBy(g => g.Id).Select(set => set.FirstOrDefault()).Where(g => g != null);

                uow.Complete();

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip > count) numSkip = numPerPage * ((count-1)/numPerPage);

                lists = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();
            }
            return true;
        }

        #endregion CheckUnblocked

        #region Unblocker
        public bool CheckIfUnblockedAll(string ubName, UnblockedType ubType)
        {
            using (var uow = _db.UnitOfWork)
            {
                var result = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(ubType))
                    .Where(x => x.Name.Equals(ubName))
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.General))
                        .Where(g => g.IsEnabled.Equals(true)),
                        gubPK => gubPK, g => g.Id,
                        (gubPK, g) => g.Id)
                    .Count();

                uow.Complete();

                if (result > 0) return true;
                return false;
            }
        }

        public bool CheckIfUnblockedAll(string mdlName, string cmdName)
        {
            using (var uow = _db.UnitOfWork)
            {
                var result = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x =>
                        (x.Type.Equals(UnblockedType.Command) && x.Name.Equals(cmdName)) ||
                        (x.Type.Equals(UnblockedType.Module) && x.Name.Equals(mdlName)) )
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.General))
                        .Where(g => g.IsEnabled.Equals(true)),
                        gubPK => gubPK, g => g.Id,
                        (gubPK, g) => g.Id)
                    .Count();

                uow.Complete();

                if (result > 0) return true;
                return false;
            }
        }

        public bool CheckIfUnblocked(string ubName, UnblockedType ubType, ulong usrId, ulong srvrId, ulong chnlId)
        {
            using (var uow = _db.UnitOfWork)
            {
                var result = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x => x.Type.Equals(ubType))
                    .Where(x => x.Name.Equals(ubName))
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Member))
                        .Where(g => g.IsEnabled.Equals(true)),
                        gubPK => gubPK, g => g.Id,
                        (gubPK, g) => g.Id)
                    .Join(uow._context.Set<GWLItemSet>(),
                        gId => gId, gi => gi.ListPK,
                        (gId, gi) => gi.ItemPK)
                    .Join(uow._context.Set<GWLItem>()
                        .Where(x =>
                            (x.Type.Equals(GWLItemType.Server) && x.ItemId.Equals(srvrId)) ||
                            (x.Type.Equals(GWLItemType.Channel) && x.ItemId.Equals(chnlId)) ||
                            (x.Type.Equals(GWLItemType.User) && x.ItemId.Equals(usrId)) ),
                        giPK => giPK, i => i.Id,
                        (giPK, i) => giPK)
                    .Count();

                uow.Complete();

                if (result > 0) return true;
                return false;
            }
        }

        public bool CheckIfUnblocked(string mdlName, string cmdName, ulong usrId, ulong srvrId, ulong chnlId)
        {
            using (var uow = _db.UnitOfWork)
            {
                var result = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x =>
                        (x.Type.Equals(UnblockedType.Command) && x.Name.Equals(cmdName)) ||
                        (x.Type.Equals(UnblockedType.Module) && x.Name.Equals(mdlName)) )
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Member))
                        .Where(g => g.IsEnabled.Equals(true)),
                        gubPK => gubPK, g => g.Id,
                        (gubPK, g) => g.Id)
                    .Join(uow._context.Set<GWLItemSet>(),
                        gId => gId, gi => gi.ListPK,
                        (gId, gi) => gi.ItemPK)
                    .Join(uow._context.Set<GWLItem>()
                        .Where(x =>
                            (x.Type.Equals(GWLItemType.Server) && x.ItemId.Equals(srvrId)) ||
                            (x.Type.Equals(GWLItemType.Channel) && x.ItemId.Equals(chnlId)) ||
                            (x.Type.Equals(GWLItemType.User) && x.ItemId.Equals(usrId)) ),
                        giPK => giPK, i => i.Id,
                        (giPK, i) => giPK)
                    .Count();

                uow.Complete();

                if (result > 0) return true;
                return false;
            }
        }

        public bool IsUserRoleUnblocked(ulong uid, string cmdName, string mdlName)
        {
            int count = 0;
            // In general, a user is likely to have many more roles than there are unblocked roles
            // So we should iterate over the unblocked roles to find any that contains our user
            // After first finding matching unblocked items

            using (var uow = _db.UnitOfWork)
            {
                // Find all roles linked to the given Unblocked data
                var roles = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(x =>
                        (x.Type.Equals(UnblockedType.Command) && x.Name.Equals(cmdName)) ||
                        (x.Type.Equals(UnblockedType.Module) && x.Name.Equals(mdlName)) )
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        ub => ub.Id, gub => gub.UnblockedPK,
                        (ub,gub) => gub.ListPK)
                    // Enabled Role GWLSets
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role))
                        .Where(g => g.IsEnabled.Equals(true)),
                        gub => gub, g => g.Id,
                        (gub, g) => g)
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => gi)
                    // Roles
                    .Join(uow._context.Set<GWLItem>()
                        .Where(i => i.Type.Equals(GWLItemType.Role)),
                        rel => rel.ItemPK, i => i.Id,
                        (rel, i) => i)
                    // Remove duplicate roles
                    .GroupBy(role => role.Id).Select(set => set.FirstOrDefault()).Where(r => r != null)
                    // Filter down to only those with the given user
                    .Where(i => GuildRoleHasUser(i.RoleServerId, i.ItemId, uid));

                uow.Complete();

                if (roles != null)	count = roles.Count();
            }
            return (count > 0);
        }

        #endregion Unblocker

        #region GetObject

        /// <summary>Assumes provided listName is converted ToLowerInvariant()</summary>
        public bool GetGroupByName(string listName, out GWLSet group)
        {
            group = null;

            if (string.IsNullOrWhiteSpace(listName)) return false;

            using (var uow = _db.UnitOfWork)
            {
                group = uow._context.Set<GWLSet>()
                    .Where(x => x.ListName.ToLowerInvariant().Equals(listName))
                    .Include(x => x.GlobalUnblockedSets)
                    .Include(x => x.GWLItemSets)
                    .FirstOrDefault();

                if (group == null) { return false; }
                else { return true; }
            }
        }

        public bool GetMemberByIdType(ulong id, GWLItemType type, out GWLItem item)
        {
            item = null;

            using (var uow = _db.UnitOfWork)
            {
                // Retrieve the member item given name
                item = uow._context.Set<GWLItem>()
                    .Where( x => x.Type.Equals(type) )
                    .Where( x => x.ItemId.Equals(id) )
                    .FirstOrDefault();

                if (item == null) { return false; }
                else { return true; }
            }
        }

        public bool GetUnblockedByNameType(string name, UnblockedType type, out UnblockedCmdOrMdl item)
        {
            item = null;

            if (string.IsNullOrWhiteSpace(name)) return false;

            using (var uow = _db.UnitOfWork)
            {
                // Retrieve the UnblockedCmdOrMdl item given name
                item = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where( x => x.Name.Equals(name) )
                    .Where( x => x.Type.Equals(type) )
                    .FirstOrDefault();

                uow.Complete();

                if (item == null) { return false; }
                else { return true; }
            }
        }

        #endregion GetObject

        #region GetGroupNames

         public bool GetGroupNames(GWLType type, int page, out string[] names, out int count)
        {
            names = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<GWLSet>()
                    .Where(g => g.Type.Equals(type));

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();

                uow.Complete();
            }
            return true;
        }

        /// <summary>
        /// Output a list of GWL with GWLType.Member for which there is
        /// at least one member with the given type and id
        /// </summary>
        public bool GetGroupNamesByMemberType(ulong id, GWLItemType type, int page, out string[] names, out int count)
        {
            names = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<GWLItem>()
                .Where(i => i.Type.Equals(type))
                .Where(i => i.RoleServerId.Equals(0))
                .Where(i => i.ItemId.Equals(id))
                .Join(uow._context.Set<GWLItemSet>(),
                    i => i.Id, gi => gi.ItemPK,
                    (i, gi) => gi.ListPK)
                .Join(uow._context.Set<GWLSet>()
                    .Where(g=>g.Type.Equals(GWLType.Member)),
                    listPK => listPK, g => g.Id,
                    (listPK, g) => g);
                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();

                uow.Complete();
            }
            return true;
        }

        public bool GetGroupNamesByUserRole(ulong uid, int page, out string[] names, out int count)
        {
            names = null; count = 0;
            using (var uow = _db.UnitOfWork)
            {
                // Find all roles linked to a Role GWLSet
                var roles = uow._context.Set<GWLSet>()
                    .Where(g=>g.Type.Equals(GWLType.Role))
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => gi)
                    .Join(uow._context.Set<GWLItem>()
                        .Where(i => i.Type.Equals(GWLItemType.Role)),
                        rel => rel.ItemPK, i => i.Id,
                        (rel, i) => i)
                    // Remove duplicates
                    .GroupBy(r => r.Id).Select(set => set.FirstOrDefault()).Where(r => r != null)
                    // Filter down to only those with the given user
                    .Where(i => GuildRoleHasUser(i.RoleServerId, i.ItemId, uid));

                // Get all groups linked to the filtered roles
                var groups = roles
                    .Join(uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i, gi) => gi.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g=>g.Type.Equals(GWLType.Role)),
                        listPK => listPK, g => g.Id,
                        (listPK, g) => g)
                    .GroupBy(g => g).Select(set => set.FirstOrDefault()).Where(g => g != null);

                uow.Complete();

                if (groups != null)	count = groups.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = groups
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? $"{enabledText} {g.ListName}" : $"{disabledText} {g.ListName}" )
                    .ToArray();
            }
            return true;
        }

        /// <summary>
        /// Output a list of GWL with GWLType.Role for which there is at least one
        /// RoleServerID-ItemID pair that matches the given server and role ID
        /// </summary>
        public bool GetGroupNamesByRole(ulong serverID, ulong roleID, int page, out string[] names, out int count)
        {
            names = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<GWLItem>()
                .Where(i => i.Type.Equals(GWLItemType.Role))
                .Where(i => i.RoleServerId.Equals(serverID))
                .Where(i => i.ItemId.Equals(roleID))
                .Join(uow._context.Set<GWLItemSet>(),
                    i => i.Id, gi => gi.ItemPK,
                    (i, gi) => gi.ListPK)
                .Join(uow._context.Set<GWLSet>()
                    .Where(g=>g.Type.Equals(GWLType.Role)),
                    listPK => listPK, g => g.Id,
                    (listPK, g) => g);
                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();

                uow.Complete();
            }
            return true;
        }

        /// <summary>
        /// Output a list of GWL with GWLType.Role for which there is at least one
        /// RoleServerID that matches serverID
        /// </summary>
        public bool GetGroupNamesByServer(ulong serverID, int page, out string[] names, out int count)
        {
            names = null;
            using (var uow = _db.UnitOfWork)
            {
                var allnames = uow._context.Set<GWLItem>()
                .Where(i => i.Type.Equals(GWLItemType.Role))
                .Where(i => i.RoleServerId.Equals(serverID))
                .Join(uow._context.Set<GWLItemSet>(),
                    i => i.Id, gi => gi.ItemPK,
                    (i, gi) => gi.ListPK)
                .Join(uow._context.Set<GWLSet>()
                    .Where(g=>g.Type.Equals(GWLType.Role)),
                    listPK => listPK, g => g.Id,
                    (listPK, g) => g)
                .GroupBy(g=>g.ListName).Select(y=>y.FirstOrDefault()) // since RoleServerID is non-unique, remove dupes https://stackoverflow.com/a/4095023
                .Where(y => y != null); // Ensure no null values from FirstOrDefault
                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();

                uow.Complete();
            }
            return true;
        }

        public bool GetGroupNamesByUnblocked(string name, UnblockedType type, GWLType typeG, int page, out string[] names, out int count)
        {
            names = null;
            count = 0;

            // Get the item
            UnblockedCmdOrMdl item;
            bool exists = GetUnblockedByNameType(name, type, out item);

            if (!exists) return false;

            using (var uow = _db.UnitOfWork)
            {
                // Retrieve a list of set names linked to GlobalUnblockedSets.ListPK
                var allnames = uow._context.Set<GWLSet>()
                        .Where(g=>g.Type.Equals(typeG))
                    .Join(
                        uow._context.Set<GlobalUnblockedSet>()
                            .Where(u => u.UnblockedPK.Equals(item.Id)),
                        g => g.Id, gu => gu.ListPK,
                        (group, relation) => group);
                uow.Complete();

                count = allnames.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = allnames
                    .OrderBy(g => g.ListName.ToLowerInvariant())
                    .Skip(numSkip)
                    .Take(numPerPage)
                    .Select(g => (g.IsEnabled) ? enabledText + g.ListName : disabledText + g.ListName)
                    .ToArray();
            }
            return true;
        }

        #endregion GetGroupNames

        #region GetGroupMembers

        public bool GetGroupMembers(GWLSet group, GWLItemType type, int page, out ulong[] results, out int count)
        {
            results = null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = group.GWLItemSets
                    .Join(uow._context.Set<GWLItem>()
                          .Where(m => m.Type.Equals(type))
                        .Where(m => m.RoleServerId.Equals(0)),
                            p => p.ItemPK,
                            m => m.Id,
                            (pair,member) => member.ItemId)
                    .OrderBy(id => id);
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                results = anon.Skip(numSkip).Take(numPerPage).ToArray();
            }
            return true;
        }

        public bool GetGroupRoles(GWLSet group, int page, out ulong[] results, out int count)
        {
            results = null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = group.GWLItemSets
                    .Join(uow._context.Set<GWLItem>()
                          .Where(m => m.Type.Equals(GWLItemType.Role))
                        .Where(m => !m.RoleServerId.Equals(0)),
                            p => p.ItemPK,
                            m => m.Id,
                            (pair,member) => member.ItemId)
                    .OrderBy(id => id);
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                results = anon.Skip(numSkip).Take(numPerPage).ToArray();
            }
            return true;
        }

        public bool GetGroupMembers(GWLSet group, GWLItemType type, ulong ctx, int page, out string[] results, out int count)
        {
            results = null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = group.GWLItemSets
                    .Join(uow._context.Set<GWLItem>()
                          .Where(m => m.Type.Equals(type))
                        .Where(m => m.RoleServerId.Equals(0)),
                            p => p.ItemPK,
                            m => m.Id,
                            (pair,member) => member.ItemId)
                    .OrderBy(id => id);
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                var anon2 = anon.Skip(numSkip).Take(numPerPage).ToArray();

                results = new string[anon2.Count()];
                for (int i=0; i<anon2.Count(); i++) {
                    var m = anon2.ElementAt(i);
                    results[i] = GetMemberNameMention(type, m, ctx);
                }
            }
            return true;
        }

        public bool GetGroupRoles(GWLSet group, ulong ctx, int page, out string[] results, out int count)
        {
            results = null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = group.GWLItemSets
                    .Join(uow._context.Set<GWLItem>()
                          .Where(m => m.Type.Equals(GWLItemType.Role))
                        .Where(m => !m.RoleServerId.Equals(0)),
                            p => p.ItemPK,
                            m => m.Id,
                            (pair,member) => new { member.ItemId, member.RoleServerId } )
                    .OrderBy(r => r.ItemId);
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                var anon2 = anon.Skip(numSkip).Take(numPerPage).ToArray();

                results = new string[anon2.Count()];
                for (int i=0; i<anon2.Count(); i++) {
                    var role = anon2.ElementAt(i);
                    results[i] = GetRoleNameMention(role.ItemId, role.RoleServerId, ctx);
                }
            }
            return true;
        }

        #endregion GetGroupMembers

        #region GetUnblockedNames

        public bool GetGroupUnblockedNames(GWLSet group, UnblockedType type, int page, out string[] names, out int count)
        {
            names = null;
            using (var uow = _db.UnitOfWork)
            {
                // Retrieve a list of unblocked names linked to group on GlobalUnblockedSets.UnblockedPK
                var anon = group.GlobalUnblockedSets
                    .Join(uow._context.Set<UnblockedCmdOrMdl>()
                          .Where(u => u.Type.Equals(type)),
                            gu => gu.UnblockedPK, u => u.Id,
                            (relation, unblocked) => unblocked.Name)
                    .OrderBy(a => a.ToLowerInvariant());
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = anon.Skip(numSkip).Take(numPerPage).ToArray();
            }
            return true;
        }

        public bool GetUnblockedNames(UnblockedType type, int page, out string[] names, out int count)
        {
            names = null;
            using (var uow = _db.UnitOfWork)
            {
                // Retrieve a list of unblocked names with at least one relationship record
                var anon = uow._context.Set<UnblockedCmdOrMdl>()
                    .Where(u => u.Type.Equals(type))
                    .GroupJoin(
                        uow._context.Set<GlobalUnblockedSet>(),
                        u => u.Id, gu => gu.UnblockedPK,
                        (unblocked, relations) => new {
                            Name = unblocked.Name,
                            NumRelations = relations.Count()
                        })
                    .OrderBy(a => a.Name.ToLowerInvariant());

                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                var subset = anon.Skip(numSkip).Take(numPerPage).ToArray();

                names = new string[subset.Count()];
                for (int i=0; i<subset.Length; i++)
                {
                    if (subset[i].NumRelations > 0)
                    {
                        string lists = (subset[i].NumRelations > 1) ? " lists)" : " list)";
                        names[i] = subset[i].Name + " (" + subset[i].NumRelations + lists;
                    } else {
                        names[i] = subset[i].Name + " (0 lists)";
                    }
                }
            }
            return true;
        }

        public bool GetUnblockedNamesForAll(UnblockedType type, int page, out string[] names, out int count)
        {
            names= null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.General))
                        .Where(g => g.IsEnabled.Equals(true))
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        g => g.Id, gub => gub.ListPK,
                        (g, gub) => gub.UnblockedPK)
                    .Join(uow._context.Set<UnblockedCmdOrMdl>()
                        .Where(x => x.Type.Equals(type)),
                        uPK => uPK, ub => ub.Id,
                        (uPK, ub) => ub.Name)
                    .GroupBy(a => a).Select(set => set.FirstOrDefault()).Where(u => u != null)
                    .OrderBy(a => a.ToLowerInvariant());
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = anon.Skip(numSkip).Take(numPerPage).ToArray();
            }
            return true;
        }

        public bool GetUnblockedNamesForMember(UnblockedType type, ulong id, GWLItemType memType, int page, out string[] names, out int count)
        {
            names= null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(memType))
                    .Where(x => x.RoleServerId.Equals(0))
                    .Where(x => x.ItemId.Equals(id))
                    .Join(uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i, gi) => gi.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Member))
                        .Where(g => g.IsEnabled.Equals(true)),
                        listPK => listPK, g => g.Id,
                        (listPK, g) => g.Id)
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        listPK => listPK, gub => gub.ListPK,
                        (listPK, gub) => gub.UnblockedPK)
                    .Join(uow._context.Set<UnblockedCmdOrMdl>()
                        .Where(x => x.Type.Equals(type)),
                        uPK => uPK, ub => ub.Id,
                        (uPK, ub) => ub.Name)
                    .GroupBy(a => a).Select(set => set.FirstOrDefault()).Where(u => u != null)
                    .OrderBy(a => a.ToLowerInvariant());
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = anon.Skip(numSkip).Take(numPerPage).ToArray();
            }
            return true;
        }

        public bool GetUnblockedNamesForUserRole(UnblockedType type, ulong uid, int page, out string[] names, out int count)
        {
            names= null; count = 0;
            using (var uow = _db.UnitOfWork)
            {
                // Find all roles linked to a Role GWLSet
                var roles = uow._context.Set<GWLSet>()
                    .Where(g=>g.Type.Equals(GWLType.Role))
                    .Join(uow._context.Set<GWLItemSet>(),
                        g => g.Id, gi => gi.ListPK,
                        (g, gi) => gi)
                    .Join(uow._context.Set<GWLItem>()
                        .Where(i => i.Type.Equals(GWLItemType.Role)),
                        rel => rel.ItemPK, i => i.Id,
                        (rel, i) => i)
                    // Remove Duplicates
                    .GroupBy(r => r.Id).Select(set => set.FirstOrDefault()).Where(r => r != null)
                    // Filter down to only those that contain the user
                    .Where(i => GuildRoleHasUser(i.RoleServerId, i.ItemId, uid));

                // Find all Unblocked data for the filtered roles
                var anon = roles
                    .Join(uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i, gi) => gi.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role))
                        .Where(g => g.IsEnabled.Equals(true)),
                        listPK => listPK, g => g.Id,
                        (listPK, g) => g.Id)
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        pk => pk, gub => gub.ListPK,
                        (pk ,gub) => gub.UnblockedPK)
                    .Join(uow._context.Set<UnblockedCmdOrMdl>()
                        .Where(x => x.Type.Equals(type)),
                        uPK => uPK, ub => ub.Id,
                        (uPK, ub) => ub.Name)
                    .GroupBy(n => n).Select(set => set.FirstOrDefault()).Where(n => n != null);

                uow.Complete();

                if (anon != null) count = anon.Count();
                if (count <= 0) return false;

                names = anon.ToArray();
            }
            return true;
        }

        public bool GetUnblockedNamesForRole(UnblockedType type, ulong id, ulong sid, int page, out string[] names, out int count)
        {
            names= null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = uow._context.Set<GWLItem>()
                    .Where(x => x.Type.Equals(GWLItemType.Role))
                    .Where(x => x.RoleServerId.Equals(sid))
                    .Where(x => x.ItemId.Equals(id))
                    .Join(uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i, gi) => gi.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Role))
                        .Where(g => g.IsEnabled.Equals(true)),
                        listPK => listPK, g => g.Id,
                        (listPK, g) => g.Id)
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        listPK => listPK, gub => gub.ListPK,
                        (listPK, gub) => gub.UnblockedPK)
                    .Join(uow._context.Set<UnblockedCmdOrMdl>()
                        .Where(x => x.Type.Equals(type)),
                        uPK => uPK, ub => ub.Id,
                        (uPK, ub) => ub.Name)
                    .GroupBy(a => a).Select(set => set.FirstOrDefault()).Where(u => u != null)
                    .OrderBy(a => a.ToLowerInvariant());
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = anon.Skip(numSkip).Take(numPerPage).ToArray();
            }
            return true;
        }

        /// <summary>
        /// Collects all commands OR modules linked to either the ChannelID or ServerID
        /// Then aggregates each set and sorts to take just what is needed for the current page
        /// </summary>
        public bool GetUnblockedNamesForContext(UnblockedType type, ulong cid, ulong sid, int page, out string[] names, out int count)
        {
            names= null;
            using (var uow = _db.UnitOfWork)
            {
                var anon = uow._context.Set<GWLItem>()
                    .Where(x => x.RoleServerId.Equals(0) &&
                        (x.Type.Equals(GWLItemType.Channel) && x.ItemId.Equals(cid)) ||
                        (x.Type.Equals(GWLItemType.Server) && x.ItemId.Equals(sid))
                    )
                    .Join(uow._context.Set<GWLItemSet>(),
                        i => i.Id, gi => gi.ItemPK,
                        (i, gi) => gi.ListPK)
                    .Join(uow._context.Set<GWLSet>()
                        .Where(g => g.Type.Equals(GWLType.Member))
                        .Where(g => g.IsEnabled.Equals(true)),
                        listPK => listPK, g => g.Id,
                        (listPK, g) => g.Id)
                    .Join(uow._context.Set<GlobalUnblockedSet>(),
                        listPK => listPK, gub => gub.ListPK,
                        (listPK, gub) => gub.UnblockedPK)
                    .Join(uow._context.Set<UnblockedCmdOrMdl>()
                        .Where(x => x.Type.Equals(type)),
                        uPK => uPK, ub => ub.Id,
                        (uPK, ub) => ub.Name)
                    .GroupBy(a => a).Select(set => set.FirstOrDefault()).Where(u => u != null)
                    .OrderBy(a => a.ToLowerInvariant());
                uow.Complete();

                count = anon.Count();
                if (count <= 0) return false;

                int numSkip = page*numPerPage;
                if (numSkip >= count) numSkip = numPerPage * ((count-1)/numPerPage);

                names = anon.Skip(numSkip).Take(numPerPage).ToArray();
            }
            return true;
        }

        #endregion GetUnblockedNames

        #region Resolve ulong IDs

        public string[] GetNameOrMentionFromId(GWLItemType type, ulong[] ids, bool inline=false)
        {
            string[] str = new string[ids.Length];
            string sep = inline ? " " : "\n\t";
            string noName = _strs.GetText("unresolvable_name", 0, "permissions");

            switch (type) {
                case GWLItemType.User:
                    for (var i = 0; i < ids.Length; i++) {
                      str[i] = $"{MentionUtils.MentionUser(ids[i])}{sep}{ids[i]}";
                    }
                    break;

                case GWLItemType.Channel:
                    for (var i = 0; i < ids.Length; i++) {
                      str[i] = $"{MentionUtils.MentionChannel(ids[i])}{sep}{ids[i]}";
                    }
                    break;

                case GWLItemType.Server:
                    for (var i = 0; i < ids.Length; i++) {
                        var guild = _client.Guilds.FirstOrDefault(g => g.Id.Equals(ids[i]));
                        string name = (guild != null) ? guild.Name : noName;
                        str[i] = $"[{name}](https://discordapp.com/channels/{ids[i]}/ '{name}'){sep}{ids[i]}";
                    }
                    break;

                case GWLItemType.Role:
                    for (var i = 0; i < ids.Length; i++) {
                      str[i] = $"{MentionUtils.MentionRole(ids[i])}{sep}{ids[i]}";
                    }
                    break;

                default:
                    for (var i = 0; i < ids.Length; i++) {
                      str[i] = ids[i].ToString();
                    }
                    break;
            }

            return str;
        }
        public string GetNameOrMentionFromId(GWLItemType type, ulong id, bool inline=false)
        {
            string sep = inline ? " " : "\n\t";
            string noName = _strs.GetText("unresolvable_name", 0, "permissions");
            string str = $"{noName}{sep}{id}";

            switch (type) {
                case GWLItemType.User:
                    str = $"{MentionUtils.MentionUser(id)}{sep}{id}";
                    break;

                case GWLItemType.Channel:
                    str = $"{MentionUtils.MentionChannel(id)}{sep}{id}";
                    break;

                case GWLItemType.Server:
                    var guild = _client.Guilds.FirstOrDefault(g => g.Id.Equals(id));
                    string name = (guild != null) ? guild.Name : noName;
                    str = $"[{name}](https://discordapp.com/channels/{id}/ '{name}'){sep}{id}";
                    break;

                case GWLItemType.Role:
                    str = $"{MentionUtils.MentionRole(id)}{sep}{id}";
                    break;

                default:
                    str = id.ToString();
                    break;
            }

            return str;
        }

        public string GetMemberNameMention(GWLItemType type, ulong id, ulong ctxId, bool inline=false)
        {
            string sep = inline ? " " : "\n\t";
            string noName = _strs.GetText("unresolvable_name", 0, "permissions");
            string str = $"{noName}{sep}{id}";

            switch (type) {
                case GWLItemType.User:
                    SocketGuild guildU = _client.Guilds
                        .Where(g => g.Id.Equals(ctxId))
                        .Where(g => g.GetUser(id) != null)
                        .FirstOrDefault();
                    if (guildU != null) return GetNameOrMentionFromId(type, id, inline);

                    string name = _client.Guilds
                        .Where(g => g.GetUser(id) != null)
                        .Select(g => g.GetUser(id).Nickname)
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(name)) str = $"[@{name}](https://discordapp.com/users/{id}/ '{name}'){sep}{id}";
                    break;

                case GWLItemType.Channel:
                    SocketGuildChannel chnl = _client.GetChannel(id) as SocketGuildChannel;
                    if (chnl != null) {
                        if (ctxId == chnl.Guild.Id) {
                            return GetNameOrMentionFromId(type, id, inline);
                        }
                        str = $"[#{chnl.Name}](https://discordapp.com/channels/{id}/ '{chnl.Guild.Name}'){sep}{id}";
                    }
                    break;

                case GWLItemType.Server:
                    SocketGuild guild = _client.Guilds.FirstOrDefault(g => g.Id.Equals(id));
                    if (guild != null) {
                        str = $"[{guild.Name}](https://discordapp.com/channels/{id}/ '{guild.Name}'){sep}{id}";
                    }
                    break;

                default:
                    break;
            }
            return str;
        }

        public string[] GetMemberNameMention(GWLItemType type, ulong[] ids, ulong ctxId, bool inline=false)
        {
            string sep = inline ? " " : "\n\t";
            string noName = _strs.GetText("unresolvable_name", 0, "permissions");
            string[] str = new string[ids.Count()];

            switch (type) {
                case GWLItemType.User:
                    for (int i=0; i<ids.Length; i++) {
                        SocketGuild guildU = _client.Guilds
                            .Where(g => g.Id.Equals(ctxId))
                            .Where(g => g.GetUser(ids[i]) != null)
                            .FirstOrDefault();
                        if (guildU != null) {
                            str[i] = GetNameOrMentionFromId(type, ids[i], inline);
                        } else {
                            string name = _client.Guilds
                                .Where(g => g.GetUser(ids[i]) != null)
                                .Select(g => g.GetUser(ids[i]).Nickname)
                                .FirstOrDefault();
                            if (!string.IsNullOrEmpty(name)) {
                                str[i] = $"[@{name}](https://discordapp.com/users/{ids[i]}/ '{name}'){sep}{ids[i]}";
                            } else {
                                str[i] = $"{noName}{sep}{ids[i]}";
                            }
                        }
                    }
                    break;

                case GWLItemType.Channel:
                    for (int i=0; i<ids.Length; i++) {
                        SocketGuildChannel chnl = _client.GetChannel(ids[i]) as SocketGuildChannel;
                        if (chnl != null) {
                            if (ctxId == chnl.Guild.Id) {
                                str[i] = GetNameOrMentionFromId(type, ids[i], inline);
                            } else {
                                str[i] = $"[#{chnl.Name}](https://discordapp.com/channels/{ids[i]}/ '{chnl.Guild.Name}'){sep}{ids[i]}";
                            }
                        } else {
                            str[i] = $"{noName}{sep}{ids[i]}";
                        }
                    }
                    break;

                case GWLItemType.Server:
                    for (int i = 0; i<ids.Length; i++) {
                        SocketGuild guild = _client.Guilds.FirstOrDefault(g => g.Id.Equals(ids[i]));
                        if (guild != null) {
                            str[i] = $"[{guild.Name}](https://discordapp.com/channels/{ids[i]}/ '{guild.Name}'){sep}{ids[i]}";
                        } else {
                            str[i] = $"{noName}{sep}{ids[i]}";
                        }
                    }
                    break;

                default:
                    break;
            }
            return str;
        }

        public string GetRoleNameMention(ulong id, ulong sid, ulong ctx, bool inline=false)
        {
            string sep = inline ? " " : "\n\t";
            string noName = _strs.GetText("unresolvable_name", 0, "permissions");
            string str = $"{noName}{sep}{id}";

            if (ctx == sid) return GetNameOrMentionFromId(GWLItemType.Role,id,inline);

            SocketGuild guild = _client.Guilds.FirstOrDefault(g => g.Id.Equals(sid));
            if (guild != null) {
                SocketRole role = guild.Roles.Where(r => r.Id.Equals(id)).FirstOrDefault();
                if (role != null) {
                    str = $"[@{role.Name}](https://discordapp.com/channels/{sid}/ '{guild.Name}'){sep}{id}";
                }
            }
            return str;
        }

        public string[] GetRoleNameMention(ulong[] ids, ulong sid, ulong ctx, bool inline=false)
        {
            string sep = inline ? " " : "\n\t";
            string noName = _strs.GetText("unresolvable_name", 0, "permissions");
            string[] str = new string[ids.Count()];

            if (ctx == sid) return GetNameOrMentionFromId(GWLItemType.Role,ids,inline);

            SocketGuild guild = _client.Guilds.FirstOrDefault(g => g.Id.Equals(sid));
            if (guild != null) {
                for (int i=0; i<ids.Count(); i++) {
                    SocketRole role = guild.Roles.Where(r => r.Id.Equals(ids[i])).FirstOrDefault();
                    if (role != null) {
                        str[i] = $"[@{role.Name}](https://discordapp.com/channels/{sid}/ '{guild.Name}'){sep}{ids[i]}";
                    } else {
                        str[i] = $"{noName}{sep}{ids[i]}";
                    }
                }
            } else {
                for (int i=0; i<ids.Count(); i++) {
                    str[i] = $"{noName}{sep}{ids[i]}";
                }
            }
            return str;
        }

        #endregion Resolve ulong IDs

        #region Retrieve Server and Role IDs

        private bool GuildRoleHasUser(ulong gid, ulong rid, ulong uid)
        {
            SocketGuild g = _client.GetGuild(gid);
            if (g == null) return false;

            SocketRole r = g.GetRole(rid);
            if (r == null) return false;

            SocketGuildUser u = g.GetUser(uid);
            if (u == null) return false;

            return r.Members.Contains(u);
        }

        /// <summary>NOTE: THIS IS VERY SLOW</summary>
        public Dictionary<ulong,ulong[]> GetRoleIDs(GWLItemType type, ulong id, ulong ctxSrvrId)
        {
            Dictionary<ulong,ulong[]> result = null;
            switch(type) {
                case GWLItemType.Role:
                    // Output serverID: ctxSrvrId, RoleID: id
                    result = new Dictionary<ulong, ulong[]>();
                    result.Add(ctxSrvrId, new ulong[]{id});
                    break;

                case GWLItemType.User:
                    // Find all servers shared with the user
                    // Then find all roles for the user in each server
                    SocketUser user = _client.GetUser(id);
                    if (user != null) {
                        result = _client.Guilds
                            .Where(g => g.GetUser(id) != null)
                            .ToDictionary(s => s.Id, rs => rs.Roles
                                .Where(r => r.Members.Contains(r.Guild.GetUser(id)))
                                .Select(r => r.Id).ToArray());
                    }
                    break;

                case GWLItemType.Channel:
                    // Find the channel's server
                    // Then find all roles in the channel
                    ulong sID = GetServerID(id);
                    if (sID > 0) {
                        result = new Dictionary<ulong, ulong[]>();
                        result.Add(sID, new ulong[]{id});
                    }
                    break;

                case GWLItemType.Server:
                    // Find all roles in this server
                    SocketGuild srvr = _client.GetGuild(id);
                    if (srvr != null) {
                        ulong[] roleIDs = srvr.Roles.Select(r => r.Id).ToArray();
                        if (roleIDs.Length > 0) {
                            result = new Dictionary<ulong, ulong[]>();
                            result.Add(id, roleIDs);
                        }
                    }
                    break;

                default:
                    break;
            }
            return result;
        }

        public ulong GetServerID(ulong cID)
        {
            ulong sID = 0;
            SocketGuildChannel chnl = _client.GetChannel(cID) as SocketGuildChannel;
            if (chnl != null) {
                sID = chnl.Guild.Id;
            }
            return sID;
        }
        #endregion Retrieve Server and Role IDs
    }
}
