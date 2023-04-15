namespace MultiplayerARPG.MMO
{
    [System.Serializable]
    public struct DefaultUserLoginManagerConfig
    {
        public string PasswordSaltPrefix { get; set; }
        public string PasswordSaltPostfix { get; set; }
    }
}
