#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterQuest(NpgsqlDataReader reader, out CharacterQuest result)
        {
            if (reader.Read())
            {
                result = new CharacterQuest();
                result.dataId = reader.GetInt32(0);
                result.randomTasksIndex = reader.GetByte(1);
                result.isComplete = reader.GetBoolean(2);
                result.completeTime = reader.GetInt64(3);
                result.isTracking = reader.GetBoolean(4);
                result.ReadKilledMonsters(reader.GetString(5));
                result.ReadCompletedTasks(reader.GetString(6));
                return true;
            }
            result = CharacterQuest.Empty;
            return false;
        }

        public async UniTask CreateCharacterQuest(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterQuest characterQuest)
        {
            string id = ZString.Concat(characterId, "_", characterQuest.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Quest {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterquest (id, characterId, dataId, randomTasksIndex, isComplete, completeTime, isTracking, killedMonsters, completedTasks) VALUES (@id, @characterId, @dataId, @randomTasksIndex, @isComplete, @completeTime, @isTracking, @killedMonsters, @completedTasks)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@dataId", characterQuest.dataId),
                new NpgsqlParameter("@randomTasksIndex", characterQuest.randomTasksIndex),
                new NpgsqlParameter("@isComplete", characterQuest.isComplete),
                new NpgsqlParameter("@completeTime", characterQuest.completeTime),
                new NpgsqlParameter("@isTracking", characterQuest.isTracking),
                new NpgsqlParameter("@killedMonsters", characterQuest.WriteKilledMonsters()),
                new NpgsqlParameter("@completedTasks", characterQuest.WriteCompletedTasks()));
        }

        public async UniTask<List<CharacterQuest>> ReadCharacterQuests(string characterId, List<CharacterQuest> result = null)
        {
            if (result == null)
                result = new List<CharacterQuest>();
            await ExecuteReader((reader) =>
            {
                CharacterQuest tempQuest;
                while (ReadCharacterQuest(reader, out tempQuest))
                {
                    result.Add(tempQuest);
                }
            }, "SELECT dataId, randomTasksIndex, isComplete, completeTime, isTracking, killedMonsters, completedTasks FROM characterquest WHERE characterId=@characterId ORDER BY id ASC",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterQuests(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterquest WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif