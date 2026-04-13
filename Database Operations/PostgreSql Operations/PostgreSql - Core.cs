/* TODO LIST - NERAX TOOLS (Database Engine)
  -----------------------------------------------------------------------------------------
  1. [PERFORMANCE] Property Caching:
     در متد ExecuteQueryToListAsync_Core، ویژگی‌های تیپ T را با ConcurrentDictionary کش کن.
     رفلکشن در هر بار فراخوانی، پرفورمنس سیستم POS را در گزارشات سنگین پایین می‌آورد.

  2. [ARCHITECTURE] Hybrid Infrastructure (Interface-based):
     ایجاد IPostgreSqlEngine و تبدیل هسته به Instance-based.
     این کار اجازه می‌دهد بدون تغییر در لاجیک، قابلیت Unit Test و Dependency Injection فراهم شود.
     (خوشبختانه به دلیل معماری متمرکز فعلی، این تغییر روی DTOها و فرمت خروجی اثری ندارد).

  3. [FILE SYSTEM] Path Virtualization:
     در متد ReadSqlFiles، به جای استفاده مستقیم از BaseDirectory، امکان تزریق مسیر (Path Injection)
     فراهم شود تا کتابخانه در محیط‌های مختلف و خارج از پوشه جاری هم کار کند.

  4. [CLEAN CODE] Logic Separation:
     انتقال متدهای SafeConvert و Mapping به یک کلاس اختصاصی (DataMapper) جهت رعایت اصل SRP.
  -----------------------------------------------------------------------------------------
*/

using NeraXTools.LogManager;
using Npgsql;
using System.Collections;
using System.Data;
using System.Text;

namespace NeraXTools.Database.PostgreSql
{
    /// <summary>
    /// Internal engine for PostgreSQL operations.
    /// تمامی منطق اصلی، مدیریت زمان (Timeout) و پردازشی دیتابیس در این کلاس هندل می‌شود.
    /// </summary>
    internal static class PostgreSql_Core
    {
        #region ================================================================= EXECUTE Sql Files  ==================================================================================

        // ================================================================================== 1. NON-QUERY ==================================================================================

        internal static async Task ExecuteNonQueryAsync_Core(string connectionString, List<string> sqlList, CancellationToken ct, int timeoutSeconds = -1, bool isSqlFile = false)
        {
            if (sqlList == null || !sqlList.Any()) return;

            const int maxAttempts = 3;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("BEGIN;\n");
                    int actualCommandCount = 0;
                    if (isSqlFile)
                    {
                        (string sql, actualCommandCount) = await ReadSqlFiles(sqlList);
                        sb.Append(sql);
                    }
                    else
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

        internal static async Task<object> ExecuteQueryScalarAsync_Core(string connectionString, string sql, CancellationToken ct, int timeoutSeconds = -1, bool isSqlFile = false)
        {
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync(ct);

                int finalTimeout = CalculateTimeout_Core(timeoutSeconds, 1);
                Logger.logForThisTool($"Scalar Execution: {TruncateLog_Core(sql)} (Timeout: {finalTimeout}s)", eLogType.Info);

                if (isSqlFile)
                    (sql, _) = await ReadSqlFiles(new List<string> { sql });
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

        internal static async Task<List<T>> ExecuteQueryToListAsync_Core<T>(string connectionString, string sql, int readRowCount, CancellationToken ct, int timeoutSeconds = -1, bool isSqlFile = false) where T : new()
        {
            var list = new List<T>();
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync(ct);

                int finalTimeout = CalculateTimeout_Core(timeoutSeconds, 1);
                if (isSqlFile)
                    (sql, _) = await ReadSqlFiles(new List<string> { sql });
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

        internal static async Task<List<T>> ExecuteQueryToSimpleListAsync_Core<T>(string connectionString, string sql, int readRowCount, CancellationToken ct, int timeoutSeconds = -1, bool isSqlFile = false)
        {
            var list = new List<T>();
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync(ct);

                int finalTimeout = CalculateTimeout_Core(timeoutSeconds, 1);
                if (isSqlFile)
                    (sql, _) = await ReadSqlFiles(new List<string> { sql });
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

        //================================================================================= 5. SCALAR PARAMETRIC ==================================================================================
        internal static async Task<T> ExecuteScalarAsync_Core<T>(string connectionString, string sql, CancellationToken ct, int timeoutSeconds = -1, params NpgsqlParameter[] parameters)
        {
            await using var conn = new NpgsqlConnection(connectionString);
            try
            {
                await conn.OpenAsync(ct);
                int finalTimeout = CalculateTimeout_Core(timeoutSeconds, 1);

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.CommandTimeout = finalTimeout;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                var result = await cmd.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value) return default;
                return (T)ChangeTypeForProperty_Core(result, typeof(T));
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"Scalar Parametric Error: {ex.Message}", eLogType.Exception);
                throw;
            }
        }

        #region ================================================================================== HELPERS ==================================================================================

        private static async Task<(string, int)> ReadSqlFiles(List<string> sqlPaths)
        {
            var sb = new StringBuilder();
            int actualCommandCount = 0;
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var path in sqlPaths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    string actualPath = Path.Combine(basePath, path);
                    if (!File.Exists(actualPath)) continue;

                    string sql = await File.ReadAllTextAsync(actualPath, cts.Token);
                    sb.Append(sql);
                    if (!sql.TrimEnd().EndsWith(";")) sb.Append(";");
                    sb.Append("\n");
                    actualCommandCount++;
                }
            }
            return (sb.ToString(), actualCommandCount);
        }

        private static int CalculateTimeout_Core(int inputSeconds, int commandCount)
        {
            return inputSeconds switch
            {
                -1 => Math.Min(Math.Max(30, commandCount * 30), 300), // داینامیک   //Max 10 Work In 1 Command
                0 => 0, // بی‌نهایت (طبق استاندار Npgsql)
                > 0 => inputSeconds // مقدار دستی کاربر
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

        #endregion ================================================================================== HELPERS ==================================================================================

        #endregion ================================================================= EXECUTE Sql Files  ==================================================================================

        #region ========================================================================= EXECUTE FUNCTION Or Procedure ==================================================================================

        //================================================================================= 6. EXECUTE BATCH (PROCEDURE OR QUERY) ==================================================================================
        internal static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteBatch_Core(
               string globalConnString,
               CancellationToken? globalCt = default,
               int? globalTimeout = -1,
               params PostgreSqlDTO[] requests)
        {
            var finalResults = new List<ResultPostgreSqlDTO<object>>();
            if (requests == null || !requests.Any()) return finalResults;

            foreach (var req in requests)
            {
                var resultItem = new ResultPostgreSqlDTO<object> { Identifier = req.QueryOrProcedureName };

                var connToUse = req.ConnectionString ?? globalConnString;
                var ctToUse = (req.ct != CancellationToken.None) ? req.ct : (globalCt ?? CancellationToken.None);
                var timeoutToUse = (req.TimeoutFromSec != -1) ? req.TimeoutFromSec : (globalTimeout ?? -1);

                try
                {
                    var data = await ExecuteSingle_Core(
                        connToUse,
                        req.QueryOrProcedureName,
                        req.ReturnType,
                        req.QueryType,
                        ctToUse,
                        timeoutToUse,
                        req.Parameters);

                    resultItem.Result = data;
                    resultItem.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    resultItem.IsSuccess = false;
                    resultItem.ErrorMessage = ex.Message;
                    Logger.logForThisTool($"Batch Item Error (Identifier: {req.QueryOrProcedureName}): {ex.Message}", eLogType.Exception);
                }
                finalResults.Add(resultItem);
            }
            Logger.logForThisTool($"Batch Execution Completed: {finalResults.Count} items processed.", eLogType.Info);
            return finalResults;
        }

        #region ================================================================================== HELPERS ==================================================================================

        //================================================================================= EXECUTE SINGLE (PROCEDURE OR QUERY) Internal Method (private) ==================================================================================
        private static async Task<object> ExecuteSingle_Core(
            string connectionString,
            string sqlOrProc,
            Type returnType,
            QueryType queryType,
            CancellationToken ct,
            int timeoutSec,
            Dictionary<string, object> parameters)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync(ct);
                using (var cmd = new Npgsql.NpgsqlCommand(sqlOrProc, conn))
                {
                    if (timeoutSec >= 0) cmd.CommandTimeout = timeoutSec;
                    cmd.CommandType = CommandType.Text;
                    string paramPlaceholder = parameters != null && parameters.Any()
                        ? string.Join(", ", parameters.Keys.Select(p => "@" + p))
                        : string.Empty;
                    if (queryType == QueryType.Procedure)
                        cmd.CommandText = $"CALL {sqlOrProc}({paramPlaceholder})";
                    else if (queryType == QueryType.Query)
                        cmd.CommandText = $"SELECT * FROM {sqlOrProc}({paramPlaceholder})";
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                        }
                    }
                    if (returnType == typeof(void))
                    {
                        return await cmd.ExecuteNonQueryAsync(ct);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync(ct))
                    {
                        return await MapReaderToDynamicList(reader, returnType, ct);
                    }
                }
            }
        }

        private static async Task<object> MapReaderToDynamicList(NpgsqlDataReader reader, Type returnType, CancellationToken ct)
        {
            try
            {
                var listType = typeof(List<>).MakeGenericType(returnType);
                var results = (IList)Activator.CreateInstance(listType);

                bool isSimpleType = returnType.IsPrimitive ||
                                    returnType == typeof(string) ||
                                    returnType == typeof(decimal) ||
                                    returnType == typeof(DateTime) ||
                                    returnType == typeof(Guid) ||
                                    returnType.IsEnum ||
                                    //
                                    Nullable.GetUnderlyingType(returnType) != null;

                var props = !isSimpleType ? returnType.GetProperties() : null;

                while (await reader.ReadAsync(ct))
                {
                    if (isSimpleType)
                    {
                        var val = reader.IsDBNull(0) ? null : reader.GetValue(0);
                        results.Add(SafeConvert(val, returnType));
                    }
                    else
                    {
                        var obj = Activator.CreateInstance(returnType);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var colName = reader.GetName(i);
                            var prop = props.FirstOrDefault(p => string.Equals(p.Name, colName, StringComparison.OrdinalIgnoreCase));

                            if (prop != null && !reader.IsDBNull(i))
                            {
                                var val = reader.GetValue(i);
                                prop.SetValue(obj, SafeConvert(val, prop.PropertyType));
                            }
                        }
                        results.Add(obj);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"Mapping Error: {ex.Message}", eLogType.Exception);
                throw;
            }
        }

        private static object SafeConvert(object value, Type targetType)
        {
            try
            {
                // DBNull.Value --> Convert Null to null
                if (value == null || value == DBNull.Value) return null;
                // ?int -> int or null -> int
                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (underlyingType.IsEnum)
                {
                    return Enum.IsDefined(underlyingType, value)
                           ? Enum.ToObject(underlyingType, value)
                           : value;
                }

                return Convert.ChangeType(value, underlyingType);
            }
            catch (Exception)
            {
                Logger.logForThisTool($"Conversion Error: Unable to convert value '{value}' to type '{targetType.Name}'. Returning original value.", eLogType.Warning);
                throw;
            }
        }

        #endregion ================================================================================== HELPERS ==================================================================================

        #region============================================================================ Core_APIs ============================================

        internal static async Task<T> UnifySingleResult_Core<T>(string conn, PostgreSqlDTO req)
        {
            var batchResults = await ExecuteBatch_Core(conn, null, null, req);
            var firstResult = batchResults.FirstOrDefault();

            if (firstResult == null || !firstResult.IsSuccess)
                throw new Exception(firstResult?.ErrorMessage ?? "Execution failed.");

            try
            {
                // در متد Core ما خروجی همیشه List<T> است، پس اولین خانه لیست را برمی‌گردانیم
                var list = (System.Collections.IList)firstResult.Result;
                if (list == null || list.Count == 0) return default;
                return (T)list[0];
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"Casting Error in {req.QueryOrProcedureName}: {ex.Message}", eLogType.Exception);
                throw;
            }
        }

        internal static async Task<List<T>> UnifyListResult_Core<T>(string conn, PostgreSqlDTO req)
        {
            var batchResults = await ExecuteBatch_Core(conn, null, null, req);
            var firstResult = batchResults.FirstOrDefault();

            if (firstResult == null || !firstResult.IsSuccess)
                throw new Exception(firstResult?.ErrorMessage ?? "Execution failed.");

            try
            {
                return (List<T>)firstResult.Result;
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"Casting Error in {req.QueryOrProcedureName}: {ex.Message}", eLogType.Exception);
                throw;
            }
        }

        #endregion ========================================================================= EXECUTE FUNCTION Or Procedure ==================================================================================

        #endregion ========================================================================= EXECUTE FUNCTION ==================================================================================
    }
}