using System.Windows;

namespace BattleCity
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) //открытие меню при запуске приложения
        {
            base.OnStartup(e);

            Menu menu = new();
            menu.ShowDialog();

        }
    }
}
