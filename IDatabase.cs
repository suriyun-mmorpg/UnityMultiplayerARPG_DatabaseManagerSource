using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial interface IDatabase
    {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
        string ValidateUserLogin(string username, string password);
        bool ValidateAccessToken(string userId, string accessToken);
        bool ValidateEmailVerification(string userId);
        long FindEmail(string email);
        byte GetUserLevel(string userId);
        int GetGold(string userId);
        void UpdateGold(string userId, int amount);
        int GetCash(string userId);
        void UpdateCash(string userId, int amount);
        void UpdateAccessToken(string userId, string accessToken);
        void CreateUserLogin(string username, string password, string email);
        long FindUsername(string username);
        long GetUserUnbanTime(string userId);
        void SetUserUnbanTimeByCharacterName(string characterName, long unbanTime);
        void SetCharacterUnmuteTimeByName(string characterName, long unmuteTime);

        void CreateCharacter(string userId, IPlayerCharacterData characterData);
        PlayerCharacterData ReadCharacter(
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
            bool withCurrencies = true);
        List<PlayerCharacterData> ReadCharacters(string userId);
        void UpdateCharacter(IPlayerCharacterData character);
        void DeleteCharacter(string userId, string id);
        List<CharacterBuff> GetSummonBuffs(string characterId);
        void SetSummonBuffs(string characterId, List<CharacterBuff> summonBuffs);
        long FindCharacterName(string characterName);
        List<SocialCharacterData> FindCharacters(string finderId, string characterName, int skip, int limit);
        void CreateFriend(string id1, string id2, byte state);
        void DeleteFriend(string id1, string id2);
        List<SocialCharacterData> ReadFriends(string id, bool readById2, byte state, int skip, int limit);
        int GetFriendRequestNotification(string characterId);
        string GetIdByCharacterName(string characterName);
        string GetUserIdByCharacterName(string characterName);

        void CreateBuilding(string mapName, IBuildingSaveData saveData);
        List<BuildingSaveData> ReadBuildings(string mapName);
        void UpdateBuilding(string mapName, IBuildingSaveData building);
        void DeleteBuilding(string mapName, string id);

        int CreateParty(bool shareExp, bool shareItem, string leaderId);
        PartyData ReadParty(int id);
        void UpdatePartyLeader(int id, string leaderId);
        void UpdateParty(int id, bool shareExp, bool shareItem);
        void DeleteParty(int id);
        void UpdateCharacterParty(string characterId, int partyId);

        int CreateGuild(string guildName, string leaderId);
        GuildData ReadGuild(int id, IEnumerable<GuildRoleData> defaultGuildRoles);
        void UpdateGuildLevel(int id, int level, int exp, int skillPoint);
        void UpdateGuildLeader(int id, string leaderId);
        void UpdateGuildMessage(int id, string guildMessage);
        void UpdateGuildMessage2(int id, string guildMessage);
        void UpdateGuildScore(int id, int score);
        void UpdateGuildOptions(int id, string options);
        void UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests);
        void UpdateGuildRank(int id, int rank);
        void UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData);
        void UpdateGuildMemberRole(string characterId, byte guildRole);
        void UpdateGuildSkillLevel(int id, int dataId, int skillLevel, int skillPoint);
        void DeleteGuild(int id);
        long FindGuildName(string guildName);
        void UpdateCharacterGuild(string characterId, int guildId, byte guildRole);
        int GetGuildGold(int guildId);
        void UpdateGuildGold(int guildId, int gold);

        void UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> storageCharacterItems);
        List<CharacterItem> ReadStorageItems(StorageType storageType, string storageOwnerId);

        List<MailListEntry> MailList(string userId, bool onlyNewMails);
        Mail GetMail(string mailId, string userId);
        long UpdateReadMailState(string mailId, string userId);
        long UpdateClaimMailItemsState(string mailId, string userId);
        long UpdateDeleteMailState(string mailId, string userId);
        int CreateMail(Mail mail);
        int GetMailNotification(string userId);

        void UpdateUserCount(int userCount);
#endif
    }
}
