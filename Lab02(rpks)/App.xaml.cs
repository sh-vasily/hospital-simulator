using System.Windows;

namespace Lab02_rpks_
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var view = new MainWindow
            {
                DataContext = new IDDPresenter()
            };
            view.InitializeComponent();
            view.Show();
        }
    }
}
