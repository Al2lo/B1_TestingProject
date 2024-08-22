using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TestingProjectWPF.Models;

namespace TestingProjectWPF.Services.Interfaces
{
    internal interface IExelDataService
    {
        Task<bool> UploadExcelFileAsync(Stream fileStream, string fileName);
        Task<List<UploadedFile>> GetUploadedFilesAsync();
        Task<List<Balance>> GetBalancesByFileIdAsync();
    }
}
