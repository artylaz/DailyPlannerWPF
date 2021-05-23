using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.ViewModels.PagesViewModels;
using DailyPlannerWPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    class AddTaskWindowViewModel : ViewModel
    {
        internal ApplicationContext db;
        internal AddTaskWindow addTaskWindow;
        internal TasksPageViewModel tasksPageVM;



        private IEnumerable<MyTask> myTasks;
        public IEnumerable<MyTask> MyTasks
        {
            get => myTasks;
            set => Set(ref myTasks, value);
        }

        private string text;
        public string Text { get => text; set => Set(ref text, value); }

        private bool isImportant;
        public bool IsImportant { get => isImportant; set => Set(ref isImportant, value); }

        private bool isNotified;
        public bool IsNotified
        {
            get => isNotified;
            set
            {
                Set(ref isNotified, value);

                if (IsNotified == true)
                    VisibilityTime = Visibility.Visible;
                else
                    VisibilityTime = Visibility.Hidden;
            }
        }

        private string reminderTime = "12:00";
        public string ReminderTime { get => reminderTime; set => Set(ref reminderTime, value); }

        private string startTime = "12:00";
        public string StartTime { get => startTime; set => Set(ref startTime, value); }

        private string endTime = "13:00";
        public string EndTime { get => endTime; set => Set(ref endTime, value); }

        private DateTime taskDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        public DateTime TaskDate { get => taskDate; set => Set(ref taskDate, value); }


        
        private Visibility visibilityButtonSave = Visibility.Collapsed;
        public Visibility VisibilityButtonSave { get => visibilityButtonSave; set => Set(ref visibilityButtonSave, value); }

        private Visibility visibilityTime = Visibility.Hidden;
        public Visibility VisibilityTime { get => visibilityTime; set => Set(ref visibilityTime, value); }

        #region Команды
        public ICommand AddTaskCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {

                    if (Text == null || Text.StartsWith(" ") || !TimeCheck(StartTime) || !TimeCheck(EndTime) || !TimeCheck(ReminderTime))
                        return;

                    if (DateTime.Parse(TaskDate.ToString("d") + " " + StartTime + ":00") > DateTime.Parse(TaskDate.ToString("d") + " " + EndTime + ":00"))
                        return;

                    bool result = false;

                    foreach (var stor in db.Storages)
                    {
                        if (stor.Name == $"Задачи на {TaskDate:D} ")
                        {
                            db.MyTasks.Add(new MyTask
                            {
                                Content = Text,
                                IsImportant = IsImportant,
                                IsNotified = IsNotified,
                                TaskDate = TaskDate.ToString("d"),
                                ReminderTime = ReminderTime,
                                StartTime = StartTime,
                                EndTime = EndTime,
                                StorageId = stor.StorageId
                            });
                            result = true;
                        }
                    }

                    if (!result)
                    {
                        Storage storage;

                        db.Storages.Add(storage = new Storage
                        {
                            Name = $"Задачи на {TaskDate:D} "
                        });

                        db.MyTasks.Add(new MyTask
                        {
                            Content = Text,
                            IsImportant = IsImportant,
                            IsNotified = IsNotified,
                            TaskDate = TaskDate.ToString("d"),
                            ReminderTime = ReminderTime,
                            StartTime = StartTime,
                            EndTime = EndTime,
                            StorageId = storage.StorageId
                        });
                    }

                    db.SaveChanges();
                    tasksPageVM.Refresh();
                    addTaskWindow.Close();
                });
            }
        }

        public MyTask task;
        public ICommand SaveTaskCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (Text == null || Text.StartsWith(" ") || !TimeCheck(StartTime) || !TimeCheck(EndTime) || !TimeCheck(ReminderTime))
                        return;

                    if (DateTime.Parse(TaskDate.ToString("d") + " " + StartTime + ":00") > DateTime.Parse(TaskDate.ToString("d") + " " + EndTime + ":00"))
                        return;

                    bool result = false;

                    foreach (var stor in db.Storages)
                    {
                        if (stor.Name == $"Задачи на {TaskDate:D} ")
                        {
                            var newTask = db.MyTasks.Find(task.MyTaskId);
                            if (newTask != null)
                            {
                                newTask.Content = Text;
                                newTask.IsImportant = IsImportant;
                                newTask.IsNotified = IsNotified;
                                newTask.ReminderTime = ReminderTime;
                                newTask.StartTime = StartTime;
                                newTask.EndTime = EndTime;
                                newTask.TaskDate = TaskDate.ToString("d");
                                newTask.StorageId = stor.StorageId;
                                db.Entry(newTask).State = EntityState.Modified;
                            }
                            result = true;
                        }
                    }

                    if (!result)
                    {
                        Storage storage;

                        db.Storages.Add(storage = new Storage
                        {
                            Name = $"Задачи на {TaskDate:D} "
                        });

                        var newTask = db.MyTasks.Find(task.MyTaskId);
                        if (newTask != null)
                        {
                            newTask.Content = Text;
                            newTask.IsImportant = IsImportant;
                            newTask.IsNotified = IsNotified;
                            newTask.ReminderTime = ReminderTime;
                            newTask.StartTime = StartTime;
                            newTask.EndTime = EndTime;
                            newTask.TaskDate = TaskDate.ToString("d");
                            newTask.StorageId = storage.StorageId;
                            db.Entry(newTask).State = EntityState.Modified;
                        }
                    }

                    foreach (var item in db.Storages)
                    {
                        if (item.MyTasks.Count == 0)
                            db.Storages.Remove(item);
                    }

                    db.SaveChanges();

                    tasksPageVM.Refresh();

                    addTaskWindow.Close();
                });
            }
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    addTaskWindow.Close();
                });
            }
        }

        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    addTaskWindow.DragMove();
                });
            }
        }
        #endregion

        /// <summary>
        /// Возвращает true, если время корректно
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool TimeCheck(string time)
        {
            if (time == null)
                return true;

            string[] timeArray = time.Split(new char[] { ':' });

            if (Convert.ToInt32(timeArray[0]) > 23 || Convert.ToInt32(timeArray[1]) > 59)
                return false;
            else
                return true;
        }
    }
}
