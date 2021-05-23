using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using System;
using System.Collections.ObjectModel;

namespace DailyPlannerWPF.ViewModels.PagesViewModels
{
    internal class HomePageViewModel : ViewModel
    {
        private ObservableCollection<MyTask> tasksToDay;
        public ObservableCollection<MyTask> TasksToDay { get => tasksToDay; set => Set(ref tasksToDay,value); }

        private ObservableCollection<MyTask> importantTasks;
        public ObservableCollection<MyTask> ImportantTasks { get => importantTasks; set => Set(ref importantTasks, value); }

        private ObservableCollection<Event> events;
        public ObservableCollection<Event> Events { get => events; set => Set(ref events, value); }

        public void AddNewTaks(ApplicationContext db)
        {
            TasksToDay = new ObservableCollection<MyTask>();
            ImportantTasks = new ObservableCollection<MyTask>();
            Events = new ObservableCollection<Event>();

            foreach (var task in db.MyTasks)
            {
                if (task.TaskDate == DateTime.Now.ToString("d"))
                    TasksToDay.Add(task);

                if (task.TaskDate == DateTime.Now.ToString("d") && task.IsImportant == true)
                    ImportantTasks.Add(task);
            }

            foreach (var enent in db.Events)
            {
                if (DateTime.Parse(DateTime.Now.ToString("d")) >= DateTime.Parse(enent.StartDate) && DateTime.Parse(DateTime.Now.ToString("d")) <= DateTime.Parse(enent.EndDate))
                {
                    Events.Add(enent);
                }
            }
        }
    }
}
