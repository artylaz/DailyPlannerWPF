using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.Models;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.ViewModels.PagesViewModels;
using DailyPlannerWPF.Views.Windows;
using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    class AddNoteWindowViewModel : ViewModel
    {
        internal ApplicationContext db;
        internal AddNoteWindow addNoteWindow;
        internal NotesPageViewModel notesPageVM;

        private string text;
        public string Text { get => text; set => Set(ref text, value); }

        private Visibility visibilityButtonSave = Visibility.Collapsed;
        public Visibility VisibilityButtonSave { get => visibilityButtonSave; set => Set(ref visibilityButtonSave, value); }

        #region Команды
        
        public ICommand AddNoteCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (Text == null || Text.StartsWith(" "))
                        return;

                    db.Notes.Add(new Note
                    {
                        Text = Text,
                        DateAdded = DateTime.Now.ToString("G")
                    });

                    db.SaveChanges();

                    if (notesPageVM.OldOrNew == true)
                        notesPageVM.FirstNew();
                    else
                        notesPageVM.FirstOld();

                    addNoteWindow.Close();
                });
            }
        }

        public object noteOb;
        public ICommand SaveNoteCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (Text == null || Text.StartsWith(" "))
                        return;

                    if (noteOb is Note note)
                    {
                        var newNote = db.Notes.Find(note.NoteId);
                        newNote.Text = Text;
                        db.Entry(newNote).State = EntityState.Modified;
                        db.SaveChanges();

                        if (notesPageVM.OldOrNew == true)
                            notesPageVM.FirstNew();
                        else
                            notesPageVM.FirstOld();

                        addNoteWindow.Close();
                    }
                });
            }
        }

        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    addNoteWindow.DragMove();
                });
            }
        }
        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    addNoteWindow.Close();
                });
            }
        }

        #endregion
    }
}
