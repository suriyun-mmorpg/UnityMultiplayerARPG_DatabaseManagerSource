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
        public const string CACHE_KEY_FILL_CHARACTER_QUESTS_UPDATE = "FILL_CHARACTER_QUESTS_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_QUESTS_INSERT = "FILL_CHARACTER_QUESTS_INSERT";
        public async UniTask FillCharacterQuests(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterQuest> characterQuests)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_QUESTS_UPDATE,
                connection, transaction,
                "character_quests",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterQuests)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_QUESTS_INSERT,
                    connection, null,
                    "character_quests",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterQuests)));
            }
        }
    }
}
#endif