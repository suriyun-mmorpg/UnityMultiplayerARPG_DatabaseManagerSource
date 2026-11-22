namespace MultiplayerARPG.MMO
{
    public interface IDatabaseUserLogin
    {
        string GetHashedPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        /// <summary>True if the stored hash uses an outdated scheme and should be replaced after a successful login.</summary>
        bool NeedsRehash(string hashedPassword);
        string GenerateNewId();
    }
}
