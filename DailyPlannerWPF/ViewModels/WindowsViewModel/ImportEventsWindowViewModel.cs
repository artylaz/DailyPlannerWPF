using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    class ImportEventsWindowViewModel : ViewModel
    {
        internal ApplicationContext db;
        internal ImportEventsWindow importEventsWindow;

        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    importEventsWindow.DragMove();
                });
            }
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    importEventsWindow.Close();
                });
            }
        }

        public ObservableCollection<Event> Events { get; set; }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    foreach (var item in Events)
                    {
                        db.Events.Add(item);
                    }
                    db.SaveChanges();
                    importEventsWindow.Close();
                });
            }
        }

    }
}
