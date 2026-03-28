namespace NeraXTools
{
    public static partial class FolderOps
    {
        // =========================
        // Get Folder Size Methods - Sync
        // =========================

        // =========================
        // Get Folder Size Methods - Sync for single folder
        // =========================
        public static long GetSize(string folderPath, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { folderPath }, recursive);

        public static double GetSizeKB(string folderPath, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { folderPath }, recursive) / 1024.0;

        public static double GetSizeMB(string folderPath, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { folderPath }, recursive) / (1024.0 * 1024.0);

        public static double GetSizeGB(string folderPath, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { folderPath }, recursive) / (1024.0 * 1024.0 * 1024.0);

        // =========================
        // Get Folder Size Methods - Sync for multiple folders
        // =========================
        /// <summary>
        /// Calculates the total size (in bytes) of the given file and folder paths.
        /// </summary>
        /// <param name="paths">
        /// A list of file and/or folder paths. Invalid or non-existing paths are ignored.
        /// </param>
        /// <param name="recursive">
        /// If true, folder sizes are calculated recursively (including all subfolders and files).
        /// If false, only the top-level files inside each folder are counted.
        /// </param>
        /// <returns>
        /// Total size in bytes.
        /// </returns>
        /// <remarks>
        /// This method is fail-safe:
        /// - Non-existing paths are skipped.
        /// - Access errors inside folders are ignored silently.
        /// - Both files and folders are supported.
        /// </remarks>
        public static long GetSize(List<string> paths, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, recursive);

        /// <summary>
        /// Calculates the total size (in kilobytes) of the given paths.
        /// </summary>
        /// <param name="paths">List of file and/or folder paths.</param>
        /// <param name="recursive">Determines whether folder traversal is recursive.</param>
        /// <returns>Total size in KB.</returns>
        /// <remarks>
        /// 1 KB = 1024 bytes.
        /// Uses double for fractional precision.
        /// </remarks>
        public static double GetSizeKB(List<string> paths, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, recursive) / 1024.0;

        /// <summary>
        /// Calculates the total size (in megabytes) of the given paths.
        /// </summary>
        /// <param name="paths">List of file and/or folder paths.</param>
        /// <param name="recursive">Determines whether folder traversal is recursive.</param>
        /// <returns>Total size in MB.</returns>
        /// <remarks>
        /// 1 MB = 1024 * 1024 bytes.
        /// Suitable for medium-sized datasets.
        /// </remarks>
        public static double GetSizeMB(List<string> paths, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, recursive) / (1024.0 * 1024.0);

        /// <summary>
        /// Calculates the total size (in gigabytes) of the given paths.
        /// </summary>
        /// <param name="paths">List of file and/or folder paths.</param>
        /// <param name="recursive">Determines whether folder traversal is recursive.</param>
        /// <returns> size in GB.</returns>
        /// <remarks>
        /// 1 GB = 1024 * 1024 * 1024 bytes.
        /// Recommended for large datasets and storage reporting.
        /// </remarks>
        public static double GetSizeGB(List<string> paths, bool recursive = true)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, recursive) / (1024.0 * 1024.0 * 1024.0);

        // =========================
        // Get Folder Size Methods - Async
        // =========================

        // =========================
        // Get Folder Size Methods - Async for single folder
        // =========================
        public static async Task<long> GetSizeAsync(string folderPath, bool recursive = true, CancellationToken token = default)
            => await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { folderPath }, recursive, token);

        public static async Task<double> GetSizeKBAsync(string folderPath, bool recursive = true, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { folderPath }, recursive, token)) / 1024.0;

        public static async Task<double> GetSizeMBAsync(string folderPath, bool recursive = true, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { folderPath }, recursive, token)) / (1024.0 * 1024.0);

        public static async Task<double> GetSizeGBAsync(string folderPath, bool recursive = true, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { folderPath }, recursive, token)) / (1024.0 * 1024.0 * 1024.0);

        /// <summary>
        /// Asynchronously calculates the total size (in bytes) of the given file and folder paths.
        /// </summary>
        /// <param name="paths">
        /// A list of file and/or folder paths. Invalid or non-existing paths are ignored.
        /// </param>
        /// <param name="recursive">
        /// If true, folder sizes are calculated recursively (including all subfolders and files).
        /// If false, only the top-level files inside each folder are counted.
        /// </param>
        /// <param name="token">
        /// Optional cancellation token to cancel the operation.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. Result is total size in bytes.
        /// </returns>
        /// <remarks>
        /// - Uses TaskSchedulerEngine for controlled async execution.
        /// - Safe against invalid paths and access exceptions.
        /// - Suitable for large directory trees without blocking the calling thread.
        /// </remarks>
        public static async Task<long> GetSizeAsync(List<string> paths, bool recursive = true, CancellationToken token = default)
            => await FileAndFolderOpsCore.GetSizeAsync_Core(paths, recursive, token);

        /// <summary>
        /// Asynchronously calculates the total size (in kilobytes) of the given paths.
        /// </summary>
        /// <param name="paths">List of file and/or folder paths.</param>
        /// <param name="recursive">Determines whether folder traversal is recursive.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns> size in KB.</returns>
        /// <remarks>
        /// 1 KB = 1024 bytes.
        /// Uses double for better precision.
        /// </remarks>
        public static async Task<double> GetSizeKBAsync(List<string> paths, bool recursive = true, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(paths, recursive, token)) / 1024.0;

        /// <summary>
        /// Asynchronously calculates the total size (in megabytes) of the given paths.
        /// </summary>
        /// <param name="paths">List of file and/or folder paths.</param>
        /// <param name="recursive">Determines whether folder traversal is recursive.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns> size in MB.</returns>
        /// <remarks>
        /// 1 MB = 1024 * 1024 bytes.
        /// Recommended for medium-sized datasets.
        /// </remarks>
        public static async Task<double> GetSizeMBAsync(List<string> paths, bool recursive = true, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(paths, recursive, token)) / (1024.0 * 1024.0);

        /// <summary>
        /// Asynchronously calculates the total size (in gigabytes) of the given paths.
        /// </summary>
        /// <param name="paths">List of file and/or folder paths.</param>
        /// <param name="recursive">Determines whether folder traversal is recursive.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns> size in GB.</returns>
        /// <remarks>
        /// 1 GB = 1024 * 1024 * 1024 bytes.
        /// Best suited for large-scale storage reporting.
        /// </remarks>
        public static async Task<double> GetSizeGBAsync(List<string> paths, bool recursive = true, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(paths, recursive, token)) / (1024.0 * 1024.0 * 1024.0);
    }

    public static partial class FileOps
    {
        // =========================
        // Get Folder Size Methods - Sync
        // =========================

        /// <summary>
        /// Calculates the file size (in bytes) of the given file paths.
        /// </summary>
        /// <param name="path">
        /// A  file path. Folder paths are accepted but only their top-level files are counted.
        /// Invalid paths are ignored.
        /// </param>
        /// <returns>file size in bytes.</returns>
        /// <remarks>
        /// This method is tolerant:
        /// - If a folder is passed, it will not recurse into subfolders.
        /// - Non-existing files are skipped.
        /// </remarks>
        public static long GetSize(string path)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { path }, false);

        /// <summary>
        /// Calculates the file size (in kilobytes) of the given file paths.
        /// </summary>
        /// <param name="path">List of file paths.</param>
        /// <returns> size in KB.</returns>
        /// <remarks>
        /// This method is tolerant:
        /// - If a folder is passed, it will not recurse into subfolders.
        /// - Non-existing files are skipped.
        /// </remarks>
        public static double GetSizeKB(string path)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { path }, false) / 1024.0;

        /// <summary>
        /// Calculates the File size (in megabytes) of the given file paths.
        /// </summary>
        /// <param name="path">List of file paths.</param>
        /// <returns>File size in MB.</returns>
        public static double GetSizeMB(string path)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { path }, false) / (1024.0 * 1024.0);

        /// <summary>
        /// Calculates the File size (in gigabytes) of the given file paths.
        /// </summary>
        /// <param name="path">List of file paths.</param>
        /// <returns>File size in GB.</returns>
        public static double GetSizeGB(string path)
            => FileAndFolderOpsCore.GetSizeSync_Core(new List<string> { path }, false) / (1024.0 * 1024.0 * 1024.0);

        /// <summary>
        /// Calculates the total size (in bytes) of the given file paths.
        /// </summary>
        /// <param name="paths">
        /// A list of file paths. Folder paths are accepted but only their top-level files are counted.
        /// Invalid paths are ignored.
        /// </param>
        /// <returns> size in bytes.</returns>
        /// <remarks>
        /// This method is tolerant:
        /// - If a folder is passed, it will not recurse into subfolders.
        /// - Non-existing files are skipped.
        /// </remarks>
        public static long GetSize(List<string> paths)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, false);

        /// <summary>
        /// Calculates the total size (in kilobytes) of the given file paths.
        /// </summary>
        /// <param name="paths">List of file paths.</param>
        /// <returns> size in KB.</returns>
        public static double GetSizeKB(List<string> paths)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, false) / 1024.0;

        /// <summary>
        /// Calculates the total size (in megabytes) of the given file paths.
        /// </summary>
        /// <param name="paths">List of file paths.</param>
        /// <returns> size in MB.</returns>
        public static double GetSizeMB(List<string> paths)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, false) / (1024.0 * 1024.0);

        /// <summary>
        /// Calculates the total size (in gigabytes) of the given file paths.
        /// </summary>
        /// <param name="paths">List of file paths.</param>
        /// <returns> size in GB.</returns>
        public static double GetSizeGB(List<string> paths)
            => FileAndFolderOpsCore.GetSizeSync_Core(paths, false) / (1024.0 * 1024.0 * 1024.0);

        // =========================
        // Get Folder Size Methods - Async
        // =========================
        /// <summary>
        /// Asynchronously calculates the file size (in bytes) of the given file paths.
        /// </summary>
        /// <param name="path">
        /// A list of file paths. Folder paths are accepted but only their top-level files are counted.
        /// Invalid paths are ignored.
        /// </param>
        /// <param name="token">
        /// Optional cancellation token to cancel the operation.
        /// </param>
        /// <returns> size in bytes.</returns>
        /// <remarks>
        /// - Non-existing files are skipped.
        /// - Folder recursion is disabled for consistency with FileOps behavior.
        /// </remarks>
        public static async Task<long> GetSizeAsync(string path, CancellationToken token = default)
            => await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { path }, false, token);

        /// <summary>
        /// Asynchronously calculates the file size (in kilobytes) of the given file paths.
        /// </summary>
        /// <param name="path">List of file paths.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns> size in KB.</returns>
        public static async Task<double> GetSizeKBAsync(string path, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { path }, false, token)) / 1024.0;

        /// <summary>
        /// Asynchronously calculates the file size (in megabytes) of the given file paths.
        /// </summary>
        /// <param name="path">List of file paths.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns> size in MB.</returns>
        public static async Task<double> GetSizeMBAsync(string path, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { path }, false, token)) / (1024.0 * 1024.0);

        /// <summary>
        /// Asynchronously calculates the file size (in gigabytes) of the given file paths.
        /// </summary>
        /// <param name="path">List of file paths.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns> size in GB.</returns>
        public static async Task<double> GetSizeGBAsync(string path, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(new List<string> { path }, false, token)) / (1024.0 * 1024.0 * 1024.0);

        /// <summary>
        /// Asynchronously calculates the total size (in bytes) of the given file paths.
        /// </summary>
        /// <param name="paths">
        /// A list of file paths. Folder paths are accepted but only their top-level files are counted.
        /// Invalid paths are ignored.
        /// </param>
        /// <param name="token">
        /// Optional cancellation token to cancel the operation.
        /// </param>
        /// <returns>Total size in bytes.</returns>
        /// <remarks>
        /// - Non-existing files are skipped.
        /// - Folder recursion is disabled for consistency with FileOps behavior.
        /// </remarks>
        public static async Task<long> GetSizeAsync(List<string> paths, CancellationToken token = default)
            => await FileAndFolderOpsCore.GetSizeAsync_Core(paths, false, token);

        /// <summary>
        /// Asynchronously calculates the total size (in kilobytes) of the given file paths.
        /// </summary>
        /// <param name="paths">List of file paths.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Total size in KB.</returns>
        public static async Task<double> GetSizeKBAsync(List<string> paths, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(paths, false, token)) / 1024.0;

        /// <summary>
        /// Asynchronously calculates the total size (in megabytes) of the given file paths.
        /// </summary>
        /// <param name="paths">List of file paths.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Total size in MB.</returns>
        public static async Task<double> GetSizeMBAsync(List<string> paths, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(paths, false, token)) / (1024.0 * 1024.0);

        /// <summary>
        /// Asynchronously calculates the total size (in gigabytes) of the given file paths.
        /// </summary>
        /// <param name="paths">List of file paths.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Total size in GB.</returns>
        public static async Task<double> GetSizeGBAsync(List<string> paths, CancellationToken token = default)
            => (await FileAndFolderOpsCore.GetSizeAsync_Core(paths, false, token)) / (1024.0 * 1024.0 * 1024.0);
    }
}