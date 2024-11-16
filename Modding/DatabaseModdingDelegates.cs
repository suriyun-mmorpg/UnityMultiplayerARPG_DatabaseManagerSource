using System.Data.Common;

namespace MultiplayerARPG.MMO
{
    public delegate PlayerCharacterData DbGetCharacterDelegate(
        IPlayerCharacterData result,
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
        bool withPublicCustomData);

    public delegate void DbCreateCharacterDelegate<TConnection, TTransaction>(
        TConnection connection,
        TTransaction transaction,
        string userId,
        IPlayerCharacterData createData)
        where TConnection : DbConnection
        where TTransaction : DbTransaction;

    public delegate void DbUpdateCharacterDelegate<TConnection, TTransaction>(
        TConnection connection,
        TTransaction transaction,
        TransactionUpdateCharacterState state,
        IPlayerCharacterData updateData)
        where TConnection : DbConnection
        where TTransaction : DbTransaction;

    public delegate void DbDeleteCharacterDelegate<TConnection, TTransaction>(
        TConnection connection,
        TTransaction transaction,
        string userId,
        string characterId)
        where TConnection : DbConnection
        where TTransaction : DbTransaction;
}
