using System.Threading.Tasks;
using System.Threading;
using System;

namespace TestingProjectWPF.Services.Interfaces
{
    public interface IFileDataService
    {
        event Action<string> StatusUpdated;
        Task GenerateFilesAsync(int fileCount, CancellationToken cancellationToken);
        Task<int> MergeFilesAsync(string filter, string mergedFilePath, CancellationToken cancellationToken);
        Task ImportDataAsync(string filePath, string connectionString, CancellationToken cancellationToken);
    }
}
