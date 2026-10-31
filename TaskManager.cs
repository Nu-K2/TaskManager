using System.Collections.Generic;

namespace TaskTracker
{
    public class TaskManager
    {
        private static readonly TaskManager _instance = new();
        public static TaskManager GetTaskManager => _instance;
        private TaskManager() { }

        private readonly List<TaskItem> tasks = new();

        public int Count => tasks.Count;

        public List<TaskItem> GetAllTasks() => tasks;

        // Перегрузка из методички (требование №2)
        public bool AddTask(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            tasks.Add(new TaskItem
            {
                Title = title,
                Description = description ?? string.Empty
            });
            return true;
        }

        public bool AddTask(TaskItem task)
        {
            if (task == null || string.IsNullOrWhiteSpace(task.Title))
                return false;
            tasks.Add(task);
            return true;
        }

        public bool RemoveTask(TaskItem task) => tasks.Remove(task);

        public void ClearAllTasks() => tasks.Clear();
    }
}