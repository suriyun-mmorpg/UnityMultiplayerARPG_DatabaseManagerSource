namespace MultiplayerARPG.MMO
{
    public partial struct UpdatePartyLeaderReq
    {
        public int PartyId { get; set; }
        public string LeaderCharacterId { get; set; }
    }
}