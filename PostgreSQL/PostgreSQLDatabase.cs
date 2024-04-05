#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Newtonsoft.Json;
using System;
using System.IO;
using Cysharp.Text;
using System.Collections.Concurrent;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase : BaseDatabase
    {
        public static readonly string LogTag = nameof(PostgreSQLDatabase);

        private string _address = "127.0.0.1";
        private int _port = 5432;
        private string _username = "postgres";
        private string _password = "localdb";
        private string _dbName = "mmorpg_kit";
        private string _connectionString = "";
        private NpgsqlDataSource _dataSource;

        public override void Initialize()
        {
            // Json file read
            bool configFileFound = false;
            string configFolder = "./Config";
            string configFilePath = configFolder + "/NpgsqlConfig.json";
            PostgreSQLConfig config = new PostgreSQLConfig()
            {
                pgAddress = _address,
                pgPort = _port,
                pgUsername = _username,
                pgPassword = _password,
                pgDbName = _dbName,
                pgConnectionString = _connectionString,
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

            _address = config.pgAddress;
            _port = config.pgPort.Value;
            _username = config.pgUsername;
            _password = config.pgPassword;
            _dbName = config.pgDbName;
            _connectionString = config.pgConnectionString;
            _dataSource = new NpgsqlDataSourceBuilder(GetConnectionString()).Build();

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
            if (!string.IsNullOrWhiteSpace(this._connectionString))
                return this._connectionString;
            string connectionString = "Host=" + _address + ";" +
                "Port=" + _port + ";" +
                "Username=" + _username + ";" +
                (string.IsNullOrEmpty(_password) ? "" : "Password=\"" + _password + "\";") +
                "Database=" + _dbName + ";";
            return connectionString;
        }

        public override async UniTask<string> ValidateUserLogin(string username, string password)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT id, password FROM users WHERE username=$1 LIMIT 1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = username;

            string id = string.Empty;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    id = reader.GetString(0);
                    string hashedPassword = reader.GetString(1);
                    if (!_userLoginManager.VerifyPassword(password, hashedPassword))
                        id = string.Empty;
                }
            }
            return id;
        }

        public override async UniTask<bool> ValidateAccessToken(string userId, string accessToken)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT COUNT(*) FROM user_accesses WHERE id=$1 AND access_token=$2");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = userId;
            cmd.Parameters[1].Value = accessToken;

            var result = await cmd.ExecuteScalarAsync();
            return (result != null ? (long)result : 0) > 0;
        }

        public override async UniTask<byte> GetUserLevel(string userId)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT level FROM user_accesses WHERE id=$1 LIMIT 1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = userId;

            byte userLevel = 0;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    userLevel = reader.GetByte(0);
                }
            }
            return userLevel;
        }

        public override async UniTask<int> GetGold(string userId)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT gold FROM user_currencies WHERE id=$1 LIMIT 1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = userId;

            byte gold = 0;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    gold = reader.GetByte(0);
                }
            }
            return gold;
        }

        public override async UniTask UpdateGold(string userId, int gold)
        {
            await using var cmd = _dataSource.CreateCommand("UPDATE user_currencies SET gold=$1 WHERE id=$2");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer });
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = gold;
            cmd.Parameters[1].Value = userId;
            await cmd.ExecuteNonQueryAsync();
        }

        public override async UniTask<int> GetCash(string userId)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT cash FROM user_currencies WHERE id=$1 LIMIT 1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = userId;

            byte cash = 0;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    cash = reader.GetByte(0);
                }
            }
            return cash;
        }

        public override async UniTask UpdateCash(string userId, int cash)
        {
            await using var cmd = _dataSource.CreateCommand("UPDATE user_currencies SET cash=$1 WHERE id=$2");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer });
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = cash;
            cmd.Parameters[1].Value = userId;
            await cmd.ExecuteNonQueryAsync();
        }

        public override async UniTask UpdateAccessToken(string userId, string accessToken)
        {
            await using var cmd = _dataSource.CreateCommand("UPDATE user_accesses SET access_token=$1 WHERE id=$2");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = accessToken;
            cmd.Parameters[1].Value = userId;
            await cmd.ExecuteNonQueryAsync();
        }

        public override async UniTask CreateUserLogin(string username, string password, string email)
        {
            var id = _userLoginManager.GenerateNewId();
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await using var cmd = new NpgsqlCommand("INSERT INTO users (id, username, password, email) VALUES (@id, @username, @password, @email) VALUES ($1, $2, $3, $4)", connection, transaction);
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
                await cmd.PrepareAsync();
                cmd.Parameters[0].Value = id;
                cmd.Parameters[1].Value = username;
                cmd.Parameters[2].Value = _userLoginManager.GetHashedPassword(password);
                cmd.Parameters[3].Value = email;
                await cmd.ExecuteNonQueryAsync();

                await using var cmd2 = new NpgsqlCommand("INSERT INTO user_accesses (id) VALUES ($1)", connection, transaction);
                cmd2.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
                await cmd2.PrepareAsync();
                cmd2.Parameters[0].Value = id;
                await cmd2.ExecuteNonQueryAsync();

                await using var cmd3 = new NpgsqlCommand("INSERT INTO user_currencies (id) VALUES ($1)", connection, transaction);
                cmd3.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
                await cmd3.PrepareAsync();
                cmd3.Parameters[0].Value = id;
                await cmd3.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "@CreateUserLogin_Transaction1");
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public override async UniTask<long> FindUsername(string username)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT COUNT(*) FROM users WHERE username LIKE $1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = username;

            var result = await cmd.ExecuteScalarAsync();
            return result != null ? (long)result : 0;
        }

        public override async UniTask<long> GetUserUnbanTime(string userId)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT unban_time FROM user_accesses WHERE id=$1 LIMIT 1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = userId;

            long unbanTime = 0;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    unbanTime = reader.GetInt64(0);
                }
            }
            return unbanTime;
        }

        public override async UniTask SetUserUnbanTimeByCharacterName(string characterName, long unbanTime)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT userId FROM characters WHERE character_name LIKE $1 LIMIT 1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = characterName;

            string userId = string.Empty;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    userId = reader.GetString(0);
                }
            }

            if (string.IsNullOrEmpty(userId))
                return;

            await using var cmd2 = _dataSource.CreateCommand("UPDATE user_accesses SET unban_time=$1 WHERE id=$2 LIMIT 1");
            cmd2.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Bigint });
            cmd2.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd2.PrepareAsync();
            cmd2.Parameters[0].Value = unbanTime;
            cmd2.Parameters[1].Value = userId;
        }

        public override async UniTask SetCharacterUnmuteTimeByName(string characterName, long unmuteTime)
        {
            await using var cmd = _dataSource.CreateCommand("UPDATE characters SET unmute_time=$1 WHERE character_name LIKE $2 LIMIT 1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Bigint });
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = unmuteTime;
            cmd.Parameters[1].Value = characterName;
            await cmd.ExecuteNonQueryAsync();
        }

        public override async UniTask<bool> ValidateEmailVerification(string userId)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT COUNT(*) FROM users WHERE id=$1 AND is_verify IS TRUE");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = userId;

            var result = await cmd.ExecuteScalarAsync();
            return (result != null ? (long)result : 0) > 0;
        }

        public override async UniTask<long> FindEmail(string email)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT COUNT(*) FROM users WHERE email IS NOT NULL AND email LIKE $1");
            cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Varchar });
            await cmd.PrepareAsync();
            cmd.Parameters[0].Value = email;

            var result = await cmd.ExecuteScalarAsync();
            return result != null ? (long)result : 0;
        }

        public override async UniTask UpdateUserCount(int userCount)
        {
            await using var cmd = _dataSource.CreateCommand("SELECT COUNT(*) FROM server_statistic WHERE 1");
            await cmd.PrepareAsync();
            var result = await cmd.ExecuteScalarAsync();

            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                await using var cmd2 = _dataSource.CreateCommand("UPDATE server_statistic SET user_count=$1");
                cmd2.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer });
                await cmd2.PrepareAsync();
                cmd2.Parameters[0].Value = userCount;
                await cmd2.ExecuteNonQueryAsync();
            }
            else
            {
                await using var cmd2 = _dataSource.CreateCommand("INSERT INTO server_statistic (user_count) VALUES ($1)");
                cmd2.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer });
                await cmd2.PrepareAsync();
                cmd2.Parameters[0].Value = userCount;
                await cmd2.ExecuteNonQueryAsync();
            }
        }
    }
}
#endif
