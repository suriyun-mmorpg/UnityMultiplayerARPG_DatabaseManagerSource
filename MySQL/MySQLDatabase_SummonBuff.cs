#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        public async UniTask CreateSummonBuff(MySqlConnection connection, MySqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterBuff summonBuff)
        {
            string id = summonBuff.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Summon buff {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO summonbuffs (id, characterId, buffId, type, dataId, level, buffRemainsDuration) VALUES (@id, @characterId, @buffId, @type, @dataId, @level, @buffRemainsDuration)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@buffId", summonBuff.id),
                new MySqlParameter("@type", (byte)summonBuff.type),
                new MySqlParameter("@dataId", summonBuff.dataId),
                new MySqlParameter("@level", summonBuff.level),
                new MySqlParameter("@buffRemainsDuration", summonBuff.buffRemainsDuration));
        }

        private bool ReadSummonBuff(MySqlDataReader reader, out CharacterBuff result)
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

        public async UniTask DeleteSummonBuff(MySqlConnection connection, MySqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM summonbuffs WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
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
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public override async UniTask UpdateSummonBuffs(string characterId, List<CharacterBuff> summonBuffs)
        {
            using MySqlConnection connection = NewConnection();
            await OpenConnection(connection);
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                await DeleteSummonBuff(connection, transaction, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < summonBuffs.Count; ++i)
                {
                    await CreateSummonBuff(connection, transaction, insertedIds, characterId, summonBuffs[i]);
                }
                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing buffs of summon: " + characterId);
                LogException(LogTag, ex);
                transaction.Rollback();
            }
        }
    }
}
#endif
