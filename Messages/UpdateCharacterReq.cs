using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateCharacterReq
    {
        [System.Flags]
        public enum EExtraContent
        {
            None = 0,
            StorageItems = 1 << 0,
            SummonBuffs = 1 << 0,
        }

        public PlayerCharacterData CharacterData { get; set; }
        public EExtraContent ExtraContent { get; set; }
        public List<CharacterItem> StorageItems { get; set; }
        public List<CharacterBuff> SummonBuffs { get; set; }

        public bool HasExtraContent(EExtraContent flag)
        {
            return (ExtraContent & flag) == flag;
        }
    }
}