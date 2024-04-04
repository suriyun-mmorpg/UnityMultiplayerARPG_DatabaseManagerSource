#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterBuff(NpgsqlDataReader reader, out CharacterBuff result)
        {
            if (reader.Read())
            {
                result = new CharacterBuff();
                result.id = reader.GetString(0);
                result.type = (BuffType)reader.GetByte(1);
                result.dataId = reader.GetInt32(2);
                result.level = reader.GetInt32(3);
                result.buffRemainsDuration = reader.GetFloat(4);
                return true;
            }
            result = CharacterBuff.Empty;
            return false;
        }

        public async UniTask CreateCharacterBuff(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterBuff characterBuff)
        {
            string id = characterBuff.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Buff {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterbuff (id, characterId, type, dataId, level, buffRemainsDuration) VALUES (@id, @characterId, @type, @dataId, @level, @buffRemainsDuration)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@type", (byte)characterBuff.type),
                new NpgsqlParameter("@dataId", characterBuff.dataId),
                new NpgsqlParameter("@level", characterBuff.level),
                new NpgsqlParameter("@buffRemainsDuration", characterBuff.buffRemainsDuration));
        }

        public async UniTask<List<CharacterBuff>> ReadCharacterBuffs(string characterId, List<CharacterBuff> result = null)
        {
            if (result == null)
                result = new List<CharacterBuff>();
            await ExecuteReader((reader) =>
            {
                CharacterBuff tempBuff;
                while (ReadCharacterBuff(reader, out tempBuff))
                {
                    result.Add(tempBuff);
                }
            }, "SELECT id, type, dataId, level, buffRemainsDuration FROM characterbuff WHERE characterId=@characterId ORDER BY buffRemainsDuration ASC",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterBuffs(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterbuff WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif