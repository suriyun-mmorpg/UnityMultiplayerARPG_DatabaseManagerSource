namespace MultiplayerARPG.MMO
{
    public partial struct UpdateBuildingReq
    {
        public string MapName { get; set; }
        public BuildingSaveData BuildingData { get; set; }
    }
}