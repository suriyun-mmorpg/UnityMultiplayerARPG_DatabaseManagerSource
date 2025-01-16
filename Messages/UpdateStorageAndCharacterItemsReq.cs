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
        public List<CharacterItem> CharacterItems { get; set; }
        public bool DeleteStorageReservation { get; set; }
    }
}
