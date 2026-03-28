using NeraXTools.TaskManager;

namespace NeraXTools
{
    internal static partial class FileAndFolderOpsCore
    {
        internal static long GetSizeSync_Core(List<string> paths, bool recursiveForFolders = true)
            => GetTotalSize_Core(paths, recursiveForFolders).GetAwaiter().GetResult();

        internal static async Task<long> GetSizeAsync_Core(List<string> paths, bool recursiveForFolders = true, CancellationToken token = default)
            => await TaskSchedulerEngine.RunAsync<long>(ct
              => GetTotalSize_Core(paths, recursiveForFolders), token);
    }
}