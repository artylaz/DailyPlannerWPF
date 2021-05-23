using DailyPlannerWPF.Models.Base;
using System;

namespace DailyPlannerWPF.Models
{
    public class MyTask : Model
    {
        public int MyTaskId { get; set; }
        public int StorageId { get; set; }
        public Storage Storage { get; set; }

        private string content;
        private string taskDate;
        private bool isDone;
        private bool isImportant;
        private bool isNotified;
        private string reminderTime;
        private string startTime;
        private string endTime;

        public string Content { get => content; set => Set(ref content, value); }
        public string TaskDate { get => taskDate; set => Set(ref taskDate, value); }
        public bool IsDone { get => isDone; set => Set(ref isDone, value); }
        public bool IsImportant { get => isImportant; set => Set(ref isImportant, value); }
        public bool IsNotified { get => isNotified; set => Set(ref isNotified, value); }
        public string ReminderTime { get => reminderTime; set => Set(ref reminderTime, value); }
        public string StartTime { get => startTime; set => Set(ref startTime, value); }
        public string EndTime { get => endTime; set => Set(ref endTime, value); }

    }
}
