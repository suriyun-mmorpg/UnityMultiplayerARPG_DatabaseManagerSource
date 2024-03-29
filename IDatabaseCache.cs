﻿using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial interface IDatabaseCache
    {
        UniTask<bool> AddUsername(string username);
        UniTask<bool> ContainsUsername(string username);
        UniTask<bool> RemoveUsername(string username);

        UniTask<bool> AddEmail(string email);
        UniTask<bool> ContainsEmail(string email);
        UniTask<bool> RemoveEmail(string email);

        UniTask<bool> AddCharacterName(string characterName);
        UniTask<bool> ContainsCharacterName(string characterName);
        UniTask<bool> RemoveCharacterName(string characterName);

        UniTask<bool> AddGuildName(string guildName);
        UniTask<bool> ContainsGuildName(string guildName);
        UniTask<bool> RemoveGuildName(string guildName);

        UniTask<bool> SetUserAccessToken(string userId, string accessToken);
        UniTask<DatabaseCacheResult<string>> GetUserAccessToken(string userId);
        UniTask<bool> RemoveUserAccessToken(string userId);

        UniTask<bool> SetUserGold(string userId, int gold);
        UniTask<DatabaseCacheResult<int>> GetUserGold(string userId);
        UniTask<bool> RemoveUserGold(string userId);

        UniTask<bool> SetUserCash(string userId, int cash);
        UniTask<DatabaseCacheResult<int>> GetUserCash(string userId);
        UniTask<bool> RemoveUserCash(string userId);

        UniTask<bool> SetPlayerCharacter(PlayerCharacterData playerCharacter);
        UniTask<DatabaseCacheResult<PlayerCharacterData>> GetPlayerCharacter(string characterId);
        UniTask<bool> RemovePlayerCharacter(string characterId);
        UniTask<bool> SetPlayerCharacterPartyId(string characterId, int partyId);
        UniTask<bool> SetPlayerCharacterGuildId(string characterId, int guildId);
        UniTask<bool> SetPlayerCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole);

        UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter);
        UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId);
        UniTask<bool> RemoveSocialCharacter(string characterId);
        UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId);
        UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId);
        UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole);

        UniTask<bool> SetBuilding(string channel, string mapName, BuildingSaveData building);
        UniTask<DatabaseCacheResult<BuildingSaveData>> GetBuilding(string channel, string mapName, string buildingId);
        UniTask<bool> RemoveBuilding(string channel, string mapName, string buildingId);

        UniTask<bool> SetBuildings(string channel, string mapName, IEnumerable<BuildingSaveData> buildings);
        UniTask<DatabaseCacheResult<IEnumerable<BuildingSaveData>>> GetBuildings(string channel, string mapName);
        UniTask<bool> RemoveBuildings(string channel, string mapName);

        UniTask<bool> SetParty(PartyData party);
        UniTask<DatabaseCacheResult<PartyData>> GetParty(int id);
        UniTask<bool> RemoveParty(int id);

        UniTask<bool> SetGuild(GuildData guild);
        UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id);
        UniTask<bool> RemoveGuild(int id);

        UniTask<bool> SetStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> items);
        UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageType storageType, string storageOwnerId);
        UniTask<bool> RemoveStorageItems(StorageType storageType, string storageOwnerId);

        UniTask<bool> SetSummonBuffs(string characterId, List<CharacterBuff> items);
        UniTask<DatabaseCacheResult<List<CharacterBuff>>> GetSummonBuffs(string characterId);
        UniTask<bool> RemoveSummonBuffs(string characterId);
    }
}
