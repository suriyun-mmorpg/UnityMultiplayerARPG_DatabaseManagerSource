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
        public const string CACHE_KEY_FILL_CHARACTER_BUFFS_UPDATE = "FILL_CHARACTER_BUFFS_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_BUFFS_INSERT = "FILL_CHARACTER_BUFFS_INSERT";
        public async UniTask FillCharacterBuffs(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterBuff> characterBuffs)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_BUFFS_UPDATE,
                connection, transaction,
                "character_buffs",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterBuffs)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_BUFFS_INSERT,
                    connection, null,
                    "character_buffs",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterBuffs)));
            }
        }
    }
}
#endif