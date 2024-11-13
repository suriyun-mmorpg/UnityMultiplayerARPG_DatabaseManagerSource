using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial interface IDatabaseCache
    {
        UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter);
        UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId);
        UniTask<bool> RemoveSocialCharacter(string characterId);
        UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId);
        UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId);
        UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole);

        UniTask<bool> SetParty(PartyData party);
        UniTask<DatabaseCacheResult<PartyData>> GetParty(int id);
        UniTask<bool> RemoveParty(int id);

        UniTask<bool> SetGuild(GuildData guild);
        UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id);
        UniTask<bool> RemoveGuild(int id);

        UniTask<bool> SetStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> items);
        UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageType storageType, string storageOwnerId);
        UniTask<bool> RemoveStorageItems(StorageType storageType, string storageOwnerId);
    }
}
