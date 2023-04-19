using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct SetSummonBuffsReq
    {
        public string CharacterId { get; set; }
        public List<CharacterBuff> SummonBuffs { get; set; }
    }
}
