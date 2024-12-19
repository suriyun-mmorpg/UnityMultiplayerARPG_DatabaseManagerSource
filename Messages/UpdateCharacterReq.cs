using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateCharacterReq
    {
        public TransactionUpdateCharacterState State { get; set; }
        public PlayerCharacterData CharacterData { get; set; }
        public List<CharacterBuff> SummonBuffs { get; set; }
        public List<CharacterItem>? PlayerStorageItems { get; set; }
        public List<CharacterItem>? ProtectedStorageItems { get; set; }
        public bool DeleteStorageReservation { get; set; }
    }
}