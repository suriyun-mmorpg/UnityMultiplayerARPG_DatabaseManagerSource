#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_INSERT_GUILD = "INSERT_GUILD";
        public const string CACHE_KEY_INSERT_GUILD_UPDATE = "INSERT_GUILD_UPDATE";
        public override async UniTask<int> CreateGuild(string guildName, string leaderId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            int id = (int)await PostgreSQLHelpers.ExecuteInsertScalar(
                CACHE_KEY_INSERT_GUILD,
                connection,
                "guilds",
                new[] {
                    new PostgreSQLHelpers.ColumnInfo("guild_name", guildName),
                    new PostgreSQLHelpers.ColumnInfo("leader_id", leaderId),
                    new PostgreSQLHelpers.ColumnInfo("options", "{}"),
                }, "id");
            if (id <= 0)
                return id;
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_INSERT_GUILD_UPDATE,
                connection, null,
                "characters",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("guild_id", id),
                },
                PostgreSQLHelpers.WhereEqualTo("id", leaderId));
            return id;
        }

        public const string CACHE_KEY_READ_GUILD = "READ_GUILD";
        public const string CACHE_KEY_READ_GUILD_ROLES = "READ_GUILD_ROLES";
        public const string CACHE_KEY_READ_GUILD_MEMBERS = "READ_GUILD_MEMBERS";
        public const string CACHE_KEY_READ_GUILD_SKILLS = "READ_GUILD_SKILLS";
        public override async UniTask<GuildData> ReadGuild(int id, IEnumerable<GuildRoleData> defaultGuildRoles)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var readerGuild = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_READ_GUILD,
                connection,
                "guilds", "guild_name, leader_id, level, exp, skill_point, guild_message, guild_message_2, gold, score, options, auto_accept_requests, rank",
                PostgreSQLHelpers.WhereEqualTo("id", id));
            // Guild data
            GuildData guild = null;
            if (readerGuild.Read())
            {
                guild = new GuildData(id,
                    readerGuild.GetString(0),
                    readerGuild.GetString(1),
                    defaultGuildRoles);
                guild.level = readerGuild.GetInt32(2);
                guild.exp = readerGuild.GetInt32(3);
                guild.skillPoint = readerGuild.GetInt32(4);
                guild.guildMessage = readerGuild.GetString(5);
                guild.guildMessage2 = readerGuild.GetString(6);
                guild.gold = readerGuild.GetInt32(7);
                guild.score = readerGuild.GetInt32(8);
                guild.options = readerGuild.GetString(9);
                guild.autoAcceptRequests = readerGuild.GetBoolean(10);
                guild.rank = readerGuild.GetInt32(11);
            }
            if (guild == null)
                return null;
            // Guild roles
            using var readerRoles = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_READ_GUILD_ROLES,
                connection,
                "guild_roles", "role, name, can_invite, can_kick, can_use_storage, share_exp_percentage",
                PostgreSQLHelpers.WhereEqualTo("id", id));
            byte guildRole;
            GuildRoleData guildRoleData;
            while (readerRoles.Read())
            {
                guildRole = readerRoles.GetByte(0);
                guildRoleData = new GuildRoleData();
                guildRoleData.roleName = readerRoles.GetString(1);
                guildRoleData.canInvite = readerRoles.GetBoolean(2);
                guildRoleData.canKick = readerRoles.GetBoolean(3);
                guildRoleData.canUseStorage = readerRoles.GetBoolean(4);
                guildRoleData.shareExpPercentage = readerRoles.GetByte(5);
                guild.SetRole(guildRole, guildRoleData);
            }
            // Guild members
            using var readerMembers = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_READ_GUILD_MEMBERS,
                connection,
                "characters", "id, data_id, character_name, level, guild_role",
                PostgreSQLHelpers.WhereEqualTo("id", id));
            SocialCharacterData guildMemberData;
            while (readerMembers.Read())
            {
                // Get some required data, other data will be set at server side
                guildMemberData = new SocialCharacterData();
                guildMemberData.id = readerMembers.GetString(0);
                guildMemberData.dataId = readerMembers.GetInt32(1);
                guildMemberData.characterName = readerMembers.GetString(2);
                guildMemberData.level = readerMembers.GetInt32(3);
                guild.AddMember(guildMemberData, (byte)readerMembers.GetInt32(4));
            }
            // Guild skills
            using var readerSkills = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_READ_GUILD_SKILLS,
                connection,
                "guild_skills", "data_id, level",
                PostgreSQLHelpers.WhereEqualTo("id", id));
            while (readerSkills.Read())
            {
                guild.SetSkillLevel(readerSkills.GetInt32(0), readerSkills.GetInt32(1));
            }
            return guild;
        }

        public const string CACHE_KEY_UPDATE_GUILD_LEVEL = "UPDATE_GUILD_LEVEL";
        public override async UniTask UpdateGuildLevel(int id, int level, int exp, int skillPoint)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_LEVEL,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("level", level),
                    new PostgreSQLHelpers.ColumnInfo("exp", exp),
                    new PostgreSQLHelpers.ColumnInfo("skill_point", skillPoint),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_LEADER = "UPDATE_GUILD_LEADER";
        public override async UniTask UpdateGuildLeader(int id, string leaderId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_LEADER,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("leader_id", leaderId),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_MESSAGE = "UPDATE_GUILD_MESSAGE";
        public override async UniTask UpdateGuildMessage(int id, string guildMessage)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_MESSAGE,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("guild_message", guildMessage),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_MESSAGE_2 = "UPDATE_GUILD_MESSAGE_2";
        public override async UniTask UpdateGuildMessage2(int id, string guildMessage)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_MESSAGE_2,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("guild_message_2", guildMessage),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_SCORE = "UPDATE_GUILD_SCORE";
        public override async UniTask UpdateGuildScore(int id, int score)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_SCORE,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("score", score),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_OPTIONS = "UPDATE_GUILD_OPTIONS";
        public override async UniTask UpdateGuildOptions(int id, string options)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_OPTIONS,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("options", options),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_AUTO_ACCEPT_REQUESTS = "UPDATE_GUILD_AUTO_ACCEPT_REQUESTS";
        public override async UniTask UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_AUTO_ACCEPT_REQUESTS,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("auto_accept_requests", autoAcceptRequests),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_RANK = "UPDATE_GUILD_RANK";
        public override async UniTask UpdateGuildRank(int id, int rank)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_RANK,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("rank", rank),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_GUILD_ROLE = "UPDATE_GUILD_ROLE";
        public override async UniTask UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            int count = await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_UPDATE_GUILD_ROLE,
                connection, null,
                "guild_roles",
                "id, role",
                new PostgreSQLHelpers.ColumnInfo("id", id),
                new PostgreSQLHelpers.ColumnInfo("role", guildRole),
                new PostgreSQLHelpers.ColumnInfo("name", guildRoleData.roleName),
                new PostgreSQLHelpers.ColumnInfo("can_invite", guildRoleData.canInvite),
                new PostgreSQLHelpers.ColumnInfo("can_kick", guildRoleData.canKick),
                new PostgreSQLHelpers.ColumnInfo("can_use_storage", guildRoleData.canUseStorage),
                new PostgreSQLHelpers.ColumnInfo("share_exp_percentage", guildRoleData.shareExpPercentage));
        }

        public const string CACHE_KEY_UPDATE_GUILD_MEMBER_ROLE = "UPDATE_GUILD_MEMBER_ROLE";
        public override async UniTask UpdateGuildMemberRole(string characterId, byte guildRole)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_MEMBER_ROLE,
                connection, null,
                "characters",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("guild_role", guildRole),
                },
                PostgreSQLHelpers.WhereEqualTo("id", characterId));
        }

        public const string CACHE_KEY_UPDATE_GUILD_SKILL_LEVEL = "UPDATE_GUILD_SKILL_LEVEL";
        public const string CACHE_KEY_UPDATE_GUILD_SKILL_LEVEL_SKILL_POINT = "UPDATE_GUILD_SKILL_LEVEL_SKILL_POINT";
        public override async UniTask UpdateGuildSkillLevel(int id, int dataId, int skillLevel, int skillPoint)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await PostgreSQLHelpers.ExecuteUpsert(
                    CACHE_KEY_UPDATE_GUILD_SKILL_LEVEL,
                    connection, transaction,
                    "guild_skills",
                    "id, data_id",
                    new PostgreSQLHelpers.ColumnInfo("id", id),
                    new PostgreSQLHelpers.ColumnInfo("data_id", dataId),
                    new PostgreSQLHelpers.ColumnInfo("level", skillLevel));
                await PostgreSQLHelpers.ExecuteUpdate(
                    CACHE_KEY_UPDATE_GUILD_SKILL_LEVEL_SKILL_POINT,
                    connection, transaction,
                    "guilds",
                    new[]
                    {
                        new PostgreSQLHelpers.ColumnInfo("skill_point", skillPoint),
                    },
                    PostgreSQLHelpers.WhereEqualTo("id", id));
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while update guild skill levels: " + id);
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public const string CACHE_KEY_DELETE_GUILD_GUILD = "DELETE_GUILD_GUILD";
        public const string CACHE_KEY_DELETE_GUILD_CHARACTER = "DELETE_GUILD_CHARACTER";
        public override async UniTask DeleteGuild(int id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await PostgreSQLHelpers.ExecuteDelete(
                    CACHE_KEY_DELETE_GUILD_GUILD,
                    connection, transaction,
                    "guilds",
                    PostgreSQLHelpers.WhereEqualTo("id", id));
                await PostgreSQLHelpers.ExecuteUpdate(
                    CACHE_KEY_DELETE_GUILD_CHARACTER,
                    connection, transaction,
                    "characters",
                    new[]
                    {
                        new PostgreSQLHelpers.ColumnInfo("guild_id", 0),
                        new PostgreSQLHelpers.ColumnInfo("guild_role", 0),
                    },
                    PostgreSQLHelpers.WhereEqualTo("guild_id", id));
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while delete guild: " + id);
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public const string CACHE_KEY_FIND_GUILD_NAME = "FIND_GUILD_NAME";
        public override async UniTask<long> FindGuildName(string guildName)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var count = await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_FIND_GUILD_NAME,
                connection,
                "guilds",
                PostgreSQLHelpers.WhereLike("guild_name", guildName));
            return count;
        }

        public const string CACHE_KEY_UPDATE_CHARACTER_GUILD = "UPDATE_CHARACTER_GUILD";
        public override async UniTask UpdateCharacterGuild(string characterId, int guildId, byte guildRole)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_CHARACTER_GUILD,
                connection, null,
                "characters",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("guild_id", guildId),
                    new PostgreSQLHelpers.ColumnInfo("guild_role", guildRole),
                },
                PostgreSQLHelpers.WhereEqualTo("id", characterId));
        }

        public const string CACHE_KEY_GET_GUILD_GOLD = "GET_GUILD_GOLD";
        public override async UniTask<int> GetGuildGold(int id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_GET_GUILD_GOLD,
                connection,
                "guilds", "gold", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("id", id));
            int gold = 0;
            if (reader.Read())
                gold = reader.GetInt32(0);
            return gold;
        }

        public const string CACHE_KEY_UPDATE_GUILD_GOLD = "UPDATE_GUILD_GOLD";
        public override async UniTask UpdateGuildGold(int id, int gold)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_GUILD_GOLD,
                connection, null,
                "guilds",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("gold", gold),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_FIND_GUILDS = "FIND_GUILDS";
        public override async UniTask<List<GuildListEntry>> FindGuilds(string finderId, string guildName, int skip, int limit)
        {
            // TODO: exclude joined guild, exclude requested guilds
            using var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_FIND_GUILDS,
                connection,
                "guilds", "id, guild_name, level, guild_message, guild_message_2, score, options, auto_accept_requests, rank, current_members, max_members", $"ORDER BY RAND() LIMIT {skip}, {limit}",
                PostgreSQLHelpers.WhereLike("guild_name", $"%{guildName}%"));
            List<GuildListEntry> result = new List<GuildListEntry>();
            GuildListEntry tempEntry;
            while (reader.Read())
            {
                // Get some required data, other data will be set at server side
                tempEntry = new GuildListEntry();
                tempEntry.Id = reader.GetInt32(0);
                tempEntry.GuildName = reader.GetString(1);
                tempEntry.Level = reader.GetInt32(2);
                tempEntry.FieldOptions = GuildListFieldOptions.All;
                tempEntry.GuildMessage = reader.GetString(3);
                tempEntry.GuildMessage2 = reader.GetString(4);
                tempEntry.Score = reader.GetInt32(5);
                tempEntry.Options = reader.GetString(6);
                tempEntry.AutoAcceptRequests = reader.GetBoolean(7);
                tempEntry.Rank = reader.GetInt32(8);
                tempEntry.CurrentMembers = reader.GetInt32(9);
                tempEntry.MaxMembers = reader.GetInt32(10);
                result.Add(tempEntry);
            }
            return result;
        }

        public const string CACHE_KEY_CREATE_GUILD_REQUEST = "CREATE_GUILD_REQUEST";
        public override async UniTask CreateGuildRequest(int guildId, string requesterId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteInsert(
                CACHE_KEY_CREATE_GUILD_REQUEST,
                connection, null,
                "guild_requests",
                new PostgreSQLHelpers.ColumnInfo("id", guildId),
                new PostgreSQLHelpers.ColumnInfo("requester_id", requesterId));
        }

        public override async UniTask DeleteGuildRequest(int guildId, string requesterId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await DeleteGuildRequest(connection, null, guildId, requesterId);
        }

        public const string CACHE_KEY_DELETE_GUILD_REQUEST = "DELETE_GUILD_REQUEST";
        public async UniTask DeleteGuildRequest(NpgsqlConnection connection, NpgsqlTransaction transaction, int guildId, string requesterId)
        {
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_GUILD_REQUEST,
                connection, transaction,
                "guild_requests",
                PostgreSQLHelpers.WhereEqualTo("id", guildId),
                PostgreSQLHelpers.AndWhereEqualTo("requester_id", requesterId));
        }

        public const string CACHE_KEY_GET_GUILD_REQUESTS = "GET_GUILD_REQUESTS";
        public override async UniTask<List<SocialCharacterData>> GetGuildRequests(int guildId, int skip, int limit)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            // Get character IDs
            using var readerIds = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_GET_GUILD_REQUESTS,
                connection,
                "guild_requests", "requester_id", $"LIMIT {skip}, {limit}",
                PostgreSQLHelpers.WhereEqualTo("id", guildId));
            List<string> characterIds = new List<string>();
            while (readerIds.Read())
            {
                characterIds.Add(readerIds.GetString(0));
            }
            return await GetSocialCharacterByIds(connection, null, characterIds);
        }

        public const string CACHE_KEY_GET_GUILD_REQUESTS_NOTIFICATION = "GET_GUILD_REQUESTS_NOTIFICATION";
        public override async UniTask<int> GetGuildRequestsNotification(int guildId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            return (int)await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_GET_GUILD_REQUESTS_NOTIFICATION,
                connection,
                "guild_requests",
                PostgreSQLHelpers.WhereEqualTo("id", guildId));
        }
    }
}
#endif