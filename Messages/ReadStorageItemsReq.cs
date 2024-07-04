namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetStorageItemsReq
    {
        public StorageType StorageType { get; set; }
        public string StorageOwnerId { get; set; }
        public string? ReserverId { get; set; }
    }
}