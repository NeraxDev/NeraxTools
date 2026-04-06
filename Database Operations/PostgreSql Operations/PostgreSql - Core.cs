using NeraXTools.LogManager;
using Npgsql;
using System.Text;

namespace NeraXTools.Database.PostgreSql
{
    /// <summary>
    /// Internal engine for PostgreSQL operations.
    /// تمامی منطق اصلی، مدیریت زمان (Timeout) و پردازشی دیتابیس در این کلاس هندل می‌شود.
    /// </summary>
    internal static class PostgreSql_Core
    {
        // ================================================================================== 1. NON-QUERY ==================================================================================

        internal static async Task ExecuteNonQueryAsync_Core(string connectionString, List<string> sqlList, int timeoutSeconds = 0, CancellationToken ct = default)
        {
            if (sqlList == null || !sqlList.Any()) return;

            const int maxAttempts = 3;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("BEGIN;");
                    int actualCommandCount = 0;
                    foreach (var sql in sqlList)
                    {
                        if (string.IsNullOrWhiteSpace(sql)) continue;
                        actualCommandCount++;
                        var trimmed = sql.Trim();
                        sb.Append(trimmed);
                        if (!trimmed.EndsWith(";")) sb.Append(";");
                        sb.AppendLine();
                    }
                    sb.AppendLine("COMMIT;");

                    if (actualCommandCount == 0) return;

                    int finalTimeout = CalculateTimeout_Core(timeoutSeconds, actualCommandCount);

                    await using var conn = new NpgsqlConnection(connectionString);
                    await conn.OpenAsync(ct);

                    await using var cmd = new NpgsqlCommand(sb.ToString(), conn);
                    cmd.CommandTimeout = finalTimeout;

                    await cmd.ExecuteNonQueryAsync(ct);

                    Logger.logForThisTool($"Executed {actualCommandCount} commands successfully. (Timeout: {(finalTimeout == 0 ? "Infinite" : finalTimeout + "s")})", eLogType.Info);
                    return;
                }
                catch (OperationCanceledException)
                {
                    Logger.logForThisTool("Database operation was cancelled by user.", eLogType.Warning);
                    throw;
                }
                catch (Exception ex) when (attempt < maxAttempts && (ex is ObjectDisposedException || ex is NpgsqlException))
                {
                    Logger.logForThisTool($"Temporary DB Error (Attempt {attempt}/{maxAttempts}): {ex.Message}", eLogType.Warning);
                    await Task.Delay(1000 * attempt, ct);
                }
                catch (Exception ex)
                {
                    Logger.logForThisTool($"Critical DB Error: {ex.Message}", eLogType.Exception);
                    throw;
                }
            }
        }

        // ================================================================================== 2. SCALAR ==================================================================================

        internal static async Task<object> ExecuteQueryScalarAsync_Core(string connectionString, string sql, int timeoutSeconds = 0, CancellationToken ct = default)
        {
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync(ct);

                int finalTimeout = CalculateTimeout_Core(timeoutSeconds, 1);
                Logger.logForThisTool($"Scalar Execution: {TruncateLog_Core(sql)} (Timeout: {finalTimeout}s)", eLogType.Info);

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.CommandTimeout = finalTimeout;

                return await cmd.ExecuteScalarAsync(ct);
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"Scalar Error: {ex.Message}", eLogType.Exception);
                throw;
            }
        }

        // ================================================================================== 3. LIST (DTO) ==================================================================================

        internal static async Task<List<T>> ExecuteQueryToListAsync_Core<T>(string connectionString, string sql, int readRowCount, int timeoutSeconds = 0, CancellationToken ct = default) where T : new()
        {
            var list = new List<T>();
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync(ct);

                int finalTimeout = CalculateTimeout_Core(timeoutSeconds, 1);

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.CommandTimeout = finalTimeout;

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                var properties = typeof(T).GetProperties();
                int count = 0;

                while (await reader.ReadAsync(ct))
                {
                    if (readRowCount > 0 && count >= readRowCount) break;

                    var item = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        var prop = properties.FirstOrDefault(p => p.Name.Equals(colName, StringComparison.OrdinalIgnoreCase));

                        if (prop != null && !reader.IsDBNull(i))
                        {
                            object val = reader.GetValue(i);
                            prop.SetValue(item, ChangeTypeForProperty_Core(val, prop.PropertyType));
                        }
                    }
                    list.Add(item);
                    count++;
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"QueryToList Error: {ex.Message}", eLogType.Exception);
                throw;
            }
        }

        // ================================================================================== 4. SIMPLE LIST ==================================================================================

        internal static async Task<List<T>> ExecuteQueryToSimpleListAsync_Core<T>(string connectionString, string sql, int readRowCount, int timeoutSeconds = 0, CancellationToken ct = default)
        {
            var list = new List<T>();
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync(ct);

                int finalTimeout = CalculateTimeout_Core(timeoutSeconds, 1);

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.CommandTimeout = finalTimeout;

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                int count = 0;
                while (await reader.ReadAsync(ct))
                {
                    if (readRowCount > 0 && count >= readRowCount) break;
                    if (!reader.IsDBNull(0))
                    {
                        list.Add((T)Convert.ChangeType(reader.GetValue(0), typeof(T)));
                    }
                    count++;
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"SimpleList Error: {ex.Message}", eLogType.Exception);
                throw;
            }
        }

        // ================================================================================== HELPERS ==================================================================================

        private static int CalculateTimeout_Core(int inputSeconds, int commandCount)
        {
            return inputSeconds switch
            {
                0 => Math.Min(Math.Max(15, commandCount * 5), 600), // داینامیک
                < 0 => 0, // بی‌نهایت (طبق استاندار Npgsql)
                _ => inputSeconds // مقدار دستی کاربر
            };
        }

        private static object ChangeTypeForProperty_Core(object value, Type targetType)
        {
            if (value == null) return null;
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType.IsInstanceOfType(value)) return value;

            if (underlyingType.IsEnum)
            {
                if (value is string s) return Enum.Parse(underlyingType, s);
                return Enum.ToObject(underlyingType, value);
            }

            if (underlyingType == typeof(Guid))
            {
                return value is Guid g ? g : Guid.Parse(value.ToString());
            }

            if (underlyingType == typeof(bool))
            {
                if (value is bool b) return b;
                if (value is string ss) return bool.Parse(ss);
                if (value is int i) return i != 0;
                if (value is long l) return l != 0L;
            }

            return Convert.ChangeType(value, underlyingType);
        }

        private static string TruncateLog_Core(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return "<empty>";
            return sql.Length > 150 ? sql.Substring(0, 150) + "..." : sql;
        }
    }
}