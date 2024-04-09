#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_FILL_CHARACTER_SKILL_USAGES_UPDATE = "FILL_CHARACTER_SKILL_USAGES_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_SKILL_USAGES_INSERT = "FILL_CHARACTER_SKILL_USAGES_INSERT";
        public async UniTask FillCharacterSkillUsages(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterSkillUsage> characterSkillUsages)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_SKILL_USAGES_UPDATE,
                connection, transaction,
                "character_skill_usages",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSkillUsages)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_SKILL_USAGES_INSERT,
                    connection, null,
                    "character_skill_usages",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSkillUsages)));
            }
        }
    }
}
#endif