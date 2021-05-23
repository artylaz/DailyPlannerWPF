using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.ViewModels.PagesViewModels;
using DailyPlannerWPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    class AddEventWindowViewModel : ViewModel
    {
        internal ApplicationContext db;
        internal AddEventWindow addEventWindow;
        internal EventsPageViewModel eventsPageVM;

        public AddEventWindowViewModel()
        {
            InTime = new List<string>()
            {
                "За 0 мин",
                "За 5 мин",
                "За 10 мин",
                "За 15 мин",
                "За 30 мин"
            };
        }

        private IEnumerable<Event> events;
        public IEnumerable<Event> Events { get => events; set => Set(ref events, value); }

        public List<string> InTime { get; set; }

        #region Свойства

        private string name;
        public string Name { get => name; set => Set(ref name, value); }

        private string place;
        public string Place { get => place; set => Set(ref place, value); }

        private DateTime startDate;
        public DateTime StartDate { get => startDate; set => Set(ref startDate, value); }

        private DateTime endDate;
        public DateTime EndDate { get => endDate; set => Set(ref endDate, value); }

        private string startTime = "00:00";
        public string StartTime { get => startTime; set => Set(ref startTime, value); }

        private string endTime = "00:00";
        public string EndTime { get => endTime; set => Set(ref endTime, value); }

        private bool isAllDay;
        public bool IsAllDay
        {
            get => isAllDay;
            set
            {
                Set(ref isAllDay, value);

                if (IsAllDay == true)
                {
                    IsEnabledStartTime = false;
                    IsEnabledEndTime = false;
                    VisibilityIsNotified = Visibility.Collapsed;
                }
                else
                {
                    IsEnabledStartTime = true;
                    IsEnabledEndTime = true;
                    VisibilityIsNotified = Visibility.Visible;
                }
            }
        }

        private bool isNotified;
        public bool IsNotified
        {
            get => isNotified;
            set
            {
                Set(ref isNotified, value);

                if (IsNotified == true)
                    VisibilityInTime = Visibility.Visible;
                else
                    VisibilityInTime = Visibility.Collapsed;
            }
        }

        private string selectedInTime = "За 5 мин";
        public string SelectedInTime { get => selectedInTime; set => Set(ref selectedInTime, value); }

        private string reminderDateTime;
        public string ReminderDateTime
        {
            get
            {
                if (reminderDateTime != null)
                    return reminderDateTime;

                string[] time = SelectedInTime.Split(new char[] { ' ' });

                return DateTime.Parse(StartDate.ToString("d") + " " + StartTime + ":00").AddMinutes(-double.Parse(time[1])).ToString("G");
            }
            set
            {
                Set(ref reminderDateTime, value);
            }
        }

        private string description;
        public string Description { get => description; set => Set(ref description, value); }

        private bool isEnabledStartTime = true;
        public bool IsEnabledStartTime { get => isEnabledStartTime; set => Set(ref isEnabledStartTime, value); }

        private bool isEnabledEndTime = true;
        public bool IsEnabledEndTime { get => isEnabledEndTime; set => Set(ref isEnabledEndTime, value); }


        private Visibility visibilityButtonSave = Visibility.Collapsed;
        public Visibility VisibilityButtonSave { get => visibilityButtonSave; set => Set(ref visibilityButtonSave, value); }

        private Visibility visibilityInTime = Visibility.Collapsed;
        public Visibility VisibilityInTime { get => visibilityInTime; set => Set(ref visibilityInTime, value); }

        private Visibility visibilityIsNotified = Visibility.Visible;
        public Visibility VisibilityIsNotified { get => visibilityIsNotified; set => Set(ref visibilityIsNotified, value); }


        #endregion

        #region Команды
        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    addEventWindow.Close();
                });
            }
        }

        public ICommand AddEventCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (Name == null || Name.StartsWith(" ") || !AddTaskWindowViewModel.TimeCheck(StartTime) || !AddTaskWindowViewModel.TimeCheck(EndTime))
                        return;

                    if (StartTime != null && EndTime != null)
                        if (DateTime.Parse(StartDate.ToString("d") + " " + StartTime + ":00") > DateTime.Parse(EndDate.ToString("d") + " " + EndTime + ":00"))
                            return;

                    db.Events.Add(new Event
                    {
                        Name = Name,
                        Place = Place,
                        StartDate = StartDate.ToString("d"),
                        EndDate = EndDate.ToString("d"),
                        IsAllDay = IsAllDay,
                        IsNotified = IsNotified,
                        StartTime = StartTime,
                        EndTime = EndTime,
                        ReminderDateTime = ReminderDateTime,
                        Description = Description

                    });

                    db.SaveChanges();
                    eventsPageVM.DateFilter();
                    addEventWindow.Close();
                });
            }
        }

        public ICommand SaveEventCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (Name == null || !AddTaskWindowViewModel.TimeCheck(StartTime) || !AddTaskWindowViewModel.TimeCheck(EndTime))
                        return;

                    if (StartTime != null && EndTime != null)
                        if (DateTime.Parse(StartDate.ToString("d") + " " + StartTime + ":00") > DateTime.Parse(EndDate.ToString("d") + " " + EndTime + ":00"))
                            return;

                    var newEvent = db.Events.Find(eventsPageVM.SelectedEvent.EventId);
                    if (newEvent != null)
                    {
                        newEvent.Name = Name;
                        newEvent.Place = Place;
                        newEvent.StartDate = StartDate.ToString("d");
                        newEvent.EndDate = EndDate.ToString("d");
                        newEvent.IsAllDay = IsAllDay;
                        newEvent.IsNotified = IsNotified;
                        newEvent.StartTime = StartTime;
                        newEvent.EndTime = EndTime;
                        newEvent.ReminderDateTime = ReminderDateTime;
                        newEvent.Description = Description;
                        db.Entry(newEvent).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    eventsPageVM.DateFilter();
                    VisibilityButtonSave = Visibility.Collapsed;
                    addEventWindow.Close();
                });
            }
        }

        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    addEventWindow.DragMove();
                });
            }
        }

        #endregion



    }
}
