namespace MultiplayerARPG.MMO
{
    public partial struct CreateCharacterReq
    {
        public string UserId { get; set; }
        public PlayerCharacterData CharacterData { get; set; }
    }
}