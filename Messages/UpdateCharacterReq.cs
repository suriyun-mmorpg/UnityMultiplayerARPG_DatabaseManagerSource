using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateCharacterReq
    {
        public PlayerCharacterData CharacterData { get; set; }
        public List<CharacterBuff> SummonBuffs { get; set; }
        public List<CharacterItem>? StorageItems { get; set; }
        public bool DeleteStorageReservation { get; set; }
    }
}