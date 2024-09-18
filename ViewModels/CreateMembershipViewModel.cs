using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gym_Reception_Management_System.Models;
using Gym_Reception_Management_System.Repositories.ReceptionistRepository;
using Gym_Reception_Management_System.Utils;
using Gym_Reception_Management_System.Views;

namespace Gym_Reception_Management_System.ViewModels
{
    public class CreateMembershipViewModel : ViewModelBase
    {
        #region Fields

        private MembershipModel _membership;

        public MembershipModel Membership
        {
            get => _membership;
            set => SetProperty(ref _membership, value);
        }

        public ObservableCollection<ServiceViewModel> ServicesCollection { get; set; }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public static bool ShowValidations;

        private readonly IReceptionistRepository _receptRepository;

        #endregion

        #region Contructors

        public CreateMembershipViewModel()
        {
            _membership = new MembershipModel();
            _isLoading = false;
            _receptRepository = new ReceptionistRepository();

            var newService = new ServiceViewModel();
            newService.PropertyChanged += OnNewServiceChanged;
            ServicesCollection = new ObservableCollection<ServiceViewModel> { newService };

            ShowValidations = false;

            Membership.PropertyChanged += OnNoServicesChanged;
            ServicesCollection.CollectionChanged += OnServiceCollectionChanged;
        }

        #endregion

        #region RelayCommands

        public ICommand ConfirmCommand =>
            new RelayCommand(async _ => await ExecuteConfirmAsync(), _ => true);

        public ICommand GoBackCommand =>
            new RelayCommand(() => ViewModelLocator.MainWindowVM.CurrentView = new MembershipListView()
                , _ => true);

        #endregion

        #region RelayCommand Parameters

        private bool CanExecuteConfirm()
        {
            if (!ShowValidations)
            {
                ShowValidations = true;
                Membership.ValidateAccountInputs(null, null);
            }

            return !Membership.HasErrors && !ServicesCollection.Any(service => service.HasErrors);
        }

        private async Task ExecuteConfirmAsync()
        {
            if (!CanExecuteConfirm())
                return;

            IsLoading = true;

            var isValidMembership = await Task.Run(() =>
                                                       _receptRepository.CreateMembership(Membership,
                                                           ServicesCollection.ToList()));

            if (isValidMembership)
            {
                MessageBox.Show(
                    $"Crearea Membership-ului pentru {Membership.LastName} {Membership.FirstName} a fost realizata cu succes !",
                    "Crearea Membership reușită", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ViewModelLocator.MembershipListVM.UpdateMembershipCollection();
            }

            IsLoading = false;
        }


        #endregion

        #region PropertyChanged

        private void OnNoServicesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Membership.NoServices)) return;

            var deltaNoServices = Membership.NoServices - Membership.PreviousNoServices;
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