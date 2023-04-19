namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateCharacterPartyReq
    {
        public int PartyId { get; set; }
        public SocialCharacterData SocialCharacterData { get; set; }
    }
}