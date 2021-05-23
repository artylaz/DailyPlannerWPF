using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.ViewModels.PagesViewModels;
using DailyPlannerWPF.Views.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    class TasksDiagramWindowViewModel : ViewModel
    {
        internal TasksPageViewModel tasksPageVM;
        internal TasksDiagramWindow tasksDiagramWindow;

        public TasksDiagramWindowViewModel()
        {
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                   Title = "Затрачено чаcов",
                   Values = new ChartValues<double>()
                }
            };

            Labels = new ObservableCollection<string>();
        }

        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private IEnumerable<Storage> storages;
        public IEnumerable<Storage> Storages
        {
            get => storages;
            set => Set(ref storages, value);
        }

        public DateTime startDate = DateTime.Now;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(ref startDate, value);
                FillDiagram();
            }
        }

        public DateTime endDate = DateTime.Now.AddDays(1);
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(ref endDate, value);
                FillDiagram();
            }
        }

        public void FillDiagram()
        {
            if (StartDate != null && EndDate != null && Storages != null)
            {

                SeriesCollection[0].Values.Clear();
                Labels.Clear();

                foreach (var itemStorage in Storages)
                {
                    if (DateTime.Parse(itemStorage.MyTasks[0].TaskDate) >= DateTime.Parse(StartDate.ToString("d")) && DateTime.Parse(itemStorage.MyTasks[0].TaskDate) <= DateTime.Parse(EndDate.ToString("d")))
                    {
                        double sum = 0;
                        foreach (var itemTask in itemStorage.MyTasks)
                        {
                            sum += ParseTime(itemTask.EndTime) - ParseTime(itemTask.StartTime);
                        }

                        SeriesCollection[0].Values.Add(sum / 60);
                        Labels.Add(itemStorage.MyTasks[0].TaskDate);

                    }
                }
                Formatter = value => value.ToString("N");
            }
        }

        /// <summary>
        /// Возвращает время в минутах
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int ParseTime(string time)
        {
            string[] timeArray = time.Split(new char[] { ':' });

            return (int.Parse(timeArray[0]) * 60) + int.Parse(timeArray[1]);
        }


        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    tasksDiagramWindow.Close();
                });
            }
        }

        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    tasksDiagramWindow.DragMove();
                });
            }
        }
    }
}
