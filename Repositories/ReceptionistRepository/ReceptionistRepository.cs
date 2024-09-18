using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;

using Gym_Reception_Management_System.Models;
using Gym_Reception_Management_System.Utils;
using Gym_Reception_Management_System.Utils.Password;
using Gym_Reception_Management_System.ViewModels;

using Oracle.ManagedDataAccess.Client;

namespace Gym_Reception_Management_System.Repositories.ReceptionistRepository
{
    public class ReceptionistRepository : RepositoryBase, IReceptionistRepository
    {
        public bool AuthenticateReceptAcc(NetworkCredential credential)
        {
            var validUser = false;

            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = new OracleCommand())
                    {
                        command.Connection = connection;

                        command.CommandText =
                            "SELECT ACCOUNT_ID FROM BDT_RECEPTIONIST_ACCOUNT WHERE USERNAME=:username AND PASSWORD=:password";

                        command.Parameters.Add("username", OracleDbType.NVarchar2).Value = credential.UserName;
                        command.Parameters.Add("password", OracleDbType.NVarchar2).Value =
                            SymmetricEncryption.Encrypt(credential.Password);


                        if (int.TryParse(command.ExecuteScalar().ToString(), out var accountId))
                            StoreReceptionistAccount(connection, accountId, credential);

                        validUser = true;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }
            }

            return validUser;
        }

        public bool CreateReceptAcc(ReceptionistAccountModel receptionistAccount)
        {
            using (var connection = GetConnection())
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (var command = new OracleCommand())
                    {
                        command.Connection = connection;

                        if (!IsUniqueUsername(connection, receptionistAccount.UserName))
                        {
                            receptionistAccount.SetPropertyError("Username is already taken",
                                nameof(receptionistAccount.UserName));

                            return false;
                        }

                        command.CommandText =
                            "INSERT INTO BDT_RECEPTIONIST_ACCOUNT VALUES (SEQ_RECEPTIONIST_ID.NEXTVAL, :username, :password)";
                        command.Parameters.Add("username", OracleDbType.NVarchar2).Value =
                            receptionistAccount.UserName;
                        command.Parameters.Add("password", OracleDbType.NVarchar2).Value =
                            SymmetricEncryption.Encrypt(receptionistAccount.Password);

                        command.ExecuteNonQuery();
                        command.Parameters.Clear();

                        command.CommandText =
                            "INSERT INTO BDT_RECEPTIONIST_DETAILS VALUES (SEQ_RECEPTIONIST_ID.CURRVAL, :firstName, :lastName, :email, :address, :phoneNumber, :hireDate)";
                        command.Parameters.Add("firstName", OracleDbType.NVarchar2).Value =
                            receptionistAccount.FirstName;
                        command.Parameters.Add("lastName", OracleDbType.NVarchar2).Value =
                            receptionistAccount.LastName;
                        command.Parameters.Add("email", OracleDbType.NVarchar2).Value =
                            receptionistAccount.Email;
                        command.Parameters.Add("address", OracleDbType.NVarchar2).Value =
                            receptionistAccount.Address;
                        command.Parameters.Add("phoneNumber", OracleDbType.Char).Value =
                            receptionistAccount.PhoneNumber;
                        command.Parameters.Add("hireDate", OracleDbType.Date).Value = DateTime.Now;

                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");

                    return false;
                }
            }
        }

        public bool CreateMembership(MembershipModel membership,
                                     List<ServiceViewModel> ServicesCollection)
        {
            using (var connection = GetConnection())
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (var command = new OracleCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        if (!IsUniqueIdSerialNumber(connection, membership.IdSerialNumber))
                        {
                            membership.SetPropertyError("Id Serial Number already is taken",
                                nameof(membership.IdSerialNumber));

                            return false;
                        }

                        command.Connection = connection;

                        command.CommandText =
                            "INSERT INTO BDT_MEMBERSHIP VALUES (SEQ_MEMBERSHIP_ID.NEXTVAL, :firstName, :lastName, :idSerialNum, :address, :phoneNumber, :noServices, :receptionistId)";

                        command.Parameters.Add("firstName", OracleDbType.NVarchar2).Value =
                            membership.FirstName;
                        command.Parameters.Add("lastName", OracleDbType.NVarchar2).Value =
                            membership.LastName;
                        command.Parameters.Add("idSerialNum", OracleDbType.NVarchar2).Value =
                            membership.IdSerialNumber;
                        command.Parameters.Add("address", OracleDbType.NVarchar2).Value =
                            membership.Address;
                        command.Parameters.Add("phoneNumber", OracleDbType.Char).Value =
                            membership.PhoneNumber;
                        command.Parameters.Add("noServices", OracleDbType.Decimal).Value =
                            membership.NoServices;
                        command.Parameters.Add("receptionistId", OracleDbType.Decimal).Value =
                            MainWindowViewModel.ReceptionistAccount.Id;

                        command.ExecuteNonQuery();
                        command.Parameters.Clear();

                        foreach (var service in ServicesCollection)
                        {
                            command.CommandText =
                                "INSERT INTO BDT_MEMBERSHIP_SERVICE VALUES (SEQ_MEMBERSHIP_ID.CURRVAL, :serviceId, :enrollDate, :serviceDuration)";

                            command.Parameters.Add("serviceId", OracleDbType.Decimal).Value =
                                service.ServiceSelectedIndex + 1;
                            command.Parameters.Add("enrollDate", OracleDbType.Date).Value = DateTime.Now;
                            command.Parameters.Add("serviceDurations", OracleDbType.Decimal).Value =
                                service.ServiceDuration;

                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }

                        transaction.Commit();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");

                    return false;
                }
            }
        }

        public bool UpdateMembershipServices(List<ServiceViewModel> servicesCollection, int membershipId)
        {
            using (var connection = GetConnection())
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (var command = new OracleCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        foreach (var service in servicesCollection)
                        {
                            if (service.ServiceDuration == 0) continue;

                            command.CommandText =
                                "UPDATE BDT_MEMBERSHIP_SERVICE SET ENROLL_DATE=:enrollDate WHERE MEMBERSHIP_ID=:membershipId AND SERVICE_ID =:serviceId";

                            command.Parameters.Add("enrollDate", OracleDbType.Date).Value = DateTime.Now;
                            command.Parameters.Add("membershipId", OracleDbType.Decimal).Value = membershipId;
                            command.Parameters.Add("serviceId", OracleDbType.Decimal).Value =
                                service.ServiceSelectedIndex + 1;

                            command.ExecuteNonQuery();
                            command.Parameters.Clear();

                            command.CommandText =
                                "UPDATE BDT_MEMBERSHIP_SERVICE SET SERVICE_DURATION=:serviceDuration WHERE MEMBERSHIP_ID=:membershipId AND SERVICE_ID =:serviceId";

                            command.Parameters.Add("serviceDuration", OracleDbType.Decimal).Value =
                                service.ServiceDuration;
                            command.Parameters.Add("membershipId", OracleDbType.Decimal).Value = membershipId;
                            command.Parameters.Add("serviceId", OracleDbType.Decimal).Value =
                                service.ServiceSelectedIndex + 1;

                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }

                        transaction.Commit();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }

                return false;
            }
        }

        public bool AddMembershipServices(List<ServiceViewModel> newServices, int membershipId, int oldNoServices)
        {
            using (var connection = GetConnection())
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (var command = new OracleCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        foreach (var service in newServices)
                        {
                            command.CommandText =
                                "INSERT INTO BDT_MEMBERSHIP_SERVICE VALUES (:membershipId, :serviceId, :enrollDate, :serviceDuration)";

                            command.Parameters.Add("membershipId", OracleDbType.Decimal).Value = membershipId;
                            command.Parameters.Add("serviceId", OracleDbType.Decimal).Value =
                                service.ServiceSelectedIndex + 1;
                            command.Parameters.Add("enrollDate", OracleDbType.Date).Value = DateTime.Now;
                            command.Parameters.Add("serviceDuration", OracleDbType.Decimal).Value =
                                service.ServiceDuration;

                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }

                        command.CommandText =
                            "UPDATE BDT_MEMBERSHIP SET SERVICES_COUNT=:serviceCount WHERE MEMBERSHIP_ID=:membershipId";

                        command.Parameters.Add("serviceCount", OracleDbType.Decimal).Value =
                            newServices.Count + oldNoServices;
                        command.Parameters.Add("membershipId", OracleDbType.Decimal).Value = membershipId;

                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }

                return false;
            }
        }

        public bool DeleteMembership(MembershipModel selectedMembership)
        {
            using (var connection = GetConnection())
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (var command = new OracleCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        command.CommandText =
                            "DELETE BDT_MEMBERSHIP_SERVICE WHERE MEMBERSHIP_ID=:membershipId";

                        command.Parameters.Add("membershipId", OracleDbType.Decimal).Value = selectedMembership.Id;

                        command.ExecuteNonQuery();
                        command.Parameters.Clear();

                        command.CommandText =
                            "DELETE BDT_MEMBERSHIP WHERE MEMBERSHIP_ID=:membershipId";

                        command.Parameters.Add("membershipId", OracleDbType.Decimal).Value = selectedMembership.Id;

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }

                return false;
            }
        }

        #region Helper

        private bool IsUniqueUsername(OracleConnection connection, string userName)
        {
            var isUnique = false;

            var receptAccModel = ViewModelLocator.SignUpVM.ReceptionistAccount;

            receptAccModel.RemovePropertyError(nameof(receptAccModel.UserName));

            using (var tempCommand = new OracleCommand())
            {
                try
                {
                    tempCommand.Connection = connection;

                    tempCommand.CommandText =
                        "SELECT * FROM BDT_RECEPTIONIST_ACCOUNT WHERE USERNAME=:username";

                    tempCommand.Parameters.Add("username", OracleDbType.NVarchar2).Value = userName;

                    isUnique = tempCommand.ExecuteScalar() == null;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }
            }

            return isUnique;
        }

        private bool IsUniqueIdSerialNumber(OracleConnection connection, string membershipIdSerialNumber)
        {
            var isUnique = false;

            var membershipModel = ViewModelLocator.CreateMembershipVM.Membership;

            membershipModel.RemovePropertyError(nameof(membershipModel.IdSerialNumber));

            using (var tempCommand = new OracleCommand())
            {
                try
                {
                    tempCommand.Connection = connection;

                    tempCommand.CommandText =
                        "SELECT * FROM BDT_MEMBERSHIP WHERE ID_SERIAL_NUMBER=:id_serial_num";

                    tempCommand.Parameters.Add("id_serial_num", OracleDbType.NVarchar2).Value =
                        membershipIdSerialNumber;

                    isUnique = tempCommand.ExecuteScalar() == null;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }
            }

            return isUnique;
        }

        private void StoreReceptionistAccount(OracleConnection connection, int accountId, NetworkCredential credential)
        {
            using (var tempCommand = new OracleCommand())
            {
                try
                {
                    tempCommand.Connection = connection;

                    tempCommand.CommandText =
                        "SELECT FIRST_NAME, LAST_NAME, EMAIL, ADDRESS, PHONE_NUMBER, HIRE_DATE FROM BDT_RECEPTIONIST_DETAILS WHERE RECEPTIONIST_ID=:id";

                    tempCommand.Parameters.Add("id", OracleDbType.NVarchar2).Value = accountId;

                    using (var reader = tempCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var firstName = reader["FIRST_NAME"] as string;
                            var lastName = reader["LAST_NAME"] as string;
                            var email = reader["EMAIL"] as string;
                            var address = reader["ADDRESS"] as string;
                            var phoneNumber = reader["PHONE_NUMBER"] as string;
                            var hireDate = reader["HIRE_DATE"] as string;

                            MainWindowViewModel.ReceptionistAccount = new ReceptionistAccountModel(accountId, firstName,
                                lastName, address, phoneNumber, email, credential.UserName, credential.Password,
                                hireDate);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }
            }
        }

        #endregion
    }
}