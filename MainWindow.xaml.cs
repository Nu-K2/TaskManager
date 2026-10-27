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

namespace TaskManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AddTaskButton_Click(object sender, RoutedEventArgs e)
    {
        Window AddTaskWindow = new AddTaskWindow();
        AddTaskWindow.ShowDialog();
    }

    private void EditTaskButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void DetailsButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void TasksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}