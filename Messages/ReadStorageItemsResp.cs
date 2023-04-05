using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial struct ReadStorageItemsResp
    {
        public List<CharacterItem> StorageCharacterItems { get; set; }
    }
}