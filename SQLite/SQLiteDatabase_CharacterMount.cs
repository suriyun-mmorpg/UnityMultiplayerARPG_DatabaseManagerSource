﻿#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        public void CreateOrUpdateCharacterMount(SqliteTransaction transaction, string characterId, CharacterMount characterMount)
        {
            ExecuteNonQuery(transaction, @"INSERT INTO charactermount 
                (id, type, dataId, mountRemainsDuration, level, exp, currentHp, currentMp) VALUES 
                (@id, @type, @dataId, @mountRemainsDuration, @level, @exp, @currentHp, @currentMp)
                ON CONFLICT(id) DO UPDATE SET
                type = @type,
                dataId = @dataId,
                mountRemainsDuration = @mountRemainsDuration,
                level = @level,
                exp = @exp,
                currentHp = @currentHp,
                currentMp = @currentMp",
                new SqliteParameter("@id", characterId),
                new SqliteParameter("@type", (byte)characterMount.type),
                new SqliteParameter("@dataId", characterMount.dataId),
                new SqliteParameter("@mountRemainsDuration", characterMount.mountRemainsDuration),
                new SqliteParameter("@level", characterMount.level),
                new SqliteParameter("@exp", characterMount.exp),
                new SqliteParameter("@currentHp", characterMount.currentHp),
                new SqliteParameter("@currentMp", characterMount.currentMp));
        }

        public void DeleteCharacterMount(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM charactermount WHERE id=@id", new SqliteParameter("@id", characterId));
        }
    }
}
#endif