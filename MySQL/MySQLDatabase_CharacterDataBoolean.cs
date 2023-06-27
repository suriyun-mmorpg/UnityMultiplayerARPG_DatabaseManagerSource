#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using System.Collections.Generic;
using Cysharp.Text;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool ReadCharacterDataBoolean(MySqlDataReader reader, out CharacterDataBoolean result)
        {
            if (reader.Read())
            {
                result = new CharacterDataBoolean();
                result.hashedKey = reader.GetInt32(0);
                result.value = reader.GetBoolean(1);
                return true;
            }
            result = default;
            return false;
        }

        public void CreateCharacterDataBoolean(MySqlConnection connection, MySqlTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataBoolean characterDataBoolean)
        {
            string id = ZString.Concat(characterId, "_", characterDataBoolean.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Boolean {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuerySync(connection, transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@hashedKey", characterDataBoolean.hashedKey),
                new MySqlParameter("@value", characterDataBoolean.value));
        }

        public List<CharacterDataBoolean> ReadCharacterDataBooleans(string tableName, string characterId, List<CharacterDataBoolean> result = null)
        {
            if (result == null)
                result = new List<CharacterDataBoolean>();
            ExecuteReaderSync((reader) =>
            {
                CharacterDataBoolean tempData;
                while (ReadCharacterDataBoolean(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterDataBooleans(MySqlConnection connection, MySqlTransaction transaction, string tableName, string characterId)
        {
            ExecuteNonQuerySync(connection, transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
#endif