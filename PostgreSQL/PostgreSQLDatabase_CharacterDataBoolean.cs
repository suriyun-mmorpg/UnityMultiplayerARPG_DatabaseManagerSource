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
        public const string CACHE_KEY_FILL_CHARACTER_DATA_BOOLEANS = "FILL_CHARACTER_DATA_BOOLEANS";
        public async UniTask FillCharacterDataBooleans(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId, IList<CharacterDataBoolean> characterDataBooleans)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                $"{CACHE_KEY_FILL_CHARACTER_DATA_BOOLEANS}_{tableName}",
                connection, transaction,
                tableName,
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterDataBooleans)));
        }
    }
}
#endif