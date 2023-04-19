namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct SetUserUnbanTimeByCharacterNameReq
    {
        public string CharacterName { get; set; }
        public long UnbanTime { get; set; }
    }
}
