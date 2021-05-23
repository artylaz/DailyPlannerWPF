using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using DailyPlannerWPF.ViewModels.WindowsViewModel;

namespace DailyPlannerWPF.ViewModels.PagesViewModels
{
    class NotesPageViewModel : ViewModel
    {
        internal ApplicationContext db;
        internal AddNoteWindowViewModel addNoteWindowVM;

        private IEnumerable<Note> notes;
        public IEnumerable<Note> Notes
        {
            get => notes;
            set => Set(ref notes, value);
        }

        private object selectedItem;
        public object SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, value);
        }

        public ObservableCollection<Note> EditDataNotes { get; set; }

        #region Команды

        public ICommand AddNoteCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    AddNoteWindow addNoteWindow = new AddNoteWindow();
                    addNoteWindowVM = new AddNoteWindowViewModel
                    {
                        db = db,
                        addNoteWindow = addNoteWindow,
                        notesPageVM = this
                    };
                    addNoteWindow.DataContext = addNoteWindowVM;
                    addNoteWindow.ShowDialog();
                });
            }
        }

        public ICommand RemoveNoteCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {

                    if (obj is Note note)
                    {
                        db.Notes.Remove(db.Notes.Find(note.NoteId));
                        db.SaveChanges();

                        if (OldOrNew == true)
                            FirstNew();
                        else
                            FirstOld();
                    }

                });
            }
        }

        public ICommand EditNoteCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (obj is Note note)
                    {
                        AddNoteWindow addNoteWindow = new AddNoteWindow();
                        addNoteWindowVM = new AddNoteWindowViewModel()
                        {
                            db = db,
                            addNoteWindow = addNoteWindow,
                            notesPageVM = this,
                            VisibilityButtonSave = Visibility.Visible,
                            noteOb = SelectedItem,
                            Text = note.Text
                        };
                        addNoteWindow.DataContext = addNoteWindowVM;
                        addNoteWindow.ShowDialog();
                    }
                });
            }
        }

        public ICommand SortFirstNewCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    FirstNew();
                });
            }
        }

        public ICommand SortFirstOldCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    FirstOld();
                });
            }
        }

        #endregion

        public NotesPageViewModel() { }

        public NotesPageViewModel(ApplicationContext db)
        {
            this.db = db;
            Notes = db.Notes.Local.ToBindingList();

            EditDataNotes = new ObservableCollection<Note>();

            FirstNew();
        }

        #region Сортировки
        internal bool OldOrNew = true;

        /// <summary>
        /// Сортирует по убыванию
        /// </summary>
        public void FirstNew()
        {
            EditDataNotes.Clear();

            var editNotes = Notes.OrderByDescending(i => DateTime.Parse(i.DateAdded));

            foreach (var item in editNotes)
            {
                EditDataNotes.Add(new Note
                {
                    NoteId = item.NoteId,
                    Text = item.Text,
                    DateAdded = DateFilter(item.DateAdded)
                });
            }
            OldOrNew = true;
        }
        /// <summary>
        /// Сортирует по возрастанию
        /// </summary>
        public void FirstOld()
        {
            EditDataNotes.Clear();

            var editNotes = Notes.OrderBy(i => DateTime.Parse(i.DateAdded));

            foreach (var item in editNotes)
            {
                EditDataNotes.Add(new Note
                {
                    NoteId = item.NoteId,
                    Text = item.Text,
                    DateAdded = DateFilter(item.DateAdded)
                });
            }


            OldOrNew = false;
        }
        #endregion

        /// <summary>
        /// Форматирует даты в зависимости от текущей даты
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public string DateFilter(string Date)
        {
            string[] datetime = Date.Split(new char[] { ' ' });
            string[] date = datetime[0].Split(new char[] { '.' });
            string[] time = datetime[1].Split(new char[] { ':' });

            if (datetime[0] == DateTime.Now.ToString("d"))
            {
                return time[0] + ":" + time[1];
            }
            else if (date[2] == DateTime.Now.Year.ToString())
            {
                return new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0])).ToString("M");
            }
            else
            {
                return datetime[0];
            }
        }
    }
}
