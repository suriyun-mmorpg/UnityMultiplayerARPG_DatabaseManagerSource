#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_READ_STORAGE_ITEMS_USERS = "READ_STORAGE_ITEMS_USERS";
        public const string CACHE_KEY_READ_STORAGE_ITEMS_GUILDS = "READ_STORAGE_ITEMS_GUILDS";
        public const string CACHE_KEY_READ_STORAGE_ITEMS_BUILDINGS = "READ_STORAGE_ITEMS_BUILDINGS";
        public override async UniTask<List<CharacterItem>> ReadStorageItems(StorageType storageType, string storageOwnerId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            NpgsqlDataReader reader = null;
            switch (storageType)
            {
                case StorageType.Player:
                    reader = await PostgreSQLHelpers.ExecuteSelect(
                        CACHE_KEY_READ_STORAGE_ITEMS_USERS,
                        connection, null,
                        "storage_users", "data",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                    break;
                case StorageType.Guild:
                    reader = await PostgreSQLHelpers.ExecuteSelect(
                        CACHE_KEY_READ_STORAGE_ITEMS_GUILDS,
                        connection, null,
                        "storage_guilds", "data",
                        PostgreSQLHelpers.WhereEqualTo("id", int.Parse(storageOwnerId)));
                    break;
                case StorageType.Building:
                    reader = await PostgreSQLHelpers.ExecuteSelect(
                        CACHE_KEY_READ_STORAGE_ITEMS_BUILDINGS,
                        connection, null,
                        "storage_buildings", "data",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                    break;
            }
            if (reader == null)
                return new List<CharacterItem>();
            List<CharacterItem> result;
            if (reader.Read())
            {
                result = JsonConvert.DeserializeObject<List<CharacterItem>>(reader.GetString(0));
            }
            else
            {
                result = new List<CharacterItem>();
            }
            await reader.DisposeAsync();
            return result;
        }

        public override async UniTask UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> characterItems)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await UpdateStorageItems(connection, null, storageType, storageOwnerId, characterItems);
        }


        public const string CACHE_KEY_UPDATE_STORAGE_ITEMS_USERS_UPDATE = "UPDATE_STORAGE_ITEMS_USERS_UPDATE";
        public const string CACHE_KEY_UPDATE_STORAGE_ITEMS_USERS_INSERT = "UPDATE_STORAGE_ITEMS_USERS_INSERT";
        public const string CACHE_KEY_UPDATE_STORAGE_ITEMS_GUILDS_UPDATE = "UPDATE_STORAGE_ITEMS_GUILDS_UPDATE";
        public const string CACHE_KEY_UPDATE_STORAGE_ITEMS_GUILDS_INSERT = "UPDATE_STORAGE_ITEMS_GUILDS_INSERT";
        public const string CACHE_KEY_UPDATE_STORAGE_ITEMS_BUILDINGS_UPDATE = "UPDATE_STORAGE_ITEMS_BUILDINGS_UPDATE";
        public const string CACHE_KEY_UPDATE_STORAGE_ITEMS_BUILDINGS_INSERT = "UPDATE_STORAGE_ITEMS_BUILDINGS_INSERT";
        public async UniTask UpdateStorageItems(NpgsqlConnection connection, NpgsqlTransaction transaction, StorageType storageType, string storageOwnerId, List<CharacterItem> characterItems)
        {
            int count;
            switch (storageType)
            {
                case StorageType.Player:
                    count = await PostgreSQLHelpers.ExecuteUpdate(
                        CACHE_KEY_UPDATE_STORAGE_ITEMS_USERS_UPDATE,
                        connection, transaction,
                        "storage_users",
                        new[]
                        {
                            new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)),
                        },
                        new[]
                        {
                            PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId),
                        });
                    if (count <= 0)
                    {
                        await PostgreSQLHelpers.ExecuteInsert(
                            CACHE_KEY_UPDATE_STORAGE_ITEMS_USERS_INSERT,
                            connection, transaction,
                            "storage_users",
                            new PostgreSQLHelpers.ColumnInfo("id", storageOwnerId),
                            new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)));
                    }
                    break;
                case StorageType.Guild:
                    count = await PostgreSQLHelpers.ExecuteUpdate(
                        CACHE_KEY_UPDATE_STORAGE_ITEMS_GUILDS_UPDATE,
                        connection, transaction,
                        "storage_guilds",
                        new[]
                        {
                            new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)),
                        },
                        new[]
                        {
                            PostgreSQLHelpers.AndWhereEqualTo("id", int.Parse(storageOwnerId)),
                        });
                    if (count <= 0)
                    {
                        await PostgreSQLHelpers.ExecuteInsert(
                            CACHE_KEY_UPDATE_STORAGE_ITEMS_GUILDS_INSERT,
                            connection, transaction,
                            "storage_guilds",
                            new PostgreSQLHelpers.ColumnInfo("id", int.Parse(storageOwnerId)),
                            new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)));
                    }
                    break;
                case StorageType.Building:
                    count = await PostgreSQLHelpers.ExecuteUpdate(
                        CACHE_KEY_UPDATE_STORAGE_ITEMS_BUILDINGS_UPDATE,
                        connection, transaction,
                        "storage_buildings",
                        new[]
                        {
                            new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)),
                        },
                        new[]
                        {
                            PostgreSQLHelpers.AndWhereEqualTo("id", storageOwnerId),
                        });
                    if (count <= 0)
                    {
                        await PostgreSQLHelpers.ExecuteInsert(
                            CACHE_KEY_UPDATE_STORAGE_ITEMS_BUILDINGS_INSERT,
                            connection, transaction,
                            "storage_buildings",
                            new PostgreSQLHelpers.ColumnInfo("id", storageOwnerId),
                            new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterItems)));
                    }
                    break;
            }
        }

        public const string CACHE_KEY_FIND_RESERVED_STORAGE_USERS = "FIND_RESERVED_STORAGE_USERS";
        public const string CACHE_KEY_FIND_RESERVED_STORAGE_GUILDS = "FIND_RESERVED_STORAGE_GUILDS";
        public const string CACHE_KEY_FIND_RESERVED_STORAGE_BUILDINGS = "FIND_RESERVED_STORAGE_BUILDINGS";
        public override async UniTask<long> FindReservedStorage(StorageType storageType, string storageOwnerId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            switch (storageType)
            {
                case StorageType.Player:
                    return await PostgreSQLHelpers.ExecuteCount(
                        CACHE_KEY_FIND_RESERVED_STORAGE_USERS,
                        connection, null,
                        "storage_reservation_users",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                case StorageType.Guild:
                    return await PostgreSQLHelpers.ExecuteCount(
                        CACHE_KEY_FIND_RESERVED_STORAGE_GUILDS,
                        connection, null,
                        "storage_reservation_guilds",
                        PostgreSQLHelpers.WhereEqualTo("id", int.Parse(storageOwnerId)));
                case StorageType.Building:
                    return await PostgreSQLHelpers.ExecuteCount(
                        CACHE_KEY_FIND_RESERVED_STORAGE_BUILDINGS,
                        connection, null,
                        "storage_reservation_buildings",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
            }
            return 0;
        }

        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_USERS_UPDATE = "UPDATE_RESERVED_STORAGE_USERS_UPDATE";
        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_USERS_INSERT = "UPDATE_RESERVED_STORAGE_USERS_INSERT";
        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_GUILDS_UPDATE = "UPDATE_RESERVED_STORAGE_GUILDS_UPDATE";
        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_GUILDS_INSERT = "UPDATE_RESERVED_STORAGE_GUILDS_INSERT";
        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_BUILDINGS_UPDATE = "UPDATE_RESERVED_STORAGE_BUILDINGS_UPDATE";
        public const string CACHE_KEY_UPDATE_RESERVED_STORAGE_BUILDINGS_INSERT = "UPDATE_RESERVED_STORAGE_BUILDINGS_INSERT";
        public override async UniTask UpdateReservedStorage(StorageType storageType, string storageOwnerId, string reserverId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            int count;
            switch (storageType)
            {
                case StorageType.Player:
                    count = await PostgreSQLHelpers.ExecuteUpdate(
                        CACHE_KEY_UPDATE_RESERVED_STORAGE_USERS_UPDATE,
                        connection, null,
                        "storage_reservation_users",
                        new[]
                        {
                            new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId),
                        },
                        new[]
                        {
                            PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId),
                        });
                    if (count <= 0)
                    {
                        await PostgreSQLHelpers.ExecuteInsert(
                            CACHE_KEY_UPDATE_RESERVED_STORAGE_USERS_INSERT,
                            connection, null,
                            "storage_reservation_users",
                            new PostgreSQLHelpers.ColumnInfo("id", storageOwnerId),
                            new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId));
                    }
                    break;
                case StorageType.Guild:
                    count = await PostgreSQLHelpers.ExecuteUpdate(
                        CACHE_KEY_UPDATE_RESERVED_STORAGE_GUILDS_UPDATE,
                        connection, null,
                        "storage_reservation_guilds",
                        new[]
                        {
                            new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId),
                        },
                        new[]
                        {
                            PostgreSQLHelpers.AndWhereEqualTo("id", int.Parse(storageOwnerId)),
                        });
                    if (count <= 0)
                    {
                        await PostgreSQLHelpers.ExecuteInsert(
                            CACHE_KEY_UPDATE_RESERVED_STORAGE_GUILDS_INSERT,
                            connection, null,
                            "storage_reservation_guilds",
                            new PostgreSQLHelpers.ColumnInfo("id", int.Parse(storageOwnerId)),
                            new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId));
                    }
                    break;
                case StorageType.Building:
                    count = await PostgreSQLHelpers.ExecuteUpdate(
                        CACHE_KEY_UPDATE_RESERVED_STORAGE_BUILDINGS_UPDATE,
                        connection, null,
                        "storage_reservation_buildings",
                        new[]
                        {
                                new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId),
                        },
                        new[]
                        {
                                PostgreSQLHelpers.AndWhereEqualTo("id", storageOwnerId),
                        });
                    if (count <= 0)
                    {
                        await PostgreSQLHelpers.ExecuteInsert(
                            CACHE_KEY_UPDATE_RESERVED_STORAGE_BUILDINGS_INSERT,
                            connection, null,
                            "storage_reservation_buildings",
                            new PostgreSQLHelpers.ColumnInfo("id", storageOwnerId),
                            new PostgreSQLHelpers.ColumnInfo("reserver_id", reserverId));
                    }
                    break;
            }
        }

        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_USERS = "DELETE_RESERVED_STORAGE_USERS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_GUILDS = "DELETE_RESERVED_STORAGE_GUILDS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BUILDINGS = "DELETE_RESERVED_STORAGE_BUILDINGS";
        public override async UniTask DeleteReservedStorage(StorageType storageType, string storageOwnerId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            switch (storageType)
            {
                case StorageType.Player:
                    await PostgreSQLHelpers.ExecuteDelete(
                        CACHE_KEY_DELETE_RESERVED_STORAGE_USERS,
                        connection, null,
                        "storage_reservation_users",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                    break;
                case StorageType.Guild:
                    await PostgreSQLHelpers.ExecuteDelete(
                        CACHE_KEY_DELETE_RESERVED_STORAGE_GUILDS,
                        connection, null,
                        "storage_reservation_guilds",
                        PostgreSQLHelpers.WhereEqualTo("id", int.Parse(storageOwnerId)));
                    break;
                case StorageType.Building:
                    await PostgreSQLHelpers.ExecuteDelete(
                        CACHE_KEY_DELETE_RESERVED_STORAGE_BUILDINGS,
                        connection, null,
                        "storage_reservation_buildings",
                        PostgreSQLHelpers.WhereEqualTo("id", storageOwnerId));
                    break;
            }
        }

        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_USERS = "DELETE_RESERVED_STORAGE_BY_RESERVER_USERS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_GUILDS = "DELETE_RESERVED_STORAGE_BY_RESERVER_GUILDS";
        public const string CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_BUILDINGS = "DELETE_RESERVED_STORAGE_BY_RESERVER_BUILDINGS";
        public override async UniTask DeleteReservedStorageByReserver(string reserverId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_USERS,
                connection, transaction,
                "storage_reservation_users",
                PostgreSQLHelpers.WhereEqualTo("reserver_id", reserverId));
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_GUILDS,
                connection, transaction,
                "storage_reservation_guilds",
                PostgreSQLHelpers.WhereEqualTo("reserver_id", reserverId));
            await PostgreSQLHelpers.ExecuteDelete(
                CACHE_KEY_DELETE_RESERVED_STORAGE_BY_RESERVER_BUILDINGS,
                connection, transaction,
                "storage_reservation_buildings",
                PostgreSQLHelpers.WhereEqualTo("reserver_id", reserverId));
            await transaction.CommitAsync();
        }

        public override async UniTask DeleteAllReservedStorage()
        {
            using var cmd = _dataSource.CreateCommand("DELETE FROM storage_reservation_users WHERE 1");
            await cmd.PrepareAsync();
            await cmd.ExecuteNonQueryAsync();
            using var cmd2 = _dataSource.CreateCommand("DELETE FROM storage_reservation_guilds WHERE 1");
            await cmd2.PrepareAsync();
            await cmd2.ExecuteNonQueryAsync();
            using var cmd3 = _dataSource.CreateCommand("DELETE FROM storage_reservation_buildings WHERE 1");
            await cmd3.PrepareAsync();
            await cmd3.ExecuteNonQueryAsync();
        }
    }
}
#endif