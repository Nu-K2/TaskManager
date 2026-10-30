using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TaskTracker
{
    internal class TaskItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }
        public string Description { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public TaskPriority Priority { get; set; } = TaskPriority.Undefined;
        public DateTime Deadline { get; set; } = DateTime.Now;
        public TaskCategory Category { get; set; } = TaskCategory.NotStarted;

        internal TaskItem(string title, string description, TaskPriority priority, DateTime deadline, TaskCategory category)
        {
            Title = title;
            Description = description;
            Priority = priority;
            Deadline = deadline;
            Category = category;
        }
    }

    enum TaskPriority { 
        High,
        Medium,
        Low,
        Undefined
    }

    enum TaskCategory{
        NotStarted,
        InProgress,
        Completed
    }
}
