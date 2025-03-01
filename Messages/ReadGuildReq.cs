namespace MultiplayerARPG.MMO
{
    public partial struct GetGuildReq
    {
        public int GuildId { get; set; }
        public bool ForceClearCache { get; set; }
    }
}