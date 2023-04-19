namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildAutoAcceptRequestsReq
    {
        public int GuildId { get; set; }
        public bool AutoAcceptRequests { get; set; }
    }
}