namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct CreateUserLoginReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}