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
        public const string CACHE_KEY_FILL_CHARACTER_BUFFS = "FILL_CHARACTER_BUFFS";
        public async UniTask FillCharacterBuffs(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterBuff> characterBuffs)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_BUFFS,
                connection, transaction,
                "character_buffs",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterBuffs)));
        }
    }
}
#endif