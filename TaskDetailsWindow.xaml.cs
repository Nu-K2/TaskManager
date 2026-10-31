using System.Windows;

namespace TaskTracker
{
    public partial class TaskDetailsWindow : Window
    {
        private readonly TaskItem _task;

        public TaskDetailsWindow(TaskItem task)
        {
            InitializeComponent();
            _task = task;
            LoadData();
        }

        private void LoadData()
        {
            TitleTextBlock.Text = _task.Title;

            IdTextBlock.Text = _task.Id;
            PriorityTextBlock.Text = _task.Priority.ToString();
            CategoryTextBlock.Text = _task.Category.ToString();
            CreatedDateTextBlock.Text = _task.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            DueDateTextBlock.Text = _task.Deadline.ToString("dd.MM.yyyy");
            DescriptionTextBlock.Text = string.IsNullOrWhiteSpace(_task.Description)
                ? "Описание отсутствует"
                : _task.Description;

            UpdateStatusUi();
        }

        private void UpdateStatusUi()
        {
            bool done = _task.Category == TaskCategory.Completed;

            StatusTextBlock.Text = done ? "Статус: Выполнена" : "Статус: Активна";
            ToggleStatusButton.Content = done
                ? "↺ Вернуть в работу"
                : "✓ Отметить выполненной";
        }

        private void ToggleStatusButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключаем категорию; DataGrid в главном окне обновится
            // благодаря INotifyPropertyChanged в TaskItem
            _task.Category = _task.Category == TaskCategory.Completed
                ? TaskCategory.InProgress
                : TaskCategory.Completed;

            UpdateStatusUi();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}