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

            public ColumnInfo(NpgsqlDbType type, string name, object value)
            {
                this.type = type;
                this.name = name;
                this.value = value;
            }

            public ColumnInfo(string name, string value)
            {
                this.type = NpgsqlDbType.Varchar;
                this.name = name;
                this.value = value;
            }

            public ColumnInfo(string name, int value)
            {
                this.type = NpgsqlDbType.Integer;
                this.name = name;
                this.value = value;
            }

            public ColumnInfo(string name, long value)
            {
                this.type = NpgsqlDbType.Bigint;
                this.name = name;
                this.value = value;
            }

            public ColumnInfo(string name, short value)
            {
                this.type = NpgsqlDbType.Smallint;
                this.name = name;
                this.value = value;
            }

            public ColumnInfo(string name, float value)
            {
                this.type = NpgsqlDbType.Real;
                this.name = name;
                this.value = value;
            }

            public ColumnInfo(string name, double value)
            {
                this.type = NpgsqlDbType.Double;
                this.name = name;
                this.value = value;
            }

            public ColumnInfo(string name, bool value)
            {
                this.type = NpgsqlDbType.Boolean;
                this.name = name;
                this.value = value;
            }
        }

        public struct WhereQuery
        {
            public NpgsqlDbType type;
            public string name;
            public object value;
            public string preOperators;
            public string operators;

            public WhereQuery(NpgsqlDbType type, string name, object value, string preOperators, string operators)
            {
                this.type = type;
                this.name = name;
                this.value = value;
                this.preOperators = preOperators;
                this.operators = operators;
            }
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

        public static WhereQuery Where(NpgsqlDbType type, IList<EOperators> preOperators, string name, IList<EOperators> operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            if (preOperators != null && preOperators.Count > 0)
            {
                for (int i = 0; i < preOperators.Count; ++i)
                {
                    AppendOperatorString(preOperators[i], ref builder);
                }
                result.preOperators = builder.ToString();
            }
            builder.Clear();
            if (operators != null && operators.Count > 0)
            {
                for (int i = 0; i < operators.Count; ++i)
                {
                    AppendOperatorString(operators[i], ref builder);
                }
                result.operators = builder.ToString();
            }
            builder.Dispose();
            return result;
        }

        public static WhereQuery Where(NpgsqlDbType type, IList<EOperators> preOperators, string name, EOperators operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            if (preOperators != null && preOperators.Count > 0)
            {
                for (int i = 0; i < preOperators.Count; ++i)
                {
                    AppendOperatorString(preOperators[i], ref builder);
                }
                result.preOperators = builder.ToString();
            }
            builder.Clear();
            AppendOperatorString(operators, ref builder);
            result.operators = builder.ToString();
            builder.Dispose();
            return result;
        }

        public static WhereQuery Where(NpgsqlDbType type, EOperators preOperators, string name, IList<EOperators> operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            AppendOperatorString(preOperators, ref builder);
            result.preOperators = builder.ToString();
            builder.Clear();
            if (operators != null && operators.Count > 0)
            {
                for (int i = 0; i < operators.Count; ++i)
                {
                    AppendOperatorString(operators[i], ref builder);
                }
                result.operators = builder.ToString();
            }
            builder.Dispose();
            return result;
        }

        public static WhereQuery Where(NpgsqlDbType type, EOperators preOperators, string name, EOperators operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            AppendOperatorString(preOperators, ref builder);
            result.preOperators = builder.ToString();
            builder.Clear();
            AppendOperatorString(operators, ref builder);
            result.operators = builder.ToString();
            builder.Dispose();
            return result;
        }

        public static WhereQuery Where(NpgsqlDbType type, string name, IList<EOperators> operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            builder.Clear();
            if (operators != null && operators.Count > 0)
            {
                for (int i = 0; i < operators.Count; ++i)
                {
                    AppendOperatorString(operators[i], ref builder);
                }
                result.operators = builder.ToString();
            }
            builder.Dispose();
            return result;
        }

        public static WhereQuery Where(NpgsqlDbType type, string name, EOperators operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            AppendOperatorString(operators, ref builder);
            result.operators = builder.ToString();
            builder.Dispose();
            return result;
        }

        public static WhereQuery AndWhere(NpgsqlDbType type, string name, IList<EOperators> operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            AppendOperatorString(EOperators.And, ref builder);
            result.preOperators = builder.ToString();
            builder.Clear();
            if (operators != null && operators.Count > 0)
            {
                for (int i = 0; i < operators.Count; ++i)
                {
                    AppendOperatorString(operators[i], ref builder);
                }
                result.operators = builder.ToString();
            }
            builder.Dispose();
            return result;
        }

        public static WhereQuery AndWhere(NpgsqlDbType type, string name, EOperators operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            AppendOperatorString(EOperators.And, ref builder);
            result.preOperators = builder.ToString();
            builder.Clear();
            AppendOperatorString(operators, ref builder);
            result.operators = builder.ToString();
            builder.Dispose();
            return result;
        }

        public static WhereQuery OrWhere(NpgsqlDbType type, string name, IList<EOperators> operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            AppendOperatorString(EOperators.Or, ref builder);
            result.preOperators = builder.ToString();
            builder.Clear();
            if (operators != null && operators.Count > 0)
            {
                for (int i = 0; i < operators.Count; ++i)
                {
                    AppendOperatorString(operators[i], ref builder);
                }
                result.operators = builder.ToString();
            }
            builder.Dispose();
            return result;
        }

        public static WhereQuery OrWhere(NpgsqlDbType type, string name, EOperators operators, object value)
        {
            var result = new WhereQuery()
            {
                type = type,
                name = name,
                value = value,
            };
            var builder = new Utf16ValueStringBuilder(false);
            AppendOperatorString(EOperators.Or, ref builder);
            result.preOperators = builder.ToString();
            builder.Clear();
            AppendOperatorString(operators, ref builder);
            result.operators = builder.ToString();
            builder.Dispose();
            return result;
        }

        public static WhereQuery WhereEqualTo(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, name, EOperators.EqualTo, value);
        }

        public static WhereQuery AndWhereEqualTo(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, EOperators.And, name, EOperators.EqualTo, value);
        }

        public static WhereQuery OrWhereEqualTo(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, EOperators.Or, name, EOperators.EqualTo, value);
        }

        public static WhereQuery WhereNotEqualTo(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery AndWhereNotEqualTo(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, EOperators.And, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery OrWhereNotEqualTo(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, EOperators.Or, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery WhereLike(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, name, EOperators.Like, value);
        }

        public static WhereQuery AndWhereLike(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, EOperators.And, name, EOperators.Like, value);
        }

        public static WhereQuery OrWhereLike(string name, string value)
        {
            return Where(NpgsqlDbType.Varchar, EOperators.Or, name, EOperators.Like, value);
        }

        public static WhereQuery WhereEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, name, EOperators.EqualTo, value);
        }

        public static WhereQuery AndWhereEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.And, name, EOperators.EqualTo, value);
        }

        public static WhereQuery OrWhereEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.Or, name, EOperators.EqualTo, value);
        }

        public static WhereQuery WhereNotEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery AndWhereNotEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.And, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery OrWhereNotEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.Or, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery WhereLessThan(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, name, EOperators.LessThan, value);
        }

        public static WhereQuery AndWhereLessThan(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.And, name, EOperators.LessThan, value);
        }

        public static WhereQuery OrWhereLessThan(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.Or, name, EOperators.LessThan, value);
        }

        public static WhereQuery WhereGreaterThan(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery AndWhereGreaterThan(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.And, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery OrWhereGreaterThan(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.Or, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery WhereLessThanOrEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery AndWhereLessThanOrEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.And, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery OrWhereLessThanOrEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.Or, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery WhereGreaterThanOrEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery AndWhereGreaterThanOrEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.And, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery OrWhereGreaterThanOrEqualTo(string name, int value)
        {
            return Where(NpgsqlDbType.Integer, EOperators.Or, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery WhereBigEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, name, EOperators.EqualTo, value);
        }

        public static WhereQuery AndWhereBigEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.And, name, EOperators.EqualTo, value);
        }

        public static WhereQuery OrWhereBigEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.Or, name, EOperators.EqualTo, value);
        }

        public static WhereQuery WhereBigNotEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery AndWhereBigNotEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.And, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery OrWhereBigNotEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.Or, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery WhereBigLessThan(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, name, EOperators.LessThan, value);
        }

        public static WhereQuery AndWhereBigLessThan(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.And, name, EOperators.LessThan, value);
        }

        public static WhereQuery OrWhereBigLessThan(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.Or, name, EOperators.LessThan, value);
        }

        public static WhereQuery WhereBigGreaterThan(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery AndWhereBigGreaterThan(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.And, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery OrWhereBigGreaterThan(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.Or, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery WhereBigLessThanOrEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery AndWhereBigLessThanOrEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.And, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery OrWhereBigLessThanOrEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.Or, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery WhereBigGreaterThanOrEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery AndWhereBigGreaterThanOrEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.And, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery OrWhereBigGreaterThanOrEqualTo(string name, long value)
        {
            return Where(NpgsqlDbType.Bigint, EOperators.Or, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery WhereSmallEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, name, EOperators.EqualTo, value);
        }

        public static WhereQuery AndWhereSmallEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.And, name, EOperators.EqualTo, value);
        }

        public static WhereQuery OrWhereSmallEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.Or, name, EOperators.EqualTo, value);
        }

        public static WhereQuery WhereSmallNotEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery AndWhereSmallNotEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.And, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery OrWhereSmallNotEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.Or, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery WhereSmallLessThan(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, name, EOperators.LessThan, value);
        }

        public static WhereQuery AndWhereSmallLessThan(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.And, name, EOperators.LessThan, value);
        }

        public static WhereQuery OrWhereSmallLessThan(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.Or, name, EOperators.LessThan, value);
        }

        public static WhereQuery WhereSmallGreaterThan(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery AndWhereSmallGreaterThan(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.And, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery OrWhereSmallGreaterThan(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.Or, name, EOperators.GreaterThan, value);
        }

        public static WhereQuery WhereSmallLessThanOrEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery AndWhereSmallLessThanOrEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.And, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery OrWhereSmallLessThanOrEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.Or, name, EOperators.LessThanOrEqualTo, value);
        }

        public static WhereQuery WhereSmallGreaterThanOrEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery AndWhereSmallGreaterThanOrEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.And, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery OrWhereSmallGreaterThanOrEqualTo(string name, short value)
        {
            return Where(NpgsqlDbType.Smallint, EOperators.Or, name, EOperators.GreaterThanOrEqualTo, value);
        }

        public static WhereQuery WhereEqualTo(string name, bool value)
        {
            return Where(NpgsqlDbType.Boolean, name, EOperators.EqualTo, value);
        }

        public static WhereQuery AndWhereEqualTo(string name, bool value)
        {
            return Where(NpgsqlDbType.Boolean, EOperators.And, name, EOperators.EqualTo, value);
        }

        public static WhereQuery OrWhereEqualTo(string name, bool value)
        {
            return Where(NpgsqlDbType.Boolean, EOperators.Or, name, EOperators.EqualTo, value);
        }

        public static WhereQuery WhereNotEqualTo(string name, bool value)
        {
            return Where(NpgsqlDbType.Boolean, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery AndWhereNotEqualTo(string name, bool value)
        {
            return Where(NpgsqlDbType.Boolean, EOperators.And, name, EOperators.NotEqualTo, value);
        }

        public static WhereQuery OrWhereNotEqualTo(string name, bool value)
        {
            return Where(NpgsqlDbType.Boolean, EOperators.Or, name, EOperators.NotEqualTo, value);
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

        public static string CreateUpdateCommandText(string cacheKey, string tableName, IList<ColumnInfo> updates, IList<WhereQuery> wheres, string additional = "")
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

        public static async UniTask<int> ExecuteUpdate(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<ColumnInfo> updates, IList<WhereQuery> wheres, string additional = "")
        {
            return await ExecuteNonQuery(connection, transaction, CreateUpdateCommandText(cacheKey, tableName, updates, wheres, additional), updates, wheres);
        }

        public static async UniTask<int> ExecuteUpdate(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<ColumnInfo> updates, WhereQuery where, string additional = "")
        {
            var wheres = new List<WhereQuery>()
            {
                where,
            };
            return await ExecuteUpdate(cacheKey, connection, transaction, tableName, updates, wheres, additional);
        }

        public static async UniTask<NpgsqlDataReader> ExecuteSelect(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<WhereQuery> wheres, string select = "*", string additional = "")
        {
            return await ExecuteReader(connection, transaction, CreateSelectCommandText(cacheKey, tableName, wheres, select, additional), wheres);
        }

        public static async UniTask<NpgsqlDataReader> ExecuteSelect(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, WhereQuery where, string select = "*", string additional = "")
        {
            var wheres = new List<WhereQuery>()
            {
                where,
            };
            return await ExecuteSelect(cacheKey, connection, transaction, tableName, wheres, select, additional);
        }

        public static async UniTask<NpgsqlDataReader> ExecuteSelect(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string select = "*", string additional = "", params WhereQuery[] wheres)
        {
            return await ExecuteSelect(cacheKey, connection, transaction, tableName, wheres, select, additional);
        }

        public static async UniTask<NpgsqlDataReader> ExecuteSelect(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string select = "*", params WhereQuery[] wheres)
        {
            return await ExecuteSelect(cacheKey, connection, transaction, tableName, wheres, select);
        }

        public static async UniTask<object> ExecuteSelectScalar(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<WhereQuery> wheres, string select = "*", string additional = "")
        {
            return await ExecuteScalar(connection, transaction, CreateSelectCommandText(cacheKey, tableName, wheres, select, additional), wheres);
        }

        public static async UniTask<object> ExecuteSelectScalar(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, WhereQuery where, string select = "*", string additional = "")
        {
            var wheres = new List<WhereQuery>()
            {
                where,
            };
            return await ExecuteSelectScalar(cacheKey, connection, transaction, tableName, wheres, select, additional);
        }

        public static async UniTask<object> ExecuteSelectScalar(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string select = "*", string additional = "", params WhereQuery[] wheres)
        {
            return await ExecuteSelectScalar(cacheKey, connection, transaction, tableName, wheres, select, additional);
        }

        public static async UniTask<object> ExecuteSelectScalar(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string select = "*", params WhereQuery[] wheres)
        {
            return await ExecuteSelectScalar(cacheKey, connection, transaction, tableName, wheres, select);
        }

        public static async UniTask<int> ExecuteDelete(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, IList<WhereQuery> wheres, string additional = "")
        {
            return await ExecuteNonQuery(connection, transaction, CreateDeleteCommandText(cacheKey, tableName, wheres, additional), wheres);
        }

        public static async UniTask<int> ExecuteDelete(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, WhereQuery where, string additional = "")
        {
            var wheres = new List<WhereQuery>()
            {
                where,
            };
            return await ExecuteDelete(cacheKey, connection, transaction, tableName, wheres, additional);
        }

        public static async UniTask<int> ExecuteDelete(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, string additional = "", params WhereQuery[] wheres)
        {
            return await ExecuteDelete(cacheKey, connection, transaction, tableName, wheres, additional);
        }

        public static async UniTask<int> ExecuteDelete(string cacheKey, NpgsqlConnection connection, NpgsqlTransaction transaction, string tableName, params WhereQuery[] wheres)
        {
            return await ExecuteDelete(cacheKey, connection, transaction, tableName, wheres);
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

        public static async UniTask<object> ExecuteScalar(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, IList<WhereQuery> wheres)
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
            object result = await cmd.ExecuteScalarAsync();
            cmd.Dispose();
            return result;
        }

        public static async UniTask<object> ExecuteScalar(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, IList<ColumnInfo> updates, IList<WhereQuery> wheres)
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
            object result = await cmd.ExecuteScalarAsync();
            cmd.Dispose();
            return result;
        }
    }
}
#endif