namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct DeleteBuildingReq
    {
        public string ChannelId { get; set; }
        public string MapName { get; set; }
        public string BuildingId { get; set; }
    }
}