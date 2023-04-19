namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdatePartyLeaderReq
    {
        public int PartyId { get; set; }
        public string LeaderCharacterId { get; set; }
    }
}