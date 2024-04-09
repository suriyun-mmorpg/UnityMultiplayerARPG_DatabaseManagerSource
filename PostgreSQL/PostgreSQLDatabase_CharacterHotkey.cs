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
        public const string CACHE_KEY_FILL_CHARACTER_HOTKEYS = "FILL_CHARACTER_HOTKEYS";
        public async UniTask FillCharacterHotkeys(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterHotkey> characterHotkeys)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_CHARACTER_HOTKEYS,
                connection, transaction,
                "character_hotkeys",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterHotkeys)));
        }
    }
}
#endif