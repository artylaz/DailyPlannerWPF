using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.Views.UserControls;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.UserControlsViewModels
{
    class NotificationUserControlViewModel : ViewModel    
    {

        public NotificationUserControl notificationUC;

        private string title;
        public string Title { get => title; set => Set(ref title, value); }

        private string name;
        public string Name { get => name; set => Set(ref name, value); }

        private string image;
        public string Image { get => image; set => Set(ref image, value); }

        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(notificationUC);
                    taskbarIcon.CloseBalloon();
                });
            }
        }

    }
}
