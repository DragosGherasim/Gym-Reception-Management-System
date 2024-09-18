using Gym_Reception_Management_System.Models;

namespace Gym_Reception_Management_System.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static ReceptionistAccountModel ReceptionistAccount { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainWindowViewModel()
        {
            ReceptionistAccount = new ReceptionistAccountModel();

            _currentView = new LogInViewModel();
        }
    }
}