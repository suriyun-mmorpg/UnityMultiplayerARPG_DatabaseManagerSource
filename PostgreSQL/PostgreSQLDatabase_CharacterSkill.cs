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
        public const string CACHE_KEY_FILL_CHARACTER_SKILLS = "FILL_CHARACTER_SKILLS";
        public async UniTask FillCharacterSkills(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterSkill> characterSkills)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_SKILLS,
                connection, transaction,
                "character_skills",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSkills)));
        }
    }
}
#endif