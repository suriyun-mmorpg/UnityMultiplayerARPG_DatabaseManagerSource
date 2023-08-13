using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial interface IDatabase
    {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
        UniTask<string> ValidateUserLogin(string username, string password);
        UniTask<bool> ValidateAccessToken(string userId, string accessToken);
        UniTask<bool> ValidateEmailVerification(string userId);
        UniTask<long> FindEmail(string email);
        UniTask<byte> GetUserLevel(string userId);
        UniTask<int> GetGold(string userId);
        UniTaskVoid UpdateGold(string userId, int amount);
        UniTask<int> GetCash(string userId);
        UniTaskVoid UpdateCash(string userId, int amount);
        UniTaskVoid UpdateAccessToken(string userId, string accessToken);
        UniTaskVoid CreateUserLogin(string username, string password, string email);
        UniTask<long> FindUsername(string username);
        UniTask<long> GetUserUnbanTime(string userId);
        UniTaskVoid SetUserUnbanTimeByCharacterName(string characterName, long unbanTime);
        UniTaskVoid SetCharacterUnmuteTimeByName(string characterName, long unmuteTime);

        UniTaskVoid CreateCharacter(string userId, IPlayerCharacterData characterData);
        UniTask<PlayerCharacterData> ReadCharacter(
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
            bool withPublicCustomData = true);
        UniTask<List<PlayerCharacterData>> ReadCharacters(string userId);
        UniTaskVoid UpdateCharacter(IPlayerCharacterData character);
        UniTaskVoid DeleteCharacter(string userId, string id);
        UniTask<List<CharacterBuff>> GetSummonBuffs(string characterId);
        UniTaskVoid SetSummonBuffs(string characterId, List<CharacterBuff> summonBuffs);
        UniTask<long> FindCharacterName(string characterName);
        UniTask<List<SocialCharacterData>> FindCharacters(string finderId, string characterName, int skip, int limit);
        UniTaskVoid CreateFriend(string id1, string id2, byte state);
        UniTaskVoid DeleteFriend(string id1, string id2);
        UniTask<List<SocialCharacterData>> ReadFriends(string id, bool readById2, byte state, int skip, int limit);
        UniTask<int> GetFriendRequestNotification(string characterId);
        UniTask<string> GetIdByCharacterName(string characterName);
        UniTask<string> GetUserIdByCharacterName(string characterName);

        UniTaskVoid CreateBuilding(string channel, string mapName, IBuildingSaveData saveData);
        UniTask<List<BuildingSaveData>> ReadBuildings(string channel, string mapName);
        UniTaskVoid UpdateBuilding(string channel, string mapName, IBuildingSaveData building);
        UniTaskVoid DeleteBuilding(string channel, string mapName, string id);

        UniTask<int> CreateParty(bool shareExp, bool shareItem, string leaderId);
        UniTask<PartyData> ReadParty(int id);
        UniTaskVoid UpdatePartyLeader(int id, string leaderId);
        UniTaskVoid UpdateParty(int id, bool shareExp, bool shareItem);
        UniTaskVoid DeleteParty(int id);
        UniTaskVoid UpdateCharacterParty(string characterId, int partyId);

        UniTask<int> CreateGuild(string guildName, string leaderId);
        UniTask<GuildData> ReadGuild(int id, IEnumerable<GuildRoleData> defaultGuildRoles);
        UniTaskVoid UpdateGuildLevel(int id, int level, int exp, int skillPoint);
        UniTaskVoid UpdateGuildLeader(int id, string leaderId);
        UniTaskVoid UpdateGuildMessage(int id, string guildMessage);
        UniTaskVoid UpdateGuildMessage2(int id, string guildMessage);
        UniTaskVoid UpdateGuildScore(int id, int score);
        UniTaskVoid UpdateGuildOptions(int id, string options);
        UniTaskVoid UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests);
        UniTaskVoid UpdateGuildRank(int id, int rank);
        UniTaskVoid UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData);
        UniTaskVoid UpdateGuildMemberRole(string characterId, byte guildRole);
        UniTaskVoid UpdateGuildSkillLevel(int id, int dataId, int skillLevel, int skillPoint);
        UniTaskVoid DeleteGuild(int id);
        UniTask<long> FindGuildName(string guildName);
        UniTaskVoid UpdateCharacterGuild(string characterId, int guildId, byte guildRole);
        UniTask<int> GetGuildGold(int guildId);
        UniTaskVoid UpdateGuildGold(int guildId, int gold);

        UniTaskVoid UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> storageCharacterItems);
        UniTask<List<CharacterItem>> ReadStorageItems(StorageType storageType, string storageOwnerId);

        UniTask<List<MailListEntry>> MailList(string userId, bool onlyNewMails);
        UniTask<Mail> GetMail(string mailId, string userId);
        UniTask<long> UpdateReadMailState(string mailId, string userId);
        UniTask<long> UpdateClaimMailItemsState(string mailId, string userId);
        UniTask<long> UpdateDeleteMailState(string mailId, string userId);
        UniTask<int> CreateMail(Mail mail);
        UniTask<int> GetMailNotification(string userId);

        UniTaskVoid UpdateUserCount(int userCount);
#endif
    }
}
