namespace NeraXTools
{
    internal static partial class FileAndFolderOpsCore
    {
        private static async Task<long> GetTotalSize_Core(List<string> paths, bool recursiveForFolders)
        {
            long totalSize = 0;

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    totalSize += new FileInfo(path).Length;
                }
                else if (Directory.Exists(path))
                {
                    if (recursiveForFolders)
                        totalSize += await GetFolderSizeParallel(new DirectoryInfo(path));
                    else
                    {
                        var dirInfo = new DirectoryInfo(path);
                        totalSize += dirInfo.GetFiles().Sum(f => f.Length);
                    }
                }
            }

            return totalSize;
        }

        private static async Task<long> GetFolderSizeParallel(DirectoryInfo dir)
        {
            try
            {
                long size = dir.GetFiles().Sum(f => f.Length);

                var subDirTasks = dir.GetDirectories().Select(d => GetFolderSizeParallel(d));
                var results = await Task.WhenAll(subDirTasks);

                return size + results.Sum();
            }
            catch
            {
                return 0;
            }
        }
    }
}