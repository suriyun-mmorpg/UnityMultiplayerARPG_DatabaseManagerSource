#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_FILL_SUMMON_BUFFS = "FILL_SUMMON_BUFFS";
        public async UniTask FillSummonBuffs(NpgsqlConnection connection, NpgsqlTransaction transaction, string characterId, IList<CharacterBuff> characterBuffs)
        {
            await PostgreSQLHelpers.ExecuteUpsert(
                CACHE_KEY_FILL_SUMMON_BUFFS,
                connection, transaction,
                "character_summon_buffs",
                "id",
                new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterBuffs)),
                new PostgreSQLHelpers.ColumnInfo("id", characterId));
        }

        public const string CACHE_KEY_GET_SUMMON_BUFFS = "GET_SUMMON_BUFFS";
        public override async UniTask<List<CharacterBuff>> GetSummonBuffs(string characterId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var reader = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_GET_SUMMON_BUFFS,
                connection, null,
                "character_summon_buffs", "data",
                PostgreSQLHelpers.WhereEqualTo("id", characterId));
            List<CharacterBuff> result;
            if (reader.Read())
            {
                result = JsonConvert.DeserializeObject<List<CharacterBuff>>(reader.GetString(0));
            }
            else
            {
                result = new List<CharacterBuff>();
            }
            return result;
        }
    }
}
#endif
