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
        public const string CACHE_KEY_FILL_CHARACTER_DATA_BOOLEANS_UPDATE = "FILL_CHARACTER_DATA_BOOLEANS_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_DATA_BOOLEANS_INSERT = "FILL_CHARACTER_DATA_BOOLEANS_INSERT";
        public async UniTask FillCharacterDataBooleans(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId, IList<CharacterDataBoolean> characterDataBooleans)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                $"{CACHE_KEY_FILL_CHARACTER_DATA_BOOLEANS_UPDATE}_{tableName}",
                connection, transaction,
                tableName,
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterDataBooleans)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    $"{CACHE_KEY_FILL_CHARACTER_DATA_BOOLEANS_INSERT}_{tableName}",
                    connection, null,
                    tableName,
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterDataBooleans)));
            }
        }
    }
}
#endif