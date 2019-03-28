using System.Windows;

namespace Интерпретатор
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var view = new MainWindow
            {
                DataContext = new ApplicationViewModel()
            };
            view.InitializeComponent();
            view.Show();
        }
    }
}
