using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using Ical.Net;
using DailyPlannerWPF.ViewModels.WindowsViewModel;

namespace DailyPlannerWPF.ViewModels.PagesViewModels
{
    class EventsPageViewModel : ViewModel
    {
        internal ApplicationContext db;
        internal AddEventWindowViewModel addEventWindowVM;
        internal ExportEventsWindowViewModel exportEventsWindowVM;
        internal ImportEventsWindowViewModel importEventsWindowVM;


        private string textt;
        public string Textt
        {
            get => textt;
            set
            {
                Set(ref textt, value);
                DateFilter();
            }
        }

        #region Экспорт и импорт событий
        public ICommand ExportEventsCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (AllEvents.Count() > 0)
                    {
                        ExportEventsWindow exportEventsWindow = new ExportEventsWindow();

                        exportEventsWindowVM = new ExportEventsWindowViewModel
                        {
                            exportEventsWindow = exportEventsWindow,
                            EventsPageVM = this
                        };
                        exportEventsWindow.DataContext = exportEventsWindowVM;
                        exportEventsWindow.ShowDialog();
                    }
                });
            }
        }

        public ICommand ImportEventsCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Calendar calendar = new Calendar();

                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.Filter = "Calendar (*.ics)|*.ics";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                        StreamReader reader = new StreamReader(fileInfo.Open(FileMode.Open, FileAccess.Read));

                        var stringEvents = reader.ReadToEnd();

                        calendar = Calendar.Load(stringEvents);

                        reader.Close();
                    }

                    ImportEventsWindow importEventsWindow = new ImportEventsWindow();

                    importEventsWindowVM = new ImportEventsWindowViewModel
                    {
                        db = db,
                        importEventsWindow = importEventsWindow,
                        Events = new ObservableCollection<Event>()
                    };
                    if (calendar.Events.Count > 0)
                    {
                        

                        foreach (var item in calendar.Events)
                        {
                            var calendarEvent = new Event
                            {
                                Name = item.Summary,
                                Place = item.Location,
                                StartDate = item.Start.Date.ToString("d"),
                                EndDate = item.End.Date.ToString("d"),
                                IsAllDay = item.IsAllDay,
                                Description = item.Description
                            };
                            if (item.IsAllDay == false)
                            {
                                calendarEvent.StartTime = item.Start.ToTimeZone(TimeZoneInfo.Local.Id).Hour.ToString() + ":" + item.Start.ToTimeZone(TimeZoneInfo.Local.Id).Minute.ToString();
                                calendarEvent.EndTime = item.End.ToTimeZone(TimeZoneInfo.Local.Id).Hour.ToString() + ":" + item.End.ToTimeZone(TimeZoneInfo.Local.Id).Minute.ToString();
                            }
                            else
                            {
                                calendarEvent.StartTime = "00:00";
                                calendarEvent.EndTime = "00:00";
                            }

                            if (item.Alarms.Count >= 1)
                            {
                                calendarEvent.IsNotified = true;
                                calendarEvent.ReminderDateTime = DateTime.Parse(calendarEvent.StartDate + " " + calendarEvent.StartTime + ":00").Add(item.Alarms[0].Trigger.Duration.Value).ToString("G");
                            }

                            importEventsWindowVM.Events.Add(calendarEvent);

                        }
                        importEventsWindow.DataContext = importEventsWindowVM;
                        importEventsWindow.ShowDialog();
                    }
                });
            }
        }

        #endregion

        public ObservableCollection<Event> Events { get; set; }

        private DateTime selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                Set(ref selectedDate, value);
                DateFilter();
            }
        }

        private Event selectedEvent;
        public Event SelectedEvent
        {
            get => selectedEvent;
            set
            {
                Set(ref selectedEvent, value);

                if (SelectedEvent != null)
                {
                    if (SelectedEvent.StartDate == SelectedEvent.EndDate)
                    {
                        VisibilityDate = Visibility.Visible;
                        VisibilityTime = Visibility.Visible;
                        VisibilityEndDate = Visibility.Collapsed;

                        if (SelectedEvent.IsAllDay == true)
                            VisibilityTime = Visibility.Collapsed;
                        else
                            VisibilityTime = Visibility.Visible;

                    }
                    else
                    {
                        VisibilityDate = Visibility.Collapsed;
                        VisibilityStartDate = Visibility.Visible;
                        VisibilityEndDate = Visibility.Visible;
                        VisibilityTime = Visibility.Collapsed;
                    }
                }
            }
        }

        #region Видимость элементов

        private Visibility visibilityDate;
        public Visibility VisibilityDate
        {
            get => visibilityDate;
            set => Set(ref visibilityDate, value);
        }

        private Visibility visibilityTime;
        public Visibility VisibilityTime
        {
            get => visibilityTime;
            set => Set(ref visibilityTime, value);
        }

        private Visibility visibilityStartDate = Visibility.Collapsed;
        public Visibility VisibilityStartDate
        {
            get => visibilityStartDate;
            set => Set(ref visibilityStartDate, value);
        }

        private Visibility visibilityEndDate = Visibility.Collapsed;
        public Visibility VisibilityEndDate
        {
            get => visibilityEndDate;
            set => Set(ref visibilityEndDate, value);
        }

        private Visibility visibilityPlace;
        public Visibility VisibilityPlace
        {
            get => visibilityPlace;
            set => Set(ref visibilityPlace, value);
        }

        private Visibility visibilityDescription;
        public Visibility VisibilityDescription
        {
            get => visibilityDescription;
            set => Set(ref visibilityDescription, value);
        }
        #endregion

        private IEnumerable<Event> allEvents;
        public IEnumerable<Event> AllEvents
        {
            get => allEvents;
            set => Set(ref allEvents, value);
        }

        public ICommand AddEventCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    AddEventWindow addEventWindow = new AddEventWindow();
                    addEventWindowVM = new AddEventWindowViewModel
                    {
                        db = db,
                        Events = db.Events,
                        StartDate = SelectedDate,
                        EndDate = SelectedDate,
                        addEventWindow = addEventWindow,
                        eventsPageVM = this
                    };
                    addEventWindow.DataContext = addEventWindowVM;
                    addEventWindow.ShowDialog();
                });
            }
        }

        public ICommand EditTaskCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (SelectedEvent is Event)
                    {
                        AddEventWindow addEventWindow = new AddEventWindow();
                        addEventWindowVM = new AddEventWindowViewModel
                        {
                            db = db,
                            Events = db.Events,
                            Name = SelectedEvent.Name,
                            Place = SelectedEvent.Place,
                            StartDate = DateTime.Parse(SelectedEvent.StartDate),
                            EndDate = DateTime.Parse(SelectedEvent.EndDate),
                            IsAllDay = SelectedEvent.IsAllDay,
                            StartTime = SelectedEvent.StartTime,
                            EndTime = SelectedEvent.EndTime,
                            IsNotified = SelectedEvent.IsNotified,
                            Description = SelectedEvent.Description,
                            addEventWindow = addEventWindow,
                            VisibilityButtonSave = Visibility.Visible,
                            eventsPageVM = this
                        };

                        if (SelectedEvent.ReminderDateTime != null)
                            addEventWindowVM.SelectedInTime = "За " + (DateTime.Parse(SelectedEvent.StartDate + " " + SelectedEvent.StartTime + ":00") - DateTime.Parse(SelectedEvent.ReminderDateTime)).TotalMinutes.ToString() + " мин";

                        if (!addEventWindowVM.InTime.Contains(addEventWindowVM.SelectedInTime))
                            addEventWindowVM.InTime.Add(addEventWindowVM.SelectedInTime);

                        addEventWindow.DataContext = addEventWindowVM;

                        addEventWindow.ShowDialog();
                    }
                });
            }
        }

        public ICommand RemoveEventCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (SelectedEvent is Event)
                    {
                        db.Events.Remove(SelectedEvent);
                        db.SaveChanges();
                        DateFilter();
                    }
                });
            }
        }

        public EventsPageViewModel() { }
        public EventsPageViewModel(ApplicationContext db)
        {
            this.db = db;
            AllEvents = db.Events.Local.ToBindingList();
            Events = new ObservableCollection<Event>();
            DateFilter();
        }
        public void DateFilter()
        {
            if (Events.Count > 0)
                Events.Clear();

            if (AllEvents != null)
                foreach (var item in AllEvents)
                {
                    if (DateTime.Parse(SelectedDate.ToString("d")) >= DateTime.Parse(item.StartDate) && DateTime.Parse(SelectedDate.ToString("d")) <= DateTime.Parse(item.EndDate))
                    {
                        Events.Add(item);
                    }
                }
        }

        public static DateTime DteParse(string dataString)
        {
            string[] datetime = dataString.Split(new char[] { ' ' });
            string[] date = datetime[0].Split(new char[] { '.' });

            DateTime newDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            return newDate;
        }
    }
}
