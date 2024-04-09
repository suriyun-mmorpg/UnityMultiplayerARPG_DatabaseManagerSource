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
        public const string CACHE_KEY_FILL_CHARACTER_DATA_INT32S_UPDATE = "FILL_CHARACTER_DATA_INT32S_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_DATA_INT32S_INSERT = "FILL_CHARACTER_DATA_INT32S_INSERT";
        public async UniTask FillCharacterDataInt32s(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId, IList<CharacterDataInt32> characterDataInt32s)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                $"{CACHE_KEY_FILL_CHARACTER_DATA_INT32S_UPDATE}_{tableName}",
                connection, transaction,
                tableName,
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterDataInt32s)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    $"{CACHE_KEY_FILL_CHARACTER_DATA_INT32S_INSERT}_{tableName}",
                    connection, null,
                    tableName,
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterDataInt32s)));
            }
        }
    }
}
#endif