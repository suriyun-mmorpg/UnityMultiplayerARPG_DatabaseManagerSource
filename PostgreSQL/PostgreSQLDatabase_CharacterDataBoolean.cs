#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterDataBoolean(NpgsqlDataReader reader, out CharacterDataBoolean result)
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

        public async UniTask CreateCharacterDataBoolean(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataBoolean characterDataBoolean)
        {
            string id = ZString.Concat(characterId, "_", characterDataBoolean.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Boolean {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@hashedKey", characterDataBoolean.hashedKey),
                new NpgsqlParameter("@value", characterDataBoolean.value));
        }

        public async UniTask<List<CharacterDataBoolean>> ReadCharacterDataBooleans(string tableName, string characterId, List<CharacterDataBoolean> result = null)
        {
            if (result == null)
                result = new List<CharacterDataBoolean>();
            await ExecuteReader((reader) =>
            {
                CharacterDataBoolean tempData;
                while (ReadCharacterDataBoolean(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterDataBooleans(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif