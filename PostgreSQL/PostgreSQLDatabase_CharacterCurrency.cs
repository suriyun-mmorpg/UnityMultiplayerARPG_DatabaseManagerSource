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
        public const string CACHE_KEY_FILL_CHARACTER_CURRENCIES_UPDATE = "FILL_CHARACTER_CURRENCIES_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_CURRENCIES_INSERT = "FILL_CHARACTER_CURRENCIES_INSERT";
        public async UniTask FillCharacterCurrencies(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterCurrency> characterCurrencies)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_CURRENCIES_UPDATE,
                connection, transaction,
                "character_currencies",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterCurrencies)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_CURRENCIES_INSERT,
                    connection, null,
                    "character_currencies",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterCurrencies)));
            }
        }
    }
}
#endif