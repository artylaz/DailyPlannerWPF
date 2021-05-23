using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.ViewModels.PagesViewModels;
using DailyPlannerWPF.Views.Windows;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    class ExportEventsWindowViewModel : ViewModel
    {
        internal ExportEventsWindow exportEventsWindow;
        internal EventsPageViewModel EventsPageVM;

        public DateTime startDate = DateTime.Now;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(ref startDate, value);
            }
        }

        public DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(ref endDate, value);
            }
        }

        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    exportEventsWindow.DragMove();
                });
            }
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    exportEventsWindow.Close();
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var calendar = new Calendar();

                    foreach (var item in EventsPageVM.AllEvents)
                    {
                        if (DateTime.Parse(item.StartDate) >= DateTime.Parse(StartDate.ToString("d")) && DateTime.Parse(item.EndDate) <= DateTime.Parse(EndDate.ToString("d")))
                        {
                            if (item.IsAllDay == false)
                                calendar.Events.Add(new CalendarEvent
                                {
                                    Start = new CalDateTime(DateTime.Parse(item.StartDate + " " + item.StartTime + ":00")),
                                    End = new CalDateTime(DateTime.Parse(item.EndDate + " " + item.EndTime + ":00")),
                                    IsAllDay = item.IsAllDay,
                                    Location = item.Name,
                                    Summary = item.Place,
                                    Description = item.Description
                                });
                            else
                                calendar.Events.Add(new CalendarEvent
                                {
                                    Start = new CalDateTime(DateTime.Parse(item.StartDate)),
                                    End = new CalDateTime(DateTime.Parse(item.EndDate)),
                                    IsAllDay = item.IsAllDay,
                                    Location = item.Name,
                                    Summary = item.Place,
                                    Description = item.Description
                                });
                        }
                    }
                    var serializer = new CalendarSerializer();
                    var serializedCalendar = serializer.SerializeToString(calendar);

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Filter = "Calendar (*.ics)|*.ics";

                    if (saveFileDialog1.ShowDialog() == true)
                    {
                        using (StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile()))
                        {
                            sw.Write(serializedCalendar);
                            sw.Close();
                        }
                    }

                    exportEventsWindow.Close();

                });
            }
        }

    }
}
