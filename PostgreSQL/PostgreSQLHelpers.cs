#if NET || NETCOREAPP
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Concurrent;

namespace MultiplayerARPG.MMO
{
    public static class PostgreSQLHelpers
    {
        public struct ColumnInfo
        {
            public NpgsqlDbType type;
            public string name;
            public object value;
        }

        public struct WhereQuery
        {
            public NpgsqlDbType type;
            public string name;
            public object value;
            public string preOperators;
            public string operators;
        }

        public delegate void WriteWhereDelegate(ref Utf16ValueStringBuilder builder, ref int positionCount);

        public enum EOperators
        {
            None,
            EqualTo,
            LessThan,
            GreaterThan,
            LessThanOrEqualTo,
            GreaterThanOrEqualTo,
            NotEqualTo,
            Like,
            And,
            Or,
            In,
            Between,
            IsNull,
            Not,
        }

        public const string OPERATOR_EQUAL_TO = " = ";
        public const string OPERATOR_LESS_THAN = " < ";
        public const string OPERATOR_GREATER_THAN = " > ";
        public const string OPERATOR_LESS_THAN_OR_EQUAL_TO = " <= ";
        public const string OPERATOR_GREATER_THAN_EQUAL_TO = " >= ";
        public const string OPERATOR_NOT_EQUAL_TO = " != ";
        public const string OPERATOR_LIKE = " LIKE ";
        public const string OPERATOR_AND = " AND ";
        public const string OPERATOR_OR = " OR ";
        public const string OPERATOR_IN = " IN ";
        public const string OPERATOR_BETWEEN = " BETWEEN ";
        public const string OPERATOR_IS_NULL = " IS NULL ";
        public const string OPERATOR_NOT = " NOT ";

        private static readonly ConcurrentDictionary<string, string> s_cachedCommandTexts = new ConcurrentDictionary<string, string>();
        private static Utf16ValueStringBuilder s_stringBuilder = new Utf16ValueStringBuilder(false);

        public static WhereQuery CreateWhereQuery(NpgsqlDbType type, string name, object value, IList<EOperators> preOperators, IList<EOperators> operators)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            for (int i = 0; i < preOperators.Count; ++i)
            {
                AppendOperatorString(preOperators[i], ref builder);
            }
            result.preOperators = builder.ToString();
            builder.Clear();
            for (int i = 0; i < operators.Count; ++i)
            {
                AppendOperatorString(operators[i], ref builder);
            }
            result.operators = builder.ToString();
            builder.Dispose();
            return result;
        }

        public static void AppendOperatorString(EOperators operators, ref Utf16ValueStringBuilder builder)
        {
            switch (operators)
            {
                case EOperators.EqualTo:
                    builder.Append(OPERATOR_EQUAL_TO);
                    break;
                case EOperators.LessThan:
                    builder.Append(OPERATOR_LESS_THAN);
                    break;
                case EOperators.GreaterThan:
                    builder.Append(OPERATOR_GREATER_THAN);
                    break;
                case EOperators.LessThanOrEqualTo:
                    builder.Append(OPERATOR_LESS_THAN_OR_EQUAL_TO);
                    break;
                case EOperators.GreaterThanOrEqualTo:
                    builder.Append(OPERATOR_GREATER_THAN_EQUAL_TO);
                    break;
                case EOperators.NotEqualTo:
                    builder.Append(OPERATOR_NOT_EQUAL_TO);
                    break;
                case EOperators.Like:
                    builder.Append(OPERATOR_LIKE);
                    break;
                case EOperators.And:
                    builder.Append(OPERATOR_AND);
                    break;
                case EOperators.Or:
                    builder.Append(OPERATOR_OR);
                    break;
                case EOperators.In:
                    builder.Append(OPERATOR_IN);
                    break;
                case EOperators.Between:
                    builder.Append(OPERATOR_BETWEEN);
                    break;
                case EOperators.IsNull:
                    builder.Append(OPERATOR_IS_NULL);
                    break;
                case EOperators.Not:
                    builder.Append(OPERATOR_NOT);
                    break;
            }
        }

        public static string CreateInsertCommandText(string cacheKey, string tableName, params ColumnInfo[] columns)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(cacheKey);
            if (!isNullOrWhiteSpace && s_cachedCommandTexts.TryGetValue(cacheKey, out string commandText))
                return commandText;
            s_stringBuilder.Clear();
            s_stringBuilder.Append("INSERT INTO ");
            s_stringBuilder.Append(tableName);
            s_stringBuilder.Append(' ');
            s_stringBuilder.Append('(');
            int i;
            for (i = 0; i < columns.Length; ++i)
            {
                if (i > 0)
                    s_stringBuilder.Append(',');
                s_stringBuilder.Append(columns[i].name);
            }
            s_stringBuilder.Append(") VALUES (");
            for (i = 0; i < columns.Length; ++i)
            {
                if (i > 0)
                    s_stringBuilder.Append(',');
                s_stringBuilder.Append('$');
                s_stringBuilder.Append(i + 1);
            }
            s_stringBuilder.Append(')');
            commandText = s_stringBuilder.ToString();
            if (!isNullOrWhiteSpace)
                s_cachedCommandTexts.TryAdd(cacheKey, commandText);
            return commandText;
        }

        public static string CreateUpdateCommandText(string cacheKey, string tableName, IList<ColumnInfo> updates, IList<WhereQuery> wheres, string additional)
        {
            return CreateUpdateCommandText(cacheKey, tableName, updates, (ref Utf16ValueStringBuilder builder, ref int positionCount) => WriteWhere(wheres, additional, ref builder, ref positionCount));
        }

        public static string CreateUpdateCommandText(string cacheKey, string tableName, IList<ColumnInfo> updates, WriteWhereDelegate writeWhere)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(cacheKey);
            if (!isNullOrWhiteSpace && s_cachedCommandTexts.TryGetValue(cacheKey, out string commandText))
                return commandText;
            s_stringBuilder.Clear();
            s_stringBuilder.Append("UPDATE ");
            s_stringBuilder.Append(tableName);
            s_stringBuilder.Append(" SET ");
            int i;
            int positionCount = 1;
            for (i = 0; i < updates.Count; ++i)
            {
                if (i > 0)
                    s_stringBuilder.Append(',');
                s_stringBuilder.Append(updates[i].name);
                s_stringBuilder.Append('=');
                s_stringBuilder.Append('$');
                s_stringBuilder.Append(positionCount++);
            }
            s_stringBuilder.Append(" WHERE ");
            writeWhere?.Invoke(ref s_stringBuilder, ref positionCount);
            commandText = s_stringBuilder.ToString();
            if (!isNullOrWhiteSpace)
                s_cachedCommandTexts.TryAdd(cacheKey, commandText);
            return commandText;
        }

        public static string CreateSelectCommandText(string cacheKey, string tableName, IList<WhereQuery> wheres, string select = "*", string additional = "")
        {
            return CreateSelectCommandText(cacheKey, tableName, (ref Utf16ValueStringBuilder builder, ref int positionCount) => WriteWhere(wheres, additional, ref builder, ref positionCount), select);
        }

        public static string CreateSelectCommandText(string cacheKey, string tableName, WriteWhereDelegate writeWhere, string select = "*")
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(cacheKey);
            if (!isNullOrWhiteSpace && s_cachedCommandTexts.TryGetValue(cacheKey, out string commandText))
                return commandText;
            s_stringBuilder.Clear();
            s_stringBuilder.Append("SELECT ");
            s_stringBuilder.Append(select);
            s_stringBuilder.Append(" FROM ");
            s_stringBuilder.Append(tableName);
            int positionCount = 1;
            s_stringBuilder.Append(" WHERE ");
            writeWhere?.Invoke(ref s_stringBuilder, ref positionCount);
            commandText = s_stringBuilder.ToString();
            if (!isNullOrWhiteSpace)
                s_cachedCommandTexts.TryAdd(cacheKey, commandText);
            return commandText;
        }

        public static string CreateDeleteCommandText(string cacheKey, string tableName, IList<WhereQuery> wheres, string additional = "")
        {
            return CreateDeleteCommandText(cacheKey, tableName, (ref Utf16ValueStringBuilder builder, ref int positionCount) => WriteWhere(wheres, additional, ref builder, ref positionCount));
        }

        public static string CreateDeleteCommandText(string cacheKey, string tableName, WriteWhereDelegate writeWhere)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(cacheKey);
            if (!isNullOrWhiteSpace && s_cachedCommandTexts.TryGetValue(cacheKey, out string commandText))
                return commandText;
            s_stringBuilder.Clear();
            s_stringBuilder.Append("DELETE FROM ");
            s_stringBuilder.Append(tableName);
            int positionCount = 1;
            s_stringBuilder.Append(" WHERE ");
            writeWhere?.Invoke(ref s_stringBuilder, ref positionCount);
            commandText = s_stringBuilder.ToString();
            if (!isNullOrWhiteSpace)
                s_cachedCommandTexts.TryAdd(cacheKey, commandText);
            return commandText;
        }

        private static void WriteWhere(IList<WhereQuery> wheres, string additional, ref Utf16ValueStringBuilder builder, ref int positionCount)
        {
            for (int i = 0; i < wheres.Count; ++i)
            {
                builder.Append(wheres[i].preOperators);
                builder.Append(wheres[i].name);
                builder.Append(wheres[i].operators);
                builder.Append('$');
                builder.Append(positionCount++);
            }
            builder.Append(additional);
        }

        public static async UniTask<int> ExecuteInsert(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, params ColumnInfo[] columns)
        {
            return await ExecuteNonQuery(connection, transaction, CreateInsertCommandText(cacheKey, tableName, columns), columns);
        }

        public static async UniTask<int> ExecuteUpdate(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<ColumnInfo> updates, IList<WhereQuery> wheres, string additional)
        {
            return await ExecuteNonQuery(connection, transaction, CreateUpdateCommandText(cacheKey, tableName, updates, wheres, additional), updates, wheres);
        }

        public static async UniTask<NpgsqlDataReader> ExecuteSelect(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<WhereQuery> wheres, string additional)
        {
            return await ExecuteReader(connection, transaction, CreateSelectCommandText(cacheKey, tableName, wheres, additional), wheres);
        }

        public static async UniTask<int> ExecuteDelete(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<WhereQuery> wheres, string additional)
        {
            return await ExecuteNonQuery(connection, transaction, CreateDeleteCommandText(cacheKey, tableName, wheres, additional), wheres);
        }

        public static async UniTask<int> ExecuteNonQuery(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, IList<ColumnInfo> columns)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection, transaction);
            int i;
            for (i = 0; i < columns.Count; ++i)
            {
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = columns[i].type });
            }
            await cmd.PrepareAsync();
            for (i = 0; i < columns.Count; ++i)
            {
                cmd.Parameters[0].Value = columns[i].value;
            }
            int result = await cmd.ExecuteNonQueryAsync();
            cmd.Dispose();
            return result;
        }

        public static async UniTask<int> ExecuteNonQuery(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, IList<WhereQuery> wheres)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection, transaction);
            int i;
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = wheres[i].type });
            }
            await cmd.PrepareAsync();
            int positionCount = 1;
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters[positionCount++].Value = wheres[i].value;
            }
            int result = await cmd.ExecuteNonQueryAsync();
            cmd.Dispose();
            return result;
        }

        public static async UniTask<int> ExecuteNonQuery(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, IList<ColumnInfo> updates, IList<WhereQuery> wheres)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection, transaction);
            int i;
            for (i = 0; i < updates.Count; ++i)
            {
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = updates[i].type });
            }
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = wheres[i].type });
            }
            await cmd.PrepareAsync();
            int positionCount = 1;
            for (i = 0; i < updates.Count; ++i)
            {
                cmd.Parameters[positionCount++].Value = updates[i].value;
            }
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters[positionCount++].Value = wheres[i].value;
            }
            int result = await cmd.ExecuteNonQueryAsync();
            cmd.Dispose();
            return result;
        }

        public static async UniTask<NpgsqlDataReader> ExecuteReader(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, IList<WhereQuery> wheres)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection, transaction);
            int i;
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = wheres[i].type });
            }
            await cmd.PrepareAsync();
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters[0].Value = wheres[i].value;
            }
            NpgsqlDataReader result = await cmd.ExecuteReaderAsync();
            cmd.Dispose();
            return result;
        }

        public static async UniTask<NpgsqlDataReader> ExecuteReader(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, IList<ColumnInfo> updates, IList<WhereQuery> wheres)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection, transaction);
            int i;
            for (i = 0; i < updates.Count; ++i)
            {
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = updates[i].type });
            }
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = wheres[i].type });
            }
            await cmd.PrepareAsync();
            int positionCount = 1;
            for (i = 0; i < updates.Count; ++i)
            {
                cmd.Parameters[positionCount++].Value = updates[i].value;
            }
            for (i = 0; i < wheres.Count; ++i)
            {
                cmd.Parameters[positionCount++].Value = wheres[i].value;
            }
            NpgsqlDataReader result = await cmd.ExecuteReaderAsync();
            cmd.Dispose();
            return result;
        }
    }
}
#endif