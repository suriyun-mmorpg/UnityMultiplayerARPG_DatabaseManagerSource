#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private async UniTask CreateCharacterItem(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, InventoryType inventoryType, CharacterItem characterItem)
        {
            string id = characterItem.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Item {id}, inventory type {inventoryType}, for character {characterId}, already inserted");
                return;
            }
            if (string.IsNullOrEmpty(id))
                return;
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characteritem (id, idx, inventoryType, characterId, dataId, level, amount, equipSlotIndex, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version) VALUES (@id, @idx, @inventoryType, @characterId, @dataId, @level, @amount, @equipSlotIndex, @durability, @exp, @lockRemainsDuration, @expireTime, @randomSeed, @ammo, @sockets, @version)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@idx", idx),
                new NpgsqlParameter("@inventoryType", (byte)inventoryType),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@dataId", characterItem.dataId),
                new NpgsqlParameter("@level", characterItem.level),
                new NpgsqlParameter("@amount", characterItem.amount),
                new NpgsqlParameter("@equipSlotIndex", characterItem.equipSlotIndex),
                new NpgsqlParameter("@durability", characterItem.durability),
                new NpgsqlParameter("@exp", characterItem.exp),
                new NpgsqlParameter("@lockRemainsDuration", characterItem.lockRemainsDuration),
                new NpgsqlParameter("@expireTime", characterItem.expireTime),
                new NpgsqlParameter("@randomSeed", characterItem.randomSeed),
                new NpgsqlParameter("@ammo", characterItem.ammo),
                new NpgsqlParameter("@sockets", characterItem.WriteSockets()),
                new NpgsqlParameter("@version", characterItem.version));
        }

        private bool ReadCharacterItem(NpgsqlDataReader reader, out CharacterItem result)
        {
            if (reader.Read())
            {
                result = new CharacterItem();
                result.id = reader.GetString(0);
                result.dataId = reader.GetInt32(1);
                result.level = reader.GetInt32(2);
                result.amount = reader.GetInt32(3);
                result.equipSlotIndex = reader.GetByte(4);
                result.durability = reader.GetFloat(5);
                result.exp = reader.GetInt32(6);
                result.lockRemainsDuration = reader.GetFloat(7);
                result.expireTime = reader.GetInt64(8);
                result.randomSeed = reader.GetInt32(9);
                result.ammo = reader.GetInt32(10);
                result.ReadSockets(reader.GetString(11));
                result.version = reader.GetByte(12);
                return true;
            }
            result = CharacterItem.Empty;
            return false;
        }

        private async UniTask<List<CharacterItem>> ReadCharacterItems(string characterId, InventoryType inventoryType, List<CharacterItem> result = null)
        {
            if (result == null)
                result = new List<CharacterItem>();
            await ExecuteReader((reader) =>
            {
                CharacterItem tempInventory;
                while (ReadCharacterItem(reader, out tempInventory))
                {
                    result.Add(tempInventory);
                }
            }, "SELECT id, dataId, level, amount, equipSlotIndex, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version FROM characteritem WHERE characterId=@characterId AND inventoryType=@inventoryType ORDER BY idx ASC",
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@inventoryType", (byte)inventoryType));
            return result;
        }

        public async UniTask<List<EquipWeapons>> ReadCharacterEquipWeapons(string characterId, List<EquipWeapons> result = null)
        {
            if (result == null)
                result = new List<EquipWeapons>();
            await ExecuteReader((reader) =>
            {
                EquipWeapons tempEquipWeapons;
                CharacterItem tempInventory;
                byte equipWeaponSet;
                InventoryType inventoryType;
                while (ReadCharacterItem(reader, out tempInventory))
                {
                    equipWeaponSet = reader.GetByte(13);
                    inventoryType = (InventoryType)reader.GetByte(14);
                    // Fill weapon sets if needed
                    while (result.Count <= equipWeaponSet)
                        result.Add(new EquipWeapons());
                    // Get equip weapon set
                    if (inventoryType == InventoryType.EquipWeaponRight)
                    {
                        tempEquipWeapons = result[equipWeaponSet];
                        tempEquipWeapons.rightHand = tempInventory;
                        result[equipWeaponSet] = tempEquipWeapons;
                    }
                    if (inventoryType == InventoryType.EquipWeaponLeft)
                    {
                        tempEquipWeapons = result[equipWeaponSet];
                        tempEquipWeapons.leftHand = tempInventory;
                        result[equipWeaponSet] = tempEquipWeapons;
                    }
                }
            }, "SELECT id, dataId, level, amount, equipSlotIndex, durability, exp, lockRemainsDuration, expireTime, randomSeed, ammo, sockets, version, idx, inventoryType FROM characteritem WHERE characterId=@characterId AND (inventoryType=@inventoryType1 OR inventoryType=@inventoryType2) ORDER BY idx ASC",
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@inventoryType1", (byte)InventoryType.EquipWeaponRight),
                new NpgsqlParameter("@inventoryType2", (byte)InventoryType.EquipWeaponLeft));
            return result;
        }

        public async UniTask CreateCharacterEquipWeapons(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, int equipWeaponSet, string characterId, EquipWeapons equipWeapons)
        {
            await CreateCharacterItem(connection, transaction, insertedIds, equipWeaponSet, characterId, InventoryType.EquipWeaponRight, equipWeapons.rightHand);
            await CreateCharacterItem(connection, transaction, insertedIds, equipWeaponSet, characterId, InventoryType.EquipWeaponLeft, equipWeapons.leftHand);
        }

        public async UniTask CreateCharacterEquipItem(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterItem characterItem)
        {
            await CreateCharacterItem(connection, transaction, insertedIds, idx, characterId, InventoryType.EquipItems, characterItem);
        }

        public async UniTask<List<CharacterItem>> ReadCharacterEquipItems(string characterId, List<CharacterItem> result = null)
        {
            return await ReadCharacterItems(characterId, InventoryType.EquipItems, result);
        }

        public async UniTask CreateCharacterNonEquipItem(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterItem characterItem)
        {
            await CreateCharacterItem(connection, transaction, insertedIds, idx, characterId, InventoryType.NonEquipItems, characterItem);
        }

        public async UniTask<List<CharacterItem>> ReadCharacterNonEquipItems(string characterId, List<CharacterItem> result = null)
        {
            return await ReadCharacterItems(characterId, InventoryType.NonEquipItems, result);
        }

        public async UniTask DeleteCharacterItems(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characteritem WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif