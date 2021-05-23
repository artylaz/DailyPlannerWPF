using System.Data.Entity;

namespace DailyPlannerWPF.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<MyTask> MyTasks { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Note> Notes { get; set; }

    }
}
