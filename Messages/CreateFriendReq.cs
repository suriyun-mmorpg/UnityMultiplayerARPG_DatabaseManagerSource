namespace MultiplayerARPG.MMO
{
    public partial struct CreateFriendReq
    {
        public string Character1Id { get; set; }
        public string Character2Id { get; set; }
        public byte State { get; set; }
    }
}