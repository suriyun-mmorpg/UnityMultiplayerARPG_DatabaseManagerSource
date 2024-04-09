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
        public const string CACHE_KEY_FILL_CHARACTER_SKILL_USAGES = "FILL_CHARACTER_SKILL_USAGES";
        public async UniTask FillCharacterSkillUsages(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterSkillUsage> characterSkillUsages)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_SKILL_USAGES,
                connection, transaction,
                "character_skill_usages",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSkillUsages)));
        }
    }
}
#endif