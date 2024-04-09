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
        public const string CACHE_KEY_FILL_CHARACTER_SUMMONS_UPDATE = "FILL_CHARACTER_SUMMONS_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_SUMMONS_INSERT = "FILL_CHARACTER_SUMMONS_INSERT";
        public async UniTask FillCharacterSummons(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterSummon> characterSummons)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_SUMMONS_UPDATE,
                connection, transaction,
                "character_summons",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSummons)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_SUMMONS_INSERT,
                    connection, null,
                    "character_summons",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterSummons)));
            }
        }
    }
}
#endif