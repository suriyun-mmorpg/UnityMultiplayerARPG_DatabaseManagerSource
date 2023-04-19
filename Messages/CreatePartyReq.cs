namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct CreatePartyReq
    {
        public bool ShareExp { get; set; }
        public bool ShareItem { get; set; }
        public string LeaderCharacterId { get; set; }
    }
}