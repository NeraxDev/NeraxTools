using NeraXTools.TaskManager;

namespace NeraXTools
{
    internal static partial class FileAndFolderOpsCore
    {
        // =========================
        // Sync Wrapper
        // =========================
        private static void Delete_Sync(
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
            IProgress<float> p = null,
            params FolderOps.FolderDeleteOptions[] options)
        {
            Func<DirectoryInfo, bool, Task> opDir = (dir, recursive) =>
            {
                dir.Delete(recursive);
                return Task.CompletedTask;
            };

            Func<FileInfo, Task> opFile = (file) =>
            {
                file.Delete();
                return Task.CompletedTask;
            };

            Delete_Core(
                paths, startWith, NameFilter, attributes,
                creationStart, creationEnd, lastWriteStart, lastWriteEnd,
                minSize, maxSize, opDir, opFile, p, options
            ).GetAwaiter().GetResult();
        }

        // =========================
        // Parallel Async Wrapper
        // =========================
        private static async Task Delete_ParallelAsync(
            List<string> paths,
            int userInputThreads = 2,
            List<string> startWith = null,
            List<string> NameFilter = null,
            FileAttributes? attributes = null,
            DateTime? creationStart = null,
            DateTime? creationEnd = null,
            DateTime? lastWriteStart = null,
            DateTime? lastWriteEnd = null,
            long? minSize = null,
            long? maxSize = null,
            IProgress<float> p = null,
            params FolderOps.FolderDeleteOptions[] options)
        {
            Func<DirectoryInfo, bool, Task> opDir = async (dir, recursive) =>
                await TaskSchedulerEngine.RunSyncAsAsync(() => dir.Delete(recursive));

            Func<FileInfo, Task> opFile = async (file) =>
                await TaskSchedulerEngine.RunSyncAsAsync(() => file.Delete());

            await Delete_Core(
                paths, startWith, NameFilter, attributes,
                creationStart, creationEnd, lastWriteStart, lastWriteEnd,
                minSize, maxSize, opDir, opFile, p, options
            );
        }
    }
}