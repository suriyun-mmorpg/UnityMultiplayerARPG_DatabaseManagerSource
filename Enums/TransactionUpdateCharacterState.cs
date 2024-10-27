namespace MultiplayerARPG.MMO
{
    [System.Flags]
    public enum TransactionUpdateCharacterState : uint
    {
        None = 0,
        LevelAndExp = 1 << 0,
        GenericStats = 1 << 1, // HP, MP, Stamina and so on
        GenericPoints = 1 << 2, // Stat Point, Skill Point, Reputation and so on
        BuiltInCharacterCurrncies = 1 << 3, // Gold and so on
        BuiltInUserCurrencies = 1 << 4, // User gold, cash and so on
        Social = 1 << 5, // Icon, Frame, Title, Faction, Party, Guild and so on
        CurrentLocation = 1 << 6, // Current channel, Current map name, Current position and so on
        RespawnLocation = 1 << 7, // Respawn map name, Respawn position and so on
        Pk = 1 << 8,
        Accessibility = 1 << 9, // Unmute time and so on
        EquipWeapons = 1 << 10,
        SelectableWeaponSets = 1 << 11,
        Attributes = 1 << 12,
        Skills = 1 << 13,
        SkillUsages = 1 << 14,
        Buffs = 1 << 15,
        EquipItems = 1 << 16,
        NonEquipItems = 1 << 17,
        Summons = 1 << 18,
        Hotkeys = 1 << 19,
        Quests = 1 << 20,
        Currencies = 1 << 21,
        ServerCustomData = 1 << 22, // Server bools, ints, floats and so on
        PrivateCustomData = 1 << 22, // Private bools, ints, floats and so on
        PublicCustomData = 1 << 22, // Public bools, ints, floats and so on
        Mount = 1 << 23,
    }

    public static class TransactionUpdateCharacterStateExtentions
    {
        public static bool Has(this TransactionUpdateCharacterState self, TransactionUpdateCharacterState flag)
        {
            return (self & flag) == flag;
        }
    }
}
