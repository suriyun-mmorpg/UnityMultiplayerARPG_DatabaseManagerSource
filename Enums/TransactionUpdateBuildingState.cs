namespace MultiplayerARPG.MMO
{
    [System.Flags]
    public enum TransactionUpdateBuildingState : int
    {
        None = 0,
        Building = 1 << 0,
    }

    public static class TransactionUpdateBuildingStateExtentions
    {
        public static bool Has(this TransactionUpdateBuildingState self, TransactionUpdateBuildingState flag)
        {
            return (self & flag) == flag;
        }
    }
}
