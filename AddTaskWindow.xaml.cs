using System;
using System.Windows;

namespace TaskTracker
{
    public partial class AddTaskWindow : Window
    {
        public TaskItem? Result { get; private set; }
        private readonly TaskItem _task;
        private readonly bool _isEditMode;

        // Режим добавления
        public AddTaskWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            _task = new TaskItem
            {
                Title = string.Empty,
                Description = string.Empty,
                Priority = TaskPriority.Medium,
                Deadline = DateTime.Now.AddDays(7),
                Category = TaskCategory.NotStarted
            };
            this.DataContext = _task;
        }

        // Режим редактирования
        public AddTaskWindow(TaskItem task)
        {
            InitializeComponent();
            _isEditMode = true;
            _task = task;
            this.DataContext = _task;

            this.Title = "Редактирование задачи";
            HeaderText.Text = "Редактирование задачи";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_task.Title))
            {
                MessageBox.Show("Название задачи не может быть пустым.");
                return;
            }

            if (!_isEditMode)
            {
                // В режиме добавления _task — это «черновик».
                // Возвращаем его наружу как Result.
                Result = _task;
            }

            this.DialogResult = true;
        }
    }
}