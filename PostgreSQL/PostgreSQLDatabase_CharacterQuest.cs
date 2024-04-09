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
        public const string CACHE_KEY_FILL_CHARACTER_QUESTS = "FILL_CHARACTER_QUESTS";
        public async UniTask FillCharacterQuests(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterQuest> characterQuests)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_QUESTS,
                connection, transaction,
                "character_quests",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterQuests)));
        }
    }
}
#endif