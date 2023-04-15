namespace MultiplayerARPG.MMO
{
    public interface IDatabaseUserLoginManager
    {
        string GetHashedPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateNewId();
    }
}
