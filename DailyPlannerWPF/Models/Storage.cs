using DailyPlannerWPF.Models.Base;
using System.Collections.ObjectModel;

namespace DailyPlannerWPF.Models
{
    public class Storage : Model
    {
        public int StorageId { get; set; }

        private string name;

        public string Name { get => name; set => Set(ref name, value); }

        public ObservableCollection<MyTask> MyTasks { get; set; }

        public Storage()
        {
            MyTasks = new ObservableCollection<MyTask>();
        }
    }
}
