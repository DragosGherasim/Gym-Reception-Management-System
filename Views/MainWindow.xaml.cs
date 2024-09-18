using Gym_Reception_Management_System.Utils;
using System.Windows;

namespace Gym_Reception_Management_System.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.MainWindowVM;
        }
    }
}