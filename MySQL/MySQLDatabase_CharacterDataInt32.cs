#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using System.Collections.Generic;
using Cysharp.Text;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool ReadCharacterDataInt32(MySqlDataReader reader, out CharacterDataInt32 result)
        {
            if (reader.Read())
            {
                result = new CharacterDataInt32();
                result.hashedKey = reader.GetInt32(0);
                result.value = reader.GetInt32(1);
                return true;
            }
            result = default;
            return false;
        }

        public void CreateCharacterDataInt32(MySqlConnection connection, MySqlTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataInt32 characterDataInt32)
        {
            string id = ZString.Concat(characterId, "_", characterDataInt32.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Int32 {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuerySync(connection, transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@hashedKey", characterDataInt32.hashedKey),
                new MySqlParameter("@value", characterDataInt32.value));
        }

        public List<CharacterDataInt32> ReadCharacterDataInt32s(string tableName, string characterId, List<CharacterDataInt32> result = null)
        {
            if (result == null)
                result = new List<CharacterDataInt32>();
            ExecuteReaderSync((reader) =>
            {
                CharacterDataInt32 tempSummon;
                while (ReadCharacterDataInt32(reader, out tempSummon))
                {
                    result.Add(tempSummon);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId ORDER BY type DESC",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterDataInt32s(MySqlConnection connection, MySqlTransaction transaction, string tableName, string characterId)
        {
            ExecuteNonQuerySync(connection, transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
#endif