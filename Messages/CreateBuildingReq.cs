namespace MultiplayerARPG.MMO
{
    public partial struct CreateBuildingReq
    {
        public string MapName { get; set; }
        public BuildingSaveData BuildingData { get; set; }
    }
}