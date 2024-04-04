﻿#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public override async UniTask<int> CreateGuild(string guildName, string leaderId)
        {
            int id = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    id = reader.GetInt32(0);
            }, "INSERT INTO guild (guildName, leaderId, options) VALUES (@guildName, @leaderId, @options);" +
                "SELECT LAST_INSERT_ID();",
                new NpgsqlParameter("@guildName", guildName),
                new NpgsqlParameter("@leaderId", leaderId),
                new NpgsqlParameter("@options", "{}"));
            if (id > 0)
            {
                await ExecuteNonQuery("UPDATE characters SET guildId=@id WHERE id=@leaderId",
                    new NpgsqlParameter("@id", id),
                    new NpgsqlParameter("@leaderId", leaderId));
            }
            return id;
        }

        public override async UniTask<GuildData> ReadGuild(int id, IEnumerable<GuildRoleData> defaultGuildRoles)
        {
            GuildData result = null;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    result = new GuildData(id,
                        reader.GetString(0),
                        reader.GetString(1),
                        defaultGuildRoles);
                    result.level = reader.GetInt32(2);
                    result.exp = reader.GetInt32(3);
                    result.skillPoint = reader.GetInt32(4);
                    result.guildMessage = reader.GetString(5);
                    result.guildMessage2 = reader.GetString(6);
                    result.gold = reader.GetInt32(7);
                    result.score = reader.GetInt32(8);
                    result.options = reader.GetString(9);
                    result.autoAcceptRequests = reader.GetBoolean(10);
                    result.rank = reader.GetInt32(11);
                }
            }, "SELECT `guildName`, `leaderId`, `level`, `exp`, `skillPoint`, `guildMessage`, `guildMessage2`, `gold`, `score`, `options`, `autoAcceptRequests`, `rank` FROM guild WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", id));
            // Read relates data if guild exists
            if (result != null)
            {
                // Guild roles
                await ExecuteReader((reader) =>
                {
                    byte guildRole;
                    GuildRoleData guildRoleData;
                    while (reader.Read())
                    {
                        guildRole = reader.GetByte(0);
                        guildRoleData = new GuildRoleData();
                        guildRoleData.roleName = reader.GetString(1);
                        guildRoleData.canInvite = reader.GetBoolean(2);
                        guildRoleData.canKick = reader.GetBoolean(3);
                        guildRoleData.canUseStorage = reader.GetBoolean(4);
                        guildRoleData.shareExpPercentage = reader.GetByte(5);
                        result.SetRole(guildRole, guildRoleData);
                    }
                }, "SELECT guildRole, name, canInvite, canKick, canUseStorage, shareExpPercentage FROM guildrole WHERE guildId=@id",
                    new NpgsqlParameter("@id", id));
                // Guild members
                await ExecuteReader((reader) =>
                {
                    SocialCharacterData guildMemberData;
                    while (reader.Read())
                    {
                        // Get some required data, other data will be set at server side
                        guildMemberData = new SocialCharacterData();
                        guildMemberData.id = reader.GetString(0);
                        guildMemberData.dataId = reader.GetInt32(1);
                        guildMemberData.characterName = reader.GetString(2);
                        guildMemberData.level = reader.GetInt32(3);
                        result.AddMember(guildMemberData, reader.GetByte(4));
                    }
                }, "SELECT id, dataId, characterName, level, guildRole FROM characters WHERE guildId=@id",
                    new NpgsqlParameter("@id", id));
                // Guild skills
                await ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        result.SetSkillLevel(reader.GetInt32(0), reader.GetInt32(1));
                    }
                }, "SELECT dataId, level FROM guildskill WHERE guildId=@id",
                    new NpgsqlParameter("@id", id));
            }
            return result;
        }

        public override async UniTask UpdateGuildLevel(int id, int level, int exp, int skillPoint)
        {
            await ExecuteNonQuery("UPDATE guild SET level=@level, exp=@exp, skillPoint=@skillPoint WHERE id=@id",
                new NpgsqlParameter("@level", level),
                new NpgsqlParameter("@exp", exp),
                new NpgsqlParameter("@skillPoint", skillPoint),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildLeader(int id, string leaderId)
        {
            await ExecuteNonQuery("UPDATE guild SET leaderId=@leaderId WHERE id=@id",
                new NpgsqlParameter("@leaderId", leaderId),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildMessage(int id, string guildMessage)
        {
            await ExecuteNonQuery("UPDATE guild SET guildMessage=@guildMessage WHERE id=@id",
                new NpgsqlParameter("@guildMessage", guildMessage),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildMessage2(int id, string guildMessage)
        {
            await ExecuteNonQuery("UPDATE guild SET guildMessage2=@guildMessage WHERE id=@id",
                new NpgsqlParameter("@guildMessage", guildMessage),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildScore(int id, int score)
        {
            await ExecuteNonQuery("UPDATE guild SET score=@score WHERE id=@id",
                new NpgsqlParameter("@score", score),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildOptions(int id, string options)
        {
            await ExecuteNonQuery("UPDATE guild SET options=@options WHERE id=@id",
                new NpgsqlParameter("@options", options),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests)
        {
            await ExecuteNonQuery("UPDATE guild SET autoAcceptRequests=@autoAcceptRequests WHERE id=@id",
                new NpgsqlParameter("@autoAcceptRequests", autoAcceptRequests),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildRank(int id, int rank)
        {
            await ExecuteNonQuery("UPDATE guild SET rank=@rank WHERE id=@id",
                new NpgsqlParameter("@rank", rank),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData)
        {
            await ExecuteNonQuery("DELETE FROM guildrole WHERE guildId=@guildId AND guildRole=@guildRole",
                new NpgsqlParameter("@guildId", id),
                new NpgsqlParameter("@guildRole", guildRole));
            await ExecuteNonQuery("INSERT INTO guildrole (guildId, guildRole, name, canInvite, canKick, canUseStorage, shareExpPercentage) " +
                "VALUES (@guildId, @guildRole, @name, @canInvite, @canKick, @canUseStorage, @shareExpPercentage)",
                new NpgsqlParameter("@guildId", id),
                new NpgsqlParameter("@guildRole", guildRole),
                new NpgsqlParameter("@name", guildRoleData.roleName),
                new NpgsqlParameter("@canInvite", guildRoleData.canInvite),
                new NpgsqlParameter("@canKick", guildRoleData.canKick),
                new NpgsqlParameter("@canUseStorage", guildRoleData.canUseStorage),
                new NpgsqlParameter("@shareExpPercentage", guildRoleData.shareExpPercentage));
        }

        public override async UniTask UpdateGuildMemberRole(string characterId, byte guildRole)
        {
            await ExecuteNonQuery("UPDATE characters SET guildRole=@guildRole WHERE id=@characterId",
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@guildRole", guildRole));
        }

        public override async UniTask UpdateGuildSkillLevel(int id, int dataId, int level, int skillPoint)
        {
            await ExecuteNonQuery("DELETE FROM guildskill WHERE guildId=@guildId AND dataId=@dataId",
                new NpgsqlParameter("@guildId", id),
                new NpgsqlParameter("@dataId", dataId));
            await ExecuteNonQuery("INSERT INTO guildskill (guildId, dataId, level) " +
                "VALUES (@guildId, @dataId, @level)",
                new NpgsqlParameter("@guildId", id),
                new NpgsqlParameter("@dataId", dataId),
                new NpgsqlParameter("@level", level));
            await ExecuteNonQuery("UPDATE guild SET skillPoint=@skillPoint WHERE id=@id",
                new NpgsqlParameter("@skillPoint", skillPoint),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask DeleteGuild(int id)
        {
            await ExecuteNonQuery("DELETE FROM guild WHERE id=@id;" +
                "UPDATE characters SET guildId=0 WHERE guildId=@id;",
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask<long> FindGuildName(string guildName)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM guild WHERE guildName LIKE @guildName",
                new NpgsqlParameter("@guildName", guildName));
            return result != null ? (long)result : 0;
        }

        public override async UniTask UpdateCharacterGuild(string characterId, int guildId, byte guildRole)
        {
            await ExecuteNonQuery("UPDATE characters SET guildId=@guildId, guildRole=@guildRole WHERE id=@characterId",
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@guildId", guildId),
                new NpgsqlParameter("@guildRole", guildRole));
        }

        public override async UniTask<int> GetGuildGold(int guildId)
        {
            int gold = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    gold = reader.GetInt32(0);
            }, "SELECT gold FROM guild WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", guildId));
            return gold;
        }

        public override async UniTask UpdateGuildGold(int guildId, int gold)
        {
            await ExecuteNonQuery("UPDATE guild SET gold=@gold WHERE id=@id",
                new NpgsqlParameter("@id", guildId),
                new NpgsqlParameter("@gold", gold));
        }

        public override async UniTask<List<GuildListEntry>> FindGuilds(string finderId, string guildName, int skip, int limit)
        {
            string excludeIdsQuery = "1";
            // TODO: exclude joined guild, exclude requested guilds
            List<GuildListEntry> result = new List<GuildListEntry>();
            await ExecuteReader((reader) =>
            {
                GuildListEntry guildListEntry;
                while (reader.Read())
                {
                    // Get some required data, other data will be set at server side
                    guildListEntry = new GuildListEntry();
                    guildListEntry.Id = reader.GetInt32(0);
                    guildListEntry.GuildName = reader.GetString(1);
                    guildListEntry.Level = reader.GetInt32(2);
                    guildListEntry.FieldOptions = GuildListFieldOptions.All;
                    guildListEntry.GuildMessage = reader.GetString(3);
                    guildListEntry.GuildMessage2 = reader.GetString(4);
                    guildListEntry.Score = reader.GetInt32(5);
                    guildListEntry.Options = reader.GetString(6);
                    guildListEntry.AutoAcceptRequests = reader.GetBoolean(7);
                    guildListEntry.Rank = reader.GetInt32(8);
                    guildListEntry.CurrentMembers = reader.GetInt32(9);
                    guildListEntry.MaxMembers = reader.GetInt32(10);
                    result.Add(guildListEntry);
                }
            }, "SELECT id, guildName, level, guildMessage, guildMessage2, score, options, autoAcceptRequests, rank, currentMembers, maxMembers FROM guild WHERE guildName LIKE @guildName AND " + excludeIdsQuery + " ORDER BY RAND() LIMIT " + skip + ", " + limit,
                new NpgsqlParameter("@guildName", "%" + guildName + "%"));
            return result;
        }

        public override async UniTask CreateGuildRequest(int guildId, string requesterId)
        {
            using (NpgsqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await ExecuteNonQuery(connection, transaction, "DELETE FROM guildrequest WHERE " +
                           "characterId1 LIKE @guildId AND " +
                           "requesterId LIKE @requesterId",
                           new NpgsqlParameter("@guildId", guildId),
                           new NpgsqlParameter("@requesterId", requesterId));
                        await ExecuteNonQuery(connection, transaction, "INSERT INTO guildrequest " +
                            "(guildId, requesterId, state) VALUES " +
                            "(@guildId, @requesterId, @state)",
                            new NpgsqlParameter("@guildId", guildId),
                            new NpgsqlParameter("@requesterId", requesterId));
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception ex)
                    {
                        LogError(LogTag, "Transaction, Error occurs while creating guildrequest: " + guildId + " " + requesterId);
                        LogException(LogTag, ex);
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        public override async UniTask DeleteGuildRequest(int guildId, string requesterId)
        {
            await ExecuteNonQuery("DELETE FROM guildrequest WHERE " +
               "guildId LIKE @guildId AND " +
               "requesterId LIKE @requesterId",
               new NpgsqlParameter("@guildId", guildId),
               new NpgsqlParameter("@requesterId", requesterId));
        }

        public override async UniTask<List<SocialCharacterData>> GetGuildRequests(int guildId, int skip, int limit)
        {
            List<SocialCharacterData> result = new List<SocialCharacterData>();
            List<string> characterIds = new List<string>();
            await ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    characterIds.Add(reader.GetString(0));
                }
            }, "SELECT requesterId FROM guildrequest WHERE guildId=@guildId LIMIT " + skip + ", " + limit,
                new NpgsqlParameter("@guildId", guildId));
            SocialCharacterData socialCharacterData;
            foreach (string characterId in characterIds)
            {
                await ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        // Get some required data, other data will be set at server side
                        socialCharacterData = new SocialCharacterData();
                        socialCharacterData.id = reader.GetString(0);
                        socialCharacterData.dataId = reader.GetInt32(1);
                        socialCharacterData.characterName = reader.GetString(2);
                        socialCharacterData.level = reader.GetInt32(3);
                        result.Add(socialCharacterData);
                    }
                }, "SELECT id, dataId, characterName, level FROM characters WHERE BINARY id = @id",
                    new NpgsqlParameter("@id", characterId));
            }
            return result;
        }

        public override async UniTask<int> GetGuildRequestsNotification(int guildId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM guildrequest WHERE guildId=@guildId",
                new NpgsqlParameter("@guildId", guildId));
            return (int)(long)result;
        }
    }
}
#endif