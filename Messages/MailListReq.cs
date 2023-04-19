namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct MailListReq
    {
        public string UserId { get; set; }
        public bool OnlyNewMails { get; set; }
    }
}