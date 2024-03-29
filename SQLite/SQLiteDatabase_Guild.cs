﻿#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif
using System.Collections.Generic;

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        public override UniTask<int> CreateGuild(string guildName, string leaderId)
        {
            int id = 0;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                    id = (int)reader.GetInt64(0);
            }, "INSERT INTO guild (guildName, leaderId, options) VALUES (@guildName, @leaderId, @options);" +
                "SELECT LAST_INSERT_ROWID();",
                new SqliteParameter("@guildName", guildName),
                new SqliteParameter("@leaderId", leaderId),
                new SqliteParameter("@options", "{}"));
            if (id > 0)
            {
                ExecuteNonQuery("UPDATE characters SET guildId=@id WHERE id=@leaderId",
                    new SqliteParameter("@id", id),
                    new SqliteParameter("@leaderId", leaderId));
            }
            return new UniTask<int>(id);
        }

        public override UniTask<GuildData> ReadGuild(int id, IEnumerable<GuildRoleData> defaultGuildRoles)
        {
            GuildData result = null;
            ExecuteReader((reader) =>
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
                new SqliteParameter("@id", id));
            if (result != null)
            {
                // Guild roles
                ExecuteReader((reader) =>
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
                    new SqliteParameter("@id", id));
                // Guild members
                ExecuteReader((reader) =>
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
                    new SqliteParameter("@id", id));
                // Guild skills
                ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        result.SetSkillLevel(reader.GetInt32(0), reader.GetInt32(1));
                    }
                }, "SELECT dataId, level FROM guildskill WHERE guildId=@id",
                    new SqliteParameter("@id", id));
            }
            return new UniTask<GuildData>(result);
        }

        public override UniTask UpdateGuildLevel(int id, int level, int exp, int skillPoint)
        {
            ExecuteNonQuery("UPDATE guild SET level=@level, exp=@exp, skillPoint=@skillPoint WHERE id=@id",
                new SqliteParameter("@level", level),
                new SqliteParameter("@exp", exp),
                new SqliteParameter("@skillPoint", skillPoint),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildLeader(int id, string leaderId)
        {
            ExecuteNonQuery("UPDATE guild SET leaderId=@leaderId WHERE id=@id",
                new SqliteParameter("@leaderId", leaderId),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildMessage(int id, string guildMessage)
        {
            ExecuteNonQuery("UPDATE guild SET guildMessage=@guildMessage WHERE id=@id",
                new SqliteParameter("@guildMessage", guildMessage),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildMessage2(int id, string guildMessage)
        {
            ExecuteNonQuery("UPDATE guild SET guildMessage2=@guildMessage WHERE id=@id",
                new SqliteParameter("@guildMessage", guildMessage),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildScore(int id, int score)
        {
            ExecuteNonQuery("UPDATE guild SET score=@score WHERE id=@id",
                new SqliteParameter("@score", score),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildOptions(int id, string options)
        {
            ExecuteNonQuery("UPDATE guild SET options=@options WHERE id=@id",
                new SqliteParameter("@options", options),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests)
        {
            ExecuteNonQuery("UPDATE guild SET autoAcceptRequests=@autoAcceptRequests WHERE id=@id",
                new SqliteParameter("@autoAcceptRequests", autoAcceptRequests),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildRank(int id, int rank)
        {
            ExecuteNonQuery("UPDATE guild SET rank=@rank WHERE id=@id",
                new SqliteParameter("@rank", rank),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData)
        {
            ExecuteNonQuery("DELETE FROM guildrole WHERE guildId=@guildId AND guildRole=@guildRole",
                new SqliteParameter("@guildId", id),
                new SqliteParameter("@guildRole", guildRole));
            ExecuteNonQuery("INSERT INTO guildrole (guildId, guildRole, name, canInvite, canKick, canUseStorage, shareExpPercentage) " +
                "VALUES (@guildId, @guildRole, @name, @canInvite, @canKick, @canUseStorage, @shareExpPercentage)",
                new SqliteParameter("@guildId", id),
                new SqliteParameter("@guildRole", guildRole),
                new SqliteParameter("@name", guildRoleData.roleName),
                new SqliteParameter("@canInvite", guildRoleData.canInvite),
                new SqliteParameter("@canKick", guildRoleData.canKick),
                new SqliteParameter("@canUseStorage", guildRoleData.canUseStorage),
                new SqliteParameter("@shareExpPercentage", guildRoleData.shareExpPercentage));
            return new UniTask();
        }

        public override UniTask UpdateGuildMemberRole(string characterId, byte guildRole)
        {
            ExecuteNonQuery("UPDATE characters SET guildRole=@guildRole WHERE id=@characterId",
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@guildRole", guildRole));
            return new UniTask();
        }

        public override UniTask UpdateGuildSkillLevel(int id, int dataId, int level, int skillPoint)
        {
            ExecuteNonQuery("DELETE FROM guildskill WHERE guildId=@guildId AND dataId=@dataId",
                new SqliteParameter("@guildId", id),
                new SqliteParameter("@dataId", dataId));
            ExecuteNonQuery("INSERT INTO guildskill (guildId, dataId, level) " +
                "VALUES (@guildId, @dataId, @level)",
                new SqliteParameter("@guildId", id),
                new SqliteParameter("@dataId", dataId),
                new SqliteParameter("@level", level));
            ExecuteNonQuery("UPDATE guild SET skillPoint=@skillPoint WHERE id=@id",
                new SqliteParameter("@skillPoint", skillPoint),
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask DeleteGuild(int id)
        {
            ExecuteNonQuery("DELETE FROM guild WHERE id=@id;" +
                "UPDATE characters SET guildId=0 WHERE guildId=@id;",
                new SqliteParameter("@id", id));
            return new UniTask();
        }

        public override UniTask<long> FindGuildName(string guildName)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM guild WHERE guildName LIKE @guildName",
                new SqliteParameter("@guildName", guildName));
            return new UniTask<long>(result != null ? (long)result : 0);
        }

        public override UniTask UpdateCharacterGuild(string characterId, int guildId, byte guildRole)
        {
            ExecuteNonQuery("UPDATE characters SET guildId=@guildId, guildRole=@guildRole WHERE id=@characterId",
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@guildId", guildId),
                new SqliteParameter("@guildRole", guildRole));
            return new UniTask();
        }

        public override UniTask<int> GetGuildGold(int guildId)
        {
            int gold = 0;
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                    gold = reader.GetInt32(0);
            }, "SELECT gold FROM guild WHERE id=@id LIMIT 1",
                new SqliteParameter("@id", guildId));
            return new UniTask<int>(gold);
        }

        public override UniTask UpdateGuildGold(int guildId, int gold)
        {
            ExecuteNonQuery("UPDATE guild SET gold=@gold WHERE id=@id",
                new SqliteParameter("@id", guildId),
                new SqliteParameter("@gold", gold));
            return new UniTask();
        }
    }
}
#endif