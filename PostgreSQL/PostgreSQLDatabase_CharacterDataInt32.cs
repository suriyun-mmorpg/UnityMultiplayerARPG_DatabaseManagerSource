#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterDataInt32(NpgsqlDataReader reader, out CharacterDataInt32 result)
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

        public async UniTask CreateCharacterDataInt32(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataInt32 characterDataInt32)
        {
            string id = ZString.Concat(characterId, "_", characterDataInt32.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Int32 {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@hashedKey", characterDataInt32.hashedKey),
                new NpgsqlParameter("@value", characterDataInt32.value));
        }

        public async UniTask<List<CharacterDataInt32>> ReadCharacterDataInt32s(string tableName, string characterId, List<CharacterDataInt32> result = null)
        {
            if (result == null)
                result = new List<CharacterDataInt32>();
            await ExecuteReader((reader) =>
            {
                CharacterDataInt32 tempData;
                while (ReadCharacterDataInt32(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterDataInt32s(NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif