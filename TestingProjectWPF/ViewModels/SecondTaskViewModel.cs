using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TestingProjectWPF.Commands;
using TestingProjectWPF.Models;
using TestingProjectWPF.Services.Interfaces;

namespace TestingProjectWPF.ViewModels
{
    internal class SecondTaskViewModel : BaseViewModel
    {
        private readonly IExelDataService _exelDataService;

        private ObservableCollection<UploadedFile> _uploadedFiles;
        private ObservableCollection<Balance> _balances;

        public ICommand UploadCommand { get; }
        public ICommand LoadBalancesCommand { get; }
        public ICommand LoadUpploadedFilesCommand { get; }

        public SecondTaskViewModel(IExelDataService exelDataService)
        {
            _exelDataService = exelDataService;

            UploadedFiles = new ObservableCollection<UploadedFile>();
            Balances = new ObservableCollection<Balance>();

            LoadUpploadedFilesCommand = new RelayCommand(async () => await LoadUploadedFilesAsync());
            UploadCommand = new RelayCommand(async () => await UploadFileAsync());
            LoadBalancesCommand = new RelayCommand(async () => await LoadBalancesAsync());
        }
        public ObservableCollection<UploadedFile> UploadedFiles
        {
            get => _uploadedFiles;
            set => _uploadedFiles = value;
        }

        public ObservableCollection<Balance> Balances
        {
            get => _balances;
            set => _balances = value;
        }
        private async Task UploadFileAsync()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx"
            };

            if (fileDialog.ShowDialog() == true)
            {
                using (var stream = fileDialog.OpenFile())
                {
                    await _exelDataService.UploadExcelFileAsync(stream, fileDialog.FileName);
                    await LoadUploadedFilesAsync();
                }
            }
        }

        private async Task LoadUploadedFilesAsync()
        {
            var files = await _exelDataService.GetUploadedFilesAsync();
            UploadedFiles.Clear();

            foreach (var file in files)
                UploadedFiles.Add(file);
        }

        private async Task LoadBalancesAsync()
        {
            var balances = await _exelDataService.GetBalancesByFileIdAsync();
            Balances.Clear();

            foreach (var balance in balances)
                Balances.Add(balance);
        }
    }
}
