using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.Views.Pages;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows.Threading;
using DailyPlannerWPF.ViewModels.PagesViewModels;
using DailyPlannerWPF.Views.UserControls;
using DailyPlannerWPF.ViewModels.UserControlsViewModels;
using System.Windows.Controls.Primitives;
using DailyPlannerWPF.Views.Windows;
using System.Windows.Media;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    internal class MainWindowViewModel : ViewModel
    {
        private ApplicationContext db;

        private readonly Page homePage;
        private readonly Page tasksPage;
        private readonly Page eventsPage;
        private readonly Page notesPage;

        private readonly HomePageViewModel homePageVM;
        private readonly TasksPageViewModel tasksPageVM;
        private readonly EventsPageViewModel eventsPageVM;
        private readonly NotesPageViewModel notesPageVM;


        #region Переходы между страницами

        #region Cursor
        private string sursormargin = "0,188,140.4,355.6"; // +60.
        public string CursorMargin
        {
            get => sursormargin;
            set => Set(ref sursormargin, value);
        }

        private string backgroundHome = "#FF2D2D30";
        public string BackgroundHome
        {
            get => backgroundHome;
            set => Set(ref backgroundHome, value);
        }
        private string backgroundTasks = "#FF252526";
        public string BackgroundTasks
        {
            get => backgroundTasks;
            set => Set(ref backgroundTasks, value);
        }
        private string backgroundNotes = "#FF252526";
        public string BackgroundNotes
        {
            get => backgroundNotes;
            set => Set(ref backgroundNotes, value);
        }
        private string backgroundEvents = "#FF252526";
        public string BackgroundEvents
        {
            get => backgroundEvents;
            set => Set(ref backgroundEvents, value);
        }

        #endregion



        private Page currentPage;
        public Page CurrentPage
        {
            get => currentPage;
            set => Set(ref currentPage, value);
        }

        public ICommand OpenHomeCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    CursorMargin = "0,188,140.4,355.6";

                    BackgroundHome = "#FF2D2D30";
                    BackgroundTasks = "#FF252526";
                    BackgroundEvents = "#FF252526";
                    BackgroundNotes = "#FF252526";

                    db.SaveChanges();

                    homePageVM.AddNewTaks(db);

                    CurrentPage = homePage;
                });
            }
        }

        public ICommand OpenTasksCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    CursorMargin = "0,248,140.4,355.6";

                    BackgroundHome = "#FF252526";
                    BackgroundTasks = "#FF2D2D30";
                    BackgroundEvents = "#FF252526";
                    BackgroundNotes = "#FF252526";

                    db.SaveChanges();
                    CurrentPage = tasksPage;
                });
            }
        }

        public ICommand OpenEvensCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    CursorMargin = "0,308,140.4,355.6";

                    BackgroundHome = "#FF252526";
                    BackgroundTasks = "#FF252526";
                    BackgroundEvents = "#FF2D2D30";
                    BackgroundNotes = "#FF252526";

                    db.SaveChanges();
                    CurrentPage = eventsPage;
                });
            }
        }

        public ICommand OpenNotesCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    CursorMargin = "0,368,140.4,355.6";

                    BackgroundHome = "#FF252526";
                    BackgroundTasks = "#FF252526";
                    BackgroundEvents = "FF252526";
                    BackgroundNotes = "#FF2D2D30";

                    CurrentPage = notesPage;
                });
            }
        }

        #endregion

        #region Exit


        public ICommand CloseMainWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    db.SaveChanges();
                    Application.Current.MainWindow.Close();
                }, (obj) => Application.Current.MainWindow != null);
            }
        }

        #endregion



        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Application.Current.MainWindow.DragMove();
                });
            }
        }

        public ICommand OpenSettingsWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SettingsWindow settingsWindow = new SettingsWindow();
                    SettingsWindowViewModel settingsWindowVM = new SettingsWindowViewModel()
                    {
                        settingsWindow = settingsWindow
                    };
                    settingsWindow.DataContext = settingsWindowVM;
                    settingsWindow.ShowDialog();
                });
            }
        }

        public MainWindowViewModel()
        {
            db = new ApplicationContext();
            db.Storages.Load();
            db.MyTasks.Load();
            db.Events.Load();
            db.Notes.Load();

            homePage = new HomePage();
            tasksPage = new TasksPage();
            eventsPage = new EventsPage();
            notesPage = new NotesPage();

            homePageVM = new HomePageViewModel();
            homePageVM.AddNewTaks(db);
            homePage.DataContext = homePageVM;

            tasksPageVM = new TasksPageViewModel(db);
            tasksPage.DataContext = tasksPageVM;

            eventsPageVM = new EventsPageViewModel(db);
            eventsPage.DataContext = eventsPageVM;

            notesPageVM = new NotesPageViewModel(db);
            notesPage.DataContext = notesPageVM;


            DispatcherTimer dispatcherTimer = new DispatcherTimer();

            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
            dispatcherTimer.Tick += (s, e) => Remind();

            CurrentPage = homePage;

        }

        public void Remind()
        {
            foreach (var item in db.MyTasks)
            {
                if (DateTime.Now >= DateTime.Parse(item.TaskDate + " " + item.ReminderTime + ":00") && item.IsNotified == true)
                {

                    NotificationUserControl notificationUC = new NotificationUserControl();
                    NotificationUserControlViewModel notificationUserControlVM = new NotificationUserControlViewModel
                    {
                        notificationUC = notificationUC,
                        Title = "Напоминаем о задаче",
                        Image = "/Icons/Task.png",
                        Name = item.Content
                    };
                    notificationUC.DataContext = notificationUserControlVM;

                    App.NotifyIcon.ShowCustomBalloon(notificationUC, PopupAnimation.Slide, 10000);

                    var player = new MediaPlayer();
                    player.Open(new Uri("Resources/NotificationSignal/Notification.mp3", UriKind.RelativeOrAbsolute));
                    player.Play();

                    item.IsNotified = false;
                }
            }

            foreach (var item in db.Events)
            {
                if (item.ReminderDateTime != null)
                    if (DateTime.Now >= DateTime.Parse(item.ReminderDateTime) && item.IsNotified == true)
                    {

                        NotificationUserControl notificationUC = new NotificationUserControl();
                        NotificationUserControlViewModel notificationUserControlVM = new NotificationUserControlViewModel
                        {
                            notificationUC = notificationUC,
                            Title = "Напоминаем о событии",
                            Image = "/Icons/Event.png",
                            Name = item.Name
                        };
                        notificationUC.DataContext = notificationUserControlVM;

                        App.NotifyIcon.ShowCustomBalloon(notificationUC, PopupAnimation.Slide, 10000);

                        var player = new MediaPlayer();
                        player.Open(new Uri("Resources/NotificationSignal/Notification.mp3", UriKind.RelativeOrAbsolute));
                        player.Play();

                        item.IsNotified = false;

                    }
            }
            db.SaveChanges();
        }
    }
}
