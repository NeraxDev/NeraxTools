using NeraXTools.LogManager;
using System.Text.Json;
using System.Text.Json.Serialization;
using Supabase;

namespace NeraXTools.Database_Operations.Cloud_Services.SupabaseOP;

/*
 * Cancellation behavior
 * ----------------------
 * CancellationToken is used here to cancel the waiting operation without
 * necessarily aborting the underlying HTTP request. The Supabase RPC API
 * (v8.13.0) does not expose a CancellationToken parameter, so the token is
 * passed to Task.WaitAsync rather than to client.Rpc. Consequently, cancelling
 * the token stops this method from waiting for the RPC result, but it does not
 * guarantee that the HTTP request already started by the Supabase client is
 * aborted.
 *
 * For actual HTTP request cancellation, configure ClientOptions.Timeout on
 * the Supabase client. The timeout is enforced by the HTTP client and should
 * be configured separately from the waiting-operation cancellation token.
 */
internal static class Supabase_Core
{
    //=============================================== Core Execution Method ===============================================
    internal static async Task<List<ResultSupabaseDTO<object>>> ExecuteRpc_Core(
        Supabase.Client globalClient,
        CancellationToken? globalCt = default,
        int? globalTimeout = Timeout.Infinite,
        params SupabaseDTO[] requests)
    {
        var finalResults = new List<ResultSupabaseDTO<object>>();

        if (requests == null || !requests.Any()) return finalResults;

        foreach (var req in requests)
        {
            var resultItem = new ResultSupabaseDTO<object> { FunctionName = req.FunctionName };

            // تنظیمات اختصاصی یا گلوبال
            var clientToUse = req.SpecificClient ?? globalClient;
            var ctToUse = (req.ct != CancellationToken.None) ? req.ct : globalCt;
            var timeoutToUse = (req.TimeOutFormSec != Timeout.Infinite) ? req.TimeOutFormSec : globalTimeout;

            try
            {
                // فراخوانی متد اصلی
                // نکته: برای اینکه داینامیک باشد، نوع بازگشتی را object می‌گیریم
                var data = await ExecuteRpc_Core<object>(
                    clientToUse,
                    req.FunctionName,
                    ctToUse ?? CancellationToken.None,
                    timeoutToUse ?? Timeout.Infinite,
                    req.Parameters);

                resultItem.Result = data;
                resultItem.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultItem.IsSuccess = false;
                resultItem.ErrorMessage = ex.Message;
                Logger.logForThisTool($"Batch Execution Failed for {req.FunctionName}: {ex.Message}", eLogType.Exception);
            }

            finalResults.Add(resultItem);
        }

        return finalResults;
    }

    //=============================================== Core_Main Execution Method ===============================================
    private static async Task<T?> ExecuteRpc_Core<T>(
       Supabase.Client client,
       string functionName,
       CancellationToken ct,
       int timeOutFormSec = Timeout.Infinite,
       params Dictionary<string, object>[] parameters)
    {
        try
        {
            // multi to one dictionary
            var finalParams = parameters?.SelectMany(d => d)
                                        .GroupBy(pair => pair.Key)
                                        .ToDictionary(g => g.Key, g => g.First().Value)
                                     ?? new Dictionary<string, object>();

            var rpcTask = client.Rpc(functionName, finalParams);
            CancellationTokenSource? timeoutCts = null;
            //Combined CTS
            CancellationToken tokenToUse = ct;
            if (timeOutFormSec != Timeout.Infinite && timeOutFormSec > 0 && !ct.IsCancellationRequested)
            {
                timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeOutFormSec));
                tokenToUse = timeoutCts.Token;
            }

            try
            {
                var response = await rpcTask.WaitAsync(tokenToUse);
                // for more transparency, we don't provide a default value and throw an exception
                if (string.IsNullOrWhiteSpace(response.Content))
                    throw new InvalidOperationException($"RPC '{functionName}' returned an empty response.");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                using JsonDocument document = JsonDocument.Parse(response.Content);
                return ConvertResult_Core<T>(document.RootElement.Clone(), options);
            }
            finally
            {
                finalParams.Clear();
                timeoutCts?.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
            Logger.logForThisTool($"RPC Task was cancelled: {functionName}", eLogType.Warning);
            throw;
        }
        catch (Exception ex)
        {
            Logger.logForThisTool($"[Internal Supabase Error] {functionName}: {ex.Message}", eLogType.Exception);
            throw;
        }
    }

    #region=============================================== Intermediate Methods ===============================================

    //============================= Unify Single Execution Method by Return only Result ================================

    internal static async Task<T> UnifySingleExecutionReturnOnlyResult_Core<T>(
        Supabase.Client client,
        string func,
        CancellationToken ct,
        int timeout,
        Dictionary<string, object> parameters = null)
    {
        var batchResult = await ExecuteRpc_Core(
            client,
            ct,
            timeout,
            new SupabaseDTO
            {
                FunctionName = func,
                Parameters = parameters ?? new Dictionary<string, object>(),
                ReturnType = typeof(T)
            });

        if (batchResult == null || batchResult.Count == 0)
            return default;

        var item = batchResult[0];

        if (item == null || !item.IsSuccess)
            return default;

        if (item.Result == null)
            return default;
        try
        {
            return ConvertResult_Core<T>(item.Result);
        }
        catch (Exception ex)
        {
            Logger.logForThisTool($"Result conversion failed for {func}: \n{ex.Message}", eLogType.Exception);
            throw new InvalidOperationException($"Failed to convert result for function {func}.", ex);
        }
    }

    internal static async Task<ResultSupabaseDTO<T>> UnifySingleExecutionReturnFullDto_Core<T>(
        Supabase.Client client,
        string func,
        CancellationToken ct,
        int timeout,
        Dictionary<string, object> parameters = null)
    {
        var batchResult = await ExecuteRpc_Core(
            client,
            ct,
            timeout,
            new SupabaseDTO
            {
                FunctionName = func,
                Parameters = parameters ?? new Dictionary<string, object>(),
                ReturnType = typeof(T)
            });

        if (batchResult == null || batchResult.Count == 0)
        {
            return new ResultSupabaseDTO<T>
            {
                FunctionName = func,
                IsSuccess = false,
                ErrorMessage = "No response from core."
            };
        }

        var item = batchResult[0];

        T finalData = default;

        if (item.IsSuccess && item.Result != null)
        {
            try
            {
                finalData = ConvertResult_Core<T>(item.Result);
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"Result conversion failed for {func}: \n{ex.Message}", eLogType.Exception);
                return new ResultSupabaseDTO<T>
                {
                    FunctionName = func,
                    IsSuccess = false,
                    ErrorMessage = $"Failed to convert result for function {func}: {ex.Message}",// Whene we have casting exeption ! we have this error message beccause server and conction dont have any error message and i dont want to return null or default value for this case
                    Result = default
                };
            }
        }

        return new ResultSupabaseDTO<T>
        {
            FunctionName = item.FunctionName,
            IsSuccess = item.IsSuccess,
            ErrorMessage = item.ErrorMessage,
            Result = finalData
        };
    }

    #endregion ============================================== Intermediate Methods ===============================================
    #region =============================================== Unify Batch Execution Methods ===============================================

    internal static async Task<List<ResultSupabaseDTO<object>>> UnifyBatchExecution_Core(
        Supabase.Client globalClient = null,
        CancellationToken? globalCt = null,
        int? globalTimeout = null,
        params SupabaseDTO[] requests)
    {
        // از همان متدی که قبلاً با هم نوشتیم (ExecuteRpc_Core نسخه Batch) استفاده می‌کند
        return await ExecuteRpc_Core(globalClient, globalCt, globalTimeout, requests);
    }

    internal static async Task<List<object>> UnifyBatchExecutionOnlyResult_Core(
        Supabase.Client globalClient = null,
        CancellationToken? globalCt = null,
        int? globalTimeout = null,
        params SupabaseDTO[] requests)
    {
        var fullResults = await ExecuteRpc_Core(globalClient, globalCt, globalTimeout, requests);

        // فقط فیلد Result را استخراج می‌کنیم
        return fullResults.Select(x => x.Result).ToList();
    }

    #endregion ============================================== Unify Batch Execution Methods =======================================

    #region=============================================== Helper Methods ===============================================

    private static T ConvertResult_Core<T>(object input, JsonSerializerOptions options = null)
    {
        if (input == null)
            return default;

        if (input is T directResult)
            return directResult;
        // Handle nullable types //---------------- null or T types ?? T type
        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        // if its generic type / remeber this part can't handel enum guid or some spicific types/
        // -------------- input is IConvertible and targetType can convert to input type (its fucking logic)
        if (input is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
            return (T)Convert.ChangeType(input, targetType);
        // or its non-generic type
        if (input is JsonElement jsonElement)
            return jsonElement.Deserialize<T>(options);
        string json = JsonSerializer.Serialize(input, options);

        return JsonSerializer.Deserialize<T>(json, options);
    }

    #endregion=============================================== Helper Methods ===============================================
}