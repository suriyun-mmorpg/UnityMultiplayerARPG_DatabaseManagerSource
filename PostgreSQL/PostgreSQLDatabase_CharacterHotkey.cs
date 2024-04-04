#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        private bool ReadCharacterHotkey(NpgsqlDataReader reader, out CharacterHotkey result)
        {
            if (reader.Read())
            {
                result = new CharacterHotkey();
                result.hotkeyId = reader.GetString(0);
                result.type = (HotkeyType)reader.GetByte(1);
                result.relateId = reader.GetString(2);
                return true;
            }
            result = CharacterHotkey.Empty;
            return false;
        }

        public async UniTask CreateCharacterHotkey(NpgsqlConnection connection, NpgsqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterHotkey characterHotkey)
        {
            string id = ZString.Concat(characterId, "_", characterHotkey.hotkeyId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Hotkey {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterhotkey (id, characterId, hotkeyId, type, relateId) VALUES (@id, @characterId, @hotkeyId, @type, @relateId)",
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@hotkeyId", characterHotkey.hotkeyId),
                new NpgsqlParameter("@type", characterHotkey.type),
                new NpgsqlParameter("@relateId", characterHotkey.relateId));
        }

        public async UniTask<List<CharacterHotkey>> ReadCharacterHotkeys(string characterId, List<CharacterHotkey> result = null)
        {
            if (result == null)
                result = new List<CharacterHotkey>();
            await ExecuteReader((reader) =>
            {
                CharacterHotkey tempHotkey;
                while (ReadCharacterHotkey(reader, out tempHotkey))
                {
                    result.Add(tempHotkey);
                }
            }, "SELECT hotkeyId, type, relateId FROM characterhotkey WHERE characterId=@characterId",
                new NpgsqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterHotkeys(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterhotkey WHERE characterId=@characterId", new NpgsqlParameter("@characterId", characterId));
        }
    }
}
#endif