using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TaskTracker
{
    internal class TaskManager
    {
        private List<TaskItem> tasks = new();
        public int Count => tasks.Count;// — возвращает количество задач.

        internal void AddTask(string title, string description, TaskPriority priority, DateTime deadline, TaskCategory category) { 
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Заголовок задачи не может быть пустым.");
                return;
            }
            if (string.IsNullOrWhiteSpace(description)) {
                MessageBox.Show("Описание задачи не может быть пустым.");
                return;
            }
            tasks.Add(new TaskItem(title, description, priority, deadline, category));
            MessageBox.Show("Задача успешно добавлена.");
        }//— добавляет задачу в список.
        internal List<TaskItem> GetAllTasks() { 
            return tasks; 
        }// — возвращает список всех задач.


        internal void ClearAllTasks() {
            tasks.Clear();
        }// — очищает список задач.
            
    }
}