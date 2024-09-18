using System.Windows.Controls;

using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.Views
{
    /// <summary>
    ///     Interaction logic for AddServiceView.xaml
    /// </summary>
    public partial class AddServiceView : UserControl
    {
        public AddServiceView()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.AddServiceVM;
        }
    }
}