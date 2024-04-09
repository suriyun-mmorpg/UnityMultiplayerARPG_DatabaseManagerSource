#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private async UniTask FillCharacterRelatesData(NpgsqlConnection connection, NpgsqlTransaction transaction, IPlayerCharacterData characterData, List<CharacterBuff> summonBuffs, List<CharacterItem> storageItems)
        {
            await FillCharacterAttributes(connection, transaction, characterData.Id, characterData.Attributes);
            await FillCharacterBuffs(connection, transaction, characterData.Id, characterData.Buffs);
            await FillCharacterHotkeys(connection, transaction, characterData.Id, characterData.Hotkeys);
            await FillSelectableWeaponSets(connection, transaction, characterData.Id, characterData.SelectableWeaponSets);
            await FillCharacterItems(connection, transaction, "character_equip_items", characterData.Id, characterData.EquipItems);
            await FillCharacterItems(connection, transaction, "character_non_equip_items", characterData.Id, characterData.NonEquipItems);
            await FillCharacterItems(connection, transaction, "character_protected_non_equip_items", characterData.Id, characterData.ProtectedNonEquipItems);
            await FillCharacterQuests(connection, transaction, characterData.Id, characterData.Quests);
            await FillCharacterCurrencies(connection, transaction, characterData.Id, characterData.Currencies);
            await FillCharacterSkills(connection, transaction, characterData.Id, characterData.Skills);
            await FillCharacterSkillUsages(connection, transaction, characterData.Id, characterData.SkillUsages);
            await FillCharacterSummons(connection, transaction, characterData.Id, characterData.Summons);

            await FillCharacterDataBooleans(connection, transaction, "character_server_boolean", characterData.Id, characterData.ServerBools);
            await FillCharacterDataInt32s(connection, transaction, "character_server_int32", characterData.Id, characterData.ServerInts);
            await FillCharacterDataFloat32s(connection, transaction, "character_server_float32", characterData.Id, characterData.ServerFloats);

            await FillCharacterDataBooleans(connection, transaction, "character_private_boolean", characterData.Id, characterData.PrivateBools);
            await FillCharacterDataInt32s(connection, transaction, "character_private_int32", characterData.Id, characterData.PrivateInts);
            await FillCharacterDataFloat32s(connection, transaction, "character_private_float32", characterData.Id, characterData.PrivateFloats);

            await FillCharacterDataBooleans(connection, transaction, "character_public_boolean", characterData.Id, characterData.PublicBools);
            await FillCharacterDataInt32s(connection, transaction, "character_public_int32", characterData.Id, characterData.PublicInts);
            await FillCharacterDataFloat32s(connection, transaction, "character_public_float32", characterData.Id, characterData.PublicFloats);

            if (summonBuffs != null)
                await FillSummonBuffs(connection, transaction, characterData.Id, summonBuffs);

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
                    new PostgreSQLHelpers.ColumnInfo("pet_data_id", character.PetDataId),
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
                result.IconDataId = reader.GetInt32(33);
                result.FrameDataId = reader.GetInt32(34);
                result.TitleDataId = reader.GetInt32(35);
                result.LastDeadTime = reader.GetInt64(36);
                result.UnmuteTime = reader.GetInt64(37);
                result.LastUpdate = ((System.DateTimeOffset)reader.GetDateTime(38)).ToUnixTimeSeconds();
                if (!reader.IsDBNull(39))
                    result.IsPkOn = reader.GetBoolean(39);
                if (!reader.IsDBNull(40))
                    result.LastPkOnTime = reader.GetInt64(40);
                if (!reader.IsDBNull(41))
                    result.PkPoint = reader.GetInt32(41);
                if (!reader.IsDBNull(42))
                    result.ConsecutivePkKills = reader.GetInt32(42);
                if (!reader.IsDBNull(43))
                    result.HighestPkPoint = reader.GetInt32(43);
                if (!reader.IsDBNull(44))
                    result.HighestConsecutivePkKills = reader.GetInt32(44);
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
            PlayerCharacterData result = null;
            await ExecuteReader((reader) =>
            {
                ReadCharacter(reader, out result);
            }, @"SELECT
                c.id, c.userId, c.dataId, c.entityId, c.factionId, c.characterName, c.level, c.exp,
                c.currentHp, c.currentMp, c.currentStamina, c.currentFood, c.currentWater,
                c.equipWeaponSet, c.statPoint, c.skillPoint, c.gold, c.partyId, c.guildId, c.guildRole, c.sharedGuildExp,
                c.currentMapName, c.currentPositionX, c.currentPositionY, c.currentPositionZ, c.currentRotationX, currentRotationY, currentRotationZ,
                c.respawnMapName, c.respawnPositionX, c.respawnPositionY, c.respawnPositionZ,
                c.mountDataId, c.iconDataId, c.frameDataId, c.titleDataId, c.lastDeadTime, c.unmuteTime, c.updateAt,
                cpk.isPkOn, cpk.lastPkOnTime, cpk.pkPoint, cpk.consecutivePkKills, cpk.highestPkPoint, cpk.highestConsecutivePkKills
                FROM characters AS c LEFT JOIN character_pk AS cpk ON c.id = cpk.id
                WHERE c.id=@id LIMIT 1",
                new NpgsqlParameter("@id", id));
            // Found character, then read its relates data
            if (result != null)
            {
                List<EquipWeapons> selectableWeaponSets = new List<EquipWeapons>();
                List<CharacterAttribute> attributes = new List<CharacterAttribute>();
                List<CharacterSkill> skills = new List<CharacterSkill>();
                List<CharacterSkillUsage> skillUsages = new List<CharacterSkillUsage>();
                List<CharacterBuff> buffs = new List<CharacterBuff>();
                List<CharacterItem> equipItems = new List<CharacterItem>();
                List<CharacterItem> nonEquipItems = new List<CharacterItem>();
                List<CharacterSummon> summons = new List<CharacterSummon>();
                List<CharacterHotkey> hotkeys = new List<CharacterHotkey>();
                List<CharacterQuest> quests = new List<CharacterQuest>();
                List<CharacterCurrency> currencies = new List<CharacterCurrency>();

                List<CharacterDataBoolean> serverBools = new List<CharacterDataBoolean>();
                List<CharacterDataInt32> serverInts = new List<CharacterDataInt32>();
                List<CharacterDataFloat32> serverFloats = new List<CharacterDataFloat32>();

                List<CharacterDataBoolean> privateBools = new List<CharacterDataBoolean>();
                List<CharacterDataInt32> privateInts = new List<CharacterDataInt32>();
                List<CharacterDataFloat32> privateFloats = new List<CharacterDataFloat32>();

                List<CharacterDataBoolean> publicBools = new List<CharacterDataBoolean>();
                List<CharacterDataInt32> publicInts = new List<CharacterDataInt32>();
                List<CharacterDataFloat32> publicFloats = new List<CharacterDataFloat32>();

                // Read data
                List<UniTask> tasks = new List<UniTask>();
                if (withEquipWeapons)
                    tasks.Add(ReadCharacterEquipWeapons(id, selectableWeaponSets));
                if (withAttributes)
                    tasks.Add(ReadCharacterAttributes(id, attributes));
                if (withSkills)
                    tasks.Add(ReadCharacterSkills(id, skills));
                if (withSkillUsages)
                    tasks.Add(ReadCharacterSkillUsages(id, skillUsages));
                if (withBuffs)
                    tasks.Add(ReadCharacterBuffs(id, buffs));
                if (withEquipItems)
                    tasks.Add(ReadCharacterEquipItems(id, equipItems));
                if (withNonEquipItems)
                    tasks.Add(ReadCharacterNonEquipItems(id, nonEquipItems));
                if (withSummons)
                    tasks.Add(ReadCharacterSummons(id, summons));
                if (withHotkeys)
                    tasks.Add(ReadCharacterHotkeys(id, hotkeys));
                if (withQuests)
                    tasks.Add(ReadCharacterQuests(id, quests));
                if (withCurrencies)
                    tasks.Add(ReadCharacterCurrencies(id, currencies));
                if (withServerCustomData)
                {
                    tasks.Add(ReadCharacterDataBooleans("character_server_boolean", id, serverBools));
                    tasks.Add(ReadCharacterDataInt32s("character_server_int32", id, serverInts));
                    tasks.Add(ReadCharacterDataFloat32s("character_server_float32", id, serverFloats));
                }
                if (withPrivateCustomData)
                {
                    tasks.Add(ReadCharacterDataBooleans("character_private_boolean", id, privateBools));
                    tasks.Add(ReadCharacterDataInt32s("character_private_int32", id, privateInts));
                    tasks.Add(ReadCharacterDataFloat32s("character_private_float32", id, privateFloats));
                }
                if (withPublicCustomData)
                {
                    tasks.Add(ReadCharacterDataBooleans("character_public_boolean", id, publicBools));
                    tasks.Add(ReadCharacterDataInt32s("character_public_int32", id, publicInts));
                    tasks.Add(ReadCharacterDataFloat32s("character_public_float32", id, publicFloats));
                }
                await UniTask.WhenAll(tasks);
                // Assign read data
                if (withEquipWeapons)
                    result.SelectableWeaponSets = selectableWeaponSets;
                if (withAttributes)
                    result.Attributes = attributes;
                if (withSkills)
                    result.Skills = skills;
                if (withSkillUsages)
                    result.SkillUsages = skillUsages;
                if (withBuffs)
                    result.Buffs = buffs;
                if (withEquipItems)
                    result.EquipItems = equipItems;
                if (withNonEquipItems)
                    result.NonEquipItems = nonEquipItems;
                if (withSummons)
                    result.Summons = summons;
                if (withHotkeys)
                    result.Hotkeys = hotkeys;
                if (withQuests)
                    result.Quests = quests;
                if (withCurrencies)
                    result.Currencies = currencies;
                if (withServerCustomData)
                {
                    result.ServerBools = serverBools;
                    result.ServerInts = serverInts;
                    result.ServerFloats = serverFloats;
                }
                if (withPrivateCustomData)
                {
                    result.PrivateBools = privateBools;
                    result.PrivateInts = privateInts;
                    result.PrivateFloats = privateFloats;
                }
                if (withPublicCustomData)
                {
                    result.PublicBools = publicBools;
                    result.PublicInts = publicInts;
                    result.PublicFloats = publicFloats;
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
            }
            return result;
        }

        public override async UniTask<List<PlayerCharacterData>> ReadCharacters(string userId)
        {
            List<PlayerCharacterData> result = new List<PlayerCharacterData>();
            List<string> characterIds = new List<string>();
            await ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    characterIds.Add(reader.GetString(0));
                }
            }, "SELECT id FROM characters WHERE userId=@userId ORDER BY updateAt DESC", new NpgsqlParameter("@userId", userId));
            foreach (string characterId in characterIds)
            {
                result.Add(await ReadCharacter(characterId, true, false, false, false, false, true, false, false, false, false, false, false, false, true));
            }
            return result;
        }

        public override async UniTask UpdateCharacter(IPlayerCharacterData character, List<CharacterBuff> summonBuffs, List<CharacterItem> storageItems, bool deleteStorageReservation)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await ExecuteNonQuery(connection, transaction, @"INSERT INTO character_pk
                            (id, isPkOn, lastPkOnTime, pkPoint, consecutivePkKills, highestPkPoint, highestConsecutivePkKills) VALUES
                            (@id, @isPkOn, @lastPkOnTime, @pkPoint, @consecutivePkKills, @highestPkPoint, @highestConsecutivePkKills)
                            ON DUPLICATE KEY UPDATE
                            isPkOn = @isPkOn,
                            lastPkOnTime = @lastPkOnTime,
                            pkPoint = @pkPoint,
                            consecutivePkKills = @consecutivePkKills,
                            highestPkPoint = @highestPkPoint,
                            highestConsecutivePkKills = @highestConsecutivePkKills",
                    new NpgsqlParameter("@id", character.Id),
                    new NpgsqlParameter("@isPkOn", character.IsPkOn),
                    new NpgsqlParameter("@lastPkOnTime", character.LastPkOnTime),
                    new NpgsqlParameter("@pkPoint", character.PkPoint),
                    new NpgsqlParameter("@consecutivePkKills", character.ConsecutivePkKills),
                    new NpgsqlParameter("@highestPkPoint", character.HighestPkPoint),
                    new NpgsqlParameter("@highestConsecutivePkKills", character.HighestConsecutivePkKills));
                await ExecuteNonQuery(connection, transaction, @"UPDATE characters SET
                            dataId=@dataId,
                            entityId=@entityId,
                            factionId=@factionId,
                            characterName=@characterName,
                            level=@level,
                            exp=@exp,
                            currentHp=@currentHp,
                            currentMp=@currentMp,
                            currentStamina=@currentStamina,
                            currentFood=@currentFood,
                            currentWater=@currentWater,
                            equipWeaponSet=@equipWeaponSet,
                            statPoint=@statPoint,
                            skillPoint=@skillPoint,
                            gold=@gold,
                            currentMapName=@currentMapName,
                            currentPositionX=@currentPositionX,
                            currentPositionY=@currentPositionY,
                            currentPositionZ=@currentPositionZ,
                            currentRotationX=@currentRotationX,
                            currentRotationY=@currentRotationY,
                            currentRotationZ=@currentRotationZ,
                            respawnMapName=@respawnMapName,
                            respawnPositionX=@respawnPositionX,
                            respawnPositionY=@respawnPositionY,
                            respawnPositionZ=@respawnPositionZ,
                            mountDataId=@mountDataId,
                            iconDataId=@iconDataId,
                            frameDataId=@frameDataId,
                            titleDataId=@titleDataId,
                            lastDeadTime=@lastDeadTime,
                            unmuteTime=@unmuteTime
                            WHERE id=@id",
                    new NpgsqlParameter("@dataId", character.DataId),
                    new NpgsqlParameter("@entityId", character.EntityId),
                    new NpgsqlParameter("@factionId", character.FactionId),
                    new NpgsqlParameter("@characterName", character.CharacterName),
                    new NpgsqlParameter("@level", character.Level),
                    new NpgsqlParameter("@exp", character.Exp),
                    new NpgsqlParameter("@currentHp", character.CurrentHp),
                    new NpgsqlParameter("@currentMp", character.CurrentMp),
                    new NpgsqlParameter("@currentStamina", character.CurrentStamina),
                    new NpgsqlParameter("@currentFood", character.CurrentFood),
                    new NpgsqlParameter("@currentWater", character.CurrentWater),
                    new NpgsqlParameter("@equipWeaponSet", character.EquipWeaponSet),
                    new NpgsqlParameter("@statPoint", character.StatPoint),
                    new NpgsqlParameter("@skillPoint", character.SkillPoint),
                    new NpgsqlParameter("@gold", character.Gold),
                    new NpgsqlParameter("@currentMapName", character.CurrentMapName),
                    new NpgsqlParameter("@currentPositionX", character.CurrentPosition.x),
                    new NpgsqlParameter("@currentPositionY", character.CurrentPosition.y),
                    new NpgsqlParameter("@currentPositionZ", character.CurrentPosition.z),
                    new NpgsqlParameter("@currentRotationX", character.CurrentRotation.x),
                    new NpgsqlParameter("@currentRotationY", character.CurrentRotation.y),
                    new NpgsqlParameter("@currentRotationZ", character.CurrentRotation.z),
                    new NpgsqlParameter("@respawnMapName", character.RespawnMapName),
                    new NpgsqlParameter("@respawnPositionX", character.RespawnPosition.x),
                    new NpgsqlParameter("@respawnPositionY", character.RespawnPosition.y),
                    new NpgsqlParameter("@respawnPositionZ", character.RespawnPosition.z),
                    new NpgsqlParameter("@mountDataId", character.MountDataId),
                    new NpgsqlParameter("@iconDataId", character.IconDataId),
                    new NpgsqlParameter("@frameDataId", character.FrameDataId),
                    new NpgsqlParameter("@titleDataId", character.TitleDataId),
                    new NpgsqlParameter("@lastDeadTime", character.LastDeadTime),
                    new NpgsqlParameter("@unmuteTime", character.UnmuteTime),
                    new NpgsqlParameter("@id", character.Id));
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

                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_server_boolean", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_server_int32", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_server_float32", id);

                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_private_boolean", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_private_int32", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_private_float32", id);

                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_public_boolean", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_public_int32", id);
                await PostgreSQLHelpers.ExecuteDeleteById(connection, transaction, "character_public_float32", id);

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
                connection, null,
                "characters",
                PostgreSQLHelpers.WhereLike("character_name", characterName));
        }

        public const string CACHE_KEY_GET_ID_BY_CHARACTER_NAME = "GET_ID_BY_CHARACTER_NAME";
        public override async UniTask<string> GetIdByCharacterName(string characterName)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            object result = PostgreSQLHelpers.ExecuteSelectScalar(
                CACHE_KEY_GET_ID_BY_CHARACTER_NAME,
                connection, null,
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
                connection, null,
                "characters", "user_id", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("character_name", characterName));
            return result != null ? (string)result : string.Empty;
        }

        public const string CACHE_KEY_FIND_CHARACTERS_SELECT_FRIENDS = "FIND_CHARACTERS_SELECT_FRIENDS";
        public override async UniTask<List<SocialCharacterData>> FindCharacters(string finderId, string characterName, int skip, int limit)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            // Exclude friend, requested characters
            using var readerIds = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_FIND_CHARACTERS_SELECT_FRIENDS,
                connection, null,
                "friends", "character_id_2",
                PostgreSQLHelpers.WhereEqualTo("character_id_1", finderId));
            string excludeIdsQuery = $"(id != '{finderId}'";
            while (readerIds.Read())
            {
                excludeIdsQuery += $" AND id != '{readerIds.GetString(0)}'";
            }
            excludeIdsQuery += ")";
            // Read some character data
            using var readerCharacters = await PostgreSQLHelpers.ExecuteSelect(
                null,
                connection, null,
                "characters", "id, data_id, character_name, level", $" AND {excludeIdsQuery} ORDER BY RAND() LIMIT {skip}, {limit}",
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
                using var readerIds = await PostgreSQLHelpers.ExecuteSelect(
                    CACHE_KEY_READ_FRIENDS_ID_1,
                    connection, null,
                    "friends", "character_id_1", $"LIMIT {skip}, {limit}",
                    PostgreSQLHelpers.WhereEqualTo("character_id_2", id),
                    PostgreSQLHelpers.AndWhereSmallEqualTo("state", state));
                while (readerIds.Read())
                {
                    characterIds.Add(readerIds.GetString(0));
                }
            }
            else
            {
                using var readerIds = await PostgreSQLHelpers.ExecuteSelect(
                    CACHE_KEY_READ_FRIENDS_ID_2,
                    connection, null,
                    "friends", "character_id_2", $"LIMIT {skip}, {limit}",
                    PostgreSQLHelpers.WhereEqualTo("character_id_1", id),
                    PostgreSQLHelpers.AndWhereSmallEqualTo("state", state));
                while (readerIds.Read())
                {
                    characterIds.Add(readerIds.GetString(0));
                }
            }
            return await GetSocialCharacterByIds(connection, null, characterIds);
        }

        public const string CACHE_KEY_GET_FRIEND_REQUESTS_NOTIFICATION = "GET_FRIEND_REQUESTS_NOTIFICATION";
        public override async UniTask<int> GetFriendRequestNotification(string characterId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            return (int)await PostgreSQLHelpers.ExecuteCount(
                CACHE_KEY_GET_FRIEND_REQUESTS_NOTIFICATION,
                connection, null,
                "friends",
                PostgreSQLHelpers.WhereEqualTo("character_id_2", characterId),
                PostgreSQLHelpers.AndWhereSmallEqualTo("state", 1));
        }

        public async UniTask<List<SocialCharacterData>> GetSocialCharacterByIds(NpgsqlConnection connection, NpgsqlTransaction transaction, IList<string> characterIds, string select = "id, data_id, character_name, level")
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
                    connection, transaction,
                    "characters",
                    characterQueries,
                    select, "LIMIT 1");
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