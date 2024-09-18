using System.ComponentModel;
using System.Text.RegularExpressions;

using Gym_Reception_Management_System.ViewModels;

namespace Gym_Reception_Management_System.Models
{
    public class ReceptionistAccountModel : EntityModelBase
    {
        #region Fields

        private string _email;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _userName;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _hireDate;

        public string HireDate
        {
            get => _hireDate;
            set => SetProperty(ref _hireDate, value);
        }

        #endregion

        #region Constructors

        public ReceptionistAccountModel()
        {
            _email = string.Empty;
            _userName = string.Empty;
            _password = string.Empty;
            _hireDate = string.Empty;
        }

        public ReceptionistAccountModel(int id, string firstName, string lastName, string address, string phoneNumber,
                                        string email, string userName, string password, string hireDate) 
            : base(id, firstName, lastName, address, phoneNumber)
        {
            _email = email;
            _userName = userName;
            _password = password;
            _hireDate = hireDate;
        }

        #endregion

        #region Validations

        public override void ValidateAccountInputs(object sender, PropertyChangedEventArgs e)
        {
            if (!SignUpViewModel.ShowValidations)
                return;

            if (e == null || e.PropertyName == nameof(FirstName))
            {
                RemovePropertyError(nameof(FirstName));
                ValidateName(nameof(FirstName));
            }

            if (e == null || e.PropertyName == nameof(LastName))
            {
                RemovePropertyError(nameof(LastName));
                ValidateName(nameof(LastName));
            }

            if (e == null || e.PropertyName == nameof(Email))
            {
                RemovePropertyError(nameof(Email));
                ValidateEmail();
            }

            if (e == null || e.PropertyName == nameof(UserName))
            {
                RemovePropertyError(nameof(UserName));
                ValidateUserName();
            }

            if (e == null || e.PropertyName == nameof(Password))
            {
                RemovePropertyError(nameof(Password));
                ValidatePassword();
            }

            if (e == null || e.PropertyName == nameof(Address))
            {
                RemovePropertyError(nameof(Address));
                ValidateAddress();
            }

            if (e == null || e.PropertyName == nameof(PhoneNumber))
            {
                RemovePropertyError(nameof(PhoneNumber));
                ValidatePhoneNumber();
            }
        }

        private void ValidateEmail()
        {
            var regexPattern = new Regex("^[a-zA-Z0-9._]{3,}@[a-zA-Z]{2,}\\.[a-zA-Z]{2,}$");

            if (!regexPattern.IsMatch(Email))
                SetPropertyError("Invalid email address format", nameof(Email));
            else if (Email.Length > 32)
                SetPropertyError("Exceeded max address length", nameof(Email));
        }

        private void ValidateUserName()
        {
            var regexPattern = new Regex("^(?=.*[a-zA-Z])(?=.*\\d)[A-Za-z\\d._]{6,}$");

            if (!regexPattern.IsMatch(UserName))
                SetPropertyError("Invalid username format", nameof(UserName));
            else if (UserName.Length > 16)
                SetPropertyError("Exceeded max username length", nameof(UserName));
        }

        private void ValidatePassword()
        {
            var regexPattern = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$");

            if (!regexPattern.IsMatch(Password))
                SetPropertyError("Invalid password format", nameof(Password));
            else if (Password.Length > 16)
                SetPropertyError("Exceeded max password length", nameof(Password));
        }

        #endregion
    }
}