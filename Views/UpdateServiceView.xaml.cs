using System.Windows.Controls;

using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.Views
{
    /// <summary>
    ///     Interaction logic for UpdateServiceView.xaml
    /// </summary>
    public partial class UpdateServiceView : UserControl
    {
        public UpdateServiceView()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.UpdateServiceVM;
        }
    }
}