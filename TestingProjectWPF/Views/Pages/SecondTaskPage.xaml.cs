using System.Windows.Controls;
using TestingProjectWPF.Data;
using TestingProjectWPF.Services;
using TestingProjectWPF.Services.Interfaces;
using TestingProjectWPF.ViewModels;

namespace TestingProjectWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для SecondTaskPage.xaml
    /// </summary>
    public partial class SecondTaskPage : Page
    {
        public SecondTaskPage()
        {
            InitializeComponent();
            ApplicationContext context = new ApplicationContext();
            IExelDataService exelDataService = new ExelDataService(context);
            DataContext = new SecondTaskViewModel(exelDataService);
        }
    }
}
