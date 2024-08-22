using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TestingProjectWPF.Commands;
using TestingProjectWPF.Services.Interfaces;

namespace TestingProjectWPF.ViewModels
{
    internal class FirstTaskViewModel : BaseViewModel
    {
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IFileDataService _fileDataService;

        private string _statusMessage;
        private string _filter;
        string _connectionString;

        public RelayCommand GenerateFilesCommand { get; }
        public RelayCommand MergeFilesCommand { get; }
        public RelayCommand ImportDataCommand { get; }
        public RelayCommand CancelTokenCommand { get; }

        public FirstTaskViewModel(IFileDataService fileDataService, string connectionString)
        {
            _fileDataService = fileDataService;
            _fileDataService.StatusUpdated += OnStatusUpdated;
            _connectionString = connectionString;
            GenerateFilesCommand = new RelayCommand(async () => await GenerateFilesAsync());
            MergeFilesCommand = new RelayCommand(async () => await MergeFilesAsync(), () => !string.IsNullOrEmpty(Filter));
            ImportDataCommand = new RelayCommand(async () => await ImportDataAsync(), () => File.Exists("merged.txt"));
            CancelTokenCommand = new RelayCommand(() =>  CancelOperation(), () => _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested);
        }
 
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
        private void OnStatusUpdated(string status)
        {
            StatusMessage = status;
        }

        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged();
                MergeFilesCommand.OnCanExecuteChanged();
            }
        }

        private async Task GenerateFilesAsync()
        {
            StatusMessage = "Генерация файлов началась...";
            _cancellationTokenSource = new CancellationTokenSource();
            CancelTokenCommand.OnCanExecuteChanged();
            try
            {
                await _fileDataService.GenerateFilesAsync(100, _cancellationTokenSource.Token);
                StatusMessage = "Генерация файлов завершена.";
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Генерация файлов отменена.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                CancelTokenCommand.OnCanExecuteChanged();
            }
        }

        private async Task MergeFilesAsync()
        {
            StatusMessage = "Слияние файлов началось...";
            _cancellationTokenSource = new CancellationTokenSource();
            CancelTokenCommand.OnCanExecuteChanged();
            try
            {
                int removedCount = await _fileDataService.MergeFilesAsync(Filter, "merged.txt", _cancellationTokenSource.Token);
                StatusMessage = $"Слияние завершено. Удалено строк: {removedCount}.";
                ImportDataCommand.OnCanExecuteChanged();
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Слияние файлов отменено.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                CancelTokenCommand.OnCanExecuteChanged();
            }
        }

        private async Task ImportDataAsync()
        {
            StatusMessage = "Импорт данных начался...";
            _cancellationTokenSource = new CancellationTokenSource();
            CancelTokenCommand.OnCanExecuteChanged();
            try
            {
                await _fileDataService.ImportDataAsync("merged.txt", _connectionString, _cancellationTokenSource.Token);
                StatusMessage = "Импорт данных завершён.";
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Импорт данных отменён.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                CancelTokenCommand.OnCanExecuteChanged();
            }
        }
        private Task CancelOperation()
        {
            _cancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }
    }
}
