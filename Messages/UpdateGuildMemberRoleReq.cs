namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildMemberRoleReq
    {
        public int GuildId { get; set; }
        public byte GuildRole { get; set; }
        public string MemberCharacterId { get; set; }
    }
}