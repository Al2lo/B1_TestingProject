using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using TestingProjectWPF.Data;
using TestingProjectWPF.Models;
using TestingProjectWPF.Services.Interfaces;
using OfficeOpenXml;
using System.Linq;
using System.Data.Entity;

namespace TestingProjectWPF.Services
{
    internal class ExelDataService: IExelDataService
    {
        private readonly ApplicationContext _context;

        public ExelDataService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<bool> UploadExcelFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                if (fileStream == null || fileStream.Length == 0)
                    throw new InvalidOperationException("File stream is null or empty.");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                fileStream.Position = 0;

                var uploadedFile = new UploadedFile
                {
                    FileName = fileName,
                    UploadedAt = DateTime.Now
                };

                using (var package = new ExcelPackage(fileStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    var balances = new List<Balance>();
                    var className = "";

                    for (int row = 9; row <= rowCount-2; row++)
                    {
                        if (worksheet.Cells[row, 1].Text.Contains("КЛАСС"))
                        {
                            className = worksheet.Cells[row, 1].Text;
                            continue;
                        }

                        if (worksheet.Cells[row, 1].Text.Count() == 2)
                            continue;

                        var accountId = worksheet.Cells[row, 1].Text;
                        var accountClass = className;
                        var openingBalanceActive = decimal.Parse(worksheet.Cells[row, 2].Text);
                        var openingBalancePassive = decimal.Parse(worksheet.Cells[row, 3].Text);
                        var turnoverDebit = decimal.Parse(worksheet.Cells[row, 4].Text);
                        var turnoverCredit = decimal.Parse(worksheet.Cells[row, 5].Text);

                        var balance = (new Balance
                        {
                            AccountId = accountId,
                            OpeningActive = openingBalanceActive,
                            OpeningPassive = openingBalancePassive,
                            TurnoverDebit = turnoverDebit,
                            TurnoverCredit = turnoverCredit,
                            UploadedFile = uploadedFile
                        });

                        balances.Add(balance);

                        _context.Accounts.Add(new Account() { Id = accountId, Class = accountClass, Balance = balance});
                    }

                    uploadedFile.Balances = balances;
                    _context.UploadedFiles.Add(uploadedFile);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
}

        public async Task<List<UploadedFile>> GetUploadedFilesAsync()
        {
            return await _context.UploadedFiles.OrderByDescending(f => f.UploadedAt).ToListAsync();
        }

        public async Task<List<Balance>> GetBalancesByFileIdAsync()
        {
            return await _context.Balances.ToListAsync();
        }
    }
}
