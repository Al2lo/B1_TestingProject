using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TestingProjectWPF.Commands;
using TestingProjectWPF.ViewModels;
using TestingProjectWPF.Views.Pages;

public class MainViewModel : BaseViewModel
{
    private Page content = new FirstTaskPage();
    private Uri source = new Uri("../Pages/FirstTaskPage.xaml", UriKind.RelativeOrAbsolute);
    public Page Content
    {
        get { return content; }
        set
        {
            if (content != value)
                content = value;

            OnPropertyChanged();
        }
    }
    public Uri Source
    {
        get { return source; }
        set
        {
            if (source != value)
                source = value;

            OnPropertyChanged();
        }
    }
    public ICommand MoveToFirstTask { get; }
    public ICommand MoveToSecondTask { get; }
    public MainViewModel()
    {
        MoveToFirstTask = new RelayCommand(ExecuteMoveToFirstTask,()=>true );
        MoveToSecondTask = new RelayCommand(ExecuteMoveToSecondTask, () => true);
    }

    private Task ExecuteMoveToFirstTask()
    {
        Content = new FirstTaskPage();
        Source = new Uri("../Pages/FirstTaskPage.xaml", UriKind.RelativeOrAbsolute);

        return Task.CompletedTask;
    }

    private Task ExecuteMoveToSecondTask()
    {
        Content = new SecondTaskPage();
        Source = new Uri("../Pages/SecondTaskPage.xaml", UriKind.RelativeOrAbsolute);

        return Task.CompletedTask;
    }
}