#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        public override UniTask DoMigration()
        {
            // Migrate fields
            if (!IsColumnExist("characterhotkey", "relateId"))
                ExecuteNonQuery("ALTER TABLE characterhotkey ADD relateId TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("characteritem", "equipSlotIndex"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD equipSlotIndex INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "durability"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD durability REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "exp"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD exp INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "lockRemainsDuration"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD lockRemainsDuration REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "expireTime"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD expireTime INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "randomSeed"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD randomSeed INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "ammo"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD ammo INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "sockets"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD sockets TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("characteritem", "version"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD version INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characteritem", "ammoDataId"))
                ExecuteNonQuery("ALTER TABLE characteritem ADD ammoDataId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "durability"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD durability REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "exp"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD exp INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "lockRemainsDuration"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD lockRemainsDuration REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "expireTime"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD expireTime INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "randomSeed"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD randomSeed INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "ammo"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD ammo INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "sockets"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD sockets TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("storageitem", "version"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD version INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("storageitem", "ammoDataId"))
                ExecuteNonQuery("ALTER TABLE storageitem ADD ammoDataId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characterquest", "randomTasksIndex"))
                ExecuteNonQuery("ALTER TABLE characterquest ADD randomTasksIndex INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characterquest", "completeTime"))
                ExecuteNonQuery("ALTER TABLE characterquest ADD completeTime INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characterquest", "isTracking"))
                ExecuteNonQuery("ALTER TABLE characterquest ADD isTracking INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characterquest", "completedTasks"))
                ExecuteNonQuery("ALTER TABLE characterquest ADD completedTasks TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("friend", "state"))
                ExecuteNonQuery("ALTER TABLE friend ADD state INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("userlogin", "gold"))
                ExecuteNonQuery("ALTER TABLE userlogin ADD gold INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("userlogin", "cash"))
                ExecuteNonQuery("ALTER TABLE userlogin ADD cash INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("userlogin", "userLevel"))
                ExecuteNonQuery("ALTER TABLE userlogin ADD userLevel INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("userlogin", "unbanTime"))
                ExecuteNonQuery("ALTER TABLE userlogin ADD unbanTime INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("userlogin", "isEmailVerified"))
                ExecuteNonQuery("ALTER TABLE userlogin ADD isEmailVerified INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("buildings", "remainsLifeTime"))
                ExecuteNonQuery("ALTER TABLE buildings ADD remainsLifeTime REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("buildings", "isLocked"))
                ExecuteNonQuery("ALTER TABLE buildings ADD isLocked INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("buildings", "lockPassword"))
                ExecuteNonQuery("ALTER TABLE buildings ADD lockPassword TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("buildings", "extraData"))
                ExecuteNonQuery("ALTER TABLE buildings ADD extraData TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("buildings", "channel"))
                ExecuteNonQuery("ALTER TABLE buildings ADD channel TEXT NOT NULL DEFAULT 'default';");

            if (!IsColumnExist("buildings", "isSceneObject"))
                ExecuteNonQuery("ALTER TABLE buildings ADD isSceneObject INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "partyId"))
                ExecuteNonQuery("ALTER TABLE characters ADD partyId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "guildId"))
                ExecuteNonQuery("ALTER TABLE characters ADD guildId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "guildRole"))
                ExecuteNonQuery("ALTER TABLE characters ADD guildRole INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "sharedGuildExp"))
                ExecuteNonQuery("ALTER TABLE characters ADD sharedGuildExp INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "entityId"))
                ExecuteNonQuery("ALTER TABLE characters ADD entityId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "factionId"))
                ExecuteNonQuery("ALTER TABLE characters ADD factionId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "equipWeaponSet"))
                ExecuteNonQuery("ALTER TABLE characters ADD equipWeaponSet INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "iconDataId"))
                ExecuteNonQuery("ALTER TABLE characters ADD iconDataId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "frameDataId"))
                ExecuteNonQuery("ALTER TABLE characters ADD frameDataId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "backgroundDataId"))
                ExecuteNonQuery("ALTER TABLE characters ADD backgroundDataId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "titleDataId"))
                ExecuteNonQuery("ALTER TABLE characters ADD titleDataId INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "currentChannel"))
                ExecuteNonQuery("ALTER TABLE characters ADD currentChannel TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("characters", "currentMapName"))
                ExecuteNonQuery("ALTER TABLE characters ADD currentMapName TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("characters", "currentRotationX"))
                ExecuteNonQuery("ALTER TABLE characters ADD currentRotationX REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "currentRotationY"))
                ExecuteNonQuery("ALTER TABLE characters ADD currentRotationY REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "currentRotationZ"))
                ExecuteNonQuery("ALTER TABLE characters ADD currentRotationZ REAL NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "currentSafeArea"))
                ExecuteNonQuery("ALTER TABLE characters ADD currentSafeArea TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("characters", "reputation"))
                ExecuteNonQuery("ALTER TABLE characters ADD reputation INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "lastDeadTime"))
                ExecuteNonQuery("ALTER TABLE characters ADD lastDeadTime INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("characters", "unmuteTime"))
                ExecuteNonQuery("ALTER TABLE characters ADD unmuteTime INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("charactersummon", "sourceId"))
                ExecuteNonQuery("ALTER TABLE charactersummon ADD sourceId TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("guild", "guildMessage2"))
                ExecuteNonQuery("ALTER TABLE guild ADD guildMessage2 TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("guild", "gold"))
                ExecuteNonQuery("ALTER TABLE guild ADD gold INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("guild", "score"))
                ExecuteNonQuery("ALTER TABLE guild ADD score INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("guild", "options"))
                ExecuteNonQuery("ALTER TABLE guild ADD options TEXT NOT NULL DEFAULT '';");

            if (!IsColumnExist("guild", "autoAcceptRequests"))
                ExecuteNonQuery("ALTER TABLE guild ADD autoAcceptRequests INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("guild", "rank"))
                ExecuteNonQuery("ALTER TABLE guild ADD rank INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("guild", "currentMembers"))
                ExecuteNonQuery("ALTER TABLE guild ADD currentMembers INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("guild", "maxMembers"))
                ExecuteNonQuery("ALTER TABLE guild ADD maxMembers INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("guildrole", "canUseStorage"))
                ExecuteNonQuery("ALTER TABLE guildrole ADD canUseStorage INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("mail", "cash"))
                ExecuteNonQuery("ALTER TABLE mail ADD cash INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("mail", "isClaim"))
                ExecuteNonQuery("ALTER TABLE mail ADD isClaim INTEGER NOT NULL DEFAULT 0;");

            if (!IsColumnExist("mail", "claimTimestamp"))
                ExecuteNonQuery("ALTER TABLE mail ADD claimTimestamp TIMESTAMP NULL DEFAULT NULL;");

            if (!IsColumnType("mail", "gold", "INTEGER"))
            {
                // Copies the columns both tables have, by name, and keeps the old table if the copy fails
                RebuildTable("mail", @"CREATE TABLE mail_modifying (
                  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                  eventId TEXT NULL DEFAULT NULL,
                  senderId TEXT NULL DEFAULT NULL,
                  senderName TEXT NULL DEFAULT NULL,
                  receiverId TEXT NOT NULL,
                  title TEXT NOT NULL,
                  content TEXT NOT NULL,
                  gold INTEGER NOT NULL DEFAULT 0,
                  cash INTEGER NOT NULL DEFAULT 0,
                  currencies TEXT NOT NULL,
                  items TEXT NOT NULL,
                  isRead INTEGER NOT NULL DEFAULT 0,
                  readTimestamp TIMESTAMP NULL DEFAULT NULL,
                  isClaim INTEGER NOT NULL DEFAULT 0,
                  claimTimestamp TIMESTAMP NULL DEFAULT NULL,
                  isDelete INTEGER NOT NULL DEFAULT 0,
                  deleteTimestamp TIMESTAMP NULL DEFAULT NULL,
                  sentTimestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                );");
            }

            if (!IsColumnType("characters", "statPoint", "REAL") || !IsColumnType("characters", "skillPoint", "REAL"))
            {
                // Copies the columns both tables have, by name, and keeps the old table if the copy fails
                RebuildTable("characters", @"CREATE TABLE characters_modifying (
                  id TEXT NOT NULL PRIMARY KEY,
                  userId TEXT NOT NULL,
                  dataId INGETER NOT NULL DEFAULT 0,
                  entityId INGETER NOT NULL DEFAULT 0,
                  factionId INGETER NOT NULL DEFAULT 0,
                  characterName TEXT NOT NULL UNIQUE,
                  level INTEGER NOT NULL,
                  exp INTEGER NOT NULL,
                  currentHp INTEGER NOT NULL,
                  currentMp INTEGER NOT NULL,
                  currentStamina INTEGER NOT NULL,
                  currentFood INTEGER NOT NULL,
                  currentWater INTEGER NOT NULL,
                  equipWeaponSet INTEGER NOT NULL DEFAULT 0,
                  statPoint REAL NOT NULL DEFAULT 0,
                  skillPoint REAL NOT NULL DEFAULT 0,
                  gold INTEGER NOT NULL,
                  partyId INTEGER NOT NULL DEFAULT 0,
                  guildId INTEGER NOT NULL DEFAULT 0,
                  guildRole INTEGER NOT NULL DEFAULT 0,
                  sharedGuildExp INTEGER NOT NULL DEFAULT 0,
                  currentChannel TEXT NOT NULL DEFAULT '',
                  currentMapName TEXT NOT NULL,
                  currentPositionX REAL NOT NULL DEFAULT 0,
                  currentPositionY REAL NOT NULL DEFAULT 0,
                  currentPositionZ REAL NOT NULL DEFAULT 0,
                  currentRotationX REAL NOT NULL DEFAULT 0,
                  currentRotationY REAL NOT NULL DEFAULT 0,
                  currentRotationZ REAL NOT NULL DEFAULT 0,
                  currentSafeArea TEXT NOT NULL DEFAULT '',
                  respawnMapName TEXT NOT NULL,
                  respawnPositionX REAL NOT NULL DEFAULT 0,
                  respawnPositionY REAL NOT NULL DEFAULT 0,
                  respawnPositionZ REAL NOT NULL DEFAULT 0,
                  iconDataId INTEGER NOT NULL DEFAULT 0,
                  frameDataId INTEGER NOT NULL DEFAULT 0,
                  backgroundDataId INTEGER NOT NULL DEFAULT 0,
                  titleDataId INTEGER NOT NULL DEFAULT 0,
                  reputation INTEGER NOT NULL DEFAULT 0,
                  lastDeadTime INTEGER NOT NULL DEFAULT 0,
                  unmuteTime INTEGER NOT NULL DEFAULT 0,
                  createAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                  updateAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                );");
            }
            return default;
        }

        /// <summary>
        /// Replaces a table with one made by createModifyingTable (which must create tableName + "_modifying"),
        /// copying the columns both tables have. It all runs in one transaction, so if the copy fails
        /// or loses rows, the old table and its rows are kept and the error is logged.
        /// </summary>
        private void RebuildTable(string tableName, string createModifyingTable)
        {
            string modifyingTableName = tableName + "_modifying";
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                ExecuteMigrationQuery(transaction, "DROP TABLE IF EXISTS " + modifyingTableName + ";");
                ExecuteMigrationQuery(transaction, createModifyingTable);
                List<string> columns = GetColumnNames(transaction, modifyingTableName);
                List<string> oldColumns = GetColumnNames(transaction, tableName);
                columns.RemoveAll(column => !oldColumns.Contains(column));
                string columnList = string.Join(", ", columns);
                ExecuteMigrationQuery(transaction, "INSERT INTO " + modifyingTableName + " (" + columnList + ") SELECT " + columnList + " FROM " + tableName + ";");
                object oldCount = ExecuteScalar(transaction, "SELECT COUNT(*) FROM " + tableName + ";");
                object newCount = ExecuteScalar(transaction, "SELECT COUNT(*) FROM " + modifyingTableName + ";");
                if (oldCount == null || !oldCount.Equals(newCount))
                    throw new System.Exception("Copied " + newCount + " of " + oldCount + " rows");
                ExecuteMigrationQuery(transaction, "ALTER TABLE " + tableName + " RENAME TO " + tableName + "_delete;");
                ExecuteMigrationQuery(transaction, "ALTER TABLE " + modifyingTableName + " RENAME TO " + tableName + ";");
                ExecuteMigrationQuery(transaction, "DROP TABLE " + tableName + "_delete;");
                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while migrating table: " + tableName + ", the old table is kept");
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            transaction.Dispose();
        }

        /// <summary>Like ExecuteNonQuery, but throws instead of logging, so a failed step stops the migration.</summary>
        private void ExecuteMigrationQuery(SqliteTransaction transaction, string sql)
        {
            using (SqliteCommand cmd = new SqliteCommand(sql, _connection))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }
        }

        private List<string> GetColumnNames(SqliteTransaction transaction, string tableName)
        {
            List<string> result = new List<string>();
            using (SqliteCommand cmd = new SqliteCommand("PRAGMA table_info(" + tableName + ");", _connection))
            {
                cmd.Transaction = transaction;
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(1));
                    }
                }
            }
            return result;
        }
    }
}
#endif