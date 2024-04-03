namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct FindGuildReq
    {
        public string GuildName { get; set; }
        public string FinderCharacterId { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public GuildListFieldOptions FieldOptions { get; set; }
    }
}