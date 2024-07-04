namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetCharacterReq
    {
        public string UserId { get; set; }
        public string CharacterId { get; set; }
        public bool ForceClearCache { get; set; }
    }
}