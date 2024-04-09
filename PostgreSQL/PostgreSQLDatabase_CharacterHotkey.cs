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
        public const string CACHE_KEY_FILL_CHARACTER_HOTKEYS_UPDATE = "FILL_CHARACTER_HOTKEYS_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_HOTKEYS_INSERT = "FILL_CHARACTER_HOTKEYS_INSERT";
        public async UniTask FillCharacterHotkeys(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterHotkey> characterHotkeys)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_CHARACTER_HOTKEYS_UPDATE,
                connection, transaction,
                "character_hotkeys",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterHotkeys)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_CHARACTER_HOTKEYS_INSERT,
                    connection, null,
                    "character_hotkeys",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterHotkeys)));
            }
        }
    }
}
#endif