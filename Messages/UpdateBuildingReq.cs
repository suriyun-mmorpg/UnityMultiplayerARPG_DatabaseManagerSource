using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateBuildingReq
    {
        [System.Flags]
        public enum EExtraContent
        {
            None = 0,
            StorageItems = 1 << 0,
        }

        public string ChannelId { get; set; }
        public string MapName { get; set; }
        public BuildingSaveData BuildingData { get; set; }
        public EExtraContent ExtraContent { get; set; }
        public List<CharacterItem> StorageItems { get; set; }

        public bool HasExtraContent(EExtraContent flag)
        {
            return (ExtraContent & flag) == flag;
        }
    }
}