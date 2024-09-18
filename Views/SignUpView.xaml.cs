using Gym_Reception_Management_System.Utils;
using System.Windows.Controls;

namespace Gym_Reception_Management_System.Views
{
    /// <summary>
    ///     Interaction logic for SignUpView.xaml
    /// </summary>
    /// 
    public partial class SignUpView : UserControl
    {
        public SignUpView()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.SignUpVM;
        }
    }
}