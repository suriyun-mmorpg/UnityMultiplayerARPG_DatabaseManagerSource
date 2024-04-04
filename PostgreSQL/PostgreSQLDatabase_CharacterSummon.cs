#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterSummon(NpgsqlDataReader reader, out CharacterSummon result)
        {
            if (reader.Read())
            {
                result = new CharacterSummon();
                result.type = (SummonType)reader.GetByte(0);
                result.dataId = reader.GetInt32(1);
                result.summonRemainsDuration = reader.GetFloat(2);
                result.level = reader.GetInt32(3);
                result.exp = reader.GetInt32(4);
                result.currentHp = reader.GetInt32(5);
                result.currentMp = reader.GetInt32(6);
                return true;
            }
            result = CharacterSummon.Empty;
            return false;
        }

        public async UniTask CreateCharacterSummon(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterSummon characterSummon)
        {
            string id = ZString.Concat(characterId, "_", (int)characterSummon.type, "_", idx);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Summon {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO charactersummon (id, characterId, type, dataId, summonRemainsDuration, level, exp, currentHp, currentMp) VALUES (@id, @characterId, @type, @dataId, @summonRemainsDuration, @level, @exp, @currentHp, @currentMp)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@type", (byte)characterSummon.type),
                new NpgsqlParameter("@dataId", characterSummon.dataId),
                new NpgsqlParameter("@summonRemainsDuration", characterSummon.summonRemainsDuration),
                new NpgsqlParameter("@level", characterSummon.level),
                new NpgsqlParameter("@exp", characterSummon.exp),
                new NpgsqlParameter("@currentHp", characterSummon.currentHp),
                new NpgsqlParameter("@currentMp", characterSummon.currentMp));
        }

        public async UniTask<List<CharacterSummon>> ReadCharacterSummons(string characterId, List<CharacterSummon> result = null)
        {
            if (result == null)
                result = new List<CharacterSummon>();
            await ExecuteReader((reader) =>
            {
                CharacterSummon tempSummon;
                while (ReadCharacterSummon(reader, out tempSummon))
                {
                    result.Add(tempSummon);
                }
            }, "SELECT type, dataId, summonRemainsDuration, level, exp, currentHp, currentMp FROM charactersummon WHERE characterId=@characterId ORDER BY type DESC",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterSummons(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM charactersummon WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif