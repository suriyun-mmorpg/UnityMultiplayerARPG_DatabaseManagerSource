#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterSkillUsage(NpgsqlDataReader reader, out CharacterSkillUsage result)
        {
            if (reader.Read())
            {
                result = new CharacterSkillUsage();
                result.type = (SkillUsageType)reader.GetByte(0);
                result.dataId = reader.GetInt32(1);
                result.coolDownRemainsDuration = reader.GetFloat(2);
                return true;
            }
            result = CharacterSkillUsage.Empty;
            return false;
        }

        public async UniTask CreateCharacterSkillUsage(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterSkillUsage characterSkillUsage)
        {
            string id = ZString.Concat(characterId, "_", (int)characterSkillUsage.type, "_", characterSkillUsage.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Skill usage {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterskillusage (id, characterId, type, dataId, coolDownRemainsDuration) VALUES (@id, @characterId, @type, @dataId, @coolDownRemainsDuration)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@type", (byte)characterSkillUsage.type),
                new NpgsqlParameter("@dataId", characterSkillUsage.dataId),
                new NpgsqlParameter("@coolDownRemainsDuration", characterSkillUsage.coolDownRemainsDuration));
        }

        public async UniTask<List<CharacterSkillUsage>> ReadCharacterSkillUsages(string characterId, List<CharacterSkillUsage> result = null)
        {
            if (result == null)
                result = new List<CharacterSkillUsage>();
            await ExecuteReader((reader) =>
            {
                CharacterSkillUsage tempSkillUsage;
                while (ReadCharacterSkillUsage(reader, out tempSkillUsage))
                {
                    result.Add(tempSkillUsage);
                }
            }, "SELECT type, dataId, coolDownRemainsDuration FROM characterskillusage WHERE characterId=@characterId ORDER BY coolDownRemainsDuration ASC",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterSkillUsages(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterskillusage WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif