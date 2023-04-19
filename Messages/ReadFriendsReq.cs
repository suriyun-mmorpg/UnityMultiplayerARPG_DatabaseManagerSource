namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct ReadFriendsReq
    {
        public string CharacterId { get; set; }
        public bool ReadById2 { get; set; }
        public byte State { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}