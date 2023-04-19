namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildMessageReq
    {
        public int GuildId { get; set; }
        public string GuildMessage { get; set; }
    }
}