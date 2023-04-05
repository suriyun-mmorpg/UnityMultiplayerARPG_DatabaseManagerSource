namespace MultiplayerARPG.MMO
{
    public partial struct MailListReq
    {
        public string UserId { get; set; }
        public bool OnlyNewMails { get; set; }
    }
}