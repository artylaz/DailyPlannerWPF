using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.ViewModels.WindowsViewModel;
using DailyPlannerWPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.PagesViewModels
{
    internal class TasksPageViewModel : ViewModel
    {
        private ApplicationContext db;
        internal AddTaskWindowViewModel addTaskWindowVM;
        internal TasksDiagramWindowViewModel tasksDiagramWindowVM;

        public TasksPageViewModel() { }

        public TasksPageViewModel(ApplicationContext db)
        {
            this.db = db;
            Storages = db.Storages.Local.ToBindingList();

            EditStorages = new ObservableCollection<Storage>();

            Refresh();
        }

        private bool isCheckedTask = true;
        public bool IsCheckedTask { get => isCheckedTask; set => Set(ref isCheckedTask, value); }

        private Storage selectedStorage;
        public Storage SelectedStorage
        {
            get => selectedStorage;
            set
            {
                Set(ref selectedStorage, value);

                if (SelectedStorage != null)
                    SelectedTask = null;
            }
        }

        private MyTask selectedTask;
        public MyTask SelectedTask
        {
            get => selectedTask;
            set
            {
                Set(ref selectedTask, value);

                if (SelectedTask != null)
                    SelectedStorage = null;
            }
        }

        private IEnumerable<Storage> storages;
        public IEnumerable<Storage> Storages
        {
            get => storages;
            set => Set(ref storages, value);
        }

        public ObservableCollection<Storage> EditStorages { get; set; }

        #region Команды

        public ICommand AddTaskCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    AddTaskWindow addTaskWindow = new AddTaskWindow();
                    addTaskWindowVM = new AddTaskWindowViewModel
                    {
                        db = db,
                        MyTasks = db.MyTasks,
                        addTaskWindow = addTaskWindow,
                        tasksPageVM = this
                    };
                    addTaskWindow.DataContext = addTaskWindowVM;
                    addTaskWindow.ShowDialog();
                });
            }
        }

        public ICommand RemoveTaskCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if(SelectedStorage != null)
                    {
                        foreach (var item in db.MyTasks)
                        {
                            if (item.StorageId == SelectedStorage.StorageId)
                                db.MyTasks.Remove(item);
                        }
                        db.Storages.Remove(db.Storages.Find(SelectedStorage.StorageId));
                        db.SaveChanges();
                        Refresh();
                    }

                    if (obj is MyTask myTask)
                    {
                        db.MyTasks.Remove(myTask);

                        if (db.Storages.Find(myTask.StorageId).MyTasks.Count == 0)
                            db.Storages.Remove(db.Storages.Find(myTask.StorageId));

                        db.SaveChanges();
                        Refresh();
                    }

                });
            }
        }

        public ICommand EditTaskCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (obj is MyTask task)
                    {
                        AddTaskWindow addTaskWindow = new AddTaskWindow();
                        addTaskWindowVM = new AddTaskWindowViewModel()
                        {
                            db = db,
                            addTaskWindow = addTaskWindow,
                            VisibilityButtonSave = Visibility.Visible,
                            tasksPageVM = this,

                            task = task,
                            Text = task.Content,
                            IsImportant = task.IsImportant,
                            IsNotified = task.IsNotified,
                            ReminderTime = task.ReminderTime,
                            StartTime = task.StartTime,
                            EndTime = task.EndTime,
                            TaskDate = DateTime.Parse(task.TaskDate)

                        };
                        addTaskWindow.DataContext = addTaskWindowVM;
                        addTaskWindow.ShowDialog();
                    }

                });
            }
        }

        public ICommand OpenDiagramCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    TasksDiagramWindow tasksDiagramWindow = new TasksDiagramWindow();
                    tasksDiagramWindowVM = new TasksDiagramWindowViewModel()
                    {
                        tasksDiagramWindow = tasksDiagramWindow,
                        Storages = Storages,
                        tasksPageVM = this
                    };
                    tasksDiagramWindowVM.FillDiagram();
                    tasksDiagramWindow.DataContext = tasksDiagramWindowVM;
                    tasksDiagramWindow.ShowDialog();
                    ;
                });
            }
        }

        #endregion

        public void Refresh()
        {
            EditStorages.Clear();


            foreach (var item in Storages)
            {
                EditStorages.Insert(0, new Storage
                {
                    StorageId = item.StorageId,
                    MyTasks = item.MyTasks,
                    Name = item.Name
                });
            }

        }

    }
}
