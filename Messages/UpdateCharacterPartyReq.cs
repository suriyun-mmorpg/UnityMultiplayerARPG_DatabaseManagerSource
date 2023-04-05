namespace MultiplayerARPG.MMO
{
    public partial struct UpdateCharacterPartyReq
    {
        public int PartyId { get; set; }
        public SocialCharacterData SocialCharacterData { get; set; }
    }
}