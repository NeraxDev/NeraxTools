namespace NeraXTools.Database_Operations.Cloud_Services.SupabaseOP
{
    public class SupabaseOP
    {
        #region ================================== Single Execution - Return Only Result ==================================

        /// <summary>
        /// Executes a Supabase RPC function and returns the direct result mapped to the specified type <typeparamref name="T"/>.
        /// <para>Returns <c>default(T)</c> if the execution fails or the result is null.</para>
        /// </summary>
        /// <typeparam name="T">The expected return type of the database function.</typeparam>
        /// <param name="client">The Supabase client instance to use for the request.</param>
        /// <param name="func">The exact name of the RPC function defined in the database.</param>
        /// <example>
        /// <code><![CDATA[
        /// var branchCount = await SupabaseOP.ExecuteAsync<int>(client, "get_total_branches");
        /// ]]></code>
        /// </example>
        public static async Task<T> ExecuteAsync<T>(Supabase.Client client, string func)
            => await Supabase_Core.UnifySingleExecutionReturnOnlyResult_Core<T>(client, func, CancellationToken.None, Timeout.Infinite);

        /// <summary>
        /// Executes a Supabase RPC function with parameters and returns the mapped result.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="parameters">A dictionary containing the function arguments (key: param name, value: param value).</param>
        /// <example>
        /// <code><![CDATA[
        /// var args = new Dictionary<string, object> { { "p_branch_id", 5 } };
        /// var branchName = await SupabaseOP.ExecuteAsync<string>(client, "get_branch_name", args);
        /// ]]></code>
        /// </example>
        public static async Task<T> ExecuteAsync<T>(Supabase.Client client, string func, Dictionary<string, object> parameters)
            => await Supabase_Core.UnifySingleExecutionReturnOnlyResult_Core<T>(client, func, CancellationToken.None, Timeout.Infinite, parameters);

        /// <summary>
        /// Executes a Supabase RPC function with support for a <see cref="CancellationToken"/>.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
        /// <example>
        /// <code><![CDATA[
        /// CancellationTokenSource cts = new CancellationTokenSource();
        /// var user = await SupabaseOP.ExecuteAsync<UserDto>(client, "get_current_user", cts.Token);
        /// ]]></code>
        /// </example>
        public static async Task<T> ExecuteAsync<T>(Supabase.Client client, string func, CancellationToken ct)
            => await Supabase_Core.UnifySingleExecutionReturnOnlyResult_Core<T>(client, func, ct, Timeout.Infinite);

        /// <summary>
        /// Executes a Supabase RPC function with cancellation support and a specific execution timeout.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Execution timeout in seconds. Set to <see cref="Timeout.Infinite"/> for no timeout.</param>
        /// <example>
        /// <code><![CDATA[
        /// // Execute with a 10-second limit
        /// var result = await SupabaseOP.ExecuteAsync<bool>(client, "is_server_ready", ct, 10);
        /// ]]></code>
        /// </example>
        public static async Task<T> ExecuteAsync<T>(Supabase.Client client, string func, CancellationToken ct, int timeout)
            => await Supabase_Core.UnifySingleExecutionReturnOnlyResult_Core<T>(client, func, ct, timeout);

        /// <summary>
        /// Executes a Supabase RPC function with parameters and cancellation support.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="parameters">Dictionary of arguments for the RPC function.</param>
        /// <example>
        /// <code><![CDATA[
        /// var args = new Dictionary<string, object> { { "search_query", "admin" } };
        /// var logs = await SupabaseOP.ExecuteAsync<List<Log>>(client, "search_logs", ct, args);
        /// ]]></code>
        /// </example>
        public static async Task<T> ExecuteAsync<T>(Supabase.Client client, string func, CancellationToken ct, Dictionary<string, object> parameters)
            => await Supabase_Core.UnifySingleExecutionReturnOnlyResult_Core<T>(client, func, ct, Timeout.Infinite, parameters);

        /// <summary>
        /// Provides the most granular control over a single Supabase RPC execution.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Execution timeout in seconds.</param>
        /// <param name="parameters">Dictionary of arguments for the RPC function.</param>
        /// <example>
        /// <code><![CDATA[
        /// var args = new Dictionary<string, object> { { "min_price", 100 } };
        /// var prods = await SupabaseOP.ExecuteAsync<List<Product>>(client, "get_prods", ct, 30, args);
        /// ]]></code>
        /// </example>
        public static async Task<T> ExecuteAsync<T>(Supabase.Client client, string func, CancellationToken ct, int timeout, Dictionary<string, object> parameters)
            => await Supabase_Core.UnifySingleExecutionReturnOnlyResult_Core<T>(client, func, ct, timeout, parameters);

        #endregion ================================== Single Execution - Return Only Result ==================================

        #region ================================== Single Execution - Return Full DTO ==================================

        /// <summary>
        /// Executes a Supabase RPC function and returns a detailed <see cref="ResultSupabaseDTO{T}"/> object.
        /// <para>Use this when you need to check for execution success or capture error messages.</para>
        /// </summary>
        /// <typeparam name="T">The expected return type of the database function.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The name of the database function.</param>
        /// <example>
        /// <code><![CDATA[
        /// var response = await SupabaseOP.ExecuteAsyncWithFullReturn<int>(client, "get_total_branches");
        /// if (response.IsSuccess) {
        ///     Console.WriteLine($"Result: {response.Result}");
        /// } else {
        ///     Console.WriteLine($"Error: {response.ErrorMessage}");
        /// }
        /// ]]></code>
        /// </example>
        public static async Task<ResultSupabaseDTO<T>> ExecuteAsyncWithFullReturn<T>(Supabase.Client client, string func)
            => await Supabase_Core.UnifySingleExecutionReturnFullDto_Core<T>(client, func, CancellationToken.None, Timeout.Infinite);

        /// <summary>
        /// Executes an RPC function with parameters and returns a comprehensive response DTO.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="parameters">A dictionary of arguments for the function.</param>
        /// <example>
        /// <code><![CDATA[
        /// var args = new Dictionary<string, object> { { "p_branch_id", 12 } };
        /// var response = await SupabaseOP.ExecuteAsyncWithFullReturn<string>(client, "get_branch_name", args);
        /// ]]></code>
        /// </example>
        public static async Task<ResultSupabaseDTO<T>> ExecuteAsyncWithFullReturn<T>(Supabase.Client client, string func, Dictionary<string, object> parameters)
            => await Supabase_Core.UnifySingleExecutionReturnFullDto_Core<T>(client, func, CancellationToken.None, Timeout.Infinite, parameters);

        /// <summary>
        /// Executes an RPC function with cancellation support, returning a detailed response DTO.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <example>
        /// <code><![CDATA[
        /// var response = await SupabaseOP.ExecuteAsyncWithFullReturn<List<UserDto>>(client, "get_active_users", ct);
        /// ]]></code>
        /// </example>
        public static async Task<ResultSupabaseDTO<T>> ExecuteAsyncWithFullReturn<T>(Supabase.Client client, string func, CancellationToken ct)
            => await Supabase_Core.UnifySingleExecutionReturnFullDto_Core<T>(client, func, ct, Timeout.Infinite);

        /// <summary>
        /// Executes an RPC function with both cancellation and timeout controls, returning a full response DTO.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Execution timeout in seconds.</param>
        /// <example>
        /// <code><![CDATA[
        /// var response = await SupabaseOP.ExecuteAsyncWithFullReturn<bool>(client, "ping_server", ct, 5);
        /// ]]></code>
        /// </example>
        public static async Task<ResultSupabaseDTO<T>> ExecuteAsyncWithFullReturn<T>(Supabase.Client client, string func, CancellationToken ct, int timeout)
            => await Supabase_Core.UnifySingleExecutionReturnFullDto_Core<T>(client, func, ct, timeout);

        /// <summary>
        /// Executes an RPC function with parameters and cancellation support, returning a full response DTO.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="parameters">A dictionary of arguments for the function.</param>
        /// <example>
        /// <code><![CDATA[
        /// var args = new Dictionary<string, object> { { "p_category", "Hardware" } };
        /// var response = await SupabaseOP.ExecuteAsyncWithFullReturn<List<Product>>(client, "get_category_products", ct, args);
        /// ]]></code>
        /// </example>
        public static async Task<ResultSupabaseDTO<T>> ExecuteAsyncWithFullReturn<T>(Supabase.Client client, string func, CancellationToken ct, Dictionary<string, object> parameters)
            => await Supabase_Core.UnifySingleExecutionReturnFullDto_Core<T>(client, func, ct, Timeout.Infinite, parameters);

        /// <summary>
        /// The complete overload for single RPC execution, providing maximum control and a detailed response DTO.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="client">The Supabase client instance.</param>
        /// <param name="func">The database function name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="timeout">Execution timeout in seconds.</param>
        /// <param name="parameters">A dictionary of arguments for the function.</param>
        /// <example>
        /// <code><![CDATA[
        /// var args = new Dictionary<string, object> { { "p_status", "Active" } };
        /// var response = await SupabaseOP.ExecuteAsyncWithFullReturn<int>(client, "update_status", ct, 15, args);
        /// ]]></code>
        /// </example>
        public static async Task<ResultSupabaseDTO<T>> ExecuteAsyncWithFullReturn<T>(Supabase.Client client, string func, CancellationToken ct, int timeout, Dictionary<string, object> parameters)
            => await Supabase_Core.UnifySingleExecutionReturnFullDto_Core<T>(client, func, ct, timeout, parameters);

        #endregion ================================== Single Execution - Return Full DTO ==================================

        #region ================================== Batch Execution - Return List of Full DTOs ==================================

        /// <summary>
        /// Executes multiple RPC functions in a batch. Global parameters (Client, Token, Timeout)
        /// will be used for each request unless overridden within individual <see cref="SupabaseDTO"/> objects.
        /// </summary>
        /// <param name="client">The global Supabase client for all requests.</param>
        /// <param name="ct">The global cancellation token for the entire batch.</param>
        /// <param name="timeout">The global timeout in seconds for each request.</param>
        /// <param name="requests">A variable-length list of <see cref="SupabaseDTO"/> objects to execute.</param>
        /// <example>
        /// <code><![CDATA[
        /// var results = await SupabaseOP.ExecuteBatchAsync(client, ct, 30,
        ///     new SupabaseDTO { FunctionName = "get_logs" },
        ///     new SupabaseDTO { FunctionName = "clear_cache" }
        /// );
        /// ]]></code>
        /// </example>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            Supabase.Client client, CancellationToken ct, int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(client, ct, timeout, requests);

        /// <summary>
        /// Executes a batch of RPC functions with a global client and cancellation token.
        /// </summary>
        /// <param name="client">The global Supabase client.</param>
        /// <param name="ct">The global cancellation token.</param>
        /// <param name="requests">The list of RPC requests.</param>
        /// <example>
        /// <code><![CDATA[
        /// var results = await SupabaseOP.ExecuteBatchAsync(client, ct, req1, req2);
        /// ]]></code>
        /// </example>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            Supabase.Client client, CancellationToken ct, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(client, ct, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a batch of RPC functions with a global client and a specific timeout.
        /// </summary>
        /// <param name="client">The global Supabase client.</param>
        /// <param name="timeout">Global timeout in seconds.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            Supabase.Client client, int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(client, CancellationToken.None, timeout, requests);

        /// <summary>
        /// Executes a batch of RPC functions using only a global client.
        /// </summary>
        /// <param name="client">The global Supabase client.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            Supabase.Client client, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(client, CancellationToken.None, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a batch where each <see cref="SupabaseDTO"/> must contain its own client,
        /// governed by global cancellation and timeout settings.
        /// </summary>
        /// <param name="ct">The global cancellation token.</param>
        /// <param name="timeout">Global timeout in seconds.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            CancellationToken ct, int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(null, ct, timeout, requests);

        /// <summary>
        /// Executes a batch with global cancellation support. Individual DTOs should handle their own client instances.
        /// </summary>
        /// <param name="ct">The global cancellation token.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            CancellationToken ct, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(null, ct, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a batch with a global timeout. Individual DTOs should handle their own client instances.
        /// </summary>
        /// <param name="timeout">Global timeout in seconds.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(null, CancellationToken.None, timeout, requests);

        /// <summary>
        /// Executes a batch of RPC functions independently. All configurations must be set within the <see cref="SupabaseDTO"/> objects.
        /// </summary>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
            params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecution_Core(null, CancellationToken.None, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a pre-constructed list of <see cref="SupabaseDTO"/> objects.
        /// </summary>
        /// <param name="requests">A <see cref="List{SupabaseDTO}"/> containing the batch operations.</param>
        /// <example>
        /// <code><![CDATA[
        /// var list = new List<SupabaseDTO> { new SupabaseDTO { FunctionName = "f1" } };
        /// var results = await SupabaseOP.ExecuteBatchAsync(list);
        /// ]]></code>
        /// </example>
        public static async Task<List<ResultSupabaseDTO<object>>> ExecuteBatchAsync(
           List<SupabaseDTO> requests)
            => await Supabase_Core.UnifyBatchExecution_Core(null, null, null, requests.ToArray());

        #endregion ================================== Batch Execution - Return List of Full DTOs ==================================

        #region ================================== Batch Execution - Return List of Results Only ==================================

        /// <summary>
        /// Executes multiple RPC functions and returns only the extracted results as a list of objects.
        /// <para>Note: If an individual request fails, its corresponding entry in the list will be <c>null</c>.</para>
        /// </summary>
        /// <param name="client">The global Supabase client.</param>
        /// <param name="ct">The global cancellation token.</param>
        /// <param name="timeout">Global timeout in seconds.</param>
        /// <param name="requests">A variable-length list of <see cref="SupabaseDTO"/> objects.</param>
        /// <example>
        /// <code><![CDATA[
        /// var data = await SupabaseOP.ExecuteBatchResultsOnlyAsync(client, ct, 30, req1, req2);
        /// var firstResult = (MyType)data[0];
        /// ]]></code>
        /// </example>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            Supabase.Client client, CancellationToken ct, int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(client, ct, timeout, requests);

        /// <summary>
        /// Executes a batch of RPC functions with global client and cancellation support, returning results only.
        /// </summary>
        /// <param name="client">The global Supabase client.</param>
        /// <param name="ct">The global cancellation token.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            Supabase.Client client, CancellationToken ct, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(client, ct, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a batch of RPC functions with a global client and timeout, returning results only.
        /// </summary>
        /// <param name="client">The global Supabase client.</param>
        /// <param name="timeout">Global timeout in seconds.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            Supabase.Client client, int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(client, CancellationToken.None, timeout, requests);

        /// <summary>
        /// Executes a batch of RPC functions with a global client, returning results only.
        /// </summary>
        /// <param name="client">The global Supabase client.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            Supabase.Client client, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(client, CancellationToken.None, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a batch with global cancellation and timeout settings, returning results only.
        /// Each DTO must specify its own client if not using a global one.
        /// </summary>
        /// <param name="ct">The global cancellation token.</param>
        /// <param name="timeout">Global timeout in seconds.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            CancellationToken ct, int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(null, ct, timeout, requests);

        /// <summary>
        /// Executes a batch with global cancellation support, returning results only.
        /// </summary>
        /// <param name="ct">The global cancellation token.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            CancellationToken ct, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(null, ct, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a batch with a global timeout, returning results only.
        /// </summary>
        /// <param name="timeout">Global timeout in seconds.</param>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            int timeout, params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(null, CancellationToken.None, timeout, requests);

        /// <summary>
        /// Executes a batch of RPC functions independently and returns a list of result objects.
        /// </summary>
        /// <param name="requests">The list of RPC requests.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            params SupabaseDTO[] requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(null, CancellationToken.None, Timeout.Infinite, requests);

        /// <summary>
        /// Executes a pre-constructed list of <see cref="SupabaseDTO"/> objects and returns a list of result objects.
        /// </summary>
        /// <param name="requests">A <see cref="List{SupabaseDTO}"/> containing the batch operations.</param>
        public static async Task<List<object>> ExecuteBatchResultsOnlyAsync(
            List<SupabaseDTO> requests)
            => await Supabase_Core.UnifyBatchExecutionOnlyResult_Core(null, null, null, requests.ToArray());

        #endregion ================================== Batch Execution - Return List of Results Only ==================================
    }
}