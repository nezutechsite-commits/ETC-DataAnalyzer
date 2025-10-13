using ETC_DataAnalyzer.ViewModels;
using ETC_DataAnalyzer.Views;
using System.Windows;

namespace ETC_DataAnalyzer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var window = new MainWindow()
        {
            DataContext = new MainWindowViewModel(),
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };
        window.Show();
    }
}
