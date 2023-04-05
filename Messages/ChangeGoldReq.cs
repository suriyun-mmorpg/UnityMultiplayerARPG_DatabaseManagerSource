namespace MultiplayerARPG.MMO
{
    public partial struct ChangeGoldReq
    {
        public string UserId { get; set; }
        public int ChangeAmount { get; set; }
    }
}