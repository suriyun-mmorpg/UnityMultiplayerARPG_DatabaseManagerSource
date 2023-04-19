namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct SetCharacterUnmuteTimeByNameReq
    {
        public string CharacterName { get; set; }
        public long UnmuteTime { get; set; }
    }
}
