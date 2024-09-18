using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gym_Reception_Management_System.Models;
using Gym_Reception_Management_System.Repositories.ReceptionistRepository;
using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.ViewModels
{
    public class UpdateServiceViewModel : ViewModelBase
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

        private readonly IReceptionistRepository _receptRepository;

        #endregion

        #region Constructors

        public UpdateServiceViewModel()
        {
            _membership = new MembershipModel();
            _isLoading = false;

            ServicesCollection = new ObservableCollection<ServiceViewModel>();

            _receptRepository = new ReceptionistRepository();
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
                    () => _receptRepository.UpdateMembershipServices(ServicesCollection.ToList(), Membership.Id));

            if (taskStatus)
            {
                MessageBox.Show(
                    $"Actualizarea serviciilor pentru membership-ul {Membership.FirstName} {Membership.LastName} s-au realizat cu succes !",
                    "Actualizare reușită", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ViewModelLocator.MembershipListVM.UpdateMembershipCollection();
            }

            IsLoading = false;
        }

        #endregion

        #region Update Properties

        public void UpdateProperties(MembershipModel selectedMembership)
        {
            Membership = selectedMembership;

            var serviceSelectedIndexMapping = new Dictionary<string, int>
            {
                { "Fitness", 0 },
                { "Cardio", 1 },
                { "Spa", 2 },
                { "Swimming", 3 }
            };

            var servicesDetails = Membership.ServicesDetails.Split(';');

            foreach (var serviceDetail in servicesDetails)
            {
                var serviceName = serviceDetail.Split(':')[0].Trim(' ');

                if (serviceSelectedIndexMapping.TryGetValue(serviceName, out var serviceSelectedIndex))
                    ServicesCollection.Add(new ServiceViewModel(serviceSelectedIndex));
            }
        }

        #endregion
    }
}