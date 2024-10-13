#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        public async UniTask CreateOrUpdateCharacterMount(MySqlConnection connection, MySqlTransaction transaction, string characterId, CharacterMount characterMount)
        {
            await ExecuteNonQuery(connection, transaction, @"INSERT INTO charactermount 
                (id, type, dataId, mountRemainsDuration, level, exp, currentHp, currentMp) VALUES 
                (@id, @type, @dataId, @mountRemainsDuration, @level, @exp, @currentHp, @currentMp)
                ON DUPLICATE KEY UPDATE
                type = @type,
                dataId = @dataId,
                mountRemainsDuration = @mountRemainsDuration,
                level = @level,
                exp = @exp,
                currentHp = @currentHp,
                currentMp = @currentMp",
                new MySqlParameter("@id", characterId),
                new MySqlParameter("@type", (byte)characterMount.type),
                new MySqlParameter("@dataId", characterMount.dataId),
                new MySqlParameter("@mountRemainsDuration", characterMount.mountRemainsDuration),
                new MySqlParameter("@level", characterMount.level),
                new MySqlParameter("@exp", characterMount.exp),
                new MySqlParameter("@currentHp", characterMount.currentHp),
                new MySqlParameter("@currentMp", characterMount.currentMp));
        }

        public async UniTask DeleteCharacterMount(MySqlConnection connection, MySqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM charactermount WHERE id=@id", new MySqlParameter("@id", characterId));
        }
    }
}
#endif