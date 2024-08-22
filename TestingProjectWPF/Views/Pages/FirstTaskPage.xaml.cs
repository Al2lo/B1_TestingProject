using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using TestingProjectWPF.Services;
using TestingProjectWPF.ViewModels;

namespace TestingProjectWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для FirstTaskPage.xaml
    /// </summary>
    public partial class FirstTaskPage : Page
    {
        public FirstTaskPage()
        {
            InitializeComponent();
            var fileDataService = new FileDataService();
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;
                DataContext = new FirstTaskViewModel(fileDataService, connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
