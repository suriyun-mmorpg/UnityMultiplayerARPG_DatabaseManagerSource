﻿#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
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
                new MySqlParameter("@guildName", guildName),
                new MySqlParameter("@leaderId", leaderId),
                new MySqlParameter("@options", "{}"));
            if (id > 0)
            {
                await ExecuteNonQuery("UPDATE characters SET guildId=@id WHERE id=@leaderId",
                    new MySqlParameter("@id", id),
                    new MySqlParameter("@leaderId", leaderId));
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
                new MySqlParameter("@id", id));
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
                    new MySqlParameter("@id", id));
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
                    new MySqlParameter("@id", id));
                // Guild skills
                await ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        result.SetSkillLevel(reader.GetInt32(0), reader.GetInt32(1));
                    }
                }, "SELECT dataId, level FROM guildskill WHERE guildId=@id",
                    new MySqlParameter("@id", id));
            }
            return result;
        }

        public override async UniTask UpdateGuildLevel(int id, int level, int exp, int skillPoint)
        {
            await ExecuteNonQuery("UPDATE guild SET level=@level, exp=@exp, skillPoint=@skillPoint WHERE id=@id",
                new MySqlParameter("@level", level),
                new MySqlParameter("@exp", exp),
                new MySqlParameter("@skillPoint", skillPoint),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildLeader(int id, string leaderId)
        {
            await ExecuteNonQuery("UPDATE guild SET leaderId=@leaderId WHERE id=@id",
                new MySqlParameter("@leaderId", leaderId),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildMessage(int id, string guildMessage)
        {
            await ExecuteNonQuery("UPDATE guild SET guildMessage=@guildMessage WHERE id=@id",
                new MySqlParameter("@guildMessage", guildMessage),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildMessage2(int id, string guildMessage)
        {
            await ExecuteNonQuery("UPDATE guild SET guildMessage2=@guildMessage WHERE id=@id",
                new MySqlParameter("@guildMessage", guildMessage),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildScore(int id, int score)
        {
            await ExecuteNonQuery("UPDATE guild SET score=@score WHERE id=@id",
                new MySqlParameter("@score", score),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildOptions(int id, string options)
        {
            await ExecuteNonQuery("UPDATE guild SET options=@options WHERE id=@id",
                new MySqlParameter("@options", options),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests)
        {
            await ExecuteNonQuery("UPDATE guild SET autoAcceptRequests=@autoAcceptRequests WHERE id=@id",
                new MySqlParameter("@autoAcceptRequests", autoAcceptRequests),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildRank(int id, int rank)
        {
            await ExecuteNonQuery("UPDATE guild SET rank=@rank WHERE id=@id",
                new MySqlParameter("@rank", rank),
                new MySqlParameter("@id", id));
        }

        public override async UniTask UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData)
        {
            await ExecuteNonQuery("DELETE FROM guildrole WHERE guildId=@guildId AND guildRole=@guildRole",
                new MySqlParameter("@guildId", id),
                new MySqlParameter("@guildRole", guildRole));
            await ExecuteNonQuery("INSERT INTO guildrole (guildId, guildRole, name, canInvite, canKick, canUseStorage, shareExpPercentage) " +
                "VALUES (@guildId, @guildRole, @name, @canInvite, @canKick, @canUseStorage, @shareExpPercentage)",
                new MySqlParameter("@guildId", id),
                new MySqlParameter("@guildRole", guildRole),
                new MySqlParameter("@name", guildRoleData.roleName),
                new MySqlParameter("@canInvite", guildRoleData.canInvite),
                new MySqlParameter("@canKick", guildRoleData.canKick),
                new MySqlParameter("@canUseStorage", guildRoleData.canUseStorage),
                new MySqlParameter("@shareExpPercentage", guildRoleData.shareExpPercentage));
        }

        public override async UniTask UpdateGuildMemberRole(string characterId, byte guildRole)
        {
            await ExecuteNonQuery("UPDATE characters SET guildRole=@guildRole WHERE id=@characterId",
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@guildRole", guildRole));
        }

        public override async UniTask UpdateGuildSkillLevel(int id, int dataId, int level, int skillPoint)
        {
            await ExecuteNonQuery("DELETE FROM guildskill WHERE guildId=@guildId AND dataId=@dataId",
                new MySqlParameter("@guildId", id),
                new MySqlParameter("@dataId", dataId));
            await ExecuteNonQuery("INSERT INTO guildskill (guildId, dataId, level) " +
                "VALUES (@guildId, @dataId, @level)",
                new MySqlParameter("@guildId", id),
                new MySqlParameter("@dataId", dataId),
                new MySqlParameter("@level", level));
            await ExecuteNonQuery("UPDATE guild SET skillPoint=@skillPoint WHERE id=@id",
                new MySqlParameter("@skillPoint", skillPoint),
                new MySqlParameter("@id", id));
        }

        public override async UniTask DeleteGuild(int id)
        {
            await ExecuteNonQuery("DELETE FROM guild WHERE id=@id;" +
                "UPDATE characters SET guildId=0 WHERE guildId=@id;",
                new MySqlParameter("@id", id));
        }

        public override async UniTask<long> FindGuildName(string guildName)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM guild WHERE guildName LIKE @guildName",
                new MySqlParameter("@guildName", guildName));
            return result != null ? (long)result : 0;
        }

        public override async UniTask UpdateCharacterGuild(string characterId, int guildId, byte guildRole)
        {
            await ExecuteNonQuery("UPDATE characters SET guildId=@guildId, guildRole=@guildRole WHERE id=@characterId",
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@guildId", guildId),
                new MySqlParameter("@guildRole", guildRole));
        }

        public override async UniTask<int> GetGuildGold(int guildId)
        {
            int gold = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    gold = reader.GetInt32(0);
            }, "SELECT gold FROM guild WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", guildId));
            return gold;
        }

        public override async UniTask UpdateGuildGold(int guildId, int gold)
        {
            await ExecuteNonQuery("UPDATE guild SET gold=@gold WHERE id=@id",
                new MySqlParameter("@id", guildId),
                new MySqlParameter("@gold", gold));
        }
    }
}
#endif