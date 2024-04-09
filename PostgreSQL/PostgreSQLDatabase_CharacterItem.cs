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
        public const string CACHE_KEY_FILL_CHARACTER_ITEMS_UPDATE = "FILL_CHARACTER_ITEMS_UPDATE";
        public const string CACHE_KEY_FILL_CHARACTER_ITEMS_INSERT = "FILL_CHARACTER_ITEMS_INSERT";
        public async UniTask FillCharacterItems(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId, IList<CharacterItem> characterItems)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                $"{CACHE_KEY_FILL_CHARACTER_ITEMS_UPDATE}_{tableName}",
                connection, transaction,
                tableName,
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    $"{CACHE_KEY_FILL_CHARACTER_ITEMS_INSERT}_{tableName}",
                    connection, null,
                    tableName,
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)));
            }
        }

        public const string CACHE_KEY_FILL_SELECTABLE_WEAPONS_SET_UPDATE = "FILL_SELECTABLE_WEAPONS_SET_UPDATE";
        public const string CACHE_KEY_FILL_SELECTABLE_WEAPONS_SET_INSERT = "FILL_SELECTABLE_WEAPONS_SET_INSERT";
        public async UniTask FillSelectableWeaponSets(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<EquipWeapons> equipWeaponSets)
        {
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_FILL_SELECTABLE_WEAPONS_SET_UPDATE,
                connection, transaction,
                "character_selectable_weapon_sets",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(equipWeaponSets)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_FILL_SELECTABLE_WEAPONS_SET_INSERT,
                    connection, null,
                    "character_selectable_weapon_sets",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(equipWeaponSets)));
            }
        }
    }
}
#endif