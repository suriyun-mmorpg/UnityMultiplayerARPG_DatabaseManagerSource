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
        public const string CACHE_KEY_FILL_CHARACTER_CURRENCIES = "FILL_CHARACTER_CURRENCIES";
        public async UniTask FillCharacterCurrencies(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterCurrency> characterCurrencies)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_CURRENCIES,
                connection, transaction,
                "character_currencies",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterCurrencies)));
        }
    }
}
#endif