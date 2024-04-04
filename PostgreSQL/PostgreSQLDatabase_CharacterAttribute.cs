#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterAttribute(NpgsqlDataReader reader, out CharacterAttribute result)
        {
            if (reader.Read())
            {
                result = new CharacterAttribute();
                result.dataId = reader.GetInt32(0);
                result.amount = reader.GetInt32(1);
                return true;
            }
            result = CharacterAttribute.Empty;
            return false;
        }

        public async UniTask CreateCharacterAttribute(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterAttribute characterAttribute)
        {
            string id = ZString.Concat(characterId, "_", characterAttribute.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Attribute {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterattribute (id, characterId, dataId, amount) VALUES (@id, @characterId, @dataId, @amount)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@dataId", characterAttribute.dataId),
                new NpgsqlParameter("@amount", characterAttribute.amount));
        }

        public async UniTask<List<CharacterAttribute>> ReadCharacterAttributes(string characterId, List<CharacterAttribute> result = null)
        {
            if (result == null)
                result = new List<CharacterAttribute>();
            await ExecuteReader((reader) =>
            {
                CharacterAttribute tempAttribute;
                while (ReadCharacterAttribute(reader, out tempAttribute))
                {
                    result.Add(tempAttribute);
                }
            }, "SELECT dataId, amount FROM characterattribute WHERE characterId=@characterId ORDER BY id ASC",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterAttributes(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterattribute WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif