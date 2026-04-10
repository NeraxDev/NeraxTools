using NeraXTools.LogManager;
using System.Text.Json;
using System.Text.Json.Serialization;
using Supabase;

namespace NeraXTools.Database_Operations.Cloud_Services.SupabaseOP
{
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
        private static async Task<T> ExecuteRpc_Core<T>(
           Supabase.Client client,
           string functionName,
           CancellationToken ct,
           int timeOutFormSec = Timeout.Infinite,
           params Dictionary<string, object>[] parameters)
        {
            try
            {
                var finalParams = parameters?.SelectMany(d => d)
                                            .GroupBy(pair => pair.Key)
                                            .ToDictionary(g => g.Key, g => g.First().Value)
                                      ?? new Dictionary<string, object>();

                var rpcTask = client.Rpc(functionName, finalParams);
                CancellationTokenSource? timeoutCts = null;
                //Combined CTS
                CancellationToken tokenToUse = ct;
                if (timeOutFormSec != Timeout.Infinite && !ct.IsCancellationRequested)
                {
                    timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeOutFormSec));
                    tokenToUse = timeoutCts.Token;
                }

                try
                {
                    var response = await rpcTask.WaitAsync(tokenToUse);

                    if (string.IsNullOrEmpty(response.Content)) return default;

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };

                    return JsonSerializer.Deserialize<T>(response.Content, options);
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
            {
                return default;
            }

            if (item.Result == null)
                return default;

            try
            {
                return (T)item.Result;
            }
            catch (Exception ex)
            {
                Logger.logForThisTool($"Casting Error for {func}: {ex.Message}", eLogType.Exception);
                throw new InvalidCastException($"Cannot cast result of '{func}' to {typeof(T).Name}. Actual: {item.Result.GetType().Name}", ex);
            }
        }

        //

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

            // تبدیل خروجی object به T به صورت امن
            T finalData = default;
            if (item.IsSuccess && item.Result != null)
            {
                try { finalData = (T)item.Result; }
                catch { /* در صورت خطای کست، مقدار دیفالت می‌ماند */ }
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
    }
}