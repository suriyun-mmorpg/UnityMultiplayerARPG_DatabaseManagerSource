namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildOptionsReq
    {
        public int GuildId { get; set; }
        public string Options { get; set; }
    }
}