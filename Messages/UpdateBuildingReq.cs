namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateBuildingReq
    {
        public string MapName { get; set; }
        public BuildingSaveData BuildingData { get; set; }
    }
}