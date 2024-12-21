namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateBuildingReq
    {
        public TransactionUpdateBuildingState State { get; set; }
        public string ChannelId { get; set; }
        public string MapName { get; set; }
        public BuildingSaveData BuildingData { get; set; }
    }
}