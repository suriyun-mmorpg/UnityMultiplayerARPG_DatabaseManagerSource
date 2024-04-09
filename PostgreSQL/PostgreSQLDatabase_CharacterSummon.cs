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
        public const string CACHE_KEY_FILL_CHARACTER_SUMMONS = "FILL_CHARACTER_SUMMONS";
        public async UniTask FillCharacterSummons(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterSummon> characterSummons)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_SUMMONS,
                connection, transaction,
                "character_summons",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSummons)));
        }
    }
}
#endif