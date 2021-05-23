using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace DailyPlannerWPF.Resources.NotifyIcons
{
    class NotifyIconViewModel : ViewModel
    {


        public ICommand OpenMainWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Application.Current.MainWindow = new MainWindow();
                    Application.Current.MainWindow.Show();
                });
            }
        }

        public ICommand CloseAppCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Application.Current.Shutdown();
                });
            }
        }
    }
}
