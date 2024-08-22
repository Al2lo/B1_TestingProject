using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestingProjectWPF.Services.Interfaces;

namespace TestingProjectWPF.Services
{
    internal class FileDataService : IFileDataService
    {
        public event Action<string> StatusUpdated;

        private static readonly Random rand = new Random();
        private int importedLines = 0;
        private int totalLines = 0;

        private readonly object lockObj = new object();

        public async Task GenerateFilesAsync(int fileCount, CancellationToken cancellationToken)
        {
            const int maxConcurrentTasks = 2;
            var tasks = new List<Task>();

            for (int i = 0; i < fileCount; i++)
            {
                tasks.Add(GenerateFileAsync(i, cancellationToken));

                if (tasks.Count >= maxConcurrentTasks)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task GenerateFileAsync(int indexFile, CancellationToken cancellationToken)
        {
            using (StreamWriter writer = new StreamWriter($"file{indexFile}.txt"))
            {
                for (int j = 0; j < 100000; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string line = $"{GenerateRandomDate():dd.MM.yyyy}||" +
                                  $"{GenerateRandomString(10, 'A', 'z')}||" +
                                  $"{GenerateRandomString(10, 'А', 'я')}||" +
                                  $"{GenerateRandomEvenInteger(1, 100000000)}||" +
                                  $"{GenerateRandomDecimal(1.00000001m, 20.0m)}||";

                    await writer.WriteLineAsync(line);
                }
            }
        }

        public async Task<int> MergeFilesAsync(string filter, string mergedFilePath, CancellationToken cancellationToken)
        {
            int totalRemoved = 0;
            var files = Directory.GetFiles(".", "file*.txt");
            var tasks = new List<Task<(int removedLines, string[] lines)>>();

            foreach (var file in files)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var lines = await Task.Run(() => File.ReadAllLines(file));
                    var filteredLines = lines.Where(line => !line.Contains(filter)).ToArray();
                    int removedLines = lines.Length - filteredLines.Length;

                    return (removedLines, filteredLines);
                }));
            }

            var results = await Task.WhenAll(tasks);

            totalRemoved = results.Sum(result => result.removedLines);

            using (StreamWriter writer = new StreamWriter(mergedFilePath))
            {
                foreach (var result in results)
                    await writer.WriteLineAsync(string.Join(Environment.NewLine, result.lines));
            }

            return totalRemoved;
        }

        private async Task ProcessLinesAsync(string[] lines, string connectionString, CancellationToken cancellationToken)
        {
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    var transaction = connection.BeginTransaction();

                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "INSERT INTO FirstTaskTable (Date, LatinString, RussianString, IntegerNumber, DecimalNumber) VALUES (@date, @latin, @russian, @integerNumber, @decimalNumber)";

                            foreach (var line in lines)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var parts = line.Split(new[] { "||" }, StringSplitOptions.None);

                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@date", DateTime.Parse(parts[0]));
                                command.Parameters.AddWithValue("@latin", parts[1]);
                                command.Parameters.AddWithValue("@russian", parts[2]);
                                command.Parameters.AddWithValue("@integerNumber", int.Parse(parts[3]));
                                command.Parameters.AddWithValue("@decimalNumber", decimal.Parse(parts[4]));

                                await command.ExecuteNonQueryAsync(cancellationToken);
                                lock (lockObj)
                                {
                                    importedLines++;
                                }

                                StatusUpdated.Invoke($"Импортировано: {importedLines}\t Осталось: {totalLines - importedLines}");

                                if (importedLines % 10000 == 0)
                                    {
                                        transaction.Commit();
                                        transaction.Dispose();
                                        transaction = connection.BeginTransaction();
                                        command.Transaction = transaction;
                                    }

                            }
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
            }
        }

        public async Task ImportDataAsync(string filePath, string connectionString, CancellationToken cancellationToken)
        {
            var lines = await Task.Run(() => File.ReadAllLines(filePath));
            totalLines = lines.Length;

            int numberOfParts = 3;
            var tasks = new List<Task>();
            int partSize = (int)Math.Ceiling((double)totalLines / numberOfParts);

            for (int i = 0; i < numberOfParts; i++)
            {
                var part = lines.Skip(i * partSize).Take(partSize).ToArray();
                tasks.Add(ProcessLinesAsync(part, connectionString, cancellationToken)); 
            }

            await Task.WhenAll(tasks);
        }
        
        private DateTime GenerateRandomDate()
        {
            int year = DateTime.Now.Year - rand.Next(0, 6);
            return new DateTime(year, rand.Next(1, 13), rand.Next(1, 29));
        }

        private string GenerateRandomString(int length, char start, char end)
        {
            return new string(Enumerable.Range(0, length)
                .Select(_ => (char)rand.Next(start, end + 1)).ToArray());
        }

        private int GenerateRandomEvenInteger(int min, int max)
        {
            return rand.Next(min / 2, (max / 2) + 1) * 2;
        }

        private decimal GenerateRandomDecimal(decimal min, decimal max)
        {
            return Math.Round((decimal)(rand.NextDouble() * (double)(max - min) + (double)min), 8);
        }
    }
}
