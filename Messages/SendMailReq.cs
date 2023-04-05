namespace MultiplayerARPG.MMO
{
    public partial struct SendMailReq
    {
        public string ReceiverId { get; set; }
        public Mail Mail { get; set; }
    }
}