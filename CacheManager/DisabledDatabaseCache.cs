using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public class DisabledDatabaseCache : IDatabaseCache
    {
        public UniTask<bool> AddUsername(string username)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> ContainsUsername(string username)
        {
            return UniTask.FromResult(false);
        }
        public UniTask<bool> RemoveUsername(string username)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> AddEmail(string email)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> ContainsEmail(string email)
        {
            return UniTask.FromResult(false);
        }
        public UniTask<bool> RemoveEmail(string email)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> AddCharacterName(string characterName)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> ContainsCharacterName(string characterName)
        {
            return UniTask.FromResult(false);
        }
        public UniTask<bool> RemoveCharacterName(string characterName)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> AddGuildName(string guildName)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> ContainsGuildName(string guildName)
        {
            return UniTask.FromResult(false);
        }
        public UniTask<bool> RemoveGuildName(string guildName)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetUserAccessToken(string userId, string accessToken)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<string>> GetUserAccessToken(string userId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<string>());
        }
        public UniTask<bool> RemoveUserAccessToken(string userId)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetUserGold(string userId, int gold)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<int>> GetUserGold(string userId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<int>());
        }
        public UniTask<bool> RemoveUserGold(string userId)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetUserCash(string userId, int cash)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<int>> GetUserCash(string userId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<int>());
        }
        public UniTask<bool> RemoveUserCash(string userId)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetPlayerCharacter(PlayerCharacterData playerCharacter)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<PlayerCharacterData>> GetPlayerCharacter(string characterId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<PlayerCharacterData>());
        }
        public UniTask<bool> RemovePlayerCharacter(string characterId)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> SetPlayerCharacterPartyId(string characterId, int partyId)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> SetPlayerCharacterGuildId(string characterId, int guildId)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> SetPlayerCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<SocialCharacterData>());
        }
        public UniTask<bool> RemoveSocialCharacter(string characterId)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetBuilding(string channel, string mapName, BuildingSaveData building)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<BuildingSaveData>> GetBuilding(string channel, string mapName, string buildingId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<BuildingSaveData>());
        }
        public UniTask<bool> RemoveBuilding(string channel, string mapName, string buildingId)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetBuildings(string channel, string mapName, IEnumerable<BuildingSaveData> buildings)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<IEnumerable<BuildingSaveData>>> GetBuildings(string channel, string mapName)
        {
            return UniTask.FromResult(new DatabaseCacheResult<IEnumerable<BuildingSaveData>>());
        }
        public UniTask<bool> RemoveBuildings(string channel, string mapName)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetParty(PartyData party)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<PartyData>> GetParty(int id)
        {
            return UniTask.FromResult(new DatabaseCacheResult<PartyData>());
        }
        public UniTask<bool> RemoveParty(int id)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetGuild(GuildData guild)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id)
        {
            return UniTask.FromResult(new DatabaseCacheResult<GuildData>());
        }
        public UniTask<bool> RemoveGuild(int id)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> items)
        {
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageType storageType, string storageOwnerId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<List<CharacterItem>>());
        }
        public UniTask<bool> RemoveStorageItems(StorageType storageType, string storageOwnerId)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<bool> SetSummonBuffs(string characterId, List<CharacterBuff> items)
        {
            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<List<CharacterBuff>>> GetSummonBuffs(string characterId)
        {
            return UniTask.FromResult(new DatabaseCacheResult<List<CharacterBuff>>());
        }

        public UniTask<bool> RemoveSummonBuffs(string characterId)
        {
            return UniTask.FromResult(true);
        }
    }
}
