namespace MultiplayerARPG.MMO
{
    public partial struct UpdatePartyReq
    {
        public int PartyId { get; set; }
        public bool ShareExp { get; set; }
        public bool ShareItem { get; set; }
    }
}