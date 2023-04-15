using ConcurrentCollections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public class LocalDatabaseCache : IDatabaseCache
    {
        private ConcurrentHashSet<string> _cachedUsernames = new ConcurrentHashSet<string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentHashSet<string> _cachedEmails = new ConcurrentHashSet<string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentHashSet<string> _cachedCharacterNames = new ConcurrentHashSet<string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentHashSet<string> _cachedGuildNames = new ConcurrentHashSet<string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentDictionary<string, string> _cachedUserAccessTokens = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, int> _cachedUserGolds = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> _cachedUserCashes = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, PlayerCharacterData> _cachedPlayerCharacters = new ConcurrentDictionary<string, PlayerCharacterData>();
        private ConcurrentDictionary<string, SocialCharacterData> _cachedSocialCharacters = new ConcurrentDictionary<string, SocialCharacterData>();
        private ConcurrentDictionary<string, ConcurrentDictionary<string, BuildingSaveData>> _cachedBuilding = new ConcurrentDictionary<string, ConcurrentDictionary<string, BuildingSaveData>>();
        private ConcurrentDictionary<int, PartyData> _cachedParties = new ConcurrentDictionary<int, PartyData>();
        private ConcurrentDictionary<int, GuildData> _cachedGuilds = new ConcurrentDictionary<int, GuildData>();
        private ConcurrentDictionary<StorageId, List<CharacterItem>> _cachedStorageItems = new ConcurrentDictionary<StorageId, List<CharacterItem>>();
        private ConcurrentDictionary<StorageId, long> _updatingStorages = new ConcurrentDictionary<StorageId, long>();

        public async UniTask<bool> AddUsername(string username)
        {
            await UniTask.Yield();
            return _cachedUsernames.Add(username);
        }
        public async UniTask<bool> ContainsUsername(string username)
        {
            await UniTask.Yield();
            return _cachedUsernames.Contains(username);
        }
        public async UniTask<bool> RemoveUsername(string username)
        {
            await UniTask.Yield();
            return _cachedUsernames.TryRemove(username);
        }

        public async UniTask<bool> AddEmail(string email)
        {
            await UniTask.Yield();
            return _cachedEmails.Add(email);
        }
        public async UniTask<bool> ContainsEmail(string email)
        {
            await UniTask.Yield();
            return _cachedEmails.Contains(email);
        }
        public async UniTask<bool> RemoveEmail(string email)
        {
            await UniTask.Yield();
            return _cachedEmails.TryRemove(email);
        }

        public async UniTask<bool> AddCharacterName(string characterName)
        {
            await UniTask.Yield();
            return _cachedCharacterNames.Add(characterName);
        }
        public async UniTask<bool> ContainsCharacterName(string characterName)
        {
            await UniTask.Yield();
            return _cachedCharacterNames.Contains(characterName);
        }
        public async UniTask<bool> RemoveCharacterName(string characterName)
        {
            await UniTask.Yield();
            return _cachedCharacterNames.TryRemove(characterName);
        }

        public async UniTask<bool> AddGuildName(string guildName)
        {
            await UniTask.Yield();
            return _cachedGuildNames.Add(guildName);
        }
        public async UniTask<bool> ContainsGuildName(string guildName)
        {
            await UniTask.Yield();
            return _cachedGuildNames.Contains(guildName);
        }
        public async UniTask<bool> RemoveGuildName(string guildName)
        {
            await UniTask.Yield();
            return _cachedGuildNames.TryRemove(guildName);
        }

        public async UniTask<bool> SetUserAccessToken(string userId, string accessToken)
        {
            await UniTask.Yield();
            _cachedUserAccessTokens[userId] = accessToken;
            return true;
        }
        public async UniTask<DatabaseCacheResult<string>> GetUserAccessToken(string userId)
        {
            await UniTask.Yield();
            if (_cachedUserAccessTokens.TryGetValue(userId, out var token))
                return new DatabaseCacheResult<string>(token);
            return default;
        }
        public async UniTask<bool> RemoveUserAccessToken(string userId)
        {
            await UniTask.Yield();
            return _cachedUserAccessTokens.TryRemove(userId, out _);
        }

        public async UniTask<bool> SetUserGold(string userId, int gold)
        {
            await UniTask.Yield();
            _cachedUserGolds[userId] = gold;
            return true;
        }
        public async UniTask<DatabaseCacheResult<int>> GetUserGold(string userId)
        {
            await UniTask.Yield();
            if (_cachedUserGolds.TryGetValue(userId, out var gold))
                return new DatabaseCacheResult<int>(gold);
            return default;
        }
        public async UniTask<bool> RemoveUserGold(string userId)
        {
            await UniTask.Yield();
            return _cachedUserGolds.TryRemove(userId, out _);
        }

        public async UniTask<bool> SetUserCash(string userId, int cash)
        {
            await UniTask.Yield();
            _cachedUserCashes[userId] = cash;
            return true;
        }
        public async UniTask<DatabaseCacheResult<int>> GetUserCash(string userId)
        {
            await UniTask.Yield();
            if (_cachedUserCashes.TryGetValue(userId, out var cash))
                return new DatabaseCacheResult<int>(cash);
            return default;
        }
        public async UniTask<bool> RemoveUserCash(string userId)
        {
            await UniTask.Yield();
            return _cachedUserCashes.TryRemove(userId, out _);
        }

        public async UniTask<bool> SetPlayerCharacter(PlayerCharacterData playerCharacter)
        {
            await UniTask.Yield();
            _cachedPlayerCharacters[playerCharacter.Id] = playerCharacter;
            return true;
        }
        public async UniTask<DatabaseCacheResult<PlayerCharacterData>> GetPlayerCharacter(string characterId)
        {
            await UniTask.Yield();
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
                return new DatabaseCacheResult<PlayerCharacterData>(playerCharacter);
            return default;
        }
        public async UniTask<bool> RemovePlayerCharacter(string characterId)
        {
            await UniTask.Yield();
            return _cachedPlayerCharacters.TryRemove(characterId, out _);
        }
        public async UniTask<bool> SetPlayerCharacterPartyId(string characterId, int partyId)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.PartyId = partyId;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetPlayerCharacterGuildId(string characterId, int guildId)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.GuildId = guildId;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetPlayerCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.GuildId = guildId;
                playerCharacter.GuildRole = guildRole;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }

        public async UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter)
        {
            await UniTask.Yield();
            _cachedSocialCharacters[playerCharacter.id] = playerCharacter;
            return true;
        }
        public async UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId)
        {
            await UniTask.Yield();
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
                return new DatabaseCacheResult<SocialCharacterData>(playerCharacter);
            return default;
        }
        public async UniTask<bool> RemoveSocialCharacter(string characterId)
        {
            await UniTask.Yield();
            return _cachedSocialCharacters.TryRemove(characterId, out _);
        }
        public async UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId)
        {
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.partyId = partyId;
                return await SetSocialCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId)
        {
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.guildId = guildId;
                return await SetSocialCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.guildId = guildId;
                playerCharacter.guildRole = guildRole;
                return await SetSocialCharacter(playerCharacter);
            }
            return false;
        }

        public async UniTask<bool> SetBuilding(string mapName, BuildingSaveData building)
        {
            await UniTask.Yield();
            if (!_cachedBuilding.ContainsKey(mapName))
                _cachedBuilding[mapName] = new ConcurrentDictionary<string, BuildingSaveData>();
            _cachedBuilding[mapName][building.Id] = building;
            return true;
        }
        public async UniTask<DatabaseCacheResult<BuildingSaveData>> GetBuilding(string mapName, string buildingId)
        {
            await UniTask.Yield();
            if (_cachedBuilding.TryGetValue(mapName, out var buildings) && buildings.TryGetValue(buildingId, out var building))
                return new DatabaseCacheResult<BuildingSaveData>(building);
            return default;
        }
        public async UniTask<bool> RemoveBuilding(string mapName, string buildingId)
        {
            await UniTask.Yield();
            return _cachedBuilding.TryGetValue(mapName, out var buildings) && buildings.TryRemove(buildingId, out _);
        }

        public async UniTask<bool> SetBuildings(string mapName, IEnumerable<BuildingSaveData> buildings)
        {
            await UniTask.Yield();
            if (!_cachedBuilding.ContainsKey(mapName))
                _cachedBuilding[mapName] = new ConcurrentDictionary<string, BuildingSaveData>();
            foreach (BuildingSaveData building in buildings)
                _cachedBuilding[mapName][building.Id] = building;
            return true;
        }
        public async UniTask<DatabaseCacheResult<IEnumerable<BuildingSaveData>>> GetBuildings(string mapName)
        {
            await UniTask.Yield();
            if (_cachedBuilding.TryGetValue(mapName, out var buildings))
                return new DatabaseCacheResult<IEnumerable<BuildingSaveData>>(buildings.Values);
            return default;
        }
        public async UniTask<bool> RemoveBuildings(string mapName)
        {
            await UniTask.Yield();
            return _cachedBuilding.TryRemove(mapName, out _);
        }

        public async UniTask<bool> SetParty(PartyData party)
        {
            await UniTask.Yield();
            _cachedParties[party.id] = party;
            return true;
        }
        public async UniTask<DatabaseCacheResult<PartyData>> GetParty(int id)
        {
            await UniTask.Yield();
            if (_cachedParties.TryGetValue(id, out var party))
                return new DatabaseCacheResult<PartyData>(party);
            return default;
        }
        public async UniTask<bool> RemoveParty(int id)
        {
            await UniTask.Yield();
            return _cachedParties.TryRemove(id, out _);
        }

        public async UniTask<bool> SetGuild(GuildData guild)
        {
            await UniTask.Yield();
            _cachedGuilds[guild.id] = guild;
            return true;
        }
        public async UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id)
        {
            await UniTask.Yield();
            if (_cachedGuilds.TryGetValue(id, out var guild))
                return new DatabaseCacheResult<GuildData>(guild);
            return default;
        }
        public async UniTask<bool> RemoveGuild(int id)
        {
            await UniTask.Yield();
            return _cachedGuilds.TryRemove(id, out _);
        }

        public async UniTask<bool> SetStorageItems(StorageId storageId, List<CharacterItem> items)
        {
            await UniTask.Yield();
            _cachedStorageItems[storageId] = items;
            return true;
        }
        public async UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageId storageId)
        {
            await UniTask.Yield();
            if (_cachedStorageItems.TryGetValue(storageId, out var items))
                return new DatabaseCacheResult<List<CharacterItem>>(items);
            return default;
        }
        public async UniTask<bool> RemoveStorageItems(StorageId storageId)
        {
            await UniTask.Yield();
            return _cachedStorageItems.TryRemove(storageId, out _);
        }

        public async UniTask<bool> SetUpdatingStorage(StorageId storageId, long time)
        {
            await UniTask.Yield();
            _updatingStorages[storageId] = time;
            return true;
        }
        public async UniTask<DatabaseCacheResult<long>> GetUpdatingStorage(StorageId storageId)
        {
            await UniTask.Yield();
            if (_updatingStorages.TryGetValue(storageId, out var time))
                return new DatabaseCacheResult<long>(time);
            return default;
        }
        public async UniTask<bool> RemoveUpdatingStorage(StorageId storageId)
        {
            await UniTask.Yield();
            return _updatingStorages.TryRemove(storageId, out _);
        }
    }
}
