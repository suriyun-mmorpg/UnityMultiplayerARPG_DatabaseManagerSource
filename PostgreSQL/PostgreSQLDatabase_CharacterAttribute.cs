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
        public const string CACHE_KEY_FILL_CHARACTER_ATTRIBUTES = "FILL_CHARACTER_ATTRIBUTES";
        public async UniTask FillCharacterAttributes(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterAttribute> characterAttributes)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_ATTRIBUTES,
                connection, transaction,
                "character_attributes",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterAttributes)));
        }
    }
}
#endif