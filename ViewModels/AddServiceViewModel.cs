using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gym_Reception_Management_System.Repositories.ReceptionistRepository;
using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.ViewModels
{
    public class AddServiceViewModel : ViewModelBase
    {
        #region Fields

        public int MembershipId { get; set; }

        public int OldNoService { get; set; }

        public int _previousNoNewServices;
        private int _noNewServices;

        public int NoNewServices
        {
            get => _noNewServices;
            set
            {
                _previousNoNewServices = _noNewServices;
                SetProperty(ref _noNewServices, value);
            }
        }

        public ObservableCollection<ServiceViewModel> ServicesCollection { get; set; }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private readonly IReceptionistRepository _receptRepository;

        #endregion

        #region Constructors

        public AddServiceViewModel()
        {
            _noNewServices = 1;
            _previousNoNewServices = _noNewServices;
            _isLoading = false;
            _receptRepository = new ReceptionistRepository();

            MembershipId = 0;
            OldNoService = 0;

            var newService = new ServiceViewModel();
            newService.PropertyChanged += OnNewServiceChanged;
            ServicesCollection = new ObservableCollection<ServiceViewModel> { newService };

            PropertyChanged += OnNoServicesChanged;
            ServicesCollection.CollectionChanged += OnServiceCollectionChanged;
        }

        #endregion

        #region RelayCommands

        public ICommand ConfirmCommand => new RelayCommand(async _ => await ExecuteConfirmAsync(), _ => true);

        #endregion

        #region RelayCommand Parameters

        private async Task ExecuteConfirmAsync()
        {
            IsLoading = true;

            var taskStatus =
                await Task.Run(
                    () => _receptRepository.AddMembershipServices(ServicesCollection.ToList(), MembershipId, OldNoService));

            if (taskStatus)
            {
                MessageBox.Show(
                    "Actualizarea serviciilor pentru membership-ul s-au realizat cu succes !",
                    "Actualizare reușită", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ViewModelLocator.MembershipListVM.UpdateMembershipCollection();
            }

            IsLoading = false;
        }

        #endregion

        #region PropertyChanged

        private void OnNoServicesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(NoNewServices)) return;

            var deltaNoServices = NoNewServices - _previousNoNewServices;
            if (deltaNoServices < 0)
                for (var index = 1; index <= -deltaNoServices; ++index)
                {
                    ServicesCollection.RemoveAt(ServicesCollection.Count - 1);
                }
            else if (deltaNoServices > 0)
                for (var index = 1; index <= deltaNoServices; ++index)
                {
                    var newService = new ServiceViewModel();
                    newService.PropertyChanged += OnNewServiceChanged;
                    ServicesCollection.Add(newService);
                }
        }

        private void OnNewServiceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ServiceViewModel serviceVM))
                return;

            if (e.PropertyName != nameof(serviceVM.ServiceSelectedIndex)) return;

            serviceVM.RemovePropertyError(nameof(serviceVM.ServiceSelectedIndex));
            OnServiceCollectionChanged(null, null);
        }

        #endregion

        #region Validations

        private void OnServiceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var uniqueServiceIndexes = new HashSet<int>();

            foreach (var service in ServicesCollection)
                if (!uniqueServiceIndexes.Add(service.ServiceSelectedIndex))
                    service.SetPropertyError("Only one type of service is allowed",
                        nameof(service.ServiceSelectedIndex));
        }

        #endregion
    }
}