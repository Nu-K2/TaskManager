using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskTracker
{
    public class TaskItem : INotifyPropertyChanged
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private DateTime _createdAt = DateTime.Now;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        private TaskPriority _priority = TaskPriority.Undefined;
        public TaskPriority Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(); }
        }

        private DateTime _deadline = DateTime.Now;
        public DateTime Deadline
        {
            get => _deadline;
            set { _deadline = value; OnPropertyChanged(); }
        }

        private TaskCategory _category = TaskCategory.NotStarted;
        public TaskCategory Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        // Удобное read-only свойство для колонки-чекбокса
        public bool IsCompleted => Category == TaskCategory.Completed;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public TaskItem(string title, string description, TaskPriority priority, DateTime deadline, TaskCategory category)
        {
            Title = title;
            Description = description;
            Priority = priority;
            Deadline = deadline;
            Category = category;
        }

        public TaskItem() { }
    }

    public enum TaskPriority
    {
        High,
        Medium,
        Low,
        Undefined
    }

    public enum TaskCategory
    {
        NotStarted,
        InProgress,
        Completed
    }
}