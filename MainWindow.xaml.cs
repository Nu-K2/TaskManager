using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TaskTracker
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private TaskItem? _selectedTask;
        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TaskItem> Tasks { get; set; }

        // Отдельная копия для фильтрации — то, что реально видно в DataGrid
        public ObservableCollection<TaskItem> VisibleTasks { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            List<TaskItem> tasksFromBd = TaskManager.GetTaskManager.GetAllTasks();
            Tasks = new ObservableCollection<TaskItem>(tasksFromBd);
            VisibleTasks = new ObservableCollection<TaskItem>(Tasks);

            this.DataContext = this;
            TasksDataGrid.ItemsSource = VisibleTasks;

            Tasks.CollectionChanged += (_, __) => { ApplyFilter(); UpdateStatus(); };
        }

        // ---------- Статус ----------
        private void UpdateStatus(string? message = null)
        {
            StatusText.Text = message ?? "Готово";
            TaskCountText.Text = $"Задач: {TaskManager.GetTaskManager.Count}";
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateStatus(message);
        }

        // ---------- Фильтр ----------
        private void ApplyFilter()
        {
            if (VisibleTasks == null || FilterComboBox == null) return;

            string mode = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Все задачи";
            VisibleTasks.Clear();

            foreach (var t in Tasks)
            {
                bool include = mode switch
                {
                    "Активные" => t.Category != TaskCategory.Completed,
                    "Выполненные" => t.Category == TaskCategory.Completed,
                    _ => true
                };
                if (include) VisibleTasks.Add(t);
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilter();

        // ---------- Добавить ----------
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow window = new() { Owner = this };
            if (window.ShowDialog() == true && window.Result != null)
            {
                Tasks.Add(window.Result);
                TaskManager.GetTaskManager.AddTask(window.Result);
                ShowInfo($"Задача добавлена. Всего задач: {TaskManager.GetTaskManager.Count}");
            }
        }

        // ---------- Редактировать ----------
        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Выберите задачу для редактирования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AddTaskWindow editWindow = new(SelectedTask) { Owner = this };
            if (editWindow.ShowDialog() == true)
            {
                UpdateStatus("Задача отредактирована");
            }
        }

        // ---------- Удалить ----------
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Выберите задачу для удаления.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить '{SelectedTask.Title}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                TaskManager.GetTaskManager.RemoveTask(SelectedTask);
                Tasks.Remove(SelectedTask);
                SelectedTask = null;
                ShowInfo($"Задача удалена. Всего задач: {TaskManager.GetTaskManager.Count}");
            }
        }

        // ---------- Подробнее ----------
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
            => OpenDetails();

        private void TasksDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => OpenDetails();

        private void OpenDetails()
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Выберите задачу для просмотра деталей.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TaskDetailsWindow details = new(SelectedTask) { Owner = this };
            details.ShowDialog();

            // Категория могла измениться в окне деталей — обновим фильтр
            ApplyFilter();
        }

        // ---------- Показать все ----------
        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            FilterComboBox.SelectedIndex = 0;   // «Все задачи»
            ApplyFilter();
            ShowInfo($"Показаны все задачи. Всего задач: {TaskManager.GetTaskManager.Count}");
        }

        // ---------- Очистить ----------
        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (Tasks.Count == 0)
            {
                MessageBox.Show("Список задач уже пуст.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Очистить список всех задач?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                TaskManager.GetTaskManager.ClearAllTasks();
                Tasks.Clear();
                SelectedTask = null;
                ShowInfo("Список очищен. Всего задач: 0");
            }
        }

        // ---------- О программе ----------
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Менеджер задач\n" +
                "Практическая работа №6: Многоконные приложения WPF\n\n" +
                "Выполнили: Бедарева Софья, Сторожева Анна\n" +
                "Группа: 9ИС-435\n" +
                "Версия: 1.0",
                "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ---------- Выход ----------
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти из приложения?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        // ---------- INotifyPropertyChanged ----------
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}