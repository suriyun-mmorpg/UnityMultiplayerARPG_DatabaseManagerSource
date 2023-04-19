namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdatePartyReq
    {
        public int PartyId { get; set; }
        public bool ShareExp { get; set; }
        public bool ShareItem { get; set; }
    }
}