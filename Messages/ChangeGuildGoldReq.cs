namespace MultiplayerARPG.MMO
{
    public partial struct ChangeGuildGoldReq
    {
        public int GuildId { get; set; }
        public int ChangeAmount { get; set; }
    }
}