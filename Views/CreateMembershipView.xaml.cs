using System.Windows.Controls;

using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.Views
{
    /// <summary>
    ///     Interaction logic for CreateMembershipView.xaml
    /// </summary>
    public partial class CreateMembershipView : UserControl
    {
        public CreateMembershipView()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.CreateMembershipVM;
        }
    }
}