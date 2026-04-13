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
        #region ================================================================================== 1. NON-QUERY WRAPPERS ==================================================================================

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
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, new List<string> { sql }, CancellationToken.None, -1);

        /// <summary>
        /// Executes a single SQLFile command asynchronously.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">The SQL command to execute.</param>
        /// <example>
        /// <![CDATA[
        /// await db.ExecuteNonQueryAsync(connStr, "UPDATE Users SET IsActive = true WHERE Id = 1");
        /// ]]>
        /// </example>
        public static async Task ExecuteNonQueryFromFileAsync(string connectionString, string sqlFile)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, new List<string> { sqlFile }, CancellationToken.None, -1, isSqlFile: true);

        /// <summary>
        /// Executes a single SQL command with cancellation and custom timeout.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">The SQL command to execute.</param>
        /// <param name="ct">Cancellation token to abort the operation.</param>
        /// <param name="timeoutSeconds">Execution timeout (-1:Dynamic, -0: Infinite).</param>
        public static async Task ExecuteNonQueryAsync(string connectionString, string sql, CancellationToken ct, int timeoutSeconds = -1)
          => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, new List<string> { sql }, ct, timeoutSeconds);

        /// <summary>
        /// Executes a single SQL File command with cancellation and custom timeout.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">The SQL command to execute.</param>
        /// <param name="ct">Cancellation token to abort the operation.</param>
        /// <param name="timeoutSeconds">Execution timeout (-1:Dynamic, -0: Infinite).</param>
        public static async Task ExecuteNonQueryFromFileAsync(string connectionString, string sqlFile, CancellationToken ct, int timeoutSeconds = -1)
          => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, new List<string> { sqlFile }, ct, timeoutSeconds, isSqlFile: true);

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
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqls.ToList(), CancellationToken.None, -1);

        /// <summary>
        /// Executes multiple SQL File commands as a single transaction using params.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFiles">Variable number of SQL commands.</param>
        /// <example>
        /// <![CDATA[
        /// await db.ExecuteNonQueryAsync(connStr, "INSERT INTO Logs...", "UPDATE Stats...");
        /// ]]>
        /// </example>
        public static async Task ExecuteNonQueryFromFileAsync(string connectionString, params string[] sqlFiles)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqlFiles.ToList(), CancellationToken.None, -1, isSqlFile: true);

        /// <summary>
        /// Executes a list of SQL commands as a single transaction.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlList">List of SQL commands to execute.</param>
        public static async Task ExecuteNonQueryAsync(string connectionString, List<string> sqlList)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqlList, CancellationToken.None, -1);

        /// <summary>
        /// Executes a list of SQL Files commands as a single transaction.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFilesList">List of SQL commands to execute.</param>
        public static async Task ExecuteNonQueryFromFileAsync(string connectionString, List<string> sqlFilesList)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqlFilesList, CancellationToken.None, -1, isSqlFile: true);

        /// <summary>
        /// Executes a list of SQL commands with full control over timeout and cancellation.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlList">List of SQL commands.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout   (-1:Dynamic, -0: Infinite).</param>
        public static async Task ExecuteNonQueryAsync(string connectionString, List<string> sqlList, CancellationToken ct, int timeoutSeconds = -1)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqlList, ct, timeoutSeconds);

        /// <summary>
        /// Executes a list of SQL commands with full control over timeout and cancellation.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFilesList">List of SQL commands.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout   (-1:Dynamic, -0: Infinite).</param>
        public static async Task ExecuteNonQueryFromFileAsync(string connectionString, List<string> sqlFilesList, CancellationToken ct, int timeoutSeconds = -1)
            => await PostgreSql_Core.ExecuteNonQueryAsync_Core(connectionString, sqlFilesList, ct, timeoutSeconds, isSqlFile: true);

        #endregion ================================================================================== 1. NON-QUERY WRAPPERS ==================================================================================

        #region ================================================================================== 2. SCALAR WRAPPERS ==================================================================================

        /// <summary>
        /// Executes query SQL and returns the first column of the first row.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query (e.g., SELECT COUNT(*)).</param>
        /// <returns>The scalar result as an object.</returns>
        public static async Task<object> ExecuteQueryScalarAsync(string connectionString, string sql)
            => await PostgreSql_Core.ExecuteQueryScalarAsync_Core(connectionString, sql, CancellationToken.None, -1);

        /// <summary>
        /// Executes query SQL File and returns the first column of the first row.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">SQL query (e.g., SELECT COUNT(*)).</param>
        /// <returns>The scalar result as an object.</returns>
        public static async Task<object> ExecuteQueryScalarFromFileAsync(string connectionString, string sqlFile)
            => await PostgreSql_Core.ExecuteQueryScalarAsync_Core(connectionString, sqlFile, CancellationToken.None, -1, isSqlFile: true);

        /// <summary>
        /// Executes scalar query SQL with cancellation and custom timeout.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout (-1:Dynamic, -0: Infinite).</param>
        public static async Task<object> ExecuteQueryScalarAsync(string connectionString, string sql, CancellationToken ct, int timeoutSeconds = -1)
            => await PostgreSql_Core.ExecuteQueryScalarAsync_Core(connectionString, sql, ct, timeoutSeconds);

        /// <summary>
        /// Executes scalar query SQL File with cancellation and custom timeout.
        /// </summary>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">SQL query.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout (-1:Dynamic, -0: Infinite).</param>
        public static async Task<object> ExecuteQueryScalarFromFileAsync(string connectionString, string sqlFile, CancellationToken ct, int timeoutSeconds = -1)
            => await PostgreSql_Core.ExecuteQueryScalarAsync_Core(connectionString, sqlFile, ct, timeoutSeconds, isSqlFile: true);

        #endregion ================================================================================== 2. SCALAR WRAPPERS ==================================================================================

        #region ================================================================================== 3. OBJECT LIST WRAPPERS (DTO) ==================================================================================

        /// <summary>
        /// Executes query SQL and maps results to a list of DTO objects.
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
            => await PostgreSql_Core.ExecuteQueryToListAsync_Core<T>(connectionString, sql, 0, CancellationToken.None, -1);

        /// <summary>
        /// Executes query SQL File and maps results to a list of DTO objects.
        /// </summary>
        /// <typeparam name="T">The type of object to map rows to (must have a parameterless constructor).</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">SQL query.</param>
        /// <example>
        /// <![CDATA[
        /// var users = await db.ExecuteQueryToListAsync<UserDto>(connStr, "SELECT * FROM Users");
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteQueryToListFromFileAsync<T>(string connectionString, string sqlFile) where T : new()
            => await PostgreSql_Core.ExecuteQueryToListAsync_Core<T>(connectionString, sqlFile, 0, CancellationToken.None, -1, isSqlFile: true);

        /// <summary>
        /// Executes query SQL and maps results with advanced configuration.
        /// </summary>
        /// <typeparam name="T">Target DTO type.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query.</param>
        /// <param name="readRowCount">Limit the number of rows to read (0 for all).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout(-1:Dynamic, -0: Infinite).</param>
        public static async Task<List<T>> ExecuteQueryToListAsync<T>(string connectionString, string sql, int readRowCount, CancellationToken ct, int timeoutSeconds = -1) where T : new()
            => await PostgreSql_Core.ExecuteQueryToListAsync_Core<T>(connectionString, sql, readRowCount, ct, timeoutSeconds);

        /// <summary>
        /// Executes query SQL File and maps results with advanced configuration.
        /// </summary>
        /// <typeparam name="T">Target DTO type.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">SQL query.</param>
        /// <param name="readRowCount">Limit the number of rows to read (0 for all).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout(-1:Dynamic, -0: Infinite).</param>
        public static async Task<List<T>> ExecuteQueryToListFromFileAsync<T>(string connectionString, string sqlFile, int readRowCount, CancellationToken ct, int timeoutSeconds = -1) where T : new()
            => await PostgreSql_Core.ExecuteQueryToListAsync_Core<T>(connectionString, sqlFile, readRowCount, ct, timeoutSeconds, isSqlFile: true);

        #endregion ================================================================================== 3. OBJECT LIST WRAPPERS (DTO) ==================================================================================

        #region ================================================================================== 4. SIMPLE LIST WRAPPERS ==================================================================================

        /// <summary>
        /// Executes query SQL and returns a list of simple types (e.g., string, int, Guid).
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
            => await PostgreSql_Core.ExecuteQueryToSimpleListAsync_Core<T>(connectionString, sql, 0, CancellationToken.None, -1);

        /// <summary>
        /// Executes query SQL File and returns a list of simple types (e.g., string, int, Guid).
        /// </summary>
        /// <typeparam name="T">The simple type to return.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">SQL query selecting a single column.</param>
        /// <example>
        /// <![CDATA[
        /// List<string> names = await db.ExecuteQueryToSimpleListAsync<string>(connStr, "SELECT Name FROM Users");
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteQueryToSimpleListFromFileAsync<T>(string connectionString, string sqlFile)
            => await PostgreSql_Core.ExecuteQueryToSimpleListAsync_Core<T>(connectionString, sqlFile, 0, CancellationToken.None, -1, isSqlFile: true);

        /// <summary>
        /// Executes simple list query SQL with advanced configuration.
        /// </summary>
        /// <typeparam name="T">Target simple type.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sql">SQL query.</param>
        /// <param name="readRowCount">Limit the number of rows to read.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout (-1:Dynamic, -0: Infinite).</param>
        public static async Task<List<T>> ExecuteQueryToSimpleListAsync<T>(string connectionString, string sql, int readRowCount, CancellationToken ct, int timeoutSeconds = -1)
            => await PostgreSql_Core.ExecuteQueryToSimpleListAsync_Core<T>(connectionString, sql, readRowCount, ct, timeoutSeconds);

        /// <summary>
        /// Executes simple list query SQL File with advanced configuration.
        /// </summary>
        /// <typeparam name="T">Target simple type.</typeparam>
        /// <param name="connectionString">The PostgreSQL connection string.</param>
        /// <param name="sqlFile">SQL query.</param>
        /// <param name="readRowCount">Limit the number of rows to read.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="timeoutSeconds">Execution timeout (-1:Dynamic, -0: Infinite).</param>
        public static async Task<List<T>> ExecuteQueryToSimpleListFromFileAsync<T>(string connectionString, string sqlFile, int readRowCount, CancellationToken ct, int timeoutSeconds = -1)
            => await PostgreSql_Core.ExecuteQueryToSimpleListAsync_Core<T>(connectionString, sqlFile, readRowCount, ct, timeoutSeconds, isSqlFile: true);

        #endregion ================================================================================== 4. SIMPLE LIST WRAPPERS ==================================================================================

        #region ================================================================================== 5. EXECUTE STORED PROCEDURE WRAPPERS ==================================================================================

        #region ========================== 5.1. Execute Functions ===========================

        #region ========================================== 5.1.1. Execute Function (Single/Scalar) ==========================================

        /// <summary>
        /// Executes a PostgreSQL function that returns a single scalar value (e.g., int, string, bool).
        /// </summary>
        /// <typeparam name="T">The expected return type of the function result.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function to call.</param>
        /// <param name="parms">A collection of parameters as KeyValuePairs.</param>
        /// <returns>A task representing the asynchronous operation, containing the function result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var balance = await db.ExecuteFunctionAsync<decimal>(connStr, "get_account_balance", new KeyValuePair<string, object>("p_acc_id", 101));
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a scalar PostgreSQL function with a cancellation token.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token to abort the request.</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <returns>The function result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// CancellationTokenSource cts = new CancellationTokenSource();
        /// var isValid = await db.ExecuteFunctionAsync<bool>(connStr, "check_token", cts.Token, new KeyValuePair<string, object>("p_token", "xyz123"));
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName, CancellationToken ct, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a scalar PostgreSQL function with a custom execution timeout.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="timeout">Timeout in seconds (-1 for dynamic, 0 for infinite).</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <returns>The function result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var userRole = await db.ExecuteFunctionAsync<string>(connStr, "get_user_role", 30, new KeyValuePair<string, object>("p_uid", 5));
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName, int timeout, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, TimeoutFromSec = timeout, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a scalar PostgreSQL function with full control over cancellation and timeout.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <returns>The function result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var result = await db.ExecuteFunctionAsync<int>(connStr, "complex_calc", cts.Token, 60, new KeyValuePair<string, object>("p_val", 500));
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName, CancellationToken ct, int timeout, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct, TimeoutFromSec = timeout, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        #endregion ========================================== 5.1.1. Execute Function (Single/Scalar) ==========================================

        #region ========================================== 5.1.2. Execute Function (Single/Scalar) - No Params ==========================================

        /// <summary>
        /// Executes a PostgreSQL function that takes no parameters and returns a single scalar value.
        /// </summary>
        /// <typeparam name="T">The expected return type of the function result.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function to call (e.g., "get_server_time").</param>
        /// <returns>A task representing the asynchronous operation, containing the result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var serverTime = await db.ExecuteFunctionAsync<DateTime>(connStr, "get_db_now");
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query });

        /// <summary>
        /// Executes a scalar PostgreSQL function (no parameters) with a cancellation token.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token to abort the operation.</param>
        /// <returns>The function result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var status = await db.ExecuteFunctionAsync<string>(connStr, "get_system_status", cts.Token);
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName, CancellationToken ct)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct });

        /// <summary>
        /// Executes a scalar PostgreSQL function (no parameters) with a custom timeout.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="timeout">Execution timeout in seconds.</param>
        /// <returns>The function result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var nextInSequence = await db.ExecuteFunctionAsync<long>(connStr, "get_next_order_id", 10);
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName, int timeout)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, TimeoutFromSec = timeout });

        /// <summary>
        /// Executes a scalar PostgreSQL function (no parameters) with full control over cancellation and timeout.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Execution timeout in seconds.</param>
        /// <returns>The function result of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var isActive = await db.ExecuteFunctionAsync<bool>(connStr, "is_maintenance_mode", cts.Token, 5);
        /// ]]>
        /// </example>
        public static async Task<T> ExecuteFunctionAsync<T>(string conn, string funcName, CancellationToken ct, int timeout)
            => await PostgreSql_Core.UnifySingleResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct, TimeoutFromSec = timeout });

        #endregion ========================================== 5.1.2. Execute Function (Single/Scalar) - No Params ==========================================

        #region ========================================== 5.1.3. Execute Function (List/DTOs) ==========================================

        /// <summary>
        /// Executes a PostgreSQL function and maps the resulting rows to a list of DTOs or simple types.
        /// </summary>
        /// <typeparam name="T">The target type for mapping (DTO class or simple type).</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function to call.</param>
        /// <param name="parms">A collection of parameters as KeyValuePairs.</param>
        /// <returns>A list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// // Example with DTO mapping:
        /// var users = await db.ExecuteFunctionListAsync<UserDto>(connStr, "get_active_users", new KeyValuePair<string, object>("p_role", "Admin"));
        ///
        /// // Example with Simple Type:
        /// var emails = await db.ExecuteFunctionListAsync<string>(connStr, "get_all_emails", new KeyValuePair<string, object>("p_domain", "gmail.com"));
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a list-returning PostgreSQL function with a cancellation token.
        /// </summary>
        /// <typeparam name="T">The target type for mapping.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token to abort the request.</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <returns>A list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var products = await db.ExecuteFunctionListAsync<ProductDto>(connStr, "search_products", cts.Token, new KeyValuePair<string, object>("p_query", "Laptop"));
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName, CancellationToken ct, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a list-returning PostgreSQL function with a custom execution timeout.
        /// </summary>
        /// <typeparam name="T">The target type for mapping.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="timeout">Timeout in seconds (-1 for dynamic, 0 for infinite).</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <returns>A list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// // For heavy reports, use a longer timeout (e.g., 60 seconds)
        /// var reportData = await db.ExecuteFunctionListAsync<ReportDto>(connStr, "generate_heavy_report", 60, new KeyValuePair<string, object>("p_year", 2024));
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName, int timeout, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, TimeoutFromSec = timeout, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a list-returning PostgreSQL function with full control over cancellation and timeout.
        /// </summary>
        /// <typeparam name="T">The target type for mapping.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <returns>A list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var results = await db.ExecuteFunctionListAsync<LogDto>(connStr, "fetch_logs", cts.Token, 30, new KeyValuePair<string, object>("p_level", "Error"));
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName, CancellationToken ct, int timeout, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct, TimeoutFromSec = timeout, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        #endregion ========================================== 5.1.3. Execute Function (List/DTOs) ==========================================

        #region ========================================== 5.1.4. Execute Function (List/DTOs) - No Params ==========================================

        /// <summary>
        /// Executes a PostgreSQL function with no parameters and maps the resulting rows to a list of DTOs or simple types.
        /// </summary>
        /// <typeparam name="T">The target type for mapping (DTO class or simple type).</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function to call.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// // Fetching a list of all branch entities
        /// var branches = await db.ExecuteFunctionListAsync<BranchDto>(connStr, "get_all_branches");
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query });

        /// <summary>
        /// Executes a list-returning PostgreSQL function (no parameters) with a cancellation token.
        /// </summary>
        /// <typeparam name="T">The target type for mapping.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token to abort the request.</param>
        /// <returns>A list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// // Fetching settings with ability to cancel the request
        /// var settings = await db.ExecuteFunctionListAsync<SettingDto>(connStr, "fetch_app_settings", cts.Token);
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName, CancellationToken ct)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct });

        /// <summary>
        /// Executes a list-returning PostgreSQL function (no parameters) with a custom execution timeout.
        /// </summary>
        /// <typeparam name="T">The target type for mapping.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="timeout">Timeout in seconds (-1 for dynamic, 0 for infinite).</param>
        /// <returns>A list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// // Executing a potentially slow global sync function with a 45-second timeout
        /// var syncData = await db.ExecuteFunctionListAsync<SyncDto>(connStr, "get_global_sync_data", 45);
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName, int timeout)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, TimeoutFromSec = timeout });

        /// <summary>
        /// Executes a list-returning PostgreSQL function (no parameters) with full control over cancellation and timeout.
        /// </summary>
        /// <typeparam name="T">The target type for mapping.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <returns>A list of mapped objects of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var results = await db.ExecuteFunctionListAsync<CategoryDto>(connStr, "get_full_category_tree", cts.Token, 30);
        /// ]]>
        /// </example>
        public static async Task<List<T>> ExecuteFunctionListAsync<T>(string conn, string funcName, CancellationToken ct, int timeout)
            => await PostgreSql_Core.UnifyListResult_Core<T>(conn, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, ct = ct, TimeoutFromSec = timeout });

        #endregion ========================================== 5.1.4. Execute Function (List/DTOs) - No Params ==========================================

        #region ========================================== 5.1.5. Execute Function (Safe Result DTO) ==========================================

        /// <summary>
        /// Executes a function safely and returns a single result wrapped in a ResultPostgreSqlDTO.
        /// Prevents exceptions from bubbling up by capturing them in the ErrorMessage.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="parms">Function parameters.</param>
        /// <returns>A safe result container including Success status, Error messages, and the Data.</returns>
        /// <example>
        /// <![CDATA[
        /// var response = await db.ExecuteFunctionSafeAsync<int>(connStr, "update_stock", new KeyValuePair<string, object>("p_id", 1));
        /// if (response.IsSuccess) {
        ///     var newStock = response.Result;
        /// } else {
        ///     MessageBox.Show(response.ErrorMessage);
        /// }
        /// ]]>
        /// </example>
        public static async Task<ResultPostgreSqlDTO<T>> ExecuteFunctionSafeAsync<T>(string conn, string funcName, params KeyValuePair<string, object>[] parms)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });
            var item = res.First();
            return new ResultPostgreSqlDTO<T> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = item.IsSuccess ? ((List<T>)item.Result).FirstOrDefault() : default };
        }

        /// <summary>
        /// Executes a function safely and returns a list of results wrapped in a ResultPostgreSqlDTO.
        /// </summary>
        /// <typeparam name="T">The target DTO or type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcName">The name of the database function.</param>
        /// <param name="parms">Function parameters.</param>
        /// <returns>A safe result container holding a List of type T.</returns>
        /// <example>
        /// <![CDATA[
        /// var response = await db.ExecuteFunctionListSafeAsync<OrderDto>(connStr, "get_recent_orders", new KeyValuePair<string, object>("p_limit", 10));
        /// if (response.IsSuccess) {
        ///    foreach(var order in response.Result) { /* ... */ }
        /// }
        /// ]]>
        /// </example>
        public static async Task<ResultPostgreSqlDTO<List<T>>> ExecuteFunctionListSafeAsync<T>(string conn, string funcName, params KeyValuePair<string, object>[] parms)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });
            var item = res.First();
            return new ResultPostgreSqlDTO<List<T>> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = item.IsSuccess ? (List<T>)item.Result : new List<T>() };
        }

        #endregion ========================================== 5.1.5. Execute Function (Safe Result DTO) ==========================================

        #region ========================================== 5.1.6. Execute Function (Safe Result DTO) - No Params ==========================================

        /// <summary>
        /// Safely executes a parameterless function returning a single value.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// var response = await db.ExecuteFunctionSafeAsync<bool>(connStr, "is_vault_open");
        /// ]]>
        /// </example>
        public static async Task<ResultPostgreSqlDTO<T>> ExecuteFunctionSafeAsync<T>(string conn, string funcName)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query });
            var item = res.First();
            return new ResultPostgreSqlDTO<T> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = item.IsSuccess ? ((List<T>)item.Result).FirstOrDefault() : default };
        }

        /// <summary>
        /// Safely executes a parameterless function returning a list of values.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// var response = await db.ExecuteFunctionListSafeAsync<NotificationDto>(connStr, "get_unread_notifications");
        /// ]]>
        /// </example>
        public static async Task<ResultPostgreSqlDTO<List<T>>> ExecuteFunctionListSafeAsync<T>(string conn, string funcName)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = funcName, ReturnType = typeof(T), QueryType = QueryType.Query });
            var item = res.First();
            return new ResultPostgreSqlDTO<List<T>> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = item.IsSuccess ? (List<T>)item.Result : new List<T>() };
        }

        #endregion ========================================== 5.1.6. Execute Function (Safe Result DTO) - No Params ==========================================

        #region ========================== 5.1.7. Execute Function Batch (Safe Result DTO) ===========================

        /// <summary>
        /// Executes multiple PostgreSQL functions in a single database round-trip.
        /// Each request in the batch can have its own parameters and return type.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="requests">An array of PostgreSqlDTO objects representing the functions to execute.</param>
        /// <returns>A list of safe results, where each result matches the corresponding request in the input array.</returns>
        /// <example>
        /// <![CDATA[
        /// var batchResults = await db.ExecuteFunctionBatchSafeAsync(connStr,
        ///     new PostgreSqlDTO { QueryOrProcedureName = "get_user_count", ReturnType = typeof(int) },
        ///     new PostgreSqlDTO { QueryOrProcedureName = "get_active_status", ReturnType = typeof(bool), Parameters = new Dictionary<string, object> { { "id", 1 } } }
        /// );
        /// ]]>
        /// </example>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchSafeAsync(string conn, params PostgreSqlDTO[] requests)
        {
            foreach (var req in requests) req.QueryType = QueryType.Query;
            return await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, requests);
        }

        /// <summary>
        /// Executes a batch of functions with a cancellation token to abort the entire sequence.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="requests">The batch of function requests.</param>
        /// <returns>A list of safe results for each function call.</returns>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchSafeAsync(string conn, CancellationToken ct, params PostgreSqlDTO[] requests)
        {
            foreach (var req in requests) req.QueryType = QueryType.Query;
            return await PostgreSql_Core.ExecuteBatch_Core(conn, ct, null, requests);
        }

        /// <summary>
        /// Executes a batch of functions with a custom timeout applied to the entire operation.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="timeout">Total execution timeout in seconds.</param>
        /// <param name="requests">The batch of function requests.</param>
        /// <returns>A list of safe results.</returns>
        /// <example>
        /// <![CDATA[
        /// // Executing a batch with a 15-second total timeout
        /// var results = await db.ExecuteFunctionBatchSafeAsync(connStr, 15, request1, request2);
        /// ]]>
        /// </example>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchSafeAsync(string conn, int timeout, params PostgreSqlDTO[] requests)
        {
            foreach (var req in requests) req.QueryType = QueryType.Query;
            return await PostgreSql_Core.ExecuteBatch_Core(conn, null, timeout, requests);
        }

        /// <summary>
        /// Executes a batch of functions with full control over cancellation and timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Total execution timeout in seconds.</param>
        /// <param name="requests">The batch of function requests.</param>
        /// <returns>A list of safe results.</returns>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchSafeAsync(string conn, CancellationToken ct, int timeout, params PostgreSqlDTO[] requests)
        {
            foreach (var req in requests) req.QueryType = QueryType.Query;
            return await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, requests);
        }

        #endregion ========================== 5.1.7. Execute Function Batch (Safe Result DTO) ===========================

        #region ========================== 5.1.8. Execute Function Batch (Quick List) ===========================

        /// <summary>
        /// Executes multiple functions (without parameters) that share the same Return Type T in a single round-trip.
        /// </summary>
        /// <typeparam name="T">The common return type for all functions in the list.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcNames">A list of function names to be executed.</param>
        /// <returns>A list of safe results for each function.</returns>
        /// <example>
        /// <![CDATA[
        /// // Quickly fetching counts from multiple summary functions
        /// var names = new List<string> { "get_total_sales", "get_total_users", "get_total_orders" };
        /// var results = await db.ExecuteFunctionBatchAsync<long>(connStr, names);
        /// ]]>
        /// </example>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchAsync<T>(string conn, List<string> funcNames)
        {
            var requests = funcNames.Select(name => new PostgreSqlDTO { QueryOrProcedureName = name, ReturnType = typeof(T), QueryType = QueryType.Query }).ToArray();
            return await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, requests);
        }

        /// <summary>
        /// Executes multiple functions with the same Return Type with a cancellation token.
        /// </summary>
        /// <typeparam name="T">The common return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcNames">A list of function names.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of safe results.</returns>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchAsync<T>(string conn, List<string> funcNames, CancellationToken ct)
        {
            var requests = funcNames.Select(name => new PostgreSqlDTO { QueryOrProcedureName = name, ReturnType = typeof(T), QueryType = QueryType.Query }).ToArray();
            return await PostgreSql_Core.ExecuteBatch_Core(conn, ct, null, requests);
        }

        /// <summary>
        /// Executes multiple functions with the same Return Type with a custom timeout.
        /// </summary>
        /// <typeparam name="T">The common return type.</typeparam>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcNames">A list of function names.</param>
        /// <param name="timeout">Total execution timeout in seconds.</param>
        /// <returns>A list of safe results.</returns>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchAsync<T>(string conn, List<string> funcNames, int timeout)
        {
            var requests = funcNames.Select(name => new PostgreSqlDTO { QueryOrProcedureName = name, ReturnType = typeof(T), QueryType = QueryType.Query }).ToArray();
            return await PostgreSql_Core.ExecuteBatch_Core(conn, null, timeout, requests);
        }

        /// <summary>
        /// Executes multiple functions with the same Return Type with full control over cancellation and timeout.
        /// </summary>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteFunctionBatchAsync<T>(string conn, List<string> funcNames, CancellationToken ct, int timeout)
        {
            var requests = funcNames.Select(name => new PostgreSqlDTO { QueryOrProcedureName = name, ReturnType = typeof(T), QueryType = QueryType.Query }).ToArray();
            return await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, requests);
        }

        /// <summary>
        /// [CAUTION: RISKY] Executes multiple functions and returns results as dynamic objects.
        /// Use this only when the return structure is unknown or highly variable.
        /// Automatic mapping may be incomplete for complex nested structures.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcNames">A list of function names.</param>
        /// <returns>A list of safe results containing dynamic data.</returns>
        /// <example>
        /// <![CDATA[
        /// var dynamicRes = await db.ExecuteFunctionBatchAsync(connStr, new List<string> { "get_raw_log", "get_meta_data" });
        /// if(dynamicRes[0].IsSuccess)
        ///    Console.WriteLine(dynamicRes[0].Result.some_dynamic_field);
        /// ]]>
        /// </example>
        public static async Task<List<ResultPostgreSqlDTO<dynamic>>> ExecuteFunctionBatchAsync(string conn, List<string> funcNames)
            => await ExecuteFunctionBatchAsync<dynamic>(conn, funcNames);

        /// <summary>
        /// [CAUTION: RISKY] Executes multiple functions and returns results as dynamic objects.
        /// Use this only when the return structure is unknown or highly variable.
        /// Automatic mapping may be incomplete for complex nested structures.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="funcNames">A list of function names.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Total execution timeout in seconds.</param>
        /// <returns>A list of safe results containing dynamic data.</returns>
        /// <example>
        /// <![CDATA[
        /// var dynamicRes = await db.ExecuteFunctionBatchAsync(connStr, new List<string> { "get_raw_log", "get_meta_data" }, ct, timeout);
        /// if(dynamicRes[0].IsSuccess)
        ///    Console.WriteLine(dynamicRes[0].Result.some_dynamic_field);
        /// ]]>
        /// </example>
        public static async Task<List<ResultPostgreSqlDTO<dynamic>>> ExecuteFunctionBatchAsync(string conn, List<string> funcNames, CancellationToken ct, int timeout)
            => await ExecuteFunctionBatchAsync<dynamic>(conn, funcNames, ct, timeout);

        #endregion ========================== 5.1.8. Execute Function Batch (Quick List) ===========================

        #endregion ========================== 5.1. Execute Functions ===========================

        #region ========================== 5.2. Execute Procedures (CALL) ===========================

        #region ========================================== 5.2.1. Execute Procedure (Void/Non-Query) ==========================================

        /// <summary>
        /// Executes a PostgreSQL procedure (CALL) with parameters. Use this for operations that don't return data (e.g., INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure to call.</param>
        /// <param name="parms">A collection of parameters for the procedure.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <example>
        /// <![CDATA[
        /// await db.ExecuteProcedureAsync(connStr, "register_sale",
        ///     new KeyValuePair<string, object>("p_customer_id", 12),
        ///     new KeyValuePair<string, object>("p_total_amount", 1500.50));
        /// ]]>
        /// </example>
        public static async Task ExecuteProcedureAsync(string conn, string procName, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a PostgreSQL procedure with parameters and a cancellation token.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <param name="ct">The cancellation token to abort the procedure.</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <example>
        /// <![CDATA[
        /// await db.ExecuteProcedureAsync(connStr, "bulk_update_prices", cts.Token,
        ///     new KeyValuePair<string, object>("p_category_id", 5),
        ///     new KeyValuePair<string, object>("p_percent", 10));
        /// ]]>
        /// </example>
        public static async Task ExecuteProcedureAsync(string conn, string procName, CancellationToken ct, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, ct, null, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a PostgreSQL procedure with parameters and a custom execution timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <param name="timeout">Timeout in seconds for the procedure execution.</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <example>
        /// <![CDATA[
        /// // Setting a 2-minute timeout for a heavy data migration procedure
        /// await db.ExecuteProcedureAsync(connStr, "migrate_old_logs", 120,
        ///     new KeyValuePair<string, object>("p_batch_size", 1000));
        /// ]]>
        /// </example>
        public static async Task ExecuteProcedureAsync(string conn, string procName, int timeout, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, null, timeout, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        /// <summary>
        /// Executes a PostgreSQL procedure with parameters, full cancellation control, and timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="parms">A collection of parameters.</param>
        public static async Task ExecuteProcedureAsync(string conn, string procName, CancellationToken ct, int timeout, params KeyValuePair<string, object>[] parms)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });

        #endregion ========================================== 5.2.1. Execute Procedure (Void/Non-Query) ==========================================

        #region ========================================== 5.2.2. Execute Procedure (Void/Non-Query) - No Params ==========================================

        /// <summary>
        /// Calls a PostgreSQL procedure (CALL) that takes no parameters and returns no data.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure to call.</param>
        /// <returns>A task representing the asynchronous execution.</returns>
        /// <example>
        /// <![CDATA[
        /// // Executing a maintenance procedure
        /// await db.ExecuteProcedureAsync(connStr, "archive_old_orders");
        /// ]]>
        /// </example>
        public static async Task ExecuteProcedureAsync(string conn, string procName)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure });

        /// <summary>
        /// Calls a parameterless procedure with a cancellation token.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <param name="ct">The cancellation token to stop the procedure execution.</param>
        /// <example>
        /// <![CDATA[
        /// await db.ExecuteProcedureAsync(connStr, "sync_external_data", cts.Token);
        /// ]]>
        /// </example>
        public static async Task ExecuteProcedureAsync(string conn, string procName, CancellationToken ct)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, ct, null, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure });

        /// <summary>
        /// Calls a parameterless procedure with a custom execution timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <param name="timeout">Timeout in seconds for the procedure to complete.</param>
        /// <example>
        /// <![CDATA[
        /// // Giving a heavy cleanup procedure 5 minutes (300 seconds) to run
        /// await db.ExecuteProcedureAsync(connStr, "full_database_cleanup", 300);
        /// ]]>
        /// </example>
        public static async Task ExecuteProcedureAsync(string conn, string procName, int timeout)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, null, timeout, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure });

        /// <summary>
        /// Calls a parameterless procedure with full control over cancellation and timeout.
        /// </summary>
        public static async Task ExecuteProcedureAsync(string conn, string procName, CancellationToken ct, int timeout)
            => await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure });

        #endregion ========================================== 5.2.2. Execute Procedure (Void/Non-Query) - No Params ==========================================

        #region ========================================== 5.2.3. Execute Procedure (Safe Result DTO) ==========================================

        /// <summary>
        /// Executes a PostgreSQL procedure safely with parameters.
        /// Captures any database or connection errors and wraps them in a ResultPostgreSqlDTO instead of throwing exceptions.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure to call (CALL).</param>
        /// <param name="parms">A collection of parameters for the procedure.</param>
        /// <returns>A ResultPostgreSqlDTO object containing execution status and potential error messages.</returns>
        /// <example>
        /// <![CDATA[
        /// var response = await db.ExecuteProcedureSafeAsync(connStr, "update_inventory",
        ///     new KeyValuePair<string, object>("p_sku", "APP-001"),
        ///     new KeyValuePair<string, object>("p_qty", 10));
        ///
        /// if (!response.IsSuccess) {
        ///     Logger.LogError(response.ErrorMessage);
        /// }
        /// ]]>
        /// </example>
        public static async Task<ResultPostgreSqlDTO<object>> ExecuteProcedureSafeAsync(string conn, string procName, params KeyValuePair<string, object>[] parms)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });
            var item = res.First();
            return new ResultPostgreSqlDTO<object> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = null };
        }

        /// <summary>
        /// Executes a procedure safely with parameters, cancellation control, and timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Execution timeout in seconds.</param>
        /// <param name="parms">A collection of parameters.</param>
        /// <returns>A safe execution result.</returns>
        public static async Task<ResultPostgreSqlDTO<object>> ExecuteProcedureSafeAsync(string conn, string procName, CancellationToken ct, int timeout, params KeyValuePair<string, object>[] parms)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure, Parameters = parms.ToDictionary(x => x.Key, x => x.Value) });
            var item = res.First();
            return new ResultPostgreSqlDTO<object> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = null };
        }

        #endregion ========================================== 5.2.3. Execute Procedure (Safe Result DTO) ==========================================

        #region ========================================== 5.2.4. Execute Procedure (Safe Result DTO) - No Params ==========================================

        /// <summary>
        /// Executes a parameterless PostgreSQL procedure safely.
        /// Returns a result DTO indicating whether the CALL was successful.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <returns>A ResultPostgreSqlDTO containing the status of the operation.</returns>
        /// <example>
        /// <![CDATA[
        /// var response = await db.ExecuteProcedureSafeAsync(connStr, "reset_daily_stats");
        /// if (response.IsSuccess) {
        ///     Console.WriteLine("Stats reset successfully.");
        /// }
        /// ]]>
        /// </example>
        public static async Task<ResultPostgreSqlDTO<object>> ExecuteProcedureSafeAsync(string conn, string procName)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure });
            var item = res.First();
            return new ResultPostgreSqlDTO<object> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = null };
        }

        /// <summary>
        /// Executes a parameterless procedure safely with cancellation and timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procName">The name of the procedure.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Execution timeout in seconds.</param>
        /// <returns>A safe execution result.</returns>
        public static async Task<ResultPostgreSqlDTO<object>> ExecuteProcedureSafeAsync(string conn, string procName, CancellationToken ct, int timeout)
        {
            var res = await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, new PostgreSqlDTO { QueryOrProcedureName = procName, ReturnType = typeof(void), QueryType = QueryType.Procedure });
            var item = res.First();
            return new ResultPostgreSqlDTO<object> { Identifier = item.Identifier, IsSuccess = item.IsSuccess, ErrorMessage = item.ErrorMessage, Result = null };
        }

        #endregion ========================================== 5.2.4. Execute Procedure (Safe Result DTO) - No Params ==========================================

        #region ========================== 5.2.5. Execute Procedure Batch (Safe Result DTO) ===========================

        /// <summary>
        /// Executes multiple PostgreSQL procedures (CALL) in a single round-trip.
        /// Ideal for grouped operations where you need to check the success of each individual procedure.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="requests">An array of PostgreSqlDTO objects representing the procedures to call.</param>
        /// <returns>A list of safe results (ResultPostgreSqlDTO), each corresponding to a procedure in the batch.</returns>
        /// <example>
        /// <![CDATA[
        /// var batchResults = await db.ExecuteProcedureBatchSafeAsync(connStr,
        ///     new PostgreSqlDTO { QueryOrProcedureName = "clear_cache", Parameters = new Dictionary<string, object> { { "p_zone", "A" } } },
        ///     new PostgreSqlDTO { QueryOrProcedureName = "log_maintenance_start" }
        /// );
        ///
        /// if (batchResults.All(r => r.IsSuccess)) {
        ///     // All procedures executed successfully
        /// }
        /// ]]>
        /// </example>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteProcedureBatchSafeAsync(string conn, params PostgreSqlDTO[] requests)
        {
            foreach (var req in requests) { req.QueryType = QueryType.Procedure; req.ReturnType = typeof(void); }
            return await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, requests);
        }

        /// <summary>
        /// Executes a batch of procedures safely with full control over cancellation and timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Total timeout in seconds for the entire batch operation.</param>
        /// <param name="requests">The batch of procedure requests.</param>
        /// <returns>A list of safe results for each procedure.</returns>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteProcedureBatchSafeAsync(string conn, CancellationToken ct, int timeout, params PostgreSqlDTO[] requests)
        {
            foreach (var req in requests) { req.QueryType = QueryType.Procedure; req.ReturnType = typeof(void); }
            return await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, requests);
        }

        #endregion ========================== 5.2.5. Execute Procedure Batch (Safe Result DTO) ===========================

        #region ========================== 5.2.6. Execute Procedure Batch (Quick List) ===========================

        /// <summary>
        /// Quickly executes a list of parameterless procedures in a single round-trip.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procNames">A list of procedure names to be called.</param>
        /// <returns>A list of safe results for the called procedures.</returns>
        /// <example>
        /// <![CDATA[
        /// var tasks = new List<string> { "proc_sync_data", "proc_rebuild_stats", "proc_validate_integrity" };
        /// var results = await db.ExecuteProcedureBatchAsync(connStr, tasks);
        /// ]]>
        /// </example>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteProcedureBatchAsync(string conn, List<string> procNames)
        {
            var requests = procNames.Select(name => new PostgreSqlDTO { QueryOrProcedureName = name, ReturnType = typeof(void), QueryType = QueryType.Procedure }).ToArray();
            return await PostgreSql_Core.ExecuteBatch_Core(conn, null, null, requests);
        }

        /// <summary>
        /// Quickly executes a list of parameterless procedures with cancellation and timeout.
        /// </summary>
        /// <param name="conn">The PostgreSQL connection string.</param>
        /// <param name="procNames">A list of procedure names.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Total execution timeout in seconds.</param>
        /// <returns>A list of safe results.</returns>
        public static async Task<List<ResultPostgreSqlDTO<object>>> ExecuteProcedureBatchAsync(string conn, List<string> procNames, CancellationToken ct, int timeout)
        {
            var requests = procNames.Select(name => new PostgreSqlDTO { QueryOrProcedureName = name, ReturnType = typeof(void), QueryType = QueryType.Procedure }).ToArray();
            return await PostgreSql_Core.ExecuteBatch_Core(conn, ct, timeout, requests);
        }

        #endregion ========================== 5.2.6. Execute Procedure Batch (Quick List) ===========================

        #endregion ========================== 5.2. Execute Procedures (CALL) ===========================

        #endregion ================================================================================== 5. EXECUTE STORED PROCEDURE WRAPPERS ==================================================================================
    }
}