namespace NeraXTools.Database.PostgreSql
{
    /// <summary>
    /// <para>English: High-level API for interacting with PostgreSQL databases.</para>
    /// <para>Farsi: رابط کاربری سطح بالا برای تعامل با دیتابیس‌های PostgreSQL.</para>
    /// </summary>
    /// <remarks>
    /// Use this class to execute commands, queries, and handle database transactions with built-in retry logic and dynamic timeouts.
    /// </remarks>
    public static class PostgreSql
    {
        // ================================================================================== 1. NON-QUERY WRAPPERS ==================================================================================

        /// <summary>
        /// Executes a single SQL command asynchronously.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">The SQL command to execute.</param>
        /// <example>
        /// <![CDATA[
        /// await db.ExecuteNonQueryAsync(connStr, "UPDATE Users SET IsActive = true WHERE Id = 1");
        /// ]]>
        /// </example>
        public static async Task ExecuteNonQueryAsync(string connectionString, string sql)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, new List<string> { sql });

        /// <summary>
        /// Executes a single SQL command with cancellation and custom timeout.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">The SQL command to execute.</param>
        /// <param name="ct">Cancellation token to abort the operation.</param>
        /// <param name="timeoutSeconds">Execution timeout (0: Dynamic, -1: Infinite).</param>
        public static async Task ExecuteNonQueryAsync(string connectionString, string sql, CancellationToken ct, int timeoutSeconds = 0)
          => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, new List<string> { sql }, timeoutSeconds, ct);

        /// <summary>
        /// Executes multiple SQL commands as a single transaction using params.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqls">Variable number of SQL commands.</param>
        /// <example>
        /// <![CDATA[
        /// await db.ExecuteNonQueryAsync(connStr, "INSERT INTO Logs...", "UPDATE Stats...");
        /// ]]>
        /// </example>
        public static async Task ExecuteNonQueryAsync(string connectionString, params string[] sqls)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, new List<string>(sqls));

        /// <summary>
        /// Executes a list of SQL commands as a single transaction.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlList">List of SQL commands to execute.</param>
        public static async Task ExecuteNonQueryAsync(string connectionString, List<string> sqlList)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqlList);

        /// <summary>
        /// Executes a list of SQL commands with full control over timeout and cancellation.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlList">List of SQL commands.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout (0: Dynamic, -1: Infinite).</param>
        public static async Task ExecuteNonQueryAsync(string connectionString, List<string> sqlList, CancellationToken ct, int timeoutSeconds = 0)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqlList, timeoutSeconds, ct);

        // ================================================================================== 2. SCALAR WRAPPERS ==================================================================================

        /// <summary>
        /// Executes query and returns the first column of the first row.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query (e.g., SELECT COUNT(*)).</param>
        /// <returns>The scalar result as an object.</returns>
        public static async Task<object> ExecuteQueryScalarAsync(string connectionString, string sql)
            => await PostgreSql_Core.ExecuteQueryScalarAsync_Core(connectionString, sql);

        /// <summary>
        /// Executes scalar query with cancellation and custom timeout.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout.</param>
        public static async Task<object> ExecuteQueryScalarAsync(string connectionString, string sql, CancellationToken ct, int timeoutSeconds)
            => await PostgreSql_Core.ExecuteQueryScalarAsync_Core(connectionString, sql, timeoutSeconds, ct);

        // ================================================================================== 3. OBJECT LIST WRAPPERS (DTO) ==================================================================================

        /// <summary>
        /// Executes query and maps results to a list of DTO objects.
        /// </summary>
        /// <typeparam name="T">The type of object to map rows to (must have a parameterless constructor).</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query.</param>
        /// <example>
        /// <![CDATA[
        /// var users = await db.ExecuteQueryToListAsync<UserDto>(connStr, "SELECT * FROM Users");
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteQueryToListAsync<T>(string connectionString, string sql) where T : new()
            => await PostgreSql_Core.ExecuteQueryToListAsync_Core<T>(connectionString, sql, 0);

        /// <summary>
        /// Executes query and maps results with advanced configuration.
        /// </summary>
        /// <typeparam name="T">Target DTO type.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query.</param>
        /// <param name="readRowCount">Limit the number of rows to read (0 for all).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout.</param>
        public static async Task<List<T>> ExecuteQueryToListAsync<T>(string connectionString, string sql, int readRowCount, CancellationToken ct, int timeoutSeconds = 0) where T : new()
            => await PostgreSql_Core.ExecuteQueryToListAsync_Core<T>(connectionString, sql, readRowCount, timeoutSeconds, ct);

        // ================================================================================== 4. SIMPLE LIST WRAPPERS ==================================================================================

        /// <summary>
        /// Executes query and returns a list of simple types (e.g., string, int, Guid).
        /// </summary>
        /// <typeparam name="T">The simple type to return.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query selecting a single column.</param>
        /// <example>
        /// <![CDATA[
        /// List<string> names = await db.ExecuteQueryToSimpleListAsync<string>(connStr, "SELECT Name FROM Users");
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteQueryToSimpleListAsync<T>(string connectionString, string sql)
            => await PostgreSql_Core.ExecuteQueryToSimpleListAsync_Core<T>(connectionString, sql, 0);

        /// <summary>
        /// Executes simple list query with advanced configuration.
        /// </summary>
        /// <typeparam name="T">Target simple type.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query.</param>
        /// <param name="readRowCount">Limit the number of rows to read.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout.</param>
        public static async Task<List<T>> ExecuteQueryToSimpleListAsync<T>(string connectionString, string sql, int readRowCount, CancellationToken ct, int timeoutSeconds = 0)
            => await PostgreSql_Core.ExecuteQueryToSimpleListAsync_Core<T>(connectionString, sql, readRowCount, timeoutSeconds, ct);
    }
}