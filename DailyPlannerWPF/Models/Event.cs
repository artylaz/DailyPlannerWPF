using DailyPlannerWPF.Models.Base;

namespace DailyPlannerWPF.Models
{
    public class Event : Model
    {
        public int EventId { get; set; }

        private string name;
        private string startDate;
        private string endDate;
        private string place;
        private bool isAllDay;
        private bool isNotified;
        private string reminderDateTime;
        private string startTime;
        private string endTime;
        private string description;

        public string Name { get => name; set => Set(ref name, value); }
        public string StartDate { get => startDate; set => Set(ref startDate, value); }
        public string EndDate { get => endDate; set => Set(ref endDate, value); }
        public string Place { get => place; set => Set(ref place, value); }
        public bool IsAllDay { get => isAllDay; set => Set(ref isAllDay, value); }
        public bool IsNotified { get => isNotified; set => Set(ref isNotified, value); }
        public string ReminderDateTime { get => reminderDateTime; set => Set(ref reminderDateTime, value); }
        public string StartTime { get => startTime; set => Set(ref startTime, value); }
        public string EndTime { get => endTime; set => Set(ref endTime, value); }
        public string Description { get => description; set => Set(ref description, value); }
    }
}
