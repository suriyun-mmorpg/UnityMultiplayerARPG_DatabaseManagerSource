#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase : BaseDatabase
    {
        public static readonly string LogTag = nameof(PostgreSQLDatabase);

        private string address = "127.0.0.1";
        private int port = 3306;
        private string username = "root";
        private string password = "";
        private string dbName = "mmorpgtemplate";
        private string connectionString = "";

        public override void Initialize()
        {
            // Json file read
            bool configFileFound = false;
            string configFolder = "./Config";
            string configFilePath = configFolder + "/NpgsqlConfig.json";
            PostgreSQLConfig config = new PostgreSQLConfig()
            {
                pgAddress = address,
                pgPort = port,
                pgUsername = username,
                pgPassword = password,
                pgDbName = dbName,
                pgConnectionString = connectionString,
            };
            LogInformation(LogTag, "Reading config file from " + configFilePath);
            if (File.Exists(configFilePath))
            {
                LogInformation(LogTag, "Found config file");
                string dataAsJson = File.ReadAllText(configFilePath);
                PostgreSQLConfig replacingConfig = JsonConvert.DeserializeObject<PostgreSQLConfig>(dataAsJson);
                if (replacingConfig.pgAddress != null)
                    config.pgAddress = replacingConfig.pgAddress;
                if (replacingConfig.pgPort.HasValue)
                    config.pgPort = replacingConfig.pgPort.Value;
                if (replacingConfig.pgUsername != null)
                    config.pgUsername = replacingConfig.pgUsername;
                if (replacingConfig.pgPassword != null)
                    config.pgPassword = replacingConfig.pgPassword;
                if (replacingConfig.pgDbName != null)
                    config.pgDbName = replacingConfig.pgDbName;
                if (replacingConfig.pgConnectionString != null)
                    config.pgConnectionString = replacingConfig.pgConnectionString;
                configFileFound = true;
            }

            address = config.pgAddress;
            port = config.pgPort.Value;
            username = config.pgUsername;
            password = config.pgPassword;
            dbName = config.pgDbName;
            connectionString = config.pgConnectionString;

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

        public NpgsqlConnection NewConnection()
        {
            return new NpgsqlConnection(GetConnectionString());
        }

        private async UniTask OpenConnection(NpgsqlConnection connection)
        {
            try
            {
                await connection.OpenAsync();
            }
            catch (NpgsqlException ex)
            {
                LogException(LogTag, ex);
            }
        }

        public async UniTask<int> ExecuteNonQuery(string sql, params NpgsqlParameter[] args)
        {
            using (NpgsqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                int result = await ExecuteNonQuery(connection, null, sql, args);
                return result;
            }
        }

        public async UniTask<int> ExecuteNonQuery(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, params NpgsqlParameter[] args)
        {
            return await ExecuteNonQuery(connection, transaction, false, sql, args);
        }

        public async UniTask<int> ExecuteNonQuery(NpgsqlConnection connection, NpgsqlTransaction transaction, bool isAsync, string sql, params NpgsqlParameter[] args)
        {
            bool createNewConnection = false;
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                LogWarning(LogTag, "Connection's state is not open yet, it will create and connect by a new one.");
                connection = null;
            }
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createNewConnection = true;
            }
            int numRows = 0;
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (NpgsqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    if (isAsync)
                        numRows = await cmd.ExecuteNonQueryAsync();
                    else
                        numRows = cmd.ExecuteNonQuery();
                }
                catch (NpgsqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createNewConnection)
                connection.Dispose();
            return numRows;
        }

        public async UniTask<object> ExecuteScalar(string sql, params NpgsqlParameter[] args)
        {
            using (NpgsqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                object result = await ExecuteScalar(connection, null, sql, args);
                return result;
            }
        }

        public async UniTask<object> ExecuteScalar(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, params NpgsqlParameter[] args)
        {
            return await ExecuteScalar(connection, transaction, false, sql, args);
        }

        public async UniTask<object> ExecuteScalar(NpgsqlConnection connection, NpgsqlTransaction transaction, bool isAsync, string sql, params NpgsqlParameter[] args)
        {
            bool createNewConnection = false;
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                LogWarning(LogTag, "Connection's state is not open yet, it will create and connect by a new one.");
                connection = null;
            }
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createNewConnection = true;
            }
            object result = null;
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (NpgsqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    if (isAsync)
                        result = await cmd.ExecuteScalarAsync();
                    else
                        result = cmd.ExecuteScalar();
                }
                catch (NpgsqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createNewConnection)
                connection.Dispose();
            return result;
        }

        public async UniTask ExecuteReader(Action<NpgsqlDataReader> onRead, string sql, params NpgsqlParameter[] args)
        {
            using (NpgsqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                await ExecuteReader(connection, null, onRead, sql, args);
            }
        }

        public async UniTask ExecuteReader(NpgsqlConnection connection, NpgsqlTransaction transaction, Action<NpgsqlDataReader> onRead, string sql, params NpgsqlParameter[] args)
        {
            bool createNewConnection = false;
            if (connection != null && connection.State != System.Data.ConnectionState.Open)
            {
                LogWarning(LogTag, "Connection's state is not open yet, it will create and connect by a new one.");
                connection = null;
            }
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                await OpenConnection(connection);
                createNewConnection = true;
            }
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (NpgsqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                try
                {
                    using (NpgsqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (onRead != null)
                            onRead.Invoke(dataReader);
                    }
                }
                catch (NpgsqlException ex)
                {
                    LogException(LogTag, ex);
                }
            }
            if (createNewConnection)
                connection.Dispose();
        }

        public override async UniTask<string> ValidateUserLogin(string username, string password)
        {
            string id = string.Empty;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    id = reader.GetString(0);
                    string hashedPassword = reader.GetString(1);
                    if (!_userLoginManager.VerifyPassword(password, hashedPassword))
                        id = string.Empty;
                }
            }, "SELECT id, password FROM userlogin WHERE username=@username AND authType=@authType LIMIT 1",
                new NpgsqlParameter("@username", username),
                new NpgsqlParameter("@authType", AUTH_TYPE_NORMAL));

            return id;
        }

        public override async UniTask<bool> ValidateAccessToken(string userId, string accessToken)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE id=@id AND accessToken=@accessToken",
                new NpgsqlParameter("@id", userId),
                new NpgsqlParameter("@accessToken", accessToken));
            return (result != null ? (long)result : 0) > 0;
        }

        public override async UniTask<byte> GetUserLevel(string userId)
        {
            byte userLevel = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    userLevel = reader.GetByte(0);
            }, "SELECT userLevel FROM userlogin WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", userId));
            return userLevel;
        }

        public override async UniTask<int> GetGold(string userId)
        {
            int gold = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    gold = reader.GetInt32(0);
            }, "SELECT gold FROM userlogin WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", userId));
            return gold;
        }

        public override async UniTask UpdateGold(string userId, int gold)
        {
            await ExecuteNonQuery("UPDATE userlogin SET gold=@gold WHERE id=@id",
                new NpgsqlParameter("@id", userId),
                new NpgsqlParameter("@gold", gold));
        }

        public override async UniTask<int> GetCash(string userId)
        {
            int cash = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    cash = reader.GetInt32(0);
            }, "SELECT cash FROM userlogin WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", userId));
            return cash;
        }

        public override async UniTask UpdateCash(string userId, int cash)
        {
            await ExecuteNonQuery("UPDATE userlogin SET cash=@cash WHERE id=@id",
                new NpgsqlParameter("@id", userId),
                new NpgsqlParameter("@cash", cash));
        }

        public override async UniTask UpdateAccessToken(string userId, string accessToken)
        {
            await ExecuteNonQuery("UPDATE userlogin SET accessToken=@accessToken WHERE id=@id",
                new NpgsqlParameter("@id", userId),
                new NpgsqlParameter("@accessToken", accessToken));
        }

        public override async UniTask CreateUserLogin(string username, string password, string email)
        {
            await ExecuteNonQuery("INSERT INTO userlogin (id, username, password, email, authType) VALUES (@id, @username, @password, @email, @authType)",
                new NpgsqlParameter("@id", _userLoginManager.GenerateNewId()),
                new NpgsqlParameter("@username", username),
                new NpgsqlParameter("@password", _userLoginManager.GetHashedPassword(password)),
                new NpgsqlParameter("@email", email),
                new NpgsqlParameter("@authType", AUTH_TYPE_NORMAL));
        }

        public override async UniTask<long> FindUsername(string username)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE username LIKE @username",
                new NpgsqlParameter("@username", username));
            return result != null ? (long)result : 0;
        }

        public override async UniTask<long> GetUserUnbanTime(string userId)
        {
            long unbanTime = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    unbanTime = reader.GetInt64(0);
                }
            }, "SELECT unbanTime FROM userlogin WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", userId));
            return unbanTime;
        }

        public override async UniTask SetUserUnbanTimeByCharacterName(string characterName, long unbanTime)
        {
            string userId = string.Empty;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    userId = reader.GetString(0);
                }
            }, "SELECT userId FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new NpgsqlParameter("@characterName", characterName));
            if (string.IsNullOrEmpty(userId))
                return;
            await ExecuteNonQuery("UPDATE userlogin SET unbanTime=@unbanTime WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", userId),
                new NpgsqlParameter("@unbanTime", unbanTime));
        }

        public override async UniTask SetCharacterUnmuteTimeByName(string characterName, long unmuteTime)
        {
            await ExecuteNonQuery("UPDATE characters SET unmuteTime=@unmuteTime WHERE characterName LIKE @characterName LIMIT 1",
                new NpgsqlParameter("@characterName", characterName),
                new NpgsqlParameter("@unmuteTime", unmuteTime));
        }

        public override async UniTask<bool> ValidateEmailVerification(string userId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE id=@userId AND isEmailVerified=1",
                new NpgsqlParameter("@userId", userId));
            return (result != null ? (long)result : 0) > 0;
        }

        public override async UniTask<long> FindEmail(string email)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM userlogin WHERE email LIKE @email",
                new NpgsqlParameter("@email", email));
            return result != null ? (long)result : 0;
        }

        public override async UniTask UpdateUserCount(int userCount)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM statistic WHERE 1");
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                await ExecuteNonQuery("UPDATE statistic SET userCount=@userCount;",
                    new NpgsqlParameter("@userCount", userCount));
            }
            else
            {
                await ExecuteNonQuery("INSERT INTO statistic (userCount) VALUES(@userCount);",
                    new NpgsqlParameter("@userCount", userCount));
            }
        }
    }
}
#endif
