using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateStorageAndCharacterItemsReq
    {
        public StorageType StorageType { get; set; }
        public string StorageOwnerId { get; set; }
        public List<CharacterItem> StorageItems { get; set; }
        public string CharacterId { get; set; }
        public List<EquipWeapons> SelectableWeaponSets { get; set; }
        public List<CharacterItem> EquipItems { get; set; }
        public List<CharacterItem> NonEquipItems { get; set; }
        public bool DeleteStorageReservation { get; set; }
    }
}
