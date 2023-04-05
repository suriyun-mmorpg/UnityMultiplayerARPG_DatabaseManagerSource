using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial struct SetSummonBuffsReq
    {
        public string CharacterId { get; set; }
        public List<CharacterBuff> SummonBuffs { get; set; }
    }
}
