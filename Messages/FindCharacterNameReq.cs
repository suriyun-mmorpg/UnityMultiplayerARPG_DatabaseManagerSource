namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct FindCharacterNameReq
    {
        public string? FinderId { get; set; }
        public string CharacterName { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}
