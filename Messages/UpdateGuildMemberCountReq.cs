namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildMemberCountReq
    {
        public int GuildId { get; set; }
        public int MaxGuildMember { get; set; }
    }
}