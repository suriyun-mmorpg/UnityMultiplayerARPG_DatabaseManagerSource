#if !NET && !NETCOREAPP
using UnityEngine;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
#endif

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase : BaseDatabase
    {
        public static readonly string LogTag = nameof(MySQLDatabase);

#if !NET && !NETCOREAPP
        [SerializeField]
#endif
        private string address = "127.0.0.1";
#if !NET && !NETCOREAPP
        [SerializeField]
#endif
        private int port = 3306;
#if !NET && !NETCOREAPP
        [SerializeField]
#endif
        private string username = "root";
#if !NET && !NETCOREAPP
        [SerializeField]
#endif
        private string password = "";
#if !NET && !NETCOREAPP
        [SerializeField]
#endif
        private string dbName = "mmorpgtemplate";
#if !NET && !NETCOREAPP
        [SerializeField]
        [Tooltip("Leave this empty to use `address`, `port`, `username`, `password` and `dbName` to build connection string")]
#endif
        private string connectionString = "";

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
        public override void Initialize()
        {
            // Json file read
            bool configFileFound = false;
            string configFolder = "./Config";
            string configFilePath = configFolder + "/mySqlConfig.json";
            MySQLConfig config = new MySQLConfig()
            {
                mySqlAddress = address,
                mySqlPort = port,
                mySqlUsername = username,
                mySqlPassword = password,
                mySqlDbName = dbName,
                mySqlConnectionString = connectionString,
            };
            LogInformation(LogTag, "Reading config file from " + configFilePath);
            if (File.Exists(configFilePath))
            {
                LogInformation(LogTag, "Found config file");
                string dataAsJson = File.ReadAllText(configFilePath);
                MySQLConfig replacingConfig = JsonConvert.DeserializeObject<MySQLConfig>(dataAsJson);
                if (replacingConfig.mySqlAddress != null)
                    config.mySqlAddress = replacingConfig.mySqlAddress;
                if (replacingConfig.mySqlPort.HasValue)
                    config.mySqlPort = replacingConfig.mySqlPort.Value;
                if (replacingConfig.mySqlUsername != null)
                    config.mySqlUsername = replacingConfig.mySqlUsername;
                if (replacingConfig.mySqlPassword != null)
                    config.mySqlPassword = replacingConfig.mySqlPassword;
                if (replacingConfig.mySqlDbName != null)
                    config.mySqlDbName = replacingConfig.mySqlDbName;
                if (replacingConfig.mySqlConnectionString != null)
                    config.mySqlConnectionString = replacingConfig.mySqlConnectionString;
                configFileFound = true;
            }

            address = config.mySqlAddress;
            port = config.mySqlPort.Value;
            username = config.mySqlUsername;
            password = config.mySqlPassword;
            dbName = config.mySqlDbName;
            connectionString = config.mySqlConnectionString;

            if (!configFileFound)
            {
                // Write config file
                LogInformation(LogTag, "Not found config file, creating a new one");
                if (!Directory.Exists(configFolder))
                    Directory.CreateDirectory(configFolder);
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            this.InvokeInstanceDevExtMethods("Init");
        }

        public string GetConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(this.connectionString))
                return this.connectionString;
            string connectionString = "Server=" + address + ";" +
            "Port=" + port + ";" +
            "Uid=" + username + ";" +
                (string.IsNullOrEmpty(password) ? "" : "Pwd=\"" + password + "\";") +
                "Database=" + dbName + ";" +
                "SSL Mode=None;";
            return connectionString;
        }

        public MySqlConnection NewConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }

        private async UniTask OpenConnection(MySqlConnection connection)
        {
            try
            {
                await connection.OpenAsync();
            }
            catch (MySqlException ex)
            {
                LogException(LogTag, ex);
            }
        }

        private void OpenConnectionSync(MySqlConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch (MySqlException ex)
            {
                LogException(LogTag, ex);
            }
        }

        public async UniTask<long> ExecuteInsertData(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            await OpenConnection(connection);
            long result = await ExecuteInsertData(connection, null, sql, args);
            await connection.CloseAsync();
            return result;
        }

        public async UniTask<long> ExecuteInsertData(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createLocalConnection = true;
            }
            long result = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    result = cmd.LastInsertedId;
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                await connection.CloseAsync();
            return result;
        }

        public long ExecuteInsertDataSync(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            OpenConnectionSync(connection);
            long result = ExecuteInsertDataSync(connection, null, sql, args);
            connection.Close();
            return result;
        }

        public long ExecuteInsertDataSync(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                OpenConnectionSync(connection);
                createLocalConnection = true;
            }
            long result = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    cmd.ExecuteNonQuery();
                    result = cmd.LastInsertedId;
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                connection.Close();
            return result;
        }

        public async UniTask<int> ExecuteNonQuery(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            await OpenConnection(connection);
            int result = await ExecuteNonQuery(connection, null, sql, args);
            await connection.CloseAsync();
            return result;
        }

        public async UniTask<int> ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createLocalConnection = true;
            }
            int numRows = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    numRows = await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                await connection.CloseAsync();
            return numRows;
        }

        public int ExecuteNonQuerySync(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            OpenConnectionSync(connection);
            int result = ExecuteNonQuerySync(connection, null, sql, args);
            connection.Close();
            return result;
        }

        public int ExecuteNonQuerySync(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                OpenConnectionSync(connection);
                createLocalConnection = true;
            }
            int numRows = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    numRows = cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                connection.Close();
            return numRows;
        }

        public async UniTask<object> ExecuteScalar(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            await OpenConnection(connection);
            object result = await ExecuteScalar(connection, null, sql, args);
            await connection.CloseAsync();
            return result;
        }

        public async UniTask<object> ExecuteScalar(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createLocalConnection = true;
            }
            object result = null;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    result = await cmd.ExecuteScalarAsync();
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                await connection.CloseAsync();
            return result;
        }

        public object ExecuteScalarSync(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            OpenConnectionSync(connection);
            object result = ExecuteScalarSync(connection, null, sql, args);
            connection.Close();
            return result;
        }

        public object ExecuteScalarSync(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                OpenConnectionSync(connection);
                createLocalConnection = true;
            }
            object result = null;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    result = cmd.ExecuteScalar();
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                connection.Close();
            return result;
        }

        public async UniTask ExecuteReader(Action<MySqlDataReader> onRead, string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            await OpenConnection(connection);
            await ExecuteReader(connection, null, onRead, sql, args);
            await connection.CloseAsync();
        }

        public async UniTask ExecuteReader(MySqlConnection connection, MySqlTransaction transaction, Action<MySqlDataReader> onRead, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createLocalConnection = true;
            }
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    MySqlDataReader dataReader = await cmd.ExecuteReaderAsync();
                    if (onRead != null) onRead.Invoke(dataReader);
                    dataReader.Close();
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                await connection.CloseAsync();
        }

        public void ExecuteReaderSync(Action<MySqlDataReader> onRead, string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            OpenConnectionSync(connection);
            ExecuteReaderSync(connection, null, onRead, sql, args);
            connection.Close();
        }

        public void ExecuteReaderSync(MySqlConnection connection, MySqlTransaction transaction, Action<MySqlDataReader> onRead, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                OpenConnectionSync(connection);
                createLocalConnection = true;
            }
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    if (onRead != null) onRead.Invoke(dataReader);
                    dataReader.Close();
                }
                catch (MySqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createLocalConnection)
                connection.Close();
        }

        public override string ValidateUserLogin(string username, string password)
        {
            string id = string.Empty;
            ExecuteReaderSync((reader) =>
            {
                if (reader.Read())
                {
                    id = reader.GetString(0);
                    string hashedPassword = reader.GetString(1);
                    if (!_userLoginManager.VerifyPassword(password, hashedPassword))
                        id = string.Empty;
                }
            }, "SELECT id, password FROM userlogin WHERE username=@username AND authType=@authType LIMIT 1",
                new MySqlParameter("@username", username),
                new MySqlParameter("@authType", AUTH_TYPE_NORMAL));

            return id;
        }

        public override bool ValidateAccessToken(string userId, string accessToken)
        {
            object result = ExecuteScalarSync("SELECT COUNT(*) FROM userlogin WHERE id=@id AND accessToken=@accessToken",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@accessToken", accessToken));
            return (result != null ? (long)result : 0) > 0;
        }

        public override byte GetUserLevel(string userId)
        {
            byte userLevel = 0;
            ExecuteReaderSync((reader) =>
            {
                if (reader.Read())
                    userLevel = reader.GetByte(0);
            }, "SELECT userLevel FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return userLevel;
        }

        public override int GetGold(string userId)
        {
            int gold = 0;
            ExecuteReaderSync((reader) =>
            {
                if (reader.Read())
                    gold = reader.GetInt32(0);
            }, "SELECT gold FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return gold;
        }

        public override void UpdateGold(string userId, int gold)
        {
            ExecuteNonQuerySync("UPDATE userlogin SET gold=@gold WHERE id=@id",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@gold", gold));
        }

        public override int GetCash(string userId)
        {
            int cash = 0;
            ExecuteReaderSync((reader) =>
            {
                if (reader.Read())
                    cash = reader.GetInt32(0);
            }, "SELECT cash FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return cash;
        }

        public override void UpdateCash(string userId, int cash)
        {
            ExecuteNonQuerySync("UPDATE userlogin SET cash=@cash WHERE id=@id",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@cash", cash));
        }

        public override void UpdateAccessToken(string userId, string accessToken)
        {
            ExecuteNonQuerySync("UPDATE userlogin SET accessToken=@accessToken WHERE id=@id",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@accessToken", accessToken));
        }

        public override void CreateUserLogin(string username, string password, string email)
        {
            ExecuteNonQuerySync("INSERT INTO userlogin (id, username, password, email, authType) VALUES (@id, @username, @password, @email, @authType)",
                new MySqlParameter("@id", _userLoginManager.GenerateNewId()),
                new MySqlParameter("@username", username),
                new MySqlParameter("@password", _userLoginManager.GetHashedPassword(password)),
                new MySqlParameter("@email", email),
                new MySqlParameter("@authType", AUTH_TYPE_NORMAL));
        }

        public override long FindUsername(string username)
        {
            object result = ExecuteScalarSync("SELECT COUNT(*) FROM userlogin WHERE username LIKE @username",
                new MySqlParameter("@username", username));
            return result != null ? (long)result : 0;
        }

        public override long GetUserUnbanTime(string userId)
        {
            long unbanTime = 0;
            ExecuteReaderSync((reader) =>
            {
                if (reader.Read())
                {
                    unbanTime = reader.GetInt64(0);
                }
            }, "SELECT unbanTime FROM userlogin WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId));
            return unbanTime;
        }

        public override void SetUserUnbanTimeByCharacterName(string characterName, long unbanTime)
        {
            string userId = string.Empty;
            ExecuteReaderSync((reader) =>
            {
                if (reader.Read())
                {
                    userId = reader.GetString(0);
                }
            }, "SELECT userId FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new MySqlParameter("@characterName", characterName));
            if (string.IsNullOrEmpty(userId))
                return;
            ExecuteNonQuerySync("UPDATE userlogin SET unbanTime=@unbanTime WHERE id=@id LIMIT 1",
                new MySqlParameter("@id", userId),
                new MySqlParameter("@unbanTime", unbanTime));
        }

        public override void SetCharacterUnmuteTimeByName(string characterName, long unmuteTime)
        {
            ExecuteNonQuerySync("UPDATE characters SET unmuteTime=@unmuteTime WHERE characterName LIKE @characterName LIMIT 1",
                new MySqlParameter("@characterName", characterName),
                new MySqlParameter("@unmuteTime", unmuteTime));
        }

        public override bool ValidateEmailVerification(string userId)
        {
            object result = ExecuteScalarSync("SELECT COUNT(*) FROM userlogin WHERE id=@userId AND isEmailVerified=1",
                new MySqlParameter("@userId", userId));
            return (result != null ? (long)result : 0) > 0;
        }

        public override long FindEmail(string email)
        {
            object result = ExecuteScalarSync("SELECT COUNT(*) FROM userlogin WHERE email LIKE @email",
                new MySqlParameter("@email", email));
            return result != null ? (long)result : 0;
        }

        public override void UpdateUserCount(int userCount)
        {
            object result = ExecuteScalarSync("SELECT COUNT(*) FROM statistic WHERE 1");
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                ExecuteNonQuerySync("UPDATE statistic SET userCount=@userCount;",
                    new MySqlParameter("@userCount", userCount));
            }
            else
            {
                ExecuteNonQuerySync("INSERT INTO statistic (userCount) VALUES(@userCount);",
                    new MySqlParameter("@userCount", userCount));
            }
        }
#endif
        }
}
