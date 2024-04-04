#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterSkill(NpgsqlDataReader reader, out CharacterSkill result)
        {
            if (reader.Read())
            {
                result = new CharacterSkill();
                result.dataId = reader.GetInt32(0);
                result.level = reader.GetInt32(1);
                return true;
            }
            result = CharacterSkill.Empty;
            return false;
        }

        public async UniTask CreateCharacterSkill(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterSkill characterSkill)
        {
            string id = ZString.Concat(characterId, "_", characterSkill.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Skill {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterskill (id, characterId, dataId, level) VALUES (@id, @characterId, @dataId, @level)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@dataId", characterSkill.dataId),
                new NpgsqlParameter("@level", characterSkill.level));
        }

        public async UniTask<List<CharacterSkill>> ReadCharacterSkills(string characterId, List<CharacterSkill> result = null)
        {
            if (result == null)
                result = new List<CharacterSkill>();
            await ExecuteReader((reader) =>
            {
                CharacterSkill tempSkill;
                while (ReadCharacterSkill(reader, out tempSkill))
                {
                    result.Add(tempSkill);
                }
            }, "SELECT dataId, level FROM characterskill WHERE characterId=@characterId ORDER BY id ASC",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterSkills(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterskill WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif