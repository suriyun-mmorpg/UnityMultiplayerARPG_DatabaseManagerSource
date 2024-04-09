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
        public const string CACHE_KEY_FILL_CHARACTER_ATTRIBUTES_UPDATE = "FILL_CHARACTER_ATTRIBUTES_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_ATTRIBUTES_INSERT = "FILL_CHARACTER_ATTRIBUTES_INSERT";
        public async UniTask FillCharacterAttributes(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterAttribute> characterAttributes)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_ATTRIBUTES_UPDATE,
                connection, transaction,
                "character_attributes",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterAttributes)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_ATTRIBUTES_INSERT,
                    connection, null,
                    "character_attributes",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterAttributes)));
            }
        }
    }
}
#endif