using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial struct CreateGuildReq
    {
        public string GuildName { get; set; }
        public string LeaderCharacterId { get; set; }
        public List<GuildRoleData> Roles { get; set; }
    }
}