﻿namespace MultiplayerARPG.MMO
{
    public partial struct IncreaseGuildExpReq
    {
        public int GuildId { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int SkillPoint { get; set; }
    }
}