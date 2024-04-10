#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private async UniTask FillCharacterRelatesData(NpgsqlConnection connection, NpgsqlTransaction transaction, IPlayerCharacterData characterData, List<CharacterBuff> summonBuffs, List<CharacterItem> storageItems)
        {
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_attributes", characterData.Id, characterData.Attributes);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_buffs", characterData.Id, characterData.Buffs);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_hotkeys", characterData.Id, characterData.Hotkeys);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_selectable_weapon_sets", characterData.Id, characterData.SelectableWeaponSets);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_equip_items", characterData.Id, characterData.EquipItems);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_non_equip_items", characterData.Id, characterData.NonEquipItems);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_quests", characterData.Id, characterData.Quests);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_currencies", characterData.Id, characterData.Currencies);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_skills", characterData.Id, characterData.Skills);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_skill_usages", characterData.Id, characterData.SkillUsages);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_summons", characterData.Id, characterData.Summons);

            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_server_booleans", characterData.Id, characterData.ServerBools);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_server_int32s", characterData.Id, characterData.ServerInts);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_server_float32s", characterData.Id, characterData.ServerFloats);

            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_private_booleans", characterData.Id, characterData.PrivateBools);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_private_int32s", characterData.Id, characterData.PrivateInts);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_private_float32s", characterData.Id, characterData.PrivateFloats);

            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_public_booleans", characterData.Id, characterData.PublicBools);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_public_int32s", characterData.Id, characterData.PublicInts);
            await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_public_float32s", characterData.Id, characterData.PublicFloats);

            if (summonBuffs != null)
                await PostgreSQLHelpers.ExecuteUpsertJson(connection, transaction, "character_summon_buffs", characterData.Id, summonBuffs);

            if (storageItems != null)
                await UpdateStorageItems(connection, transaction, StorageType.Player, characterData.UserId, storageItems);
        }

        public const string CACHE_KEY_CREATE_CHARACTER = "CREATE_CHARACTER";
        public override async UniTask CreateCharacter(string userId, IPlayerCharacterData character)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_CREATE_CHARACTER,
                    connection, transaction,
                    "characters",
                    new PostgreSQLHelpers.ColumnInfo("id", character.Id),
                    new PostgreSQLHelpers.ColumnInfo("user_id", userId),
                    new PostgreSQLHelpers.ColumnInfo("entity_id", character.EntityId),
                    new PostgreSQLHelpers.ColumnInfo("data_id", character.DataId),
                    new PostgreSQLHelpers.ColumnInfo("faction_id", character.FactionId),
                    new PostgreSQLHelpers.ColumnInfo("character_name", character.CharacterName),
                    new PostgreSQLHelpers.ColumnInfo("level", character.Level),
                    new PostgreSQLHelpers.ColumnInfo("exp", character.Exp),
                    new PostgreSQLHelpers.ColumnInfo("current_hp", character.CurrentHp),
                    new PostgreSQLHelpers.ColumnInfo("current_mp", character.CurrentMp),
                    new PostgreSQLHelpers.ColumnInfo("current_stamina", character.CurrentStamina),
                    new PostgreSQLHelpers.ColumnInfo("current_food", character.CurrentFood),
                    new PostgreSQLHelpers.ColumnInfo("current_water", character.CurrentWater),
                    new PostgreSQLHelpers.ColumnInfo("equip_weapon_set", character.EquipWeaponSet),
                    new PostgreSQLHelpers.ColumnInfo("stat_point", character.StatPoint),
                    new PostgreSQLHelpers.ColumnInfo("skill_point", character.SkillPoint),
                    new PostgreSQLHelpers.ColumnInfo("gold", character.Gold),
                    new PostgreSQLHelpers.ColumnInfo("current_map_name", character.CurrentMapName),
                    new PostgreSQLHelpers.ColumnInfo("current_position_x", character.CurrentPosition.x),
                    new PostgreSQLHelpers.ColumnInfo("current_position_y", character.CurrentPosition.y),
                    new PostgreSQLHelpers.ColumnInfo("current_position_z", character.CurrentPosition.z),
                    new PostgreSQLHelpers.ColumnInfo("current_rotation_x", character.CurrentRotation.x),
                    new PostgreSQLHelpers.ColumnInfo("current_rotation_y", character.CurrentRotation.y),
                    new PostgreSQLHelpers.ColumnInfo("current_rotation_z", character.CurrentRotation.z),
                    new PostgreSQLHelpers.ColumnInfo("respawn_map_name", character.RespawnMapName),
                    new PostgreSQLHelpers.ColumnInfo("respawn_position_x", character.RespawnPosition.x),
                    new PostgreSQLHelpers.ColumnInfo("respawn_position_y", character.RespawnPosition.y),
                    new PostgreSQLHelpers.ColumnInfo("respawn_position_z", character.RespawnPosition.z),
                    new PostgreSQLHelpers.ColumnInfo("mount_data_id", character.MountDataId),
                    new PostgreSQLHelpers.ColumnInfo("icon_data_id", character.IconDataId),
                    new PostgreSQLHelpers.ColumnInfo("frame_data_id", character.FrameDataId),
                    new PostgreSQLHelpers.ColumnInfo("title_data_id", character.TitleDataId));
                await FillCharacterRelatesData(connection, transaction, character, null, null);
                this.InvokeInstanceDevExtMethods("CreateCharacter", connection, transaction, userId, character);
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while create character: " + character.Id);
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        private bool ReadCharacter(NpgsqlDataReader reader, out PlayerCharacterData result)
        {
            if (reader.Read())
            {
                result = new PlayerCharacterData();
                result.Id = reader.GetString(0);
                result.UserId = reader.GetString(1);
                result.DataId = reader.GetInt32(2);
                result.EntityId = reader.GetInt32(3);
                result.FactionId = reader.GetInt32(4);
                result.CharacterName = reader.GetString(5);
                result.Level = reader.GetInt32(6);
                result.Exp = reader.GetInt32(7);
                result.CurrentHp = reader.GetInt32(8);
                result.CurrentMp = reader.GetInt32(9);
                result.CurrentStamina = reader.GetInt32(10);
                result.CurrentFood = reader.GetInt32(11);
                result.CurrentWater = reader.GetInt32(12);
                result.EquipWeaponSet = reader.GetByte(13);
                result.StatPoint = reader.GetFloat(14);
                result.SkillPoint = reader.GetFloat(15);
                result.Gold = reader.GetInt32(16);
                result.PartyId = reader.GetInt32(17);
                result.GuildId = reader.GetInt32(18);
                result.GuildRole = reader.GetByte(19);
                result.SharedGuildExp = reader.GetInt32(20);
                result.CurrentMapName = reader.GetString(21);
                result.CurrentPosition = new Vec3(reader.GetFloat(22), reader.GetFloat(23), reader.GetFloat(24));
                result.CurrentRotation = new Vec3(reader.GetFloat(25), reader.GetFloat(26), reader.GetFloat(27));
                result.RespawnMapName = reader.GetString(28);
                result.RespawnPosition = new Vec3(reader.GetFloat(29), reader.GetFloat(30), reader.GetFloat(31));
                result.MountDataId = reader.GetInt32(32);
                result.IconDataId = reader.GetInt32(34);
                result.FrameDataId = reader.GetInt32(35);
                result.TitleDataId = reader.GetInt32(36);
                result.LastDeadTime = reader.GetInt64(38);
                result.UnmuteTime = reader.GetInt64(39);
                result.LastUpdate = ((System.DateTimeOffset)reader.GetDateTime(40)).ToUnixTimeSeconds();
                if (!reader.IsDBNull(41))
                    result.IsPkOn = reader.GetBoolean(41);
                if (!reader.IsDBNull(42))
                    result.LastPkOnTime = reader.GetInt64(42);
                if (!reader.IsDBNull(43))
                    result.PkPoint = reader.GetInt32(43);
                if (!reader.IsDBNull(44))
                    result.ConsecutivePkKills = reader.GetInt32(44);
                if (!reader.IsDBNull(45))
                    result.HighestPkPoint = reader.GetInt32(45);
                if (!reader.IsDBNull(46))
                    result.HighestConsecutivePkKills = reader.GetInt32(46);
                return true;
            }
            result = null;
            return false;
        }

        public override async UniTask<PlayerCharacterData> ReadCharacter(
            string id,
            bool withEquipWeapons = true,
            bool withAttributes = true,
            bool withSkills = true,
            bool withSkillUsages = true,
            bool withBuffs = true,
            bool withEquipItems = true,
            bool withNonEquipItems = true,
            bool withSummons = true,
            bool withHotkeys = true,
            bool withQuests = true,
            bool withCurrencies = true,
            bool withServerCustomData = true,
            bool withPrivateCustomData = true,
            bool withPublicCustomData = true)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            return await ReadCharacter(
                connection,
                id,
                withEquipWeapons,
                withAttributes,
                withSkills,
                withSkillUsages,
                withBuffs,
                withEquipItems,
                withNonEquipItems,
                withSummons,
                withHotkeys,
                withQuests,
                withCurrencies,
                withServerCustomData,
                withPrivateCustomData,
                withPublicCustomData);
        }

        public async UniTask<PlayerCharacterData> ReadCharacter(
            NpgsqlConnection connection,
            string id,
            bool withEquipWeapons,
            bool withAttributes,
            bool withSkills,
            bool withSkillUsages,
            bool withBuffs,
            bool withEquipItems,
            bool withNonEquipItems,
            bool withSummons,
            bool withHotkeys,
            bool withQuests,
            bool withCurrencies,
            bool withServerCustomData,
            bool withPrivateCustomData,
            bool withPublicCustomData)
        {
            PlayerCharacterData result;
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT
                c.id, c.user_id, c.data_id, c.entity_id, c.faction_id, c.character_name, c.level, c.exp,
                c.current_hp, c.current_mp, c.current_stamina, c.current_food, c.current_water,
                c.equip_weapon_set, c.stat_point, c.skill_point, c.gold, c.party_id, c.guild_id, c.guild_role, c.shared_guild_exp,
                c.current_map_name, c.current_position_x, c.current_position_y, c.current_position_z, c.current_rotation_x, current_rotation_y, current_rotation_z,
                c.respawn_map_name, c.respawn_position_x, c.respawn_position_y, c.respawn_position_z,
                c.mount_data_id, c.pet_data_id, c.icon_data_id, c.frame_data_id, c.title_data_id, c.reputation, c.last_dead_time, c.unmute_time, c.update_time,
                cpk.is_pk_on, cpk.last_pk_on_time, cpk.pk_point, cpk.consecutive_pk_kills, cpk.highest_pk_point, cpk.highest_consecutive_pk_kills
                FROM characters AS c LEFT JOIN character_pk AS cpk ON c.id = cpk.id
                WHERE c.id=$1 LIMIT 1", connection);
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = id;
            var reader = await cmd.ExecuteReaderAsync();
            if (!ReadCharacter(reader, out result))
            {
                reader.Dispose();
                return result;
            }
            reader.Dispose();

            // Found character, then read its relates data
            if (withEquipWeapons)
                result.SelectableWeaponSets = await PostgreSQLHelpers.ExecuteSelectJson<EquipWeapons[]>(connection, "character_selectable_weapon_sets", id);
            if (withAttributes)
                result.Attributes = await PostgreSQLHelpers.ExecuteSelectJson<CharacterAttribute[]>(connection, "character_attributes", id);
            if (withSkills)
                result.Skills = await PostgreSQLHelpers.ExecuteSelectJson<CharacterSkill[]>(connection, "character_skills", id);
            if (withSkillUsages)
                result.SkillUsages = await PostgreSQLHelpers.ExecuteSelectJson<CharacterSkillUsage[]>(connection, "character_skill_usages", id);
            if (withBuffs)
                result.Buffs = await PostgreSQLHelpers.ExecuteSelectJson<CharacterBuff[]>(connection, "character_buffs", id);
            if (withEquipItems)
                result.EquipItems = await PostgreSQLHelpers.ExecuteSelectJson<CharacterItem[]>(connection, "character_equip_items", id);
            if (withNonEquipItems)
                result.NonEquipItems = await PostgreSQLHelpers.ExecuteSelectJson<CharacterItem[]>(connection, "character_non_equip_items", id);
            if (withSummons)
                result.Summons = await PostgreSQLHelpers.ExecuteSelectJson<CharacterSummon[]>(connection, "character_summons", id);
            if (withHotkeys)
                result.Hotkeys = await PostgreSQLHelpers.ExecuteSelectJson<CharacterHotkey[]>(connection, "character_hotkeys", id);
            if (withQuests)
                result.Quests = await PostgreSQLHelpers.ExecuteSelectJson<CharacterQuest[]>(connection, "character_quests", id);
            if (withCurrencies)
                result.Currencies = await PostgreSQLHelpers.ExecuteSelectJson<CharacterCurrency[]>(connection, "character_currencies", id);
            if (withServerCustomData)
            {
                result.ServerBools = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataBoolean[]>(connection, "character_server_booleans", id);
                result.ServerInts = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataInt32[]>(connection, "character_server_int32s", id);
                result.ServerFloats = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataFloat32[]>(connection, "character_server_float32s", id);
            }
            if (withPrivateCustomData)
            {
                result.PrivateBools = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataBoolean[]>(connection, "character_private_booleans", id);
                result.PrivateInts = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataInt32[]>(connection, "character_private_int32s", id);
                result.PrivateFloats = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataFloat32[]>(connection, "character_private_float32s", id);
            }
            if (withPublicCustomData)
            {
                result.PublicBools = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataBoolean[]>(connection, "character_public_booleans", id);
                result.PublicInts = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataInt32[]>(connection, "character_public_int32s", id);
                result.PublicFloats = await PostgreSQLHelpers.ExecuteSelectJson<CharacterDataFloat32[]>(connection, "character_public_float32s", id);
            }
            // Invoke dev extension methods
            this.InvokeInstanceDevExtMethods("ReadCharacter",
                result,
                withEquipWeapons,
                withAttributes,
                withSkills,
                withSkillUsages,
                withBuffs,
                withEquipItems,
                withNonEquipItems,
                withSummons,
                withHotkeys,
                withQuests,
                withCurrencies,
                withServerCustomData,
                withPrivateCustomData,
                withPublicCustomData);
            return result;
        }

        public const string CACHE_KEY_READ_CHARACTERS = "READ_CHARACTERS";
        public override async UniTask<List<PlayerCharacterData>> ReadCharacters(string userId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_READ_CHARACTERS,
                connection,
                "characters", "id", "ORDER BY update_time DESC",
                PostgreSQLHelpers.WhereEqualTo("user_id", userId));

            List<string> characterIds = new List<string>();
            while (reader.Read())
            {
                characterIds.Add(reader.GetString(0));
            }
            reader.Dispose();

            List<PlayerCharacterData> result = new List<PlayerCharacterData>();
            foreach (string characterId in characterIds)
            {
                result.Add(await ReadCharacter(connection, characterId, true, false, false, false, false, true, false, false, false, false, false, false, false, true));
            }
            return result;
        }

        public const string CACHE_KEY_UPDATE_CHARACTER_PK = "UPDATE_CHARACTER_PK";
        public async UniTask UpdateCharacterPk(NpgsqlConnection connection, NpgsqlTransaction transaction, IPlayerCharacterData character)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_UPDATE_CHARACTER_PK,
                connection, transaction,
                "character_pk",
                "id",
                new PostgreSQLHelpers.ColumnInfo("id", character.Id),
                new PostgreSQLHelpers.ColumnInfo("is_pk_on", character.IsPkOn),
                new PostgreSQLHelpers.ColumnInfo("last_pk_on_time", character.LastPkOnTime),
                new PostgreSQLHelpers.ColumnInfo("pk_point", character.PkPoint),
                new PostgreSQLHelpers.ColumnInfo("consecutive_pk_kills", character.ConsecutivePkKills),
                new PostgreSQLHelpers.ColumnInfo("highest_pk_point", character.HighestPkPoint),
                new PostgreSQLHelpers.ColumnInfo("highest_consecutive_pk_kills", character.HighestConsecutivePkKills));
        }

        public const string CACHE_KEY_UPDATE_CHARACTER = "UPDATE_CHARACTER";
        public override async UniTask UpdateCharacter(IPlayerCharacterData character, List<CharacterBuff> summonBuffs, List<CharacterItem> storageItems, bool deleteStorageReservation)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await UpdateCharacterPk(connection, transaction, character);
                await PostgreSQLHelpers.ExecuteUpdate(
                    CACHE_KEY_UPDATE_CHARACTER,
                    connection, transaction,
                    "characters",
                    new[]
                    {
                        new PostgreSQLHelpers.ColumnInfo("entity_id", character.EntityId),
                        new PostgreSQLHelpers.ColumnInfo("data_id", character.DataId),
                        new PostgreSQLHelpers.ColumnInfo("faction_id", character.FactionId),
                        new PostgreSQLHelpers.ColumnInfo("character_name", character.CharacterName),
                        new PostgreSQLHelpers.ColumnInfo("level", character.Level),
                        new PostgreSQLHelpers.ColumnInfo("exp", character.Exp),
                        new PostgreSQLHelpers.ColumnInfo("current_hp", character.CurrentHp),
                        new PostgreSQLHelpers.ColumnInfo("current_mp", character.CurrentMp),
                        new PostgreSQLHelpers.ColumnInfo("current_stamina", character.CurrentStamina),
                        new PostgreSQLHelpers.ColumnInfo("current_food", character.CurrentFood),
                        new PostgreSQLHelpers.ColumnInfo("current_water", character.CurrentWater),
                        new PostgreSQLHelpers.ColumnInfo("equip_weapon_set", character.EquipWeaponSet),
                        new PostgreSQLHelpers.ColumnInfo("stat_point", character.StatPoint),
                        new PostgreSQLHelpers.ColumnInfo("skill_point", character.SkillPoint),
                        new PostgreSQLHelpers.ColumnInfo("gold", character.Gold),
                        new PostgreSQLHelpers.ColumnInfo("current_map_name", character.CurrentMapName),
                        new PostgreSQLHelpers.ColumnInfo("current_position_x", character.CurrentPosition.x),
                        new PostgreSQLHelpers.ColumnInfo("current_position_y", character.CurrentPosition.y),
                        new PostgreSQLHelpers.ColumnInfo("current_position_z", character.CurrentPosition.z),
                        new PostgreSQLHelpers.ColumnInfo("current_rotation_x", character.CurrentRotation.x),
                        new PostgreSQLHelpers.ColumnInfo("current_rotation_y", character.CurrentRotation.y),
                        new PostgreSQLHelpers.ColumnInfo("current_rotation_z", character.CurrentRotation.z),
                        new PostgreSQLHelpers.ColumnInfo("respawn_map_name", character.RespawnMapName),
                        new PostgreSQLHelpers.ColumnInfo("respawn_position_x", character.RespawnPosition.x),
                        new PostgreSQLHelpers.ColumnInfo("respawn_position_y", character.RespawnPosition.y),
                        new PostgreSQLHelpers.ColumnInfo("respawn_position_z", character.RespawnPosition.z),
                        new PostgreSQLHelpers.ColumnInfo("mount_data_id", character.MountDataId),
                        new PostgreSQLHelpers.ColumnInfo("icon_data_id", character.IconDataId),
                        new PostgreSQLHelpers.ColumnInfo("frame_data_id", character.FrameDataId),
                        new PostgreSQLHelpers.ColumnInfo("title_data_id", character.TitleDataId),
                        new PostgreSQLHelpers.ColumnInfo("last_dead_time", character.LastDeadTime),
                        new PostgreSQLHelpers.ColumnInfo("unmute_time", character.UnmuteTime),
                    },
                    PostgreSQLHelpers.WhereEqualTo("id", character.Id));
                await FillCharacterRelatesData(connection, transaction, character, summonBuffs, storageItems);
                if (deleteStorageReservation)
                {
                    await DeleteReservedStorageByReserver(character.Id);
                }
                this.InvokeInstanceDevExtMethods("UpdateCharacter", connection, transaction, character);
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while update character: " + character.Id);
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public override async UniTask DeleteCharacter(string userId, string id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_attributes", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_buffs", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_currencies", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_hotkeys", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_selectable_weapon_sets", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_equip_items", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_non_equip_items", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_protected_non_equip_items", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_quests", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_skills", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_skill_usages", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_summons", id);

                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_server_booleans", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_server_int32s", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_server_float32s", id);

                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_private_booleans", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_private_int32s", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_private_float32s", id);

                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_public_booleans", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_public_int32s", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_public_float32s", id);

                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "characters", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_pk", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "friends", "character_id_1", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "friends", "character_id_2", id);

                this.InvokeInstanceDevExtMethods("DeleteCharacter", connection, transaction, userId, id);
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while deleting character: " + id);
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public const string CACHE_KEY_FIND_CHARACTER_NAME = "FIND_CHARACTER_NAME";
        public override async UniTask<long> FindCharacterName(string characterName)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            return await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_FIND_CHARACTER_NAME,
                connection,
                "characters",
                PostgreSQLHelpers.WhereLike("character_name", characterName));
        }

        public const string CACHE_KEY_GET_ID_BY_CHARACTER_NAME = "GET_ID_BY_CHARACTER_NAME";
        public override async UniTask<string> GetIdByCharacterName(string characterName)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            object result = PostgreSQLHelpers.ExecuteSelectScalar(
                CACHE_KEY_GET_ID_BY_CHARACTER_NAME,
                connection,
                "characters", "id", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("character_name", characterName));
            return result != null ? (string)result : string.Empty;
        }

        public const string CACHE_KEY_GET_USER_ID_BY_CHARACTER_NAME = "GET_USER_ID_BY_CHARACTER_NAME";
        public override async UniTask<string> GetUserIdByCharacterName(string characterName)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            object result = PostgreSQLHelpers.ExecuteSelectScalar(
                CACHE_KEY_GET_USER_ID_BY_CHARACTER_NAME,
                connection,
                "characters", "user_id", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("character_name", characterName));
            return result != null ? (string)result : string.Empty;
        }

        public const string CACHE_KEY_FIND_CHARACTERS_SELECT_FRIENDS = "FIND_CHARACTERS_SELECT_FRIENDS";
        public override async UniTask<List<SocialCharacterData>> FindCharacters(string finderId, string characterName, int skip, int limit)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            // Exclude friend, requested characters
            var readerIds = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_FIND_CHARACTERS_SELECT_FRIENDS,
                connection,
                "friends", "character_id_2",
                PostgreSQLHelpers.WhereEqualTo("character_id_1", finderId));
            string excludeIdsQuery = $"(id != '{finderId}'";
            while (readerIds.Read())
            {
                excludeIdsQuery += $" AND id != '{readerIds.GetString(0)}'";
            }
            excludeIdsQuery += ")";
            readerIds.Dispose();
            // Read some character data
            using var readerCharacters = await PostgreSQLHelpers.ExecuteSelect(
                null,
                connection,
                "characters", "id, data_id, character_name, level", $"AND {excludeIdsQuery} ORDER BY RANDOM() OFFSET {skip} LIMIT {limit}",
                PostgreSQLHelpers.WhereLike("character_name", $"%{characterName}%"));
            List<SocialCharacterData> characters = new List<SocialCharacterData>();
            SocialCharacterData tempCharacter;
            while (readerCharacters.Read())
            {
                tempCharacter = new SocialCharacterData();
                tempCharacter.id = readerCharacters.GetString(0);
                tempCharacter.dataId = readerCharacters.GetInt32(1);
                tempCharacter.characterName = readerCharacters.GetString(2);
                tempCharacter.level = readerCharacters.GetInt32(3);
                characters.Add(tempCharacter);
            }
            return characters;
        }

        public const string CACHE_KEY_CREATE_FRIEND = "CREATE_FRIEND";
        public override async UniTask CreateFriend(string id1, string id2, byte state)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_CREATE_FRIEND,
                connection, null,
                "friends",
                "character_id_1, character_id_2",
                new PostgreSQLHelpers.ColumnInfo("state", state),
                new PostgreSQLHelpers.ColumnInfo("character_id_1", id1),
                new PostgreSQLHelpers.ColumnInfo("character_id_2", id2));
        }

        public const string CACHE_KEY_DELETE_FRIEND = "DELETE_FRIEND";
        public override async UniTask DeleteFriend(string id1, string id2)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_FRIEND,
                connection, null,
                "friends",
                PostgreSQLHelpers.WhereEqualTo("character_id_1", id1),
                PostgreSQLHelpers.AndWhereEqualTo("character_id_2", id2));
        }

        public const string CACHE_KEY_READ_FRIENDS_ID_1 = "READ_FRIENDS_ID_1";
        public const string CACHE_KEY_READ_FRIENDS_ID_2 = "READ_FRIENDS_ID_2";
        public override async UniTask<List<SocialCharacterData>> ReadFriends(string id, bool readById2, byte state, int skip, int limit)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            List<string> characterIds = new List<string>();
            if (readById2)
            {
                var readerIds = await PostgreSQLHelpers.ExecuteSelect(
                    CACHE_KEY_READ_FRIENDS_ID_1,
                    connection,
                    "friends", "character_id_1", $"OFFSET {skip} LIMIT {limit}",
                    PostgreSQLHelpers.WhereEqualTo("character_id_2", id),
                    PostgreSQLHelpers.AndWhereSmallEqualTo("state", state));
                while (readerIds.Read())
                {
                    characterIds.Add(readerIds.GetString(0));
                }
                readerIds.Dispose();
            }
            else
            {
                var readerIds = await PostgreSQLHelpers.ExecuteSelect(
                    CACHE_KEY_READ_FRIENDS_ID_2,
                    connection,
                    "friends", "character_id_2", $"OFFSET {skip} LIMIT {limit}",
                    PostgreSQLHelpers.WhereEqualTo("character_id_1", id),
                    PostgreSQLHelpers.AndWhereSmallEqualTo("state", state));
                while (readerIds.Read())
                {
                    characterIds.Add(readerIds.GetString(0));
                }
                readerIds.Dispose();
            }
            return await GetSocialCharacterByIds(connection, characterIds);
        }

        public const string CACHE_KEY_GET_FRIEND_REQUESTS_NOTIFICATION = "GET_FRIEND_REQUESTS_NOTIFICATION";
        public override async UniTask<int> GetFriendRequestNotification(string characterId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            return (int)await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_GET_FRIEND_REQUESTS_NOTIFICATION,
                connection,
                "friends",
                PostgreSQLHelpers.WhereEqualTo("character_id_2", characterId),
                PostgreSQLHelpers.AndWhereSmallEqualTo("state", 1));
        }

        public async UniTask<List<SocialCharacterData>> GetSocialCharacterByIds(NpgsqlConnection connection, IList<string> characterIds, string select = "id, data_id, character_name, level")
        {
            List<SocialCharacterData> characters = new List<SocialCharacterData>();
            if (characterIds.Count > 0)
            {
                List<PostgreSQLHelpers.WhereQuery> characterQueries = new List<PostgreSQLHelpers.WhereQuery>()
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterIds[0]),
                };
                for (int i = 1; i < characterIds.Count; ++i)
                {
                    characterQueries.Add(PostgreSQLHelpers.OrWhereEqualTo("id", characterIds[i]));
                }
                using var readerCharacters = await PostgreSQLHelpers.ExecuteSelect(
                    null,
                    connection,
                    "characters",
                    characterQueries,
                    select);
                SocialCharacterData tempCharacter;
                while (readerCharacters.Read())
                {
                    tempCharacter = new SocialCharacterData();
                    tempCharacter.id = readerCharacters.GetString(0);
                    tempCharacter.dataId = readerCharacters.GetInt32(1);
                    tempCharacter.characterName = readerCharacters.GetString(2);
                    tempCharacter.level = readerCharacters.GetInt32(3);
                    characters.Add(tempCharacter);
                }
            }
            return characters;
        }
    }
}
#endif