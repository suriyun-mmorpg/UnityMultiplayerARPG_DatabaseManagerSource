#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public async UniTask CreateSummonBuff(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterBuff summonBuff)
        {
            string id = summonBuff.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Summon buff {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO summonbuffs (id, characterId, buffId, type, dataId, level, buffRemainsDuration) VALUES (@id, @characterId, @buffId, @type, @dataId, @level, @buffRemainsDuration)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@buffId", summonBuff.id),
                new NpgsqlParameter("@type", (byte)summonBuff.type),
                new NpgsqlParameter("@dataId", summonBuff.dataId),
                new NpgsqlParameter("@level", summonBuff.level),
                new NpgsqlParameter("@buffRemainsDuration", summonBuff.buffRemainsDuration));
        }

        private bool ReadSummonBuff(NpgsqlDataReader reader, out CharacterBuff result)
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

        public async UniTask DeleteSummonBuff(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM summonbuffs WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }

        public override async UniTask<List<CharacterBuff>> GetSummonBuffs(string characterId)
        {
            List<CharacterBuff> result = new List<CharacterBuff>();
            await ExecuteReader((reader) =>
            {
                CharacterBuff tempBuff;
                while (ReadSummonBuff(reader, out tempBuff))
                {
                    result.Add(tempBuff);
                }
            }, "SELECT buffId, type, dataId, level, buffRemainsDuration FROM summonbuffs WHERE characterId=@characterId ORDER BY buffRemainsDuration ASC",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }
    }
}
#endif
