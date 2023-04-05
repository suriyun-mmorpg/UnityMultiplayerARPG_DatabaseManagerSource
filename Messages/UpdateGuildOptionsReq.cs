namespace MultiplayerARPG.MMO
{
    public partial struct UpdateGuildOptionsReq
    {
        public int GuildId { get; set; }
        public string Options { get; set; }
    }
}