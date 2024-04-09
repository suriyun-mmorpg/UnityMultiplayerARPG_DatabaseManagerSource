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
        public const string CACHE_KEY_FILL_CHARACTER_SKILLS_UPDATE = "FILL_CHARACTER_SKILLS_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_SKILLS_INSERT = "FILL_CHARACTER_SKILLS_INSERT";
        public async UniTask FillCharacterSkills(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterSkill> characterSkills)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_SKILLS_UPDATE,
                connection, transaction,
                "character_skills",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSkills)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_SKILLS_INSERT,
                    connection, null,
                    "character_skills",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSkills)));
            }
        }
    }
}
#endif