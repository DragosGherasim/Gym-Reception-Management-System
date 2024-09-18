using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gym_Reception_Management_System.Models;
using Gym_Reception_Management_System.Repositories.ReceptionistRepository;
using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        #region Fields

        private ReceptionistAccountModel _receptionistAccount;

        public ReceptionistAccountModel ReceptionistAccount
        {
            get => _receptionistAccount;
            set => SetProperty(ref _receptionistAccount, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public static bool ShowValidations;

        private readonly IReceptionistRepository _receptRepository;

        #endregion

        #region Constructors

        public SignUpViewModel()
        {
            _receptionistAccount = new ReceptionistAccountModel();
            _isLoading = false;
            _receptRepository = new ReceptionistRepository();

            ShowValidations = false;
        }

        #endregion

        #region RelayCommands

        public ICommand CreateAccCommand =>
            new RelayCommand(async _ => await ExecuteCreateAccAsync(), _ => true);

        public ICommand GoBackCommand =>
            new RelayCommand(() => ViewModelLocator.MainWindowVM.CurrentView = new LogInViewModel()
                , _ => true);

        #endregion

        #region RelayCommand Parameters

        private bool CanExecuteCreateAcc()
        {
            if (!ShowValidations)
            {
                ShowValidations = true;
                ReceptionistAccount.ValidateAccountInputs(null, null);
            }

            return !ReceptionistAccount.HasErrors;
        }

        private async Task ExecuteCreateAccAsync()
        {
            if (!CanExecuteCreateAcc())
                return;

            IsLoading = true;

            var isValidAcc = await Task.Run(() =>
                                                _receptRepository.CreateReceptAcc(ReceptionistAccount));

            if (isValidAcc)
            {
                MessageBox.Show(
                    "Crearea contului s-a realizat cu succes! Sunteti redirectionat catre pagina de Log In !",
                    "Crearea contului reușită", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ViewModelLocator.MainWindowVM.CurrentView = new LogInViewModel();
            }

            IsLoading = false;
        }

        private void ExecuteGoBack()
        {
            ViewModelLocator.MainWindowVM.CurrentView = new LogInViewModel();
        }

        #endregion
    }
}