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
        public const string CACHE_KEY_FILL_CHARACTER_ITEMS = "FILL_CHARACTER_ITEMS";
        public async UniTask FillCharacterItems(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId, IList<CharacterItem> characterItems)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                $"{CACHE_KEY_FILL_CHARACTER_ITEMS}_{tableName}",
                connection, transaction,
                tableName,
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)));
        }

        public const string CACHE_KEY_FILL_SELECTABLE_WEAPON_SET = "FILL_SELECTABLE_WEAPON_SET";
        public async UniTask FillSelectableWeaponSets(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<EquipWeapons> equipWeaponSets)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_SELECTABLE_WEAPON_SET,
                connection, transaction,
                "character_selectable_weapon_sets",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", characterId),
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(equipWeaponSets)));
        }
    }
}
#endif