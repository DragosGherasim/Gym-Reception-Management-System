using System.Collections.Generic;

namespace Gym_Reception_Management_System.ViewModels
{
    public class ServiceViewModel : ViewModelBase
    {
        #region Fields

        public List<string> Services { get; set; }

        private int _serviceSelectedIndex;

        public int ServiceSelectedIndex
        {
            get => _serviceSelectedIndex;
            set => SetProperty(ref _serviceSelectedIndex, value);
        }

        private int _serviceDuration;

        public int ServiceDuration
        {
            get => _serviceDuration;
            set => SetProperty(ref _serviceDuration, value);
        }

        #endregion

        #region Constructors

        public ServiceViewModel()
        {
            Services = new List<string> { "Fitness", "Cardio", "Spa", "Swimming" };

            _serviceSelectedIndex = 0;
            _serviceDuration = 1;
        }

        public ServiceViewModel(int serviceIndex, int serviceDuration = 0)
        {
            Services = new List<string> { "Fitness", "Cardio", "Spa", "Swimming" };

            _serviceSelectedIndex = serviceIndex;
            _serviceDuration = serviceDuration;
        }

        #endregion
    }
}