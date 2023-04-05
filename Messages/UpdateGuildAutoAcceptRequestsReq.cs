namespace MultiplayerARPG.MMO
{
    public partial struct UpdateGuildAutoAcceptRequestsReq
    {
        public int GuildId { get; set; }
        public bool AutoAcceptRequests { get; set; }
    }
}