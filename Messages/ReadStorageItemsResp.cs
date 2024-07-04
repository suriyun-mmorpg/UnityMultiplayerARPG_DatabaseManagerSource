using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetStorageItemsResp
    {
        public UITextKeys Error { get; set; }
        public List<CharacterItem> StorageItems { get; set; }
    }
}