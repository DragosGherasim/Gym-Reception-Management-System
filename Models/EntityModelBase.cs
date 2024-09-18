using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Gym_Reception_Management_System.Models
{
    public abstract class EntityModelBase : ModelBase
    {
        #region Fields

        private int _id;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _firstName;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _address;

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        #endregion

        #region Constructors

        protected EntityModelBase()
        {
            _id = 0;
            _firstName = string.Empty;
            _lastName = string.Empty;
            _address = string.Empty;
            _phoneNumber = string.Empty;

            PropertyChanged += ValidateAccountInputs;
        }

        protected EntityModelBase(int id, string firstName, string lastName, string address, string phoneNumber)
        {
            _id = id;
            _firstName = firstName;
            _lastName = lastName;
            _address = address;
            _phoneNumber = phoneNumber;

            PropertyChanged += ValidateAccountInputs;
        }

        public abstract void ValidateAccountInputs(object sender, PropertyChangedEventArgs e);

        #endregion

        #region Validations

        protected void ValidateName(string propertyName)
        {
            var regexPattern = new Regex("^[A-Za-z]+$");
            var classProperty = GetType().GetProperty(propertyName);

            if (classProperty == null || !(classProperty.GetValue(this) is string propertyValue))
                return;

            if (!regexPattern.IsMatch(propertyValue))
                SetPropertyError("Invalid name format", propertyName);
            else if (propertyValue.Length > 16) SetPropertyError("Exceeded max length", propertyName);
        }

        protected void ValidateAddress()
        {
            var regexPattern = new Regex("^[A-Za-z0-9\\s\\.,#\\-]+$");

            if (!regexPattern.IsMatch(Address))
                SetPropertyError("Invalid address format", nameof(Address));
            else if (Address.Length > 125)
                SetPropertyError("Exceeded max address length", nameof(Address));
        }

        protected void ValidatePhoneNumber()
        {
            var regexPattern = new Regex("^[0-9]{10}$");

            if (!regexPattern.IsMatch(PhoneNumber))
                SetPropertyError("Invalid phone format", nameof(PhoneNumber));
        }

        #endregion
    }
}