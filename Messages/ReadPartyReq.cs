namespace MultiplayerARPG.MMO
{
    public partial struct GetPartyReq
    {
        public int PartyId { get; set; }
        public bool ForceClearCache { get; set; }
    }
}