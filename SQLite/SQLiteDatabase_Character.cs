#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        private void FillCharacterAttributes(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterAttributes(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Attributes.Count; ++i)
                {
                    CreateCharacterAttribute(transaction, insertedIds, i, characterData.Id, characterData.Attributes[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing attributes of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterBuffs(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterBuffs(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Buffs.Count; ++i)
                {
                    CreateCharacterBuff(transaction, insertedIds, characterData.Id, characterData.Buffs[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing buffs of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterHotkeys(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterHotkeys(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Hotkeys.Count; ++i)
                {
                    CreateCharacterHotkey(transaction, insertedIds, characterData.Id, characterData.Hotkeys[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing hotkeys of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterItems(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterItems(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.SelectableWeaponSets.Count; ++i)
                {
                    CreateCharacterEquipWeapons(transaction, insertedIds, i, characterData.Id, characterData.SelectableWeaponSets[i]);
                }
                for (i = 0; i < characterData.EquipItems.Count; ++i)
                {
                    CreateCharacterEquipItem(transaction, insertedIds, i, characterData.Id, characterData.EquipItems[i]);
                }
                for (i = 0; i < characterData.NonEquipItems.Count; ++i)
                {
                    CreateCharacterNonEquipItem(transaction, insertedIds, i, characterData.Id, characterData.NonEquipItems[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing items of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterQuests(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterQuests(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Quests.Count; ++i)
                {
                    CreateCharacterQuest(transaction, insertedIds, i, characterData.Id, characterData.Quests[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing quests of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterCurrencies(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterCurrencies(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Currencies.Count; ++i)
                {
                    CreateCharacterCurrency(transaction, insertedIds, i, characterData.Id, characterData.Currencies[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing currencies of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterSkills(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterSkills(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Skills.Count; ++i)
                {
                    CreateCharacterSkill(transaction, insertedIds, i, characterData.Id, characterData.Skills[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skills of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterSkillUsages(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterSkillUsages(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.SkillUsages.Count; ++i)
                {
                    CreateCharacterSkillUsage(transaction, insertedIds, characterData.Id, characterData.SkillUsages[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skill usages of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterSummons(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterSummons(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Summons.Count; ++i)
                {
                    CreateCharacterSummon(transaction, insertedIds, i, characterData.Id, characterData.Summons[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skill usages of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterDataBooleans(SqliteTransaction transaction, string tableName, string characterId, IList<CharacterDataBoolean> list)
        {
            try
            {
                DeleteCharacterDataBooleans(transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    CreateCharacterDataBoolean(transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom boolean of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterDataInt32s(SqliteTransaction transaction, string tableName, string characterId, IList<CharacterDataInt32> list)
        {
            try
            {
                DeleteCharacterDataInt32s(transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    CreateCharacterDataInt32(transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom int32 of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterDataFloat32s(SqliteTransaction transaction, string tableName, string characterId, IList<CharacterDataFloat32> list)
        {
            try
            {
                DeleteCharacterDataFloat32s(transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    CreateCharacterDataFloat32(transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom float32 of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterRelatesData(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            FillCharacterItems(transaction, characterData);
            FillCharacterQuests(transaction, characterData);
            FillCharacterSkills(transaction, characterData);
            FillCharacterSkillUsages(transaction, characterData);
            FillCharacterSummons(transaction, characterData);

            FillCharacterDataBooleans(transaction, "character_server_boolean", characterData.Id, characterData.ServerBools);
            FillCharacterDataInt32s(transaction, "character_server_int32", characterData.Id, characterData.ServerInts);
            FillCharacterDataFloat32s(transaction, "character_server_float32", characterData.Id, characterData.ServerFloats);

            FillCharacterDataBooleans(transaction, "character_private_boolean", characterData.Id, characterData.PrivateBools);
            FillCharacterDataInt32s(transaction, "character_private_int32", characterData.Id, characterData.PrivateInts);
            FillCharacterDataFloat32s(transaction, "character_private_float32", characterData.Id, characterData.PrivateFloats);

            FillCharacterDataBooleans(transaction, "character_public_boolean", characterData.Id, characterData.PublicBools);
            FillCharacterDataInt32s(transaction, "character_public_int32", characterData.Id, characterData.PublicInts);
            FillCharacterDataFloat32s(transaction, "character_public_float32", characterData.Id, characterData.PublicFloats);
        }

        public override void CreateCharacter(string userId, IPlayerCharacterData character)
        {
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                ExecuteNonQuery("INSERT INTO characters " +
                    "(id, userId, dataId, entityId, factionId, characterName, level, exp, currentHp, currentMp, currentStamina, currentFood, currentWater, equipWeaponSet, statPoint, skillPoint, gold, currentMapName, currentPositionX, currentPositionY, currentPositionZ, currentRotationX, currentRotationY, currentRotationZ, respawnMapName, respawnPositionX, respawnPositionY, respawnPositionZ, mountDataId, iconDataId, frameDataId, titleDataId) VALUES " +
                    "(@id, @userId, @dataId, @entityId, @factionId, @characterName, @level, @exp, @currentHp, @currentMp, @currentStamina, @currentFood, @currentWater, @equipWeaponSet, @statPoint, @skillPoint, @gold, @currentMapName, @currentPositionX, @currentPositionY, @currentPositionZ, @currentRotationX, @currentRotationY, @currentRotationZ, @respawnMapName, @respawnPositionX, @respawnPositionY, @respawnPositionZ, @mountDataId, @iconDataId, @frameDataId, @titleDataId)",
                    new SqliteParameter("@id", character.Id),
                    new SqliteParameter("@userId", userId),
                    new SqliteParameter("@dataId", character.DataId),
                    new SqliteParameter("@entityId", character.EntityId),
                    new SqliteParameter("@factionId", character.FactionId),
                    new SqliteParameter("@characterName", character.CharacterName),
                    new SqliteParameter("@level", character.Level),
                    new SqliteParameter("@exp", character.Exp),
                    new SqliteParameter("@currentHp", character.CurrentHp),
                    new SqliteParameter("@currentMp", character.CurrentMp),
                    new SqliteParameter("@currentStamina", character.CurrentStamina),
                    new SqliteParameter("@currentFood", character.CurrentFood),
                    new SqliteParameter("@currentWater", character.CurrentWater),
                    new SqliteParameter("@equipWeaponSet", character.EquipWeaponSet),
                    new SqliteParameter("@statPoint", character.StatPoint),
                    new SqliteParameter("@skillPoint", character.SkillPoint),
                    new SqliteParameter("@gold", character.Gold),
                    new SqliteParameter("@currentMapName", character.CurrentMapName),
                    new SqliteParameter("@currentPositionX", character.CurrentPosition.x),
                    new SqliteParameter("@currentPositionY", character.CurrentPosition.y),
                    new SqliteParameter("@currentPositionZ", character.CurrentPosition.z),
                    new SqliteParameter("@currentRotationX", character.CurrentRotation.x),
                    new SqliteParameter("@currentRotationY", character.CurrentRotation.y),
                    new SqliteParameter("@currentRotationZ", character.CurrentRotation.z),
                    new SqliteParameter("@respawnMapName", character.RespawnMapName),
                    new SqliteParameter("@respawnPositionX", character.RespawnPosition.x),
                    new SqliteParameter("@respawnPositionY", character.RespawnPosition.y),
                    new SqliteParameter("@respawnPositionZ", character.RespawnPosition.z),
                    new SqliteParameter("@mountDataId", character.MountDataId),
                    new SqliteParameter("@iconDataId", character.IconDataId),
                    new SqliteParameter("@frameDataId", character.FrameDataId),
                    new SqliteParameter("@titleDataId", character.TitleDataId));
                FillCharacterRelatesData(transaction, character);
                transaction.Commit();
                this.InvokeInstanceDevExtMethods("CreateCharacter", userId, character);
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while create character: " + character.Id);
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            transaction.Dispose();
        }

        private bool ReadCharacter(SqliteDataReader reader, out PlayerCharacterData result)
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
                return true;
            }
            result = null;
            return false;
        }

        public override PlayerCharacterData ReadCharacter(
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
            ExecuteReader((reader) =>
            {
                ReadCharacter(reader, out result);
            }, "SELECT " +
                "id, userId, dataId, entityId, factionId, characterName, level, exp, " +
                "currentHp, currentMp, currentStamina, currentFood, currentWater, " +
                "equipWeaponSet, statPoint, skillPoint, gold, partyId, guildId, guildRole, sharedGuildExp, " +
                "currentMapName, currentPositionX, currentPositionY, currentPositionZ, currentRotationX, currentRotationY, currentRotationZ," +
                "respawnMapName, respawnPositionX, respawnPositionY, respawnPositionZ," +
                "mountDataId, iconDataId, frameDataId, titleDataId, lastDeadTime, unmuteTime, updateAt FROM characters WHERE id=@id LIMIT 1",
                new SqliteParameter("@id", id));
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
                if (withEquipWeapons)
                    ReadCharacterEquipWeapons(id, selectableWeaponSets);
                if (withAttributes)
                    ReadCharacterAttributes(id, attributes);
                if (withSkills)
                    ReadCharacterSkills(id, skills);
                if (withSkillUsages)
                    ReadCharacterSkillUsages(id, skillUsages);
                if (withBuffs)
                    ReadCharacterBuffs(id, buffs);
                if (withEquipItems)
                    ReadCharacterEquipItems(id, equipItems);
                if (withNonEquipItems)
                    ReadCharacterNonEquipItems(id, nonEquipItems);
                if (withSummons)
                    ReadCharacterSummons(id, summons);
                if (withHotkeys)
                    ReadCharacterHotkeys(id, hotkeys);
                if (withQuests)
                    ReadCharacterQuests(id, quests);
                if (withCurrencies)
                    ReadCharacterCurrencies(id, currencies);
                if (withServerCustomData)
                {
                    ReadCharacterDataBooleans("character_server_boolean", id, serverBools);
                    ReadCharacterDataInt32s("character_server_int32", id, serverInts);
                    ReadCharacterDataFloat32s("character_server_float32", id, serverFloats);
                }
                if (withPrivateCustomData)
                {
                    ReadCharacterDataBooleans("character_private_boolean", id, privateBools);
                    ReadCharacterDataInt32s("character_private_int32", id, privateInts);
                    ReadCharacterDataFloat32s("character_private_float32", id, privateFloats);
                }
                if (withPublicCustomData)
                {
                    ReadCharacterDataBooleans("character_public_boolean", id, publicBools);
                    ReadCharacterDataInt32s("character_public_int32", id, publicInts);
                    ReadCharacterDataFloat32s("character_public_float32", id, publicFloats);
                }
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

        public override List<PlayerCharacterData> ReadCharacters(string userId)
        {
            List<PlayerCharacterData> result = new List<PlayerCharacterData>();
            List<string> characterIds = new List<string>();
            ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    characterIds.Add(reader.GetString(0));
                }
            }, "SELECT id FROM characters WHERE userId=@userId ORDER BY updateAt DESC", new SqliteParameter("@userId", userId));
            foreach (string characterId in characterIds)
            {
                result.Add(ReadCharacter(characterId, true, true, true, false, false, true, false, false, false, false, false, false, false, true));
            }
            return result;
        }

        public override void UpdateCharacter(IPlayerCharacterData character)
        {
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                ExecuteNonQuery("UPDATE characters SET " +
                    "dataId=@dataId, " +
                    "entityId=@entityId, " +
                    "factionId=@factionId, " +
                    "characterName=@characterName, " +
                    "level=@level, " +
                    "exp=@exp, " +
                    "currentHp=@currentHp, " +
                    "currentMp=@currentMp, " +
                    "currentStamina=@currentStamina, " +
                    "currentFood=@currentFood, " +
                    "currentWater=@currentWater, " +
                    "equipWeaponSet=@equipWeaponSet, " +
                    "statPoint=@statPoint, " +
                    "skillPoint=@skillPoint, " +
                    "gold=@gold, " +
                    "currentMapName=@currentMapName, " +
                    "currentPositionX=@currentPositionX, " +
                    "currentPositionY=@currentPositionY, " +
                    "currentPositionZ=@currentPositionZ, " +
                    "currentRotationX=@currentRotationX, " +
                    "currentRotationY=@currentRotationY, " +
                    "currentRotationZ=@currentRotationZ, " +
                    "respawnMapName=@respawnMapName, " +
                    "respawnPositionX=@respawnPositionX, " +
                    "respawnPositionY=@respawnPositionY, " +
                    "respawnPositionZ=@respawnPositionZ, " +
                    "mountDataId=@mountDataId, " +
                    "iconDataId=@iconDataId, " +
                    "frameDataId=@frameDataId, " +
                    "titleDataId=@titleDataId, " +
                    "lastDeadTime=@lastDeadTime, " +
                    "unmuteTime=@unmuteTime " +
                    "WHERE id=@id",
                    new SqliteParameter("@dataId", character.DataId),
                    new SqliteParameter("@entityId", character.EntityId),
                    new SqliteParameter("@factionId", character.FactionId),
                    new SqliteParameter("@characterName", character.CharacterName),
                    new SqliteParameter("@level", character.Level),
                    new SqliteParameter("@exp", character.Exp),
                    new SqliteParameter("@currentHp", character.CurrentHp),
                    new SqliteParameter("@currentMp", character.CurrentMp),
                    new SqliteParameter("@currentStamina", character.CurrentStamina),
                    new SqliteParameter("@currentFood", character.CurrentFood),
                    new SqliteParameter("@currentWater", character.CurrentWater),
                    new SqliteParameter("@equipWeaponSet", character.EquipWeaponSet),
                    new SqliteParameter("@statPoint", character.StatPoint),
                    new SqliteParameter("@skillPoint", character.SkillPoint),
                    new SqliteParameter("@gold", character.Gold),
                    new SqliteParameter("@currentMapName", character.CurrentMapName),
                    new SqliteParameter("@currentPositionX", character.CurrentPosition.x),
                    new SqliteParameter("@currentPositionY", character.CurrentPosition.y),
                    new SqliteParameter("@currentPositionZ", character.CurrentPosition.z),
                    new SqliteParameter("@currentRotationX", character.CurrentRotation.x),
                    new SqliteParameter("@currentRotationY", character.CurrentRotation.y),
                    new SqliteParameter("@currentRotationZ", character.CurrentRotation.z),
                    new SqliteParameter("@respawnMapName", character.RespawnMapName),
                    new SqliteParameter("@respawnPositionX", character.RespawnPosition.x),
                    new SqliteParameter("@respawnPositionY", character.RespawnPosition.y),
                    new SqliteParameter("@respawnPositionZ", character.RespawnPosition.z),
                    new SqliteParameter("@mountDataId", character.MountDataId),
                    new SqliteParameter("@iconDataId", character.IconDataId),
                    new SqliteParameter("@frameDataId", character.FrameDataId),
                    new SqliteParameter("@titleDataId", character.TitleDataId),
                    new SqliteParameter("@lastDeadTime", character.LastDeadTime),
                    new SqliteParameter("@unmuteTime", character.UnmuteTime),
                    new SqliteParameter("@id", character.Id));
                FillCharacterRelatesData(transaction, character);
                transaction.Commit();
                this.InvokeInstanceDevExtMethods("UpdateCharacter", character);
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while update character: " + character.Id);
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            transaction.Dispose();
        }

        public override void DeleteCharacter(string userId, string id)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM characters WHERE id=@id AND userId=@userId",
                new SqliteParameter("@id", id),
                new SqliteParameter("@userId", userId));
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                SqliteTransaction transaction = _connection.BeginTransaction();
                try
                {
                    ExecuteNonQuery(transaction, "DELETE FROM characters WHERE id=@characterId", new SqliteParameter("@characterId", id));
                    ExecuteNonQuery(transaction, "DELETE FROM friend WHERE characterId1 LIKE @characterId OR characterId2 LIKE @characterId", new SqliteParameter("@characterId", id));
                    DeleteCharacterAttributes(transaction, id);
                    DeleteCharacterCurrencies(transaction, id);
                    DeleteCharacterBuffs(transaction, id);
                    DeleteCharacterHotkeys(transaction, id);
                    DeleteCharacterItems(transaction, id);
                    DeleteCharacterQuests(transaction, id);
                    DeleteCharacterSkills(transaction, id);
                    DeleteCharacterSkillUsages(transaction, id);
                    DeleteCharacterSummons(transaction, id);

                    DeleteCharacterDataBooleans(transaction, "character_server_boolean", id);
                    DeleteCharacterDataInt32s(transaction, "character_server_int32", id);
                    DeleteCharacterDataFloat32s(transaction, "character_server_float32", id);

                    DeleteCharacterDataBooleans(transaction, "character_private_boolean", id);
                    DeleteCharacterDataInt32s(transaction, "character_private_int32", id);
                    DeleteCharacterDataFloat32s(transaction, "character_private_float32", id);

                    DeleteCharacterDataBooleans(transaction, "character_public_boolean", id);
                    DeleteCharacterDataInt32s(transaction, "character_public_int32", id);
                    DeleteCharacterDataFloat32s(transaction, "character_public_float32", id);
                    transaction.Commit();
                }
                catch (System.Exception ex)
                {
                    LogError(LogTag, "Transaction, Error occurs while deleting character: " + id);
                    LogException(LogTag, ex);
                    transaction.Rollback();
                }
                transaction.Dispose();
                this.InvokeInstanceDevExtMethods("DeleteCharacter", userId, id);
            }
        }

        public override long FindCharacterName(string characterName)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM characters WHERE characterName LIKE @characterName",
                new SqliteParameter("@characterName", characterName));
            return result != null ? (long)result : 0;
        }

        public override string GetIdByCharacterName(string characterName)
        {
            object result = ExecuteScalar("SELECT id FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new SqliteParameter("@characterName", characterName));
            return result != null ? (string)result : string.Empty;
        }

        public override string GetUserIdByCharacterName(string characterName)
        {
            object result = ExecuteScalar("SELECT userId FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new SqliteParameter("@characterName", characterName));
            return result != null ? (string)result : string.Empty;
        }

        public override List<SocialCharacterData> FindCharacters(string finderId, string characterName, int skip, int limit)
        {
            string excludeIdsQuery = "(id!='" + finderId + "'";
            // Exclude friend, requested characters
            ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    excludeIdsQuery += " AND id!='" + reader.GetString(0) + "'";
                }
            }, "SELECT characterId2 FROM friend WHERE characterId1='" + finderId + "'");
            excludeIdsQuery += ")";
            List<SocialCharacterData> result = new List<SocialCharacterData>();
            ExecuteReader((reader) =>
            {
                SocialCharacterData socialCharacterData;
                while (reader.Read())
                {
                    // Get some required data, other data will be set at server side
                    socialCharacterData = new SocialCharacterData();
                    socialCharacterData.id = reader.GetString(0);
                    socialCharacterData.dataId = reader.GetInt32(1);
                    socialCharacterData.characterName = reader.GetString(2);
                    socialCharacterData.level = reader.GetInt32(3);
                    result.Add(socialCharacterData);
                }
            }, "SELECT id, dataId, characterName, level FROM characters WHERE characterName LIKE @characterName AND " + excludeIdsQuery + " LIMIT " + skip + ", " + limit,
                new SqliteParameter("@characterName", "%" + characterName + "%"));
            return result;
        }

        public override void CreateFriend(string id1, string id2, byte state)
        {
            DeleteFriend(id1, id2);
            ExecuteNonQuery("INSERT INTO friend " +
                "(characterId1, characterId2, state) VALUES " +
                "(@characterId1, @characterId2, @state)",
                new SqliteParameter("@characterId1", id1),
                new SqliteParameter("@characterId2", id2),
                new SqliteParameter("@state", (int)state));
        }

        public override void DeleteFriend(string id1, string id2)
        {
            ExecuteNonQuery("DELETE FROM friend WHERE " +
                "characterId1 LIKE @characterId1 AND " +
                "characterId2 LIKE @characterId2",
                new SqliteParameter("@characterId1", id1),
                new SqliteParameter("@characterId2", id2));
        }

        public override List<SocialCharacterData> ReadFriends(string id, bool readById2, byte state, int skip, int limit)
        {
            List<SocialCharacterData> result = new List<SocialCharacterData>();
            List<string> characterIds = new List<string>();
            if (readById2)
            {
                ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        characterIds.Add(reader.GetString(0));
                    }
                }, "SELECT characterId1 FROM friend WHERE characterId2=@id AND state=" + state + " LIMIT " + skip + ", " + limit,
                    new SqliteParameter("@id", id));
            }
            else
            {
                ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        characterIds.Add(reader.GetString(0));
                    }
                }, "SELECT characterId2 FROM friend WHERE characterId1=@id AND state=" + state + " LIMIT " + skip + ", " + limit,
                    new SqliteParameter("@id", id));
            }
            SocialCharacterData socialCharacterData;
            foreach (string characterId in characterIds)
            {
                ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        // Get some required data, other data will be set at server side
                        socialCharacterData = new SocialCharacterData();
                        socialCharacterData.id = reader.GetString(0);
                        socialCharacterData.dataId = reader.GetInt32(1);
                        socialCharacterData.characterName = reader.GetString(2);
                        socialCharacterData.level = reader.GetInt32(3);
                        result.Add(socialCharacterData);
                    }
                }, "SELECT id, dataId, characterName, level FROM characters WHERE id LIKE @id",
                    new SqliteParameter("@id", characterId));
            }
            return result;
        }

        public override int GetFriendRequestNotification(string characterId)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM friend WHERE characterId2=@characterId AND state=1",
                new SqliteParameter("@characterId", characterId));
            return (int)(long)result;
        }
    }
}
#endif