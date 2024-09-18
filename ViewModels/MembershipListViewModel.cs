using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gym_Reception_Management_System.Models;
using Gym_Reception_Management_System.Repositories.ReceptionistRepository;
using Gym_Reception_Management_System.Repositories.SystemRepository;
using Gym_Reception_Management_System.Utils;
using Gym_Reception_Management_System.Views;

namespace Gym_Reception_Management_System.ViewModels
{
    public class MembershipListViewModel : ViewModelBase
    {
        #region Fields

        public ObservableCollection<MembershipModel> MembershipsCollection { get; set; }

        private MembershipModel _selectedMembership;

        public MembershipModel SelectedMembership
        {
            get => _selectedMembership;
            set => SetProperty(ref _selectedMembership, value);
        }

        private bool _membershipCommandsAreEnabled;

        public bool MembershipCommandsAreEnabled
        {
            get => _membershipCommandsAreEnabled;
            set => SetProperty(ref _membershipCommandsAreEnabled, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private readonly ISystemRepository _systemRepository;

        private readonly IReceptionistRepository _receptionistRepository;

        #endregion

        #region Constructors

        public MembershipListViewModel()
        {
            MembershipsCollection = new ObservableCollection<MembershipModel>();

            _selectedMembership = new MembershipModel();
            _membershipCommandsAreEnabled = false;
            _isLoading = false;
            _systemRepository = new SystemRepository();
            _receptionistRepository = new ReceptionistRepository();

            PropertyChanged += OnSelectedMembershipChanged;
        }

        #endregion

        #region RelayCommands

        public ICommand UpdateMembershipCommand =>
            new RelayCommand(_ => ExecuteUpdateMembership(), _ => true);

        public ICommand AddMembershipCommand =>
            new RelayCommand(_ => ExecuteAddMembership(), _ => true);

        public ICommand DeleteMembershipCommand =>
            new RelayCommand(async _ => await ExecuteDeleteMembershipAsync(), _ => true);

        public ICommand CreateMembershipCommand =>
            new RelayCommand(() => ViewModelLocator.MainWindowVM.CurrentView = new CreateMembershipView()
                , _ => true);

        public ICommand GoBackCommand =>
            new RelayCommand(() => ViewModelLocator.MainWindowVM.CurrentView = new LogInViewModel()
                , _ => true);

        #endregion

        #region RellayCommands Parameters

        private void ExecuteUpdateMembership()
        {
            ViewModelLocator.UpdateServiceVM.UpdateProperties(SelectedMembership);

            var updateServiceWindow = new UpdateServiceWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            updateServiceWindow.ShowDialog();

            SelectedMembership = null;
            ViewModelLocator.UpdateServiceVM.ServicesCollection.Clear();
        }

        private void ExecuteAddMembership()
        {
            ViewModelLocator.AddServiceVM.MembershipId = SelectedMembership.Id;
            ViewModelLocator.AddServiceVM.OldNoService = SelectedMembership.NoServices;

            var addServiceWindow = new AddServiceWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            addServiceWindow.ShowDialog();

            SelectedMembership = null;
        }

        private async Task ExecuteDeleteMembershipAsync()
        {
            IsLoading = true;

            var taskStatus = await Task.Run(() => _receptionistRepository.DeleteMembership(SelectedMembership));

            if (taskStatus)
            {
                MessageBox.Show(
                    $"Stergea membership-ului {SelectedMembership.FirstName} {SelectedMembership.LastName} s-a realizat cu succes !",
                    "Stergere reușită", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                UpdateMembershipCollection();
            }

            IsLoading = false;
        }

        private void ExecuteCreateMembership() { }

        #endregion

        #region PropertyChanged

        private void OnSelectedMembershipChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedMembership)) MembershipCommandsAreEnabled = SelectedMembership != null;
        }

        #endregion

        #region Update Properties

        public void UpdateMembershipCollection()
        {
            MembershipsCollection.Clear();

            _systemRepository.UpdateMembershipCollection(MembershipsCollection);
        }

        #endregion
    }
}