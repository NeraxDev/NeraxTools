using NeraXTools.LogManager;
using System.IO;

namespace NeraXTools
{
    internal static partial class FileAndFolderOpsCore
    {
        private static async Task<Result> Delete_Core(
            List<string> paths,
            List<string> startWith = null,
            List<string> NameFilter = null,
            FileAttributes? attributes = null,
            DateTime? creationStart = null,
            DateTime? creationEnd = null,
            DateTime? lastWriteStart = null,
            DateTime? lastWriteEnd = null,
            long? minSize = null,
            long? maxSize = null,
            Func<DirectoryInfo, bool, Task> opForDir = null,
            Func<FileInfo, Task> opForFile = null,
            IProgress<float> p = null,
            params FolderOps.FolderDeleteOptions[] options)
        {
            var opt = new
            {
                Recursive = options.Contains(FolderOps.FolderDeleteOptions.recursive),
                Retry = options.Contains(FolderOps.FolderDeleteOptions.RetryIfInUse),
                Log = options.Contains(FolderOps.FolderDeleteOptions.Logger),
                filterAndRemoveEmptyFolders = options.Contains(FolderOps.FolderDeleteOptions.filterAnRemoveEmptyFolders),
                Backup = options.Contains(FolderOps.FolderDeleteOptions.BackupBeforeDeleate)
            };
            long fullSize = await FolderOps.GetSizeAsync(paths, opt.Recursive);
            int _successCount = 0;
            int _failureCount = 0;

            for (int i = 0; i < paths.Count; i++)
            {
                string currentPath = paths[i];
                if (!Directory.Exists(currentPath) && !File.Exists(currentPath))
                {
                    _failureCount++;
                    continue;
                }

                FileSystemInfo fsInfo = Directory.Exists(currentPath)
                    ? new DirectoryInfo(currentPath)
                    : new FileInfo(currentPath);

                try
                {
                    if (fsInfo is DirectoryInfo dir)
                    {
                        if (opt.filterAndRemoveEmptyFolders && dir.GetDirectories().Length == 0 && dir.GetFiles().Length == 0)
                        {
                            dir.Delete();
                            _successCount++;
                            continue;
                        }
                        if (opt.Recursive)
                        {
                            var subDirs = dir.GetDirectories();
                            for (int y = subDirs.Length - 1; y >= 0; y--)
                                paths.Insert(i + 1, subDirs[y].FullName);

                            var subFiles = dir.GetFiles();
                            for (int y = subFiles.Length - 1; y >= 0; y--)
                                paths.Insert(i + 1, subFiles[y].FullName);

                            continue;
                        }
                    }
                    long size = (fsInfo is DirectoryInfo d) ? await GetFolderSizeParallel(d) : ((FileInfo)fsInfo).Length;
                    if (!PassesCommonFilters(fsInfo, size, NameFilter, minSize, maxSize, creationStart, creationEnd, lastWriteStart, lastWriteEnd, startWith, attributes))
                        continue;

                    await ExecuteWithRetry(async () =>
                    {
                        if (opt.Backup)
                        {
                            string backupPath = Path.Combine(Path.GetTempPath(), $"{fsInfo.Name}_Backup_{DateTime.Now.Ticks}");
                            if (fsInfo is DirectoryInfo dInfo) await FolderOps.CopyFolder_Async(dInfo.FullName, backupPath);
                            else File.Copy(fsInfo.FullName, backupPath);
                        }

                        if (fsInfo is DirectoryInfo dirInfo) await opForDir(dirInfo, opt.Recursive);
                        else if (fsInfo is FileInfo fileInfo) await opForFile(fileInfo);

                        if (opt.Log) Logger.logForThisTool($"Action completed on: {fsInfo.FullName}");
                    }, opt.Retry);
                    float Percent = (float)(size / fullSize) / 100;
                    p?.Report(Percent);
                    _successCount++;
                }
                catch (Exception ex)
                {
                    _failureCount++;
                    if (opt.Log) Logger.logForThisTool($"Error on {fsInfo.FullName}: {ex.Message}");
                }
            }

            return new Result { FailedCount = _failureCount, SuccessCount = _successCount, Success = _failureCount == 0 };
        }

        // =========================
        // Retry Logic
        // =========================
        private static async Task ExecuteWithRetry(Func<Task> action, bool retryIfInUse)
        {
            int retryCount = 0;
            const int maxRetry = 3;

            while (true)
            {
                try
                {
                    await action();
                    break;
                }
                catch (IOException) when (retryIfInUse && retryCount < maxRetry)
                {
                    retryCount++;
                    await Task.Delay(5000);
                }
            }
        }

        // =========================
        // Filtering
        // =========================
        private static bool PassesCommonFilters(FileSystemInfo item, long size, List<string> namesFilter, long? minSize, long? maxSize, DateTime? creationStart, DateTime? creationEnd, DateTime? lastWriteStart, DateTime? lastWriteEnd, List<string> startWith, FileAttributes? attributes)
        {
            if (namesFilter?.Count > 0 && !namesFilter.Contains(item.Name)) return false;
            if (startWith?.Count > 0 && !startWith.Any(p => item.Name.StartsWith(p, StringComparison.OrdinalIgnoreCase))) return false;

            if ((creationStart.HasValue && item.CreationTime < creationStart.Value) || (creationEnd.HasValue && item.CreationTime > creationEnd.Value)) return false;
            if ((lastWriteStart.HasValue && item.LastWriteTime < lastWriteStart.Value) || (lastWriteEnd.HasValue && item.LastWriteTime > lastWriteEnd.Value)) return false;

            if (attributes.HasValue && (item.Attributes & attributes.Value) != attributes.Value) return false;

            if (minSize.HasValue && size < minSize.Value) return false;
            if (maxSize.HasValue && size > maxSize.Value) return false;

            return true;
        }
    }
}