namespace MultiplayerARPG.MMO
{
    public partial struct UpdateGuildLeaderReq
    {
        public int GuildId { get; set; }
        public string LeaderCharacterId { get; set; }
    }
}