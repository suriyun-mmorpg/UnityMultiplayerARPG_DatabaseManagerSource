namespace MultiplayerARPG.MMO
{
    public partial struct UpdateCharacterGuildReq
    {
        public int GuildId { get; set; }
        public byte GuildRole { get; set; }
        public SocialCharacterData SocialCharacterData { get; set; }
    }
}