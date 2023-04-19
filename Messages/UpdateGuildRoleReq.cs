namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildRoleReq
    {
        public int GuildId { get; set; }
        public byte GuildRole { get; set; }
        public GuildRoleData GuildRoleData { get; set; }
    }
}