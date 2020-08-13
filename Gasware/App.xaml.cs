using Gasware.Common;
using System.CodeDom;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = Containers.GetAllContaines();


            //var mainWindowViewModel = container.Resolve<MainWindow>();
            //var window = new MainWindow { DataContext = mainWindowViewModel };
            var window = container.Resolve<MainWindow>();
            window = new MainWindow(container);
            window.Show();
        }
    }
}
