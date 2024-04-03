namespace MultiplayerARPG.MMO
{
    public partial struct GetGuildRequestsReq
    {
        public int GuildId { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}