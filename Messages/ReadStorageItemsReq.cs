namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct ReadStorageItemsReq
    {
        public StorageType StorageType { get; set; }
        public string StorageOwnerId { get; set; }
        public bool ReadForUpdate { get; set; }
    }
}