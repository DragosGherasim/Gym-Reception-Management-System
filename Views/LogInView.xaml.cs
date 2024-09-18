using System.Windows;
using System.Windows.Controls;

using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.Views
{
    /// <summary>
    ///     Interaction logic for LogInView.xaml
    /// </summary>
    public partial class LogInView : UserControl
    {
        public LogInView()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.LoginInVM;
        }
    }
}