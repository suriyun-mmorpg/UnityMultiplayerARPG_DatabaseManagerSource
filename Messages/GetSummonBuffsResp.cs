using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial struct GetSummonBuffsResp
    {
        public List<CharacterBuff> SummonBuffs { get; set; }
    }
}
