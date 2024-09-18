using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

using Gym_Reception_Management_System.ViewModels;

namespace Gym_Reception_Management_System.Models
{
    public class MembershipModel : EntityModelBase
    {
        #region Fields

        private string _idSerialNumber;

        public string IdSerialNumber
        {
            get => _idSerialNumber;
            set => SetProperty(ref _idSerialNumber, value);
        }

        public int PreviousNoServices;
        private int _noServices;

        public int NoServices
        {
            get => _noServices;
            set
            {
                PreviousNoServices = _noServices;
                SetProperty(ref _noServices, value);
            }
        }

        private string _servicesDetails;

        public string ServicesDetails
        {
            get => _servicesDetails;
            set => SetProperty(ref _servicesDetails, value);
        }

        #endregion

        #region Constructors

        public MembershipModel()
        {
            _idSerialNumber = string.Empty;
            _noServices = 1;
            _servicesDetails = string.Empty;

            PreviousNoServices = _noServices;
        }

        public MembershipModel(int id, string firstName, string lastName, string idSerialNumber, string address,
                               string phoneNumber, int noServices, string servicesDetails)
            : base (id, firstName, lastName, address, phoneNumber)
        {
            _idSerialNumber = idSerialNumber;
            _noServices = noServices;
            _servicesDetails = servicesDetails;

            PreviousNoServices = _noServices;
        }

        #endregion

        #region Validations

        public override void ValidateAccountInputs(object sender, PropertyChangedEventArgs e)
        {
            if (!CreateMembershipViewModel.ShowValidations)
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

            if (e == null || e.PropertyName == nameof(IdSerialNumber))
            {
                RemovePropertyError(nameof(IdSerialNumber));
                ValidateIdSerialNumber();
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

        private void ValidateIdSerialNumber()
        {
            var regexPattern = new Regex("^[A-Z]{2}[0-9]{6}$");

            if (!regexPattern.IsMatch(IdSerialNumber))
                SetPropertyError("Invalid ID Serial Number format", nameof(IdSerialNumber));
        }

        #endregion
    }
}