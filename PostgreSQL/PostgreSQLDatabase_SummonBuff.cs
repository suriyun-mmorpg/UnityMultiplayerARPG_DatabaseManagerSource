#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NpgsqlTypes;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_SET_SUMMON_BUFFS_UPDATE = "SET_SUMMON_BUFFS_UPDATE";
        public const string CACHE_KEY_SET_SUMMON_BUFFS_INSERT = "SET_SUMMON_BUFFS_INSERT";
        public async UniTask SetSummonBuffs(string characterId, IList<CharacterBuff> characterBuffs)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            int count = await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_SET_SUMMON_BUFFS_UPDATE,
                connection, null,
                "character_summon_buffs",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterBuffs)),
                },
                new[]
                {
                    PostgreSQLHelpers.WhereEqualTo("id", characterId),
                });
            if (count <= 0)
            {
                await PostgreSQLHelpers.ExecuteInsert(
                    CACHE_KEY_SET_SUMMON_BUFFS_INSERT,
                    connection, null,
                    "character_summon_buffs",
                    new PostgreSQLHelpers.ColumnInfo("id", characterId),
                    new PostgreSQLHelpers.ColumnInfo(NpgsqlDbType.Jsonb, "data", JsonConvert.SerializeObject(characterBuffs)));
            }
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
