using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskTracker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly TaskManager _manager = new();
    private ObservableCollection<TaskItem> _tasksView;
    public MainWindow()
    {
        InitializeComponent();
        LoadTasks();
    }
    private void LoadTasks()
    {
        _tasksView = new ObservableCollection<TaskItem>(_manager.GetAllTasks());
        TasksDataGrid.ItemsSource = _tasksView;
    }
    private void AddTaskButton_Click(object sender, RoutedEventArgs e)
    {
        Window AddTaskWindow = new AddTaskWindow();
        AddTaskWindow.ShowDialog();

        LoadTasks();
    }
}