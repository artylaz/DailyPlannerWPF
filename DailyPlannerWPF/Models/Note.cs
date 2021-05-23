using DailyPlannerWPF.Models.Base;

namespace DailyPlannerWPF.Models
{
    public class Note : Model
    {
        public int NoteId { get; set; }

        private string text;
        private string dateAdded;

        public string Text { get => text; set => Set(ref text, value); }
        public string DateAdded { get => dateAdded; set => Set(ref dateAdded, value); }


    }
}
