using Gym_Reception_Management_System.Utils;
using System.Windows.Controls;

namespace Gym_Reception_Management_System.Views
{
    /// <summary>
    ///     Interaction logic for MembershipListView.xaml
    /// </summary>
    public partial class MembershipListView : UserControl
    {
        public MembershipListView()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.MembershipListVM;
        }
    }
}