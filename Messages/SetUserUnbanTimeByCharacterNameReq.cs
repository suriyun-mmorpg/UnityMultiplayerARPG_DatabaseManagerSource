namespace MultiplayerARPG.MMO
{
    public partial struct SetUserUnbanTimeByCharacterNameReq
    {
        public string CharacterName { get; set; }
        public long UnbanTime { get; set; }
    }
}
