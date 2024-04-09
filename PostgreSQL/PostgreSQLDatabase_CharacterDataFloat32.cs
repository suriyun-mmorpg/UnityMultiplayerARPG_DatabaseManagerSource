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
        public const string CACHE_KEY_FILL_CHARACTER_DATA_FLOAT32S = "FILL_CHARACTER_DATA_FLOAT32S";
        public async UniTask FillCharacterDataFloat32s(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId, IList<CharacterDataFloat32> characterDataFloat32s)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                $"{CACHE_KEY_FILL_CHARACTER_DATA_FLOAT32S}_{tableName}",
                connection, transaction,
                tableName,
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterDataFloat32s)));
        }
    }
}
#endif