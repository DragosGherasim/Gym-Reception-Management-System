using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;

using Gym_Reception_Management_System.Models;

using Oracle.ManagedDataAccess.Client;

namespace Gym_Reception_Management_System.Repositories.SystemRepository
{
    public class SystemRepository : RepositoryBase, ISystemRepository
    {
        public void UpdateMembershipCollection(ObservableCollection<MembershipModel> membershipsCollection)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var commandMember = new OracleCommand())
                    {
                        commandMember.Connection = connection;

                        commandMember.CommandText =
                            "SELECT MEMBERSHIP_ID, FIRST_NAME, LAST_NAME, ID_SERIAL_NUMBER, ADRESS, PHONE_NUMBER, SERVICES_COUNT FROM BDT_MEMBERSHIP ORDER BY MEMBERSHIP_ID";

                        using (var reader = commandMember.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var membershipId = Convert.ToInt32(reader["MEMBERSHIP_ID"]);
                                var firstName = reader["FIRST_NAME"] as string;
                                var lastName = reader["LAST_NAME"] as string;
                                var idSerialNumber = reader["ID_SERIAL_NUMBER"] as string;
                                var address = reader["ADRESS"] as string;
                                var phoneNumber = reader["PHONE_NUMBER"] as string;
                                var noServices = Convert.ToInt32(reader["SERVICES_COUNT"]);

                                var servicesDetails = GetMembershipServicesDetails(connection, membershipId);

                                membershipsCollection.Add(new MembershipModel(membershipId, firstName, lastName,
                                    idSerialNumber, address, phoneNumber, noServices, servicesDetails));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error occurred: {e.Message}", "Error");
                }
            }
        }

        #region Helper

        private string GetMembershipServicesDetails(OracleConnection connection, int membershipId)
        {
            var servicesDetails = new StringBuilder();

            try
            {
                using (var tempCommand = new OracleCommand())
                {
                    tempCommand.Connection = connection;

                    tempCommand.CommandText =
                        "SELECT ENROLL_DATE, SERVICE_DURATION, SERVICE_NAME FROM BDT_SERVICE JOIN BDT_MEMBERSHIP_SERVICE USING (SERVICE_ID) WHERE MEMBERSHIP_ID=:membershipId";

                    tempCommand.Parameters.Add("membershipId", OracleDbType.Decimal).Value = membershipId;

                    using (var reader = tempCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var enrollDate = reader["ENROLL_DATE"] as DateTime? ?? default;
                            var serviceDuration = Convert.ToInt32(reader["SERVICE_DURATION"]);

                            var expiringDate = enrollDate.AddMonths(serviceDuration).ToString("dd-MMM-yyyy").ToUpper();

                            servicesDetails.Append($"{reader["SERVICE_NAME"]} : {expiringDate}; ");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred: {e.Message}", "Error");
            }

            return servicesDetails.ToString();
        }

        #endregion
    }
}