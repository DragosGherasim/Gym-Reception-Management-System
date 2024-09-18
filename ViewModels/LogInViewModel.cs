using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gym_Reception_Management_System.Repositories.ReceptionistRepository;
using Gym_Reception_Management_System.Utils;

namespace Gym_Reception_Management_System.ViewModels
{
    public class LogInViewModel : ViewModelBase
    {
        #region Fields

        private string _userName;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private SecureString _password;

        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private readonly IReceptionistRepository _receptRepository;

        #endregion

        #region Constructors

        public LogInViewModel()
        {
            _userName = string.Empty;
            _password = new SecureString();
            _isLoading = false;

            _receptRepository = new ReceptionistRepository();
        }

        #endregion

        #region RelayCommands

        public ICommand LogInCommand => new RelayCommand(
            async _ => await ExecuteLogInAsync(), _ => !(string.IsNullOrEmpty(UserName) || Password.Length == 0));

        public ICommand SignUpCommand => new RelayCommand(
            () => ViewModelLocator.MainWindowVM.CurrentView = new SignUpViewModel(), _ => true);

        #endregion

        #region RelayCommand Parameters

        private async Task ExecuteLogInAsync()
        {
            IsLoading = true;

            var isValidAcc = await Task.Run(
                                 () =>
                                     _receptRepository.AuthenticateReceptAcc(
                                         new NetworkCredential(UserName, Password)));

            if (isValidAcc)
            {
                RemoveAllPropertiesErrors();

                ViewModelLocator.MembershipListVM.UpdateMembershipCollection();

                ViewModelLocator.MainWindowVM.CurrentView = new MembershipListViewModel();

                MessageBox.Show($"Bun venit, {UserName}!", "Autentificare reușită", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
                PromptAuthenticationError();

            IsLoading = false;
        }

        private void ExecuteSignUp() { }

        #endregion

        #region Validation

        private void PromptAuthenticationError()
        {
            RemoveAllPropertiesErrors();

            SetPropertyError("Sorry, we couldn't find an account with that username and password", nameof(UserName));

            SetPropertyError("Sorry, we couldn't find an account with that username and password", nameof(Password));
        }

        #endregion
    }
}