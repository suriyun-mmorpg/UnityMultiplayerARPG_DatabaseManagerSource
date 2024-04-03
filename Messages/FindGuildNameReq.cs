namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct FindGuildNameReq
    {
        public string? FinderId { get; set; }
        public string GuildName { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}