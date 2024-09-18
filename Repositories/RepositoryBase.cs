using System.Data.SqlClient;

using Oracle.ManagedDataAccess.Client;

namespace Gym_Reception_Management_System.Repositories
{
    public abstract class RepositoryBase
    {
        private const string _connectionString = "User Id=bd008;Password=bd008;Data Source=81.180.214.85:1539/orcl";

        protected OracleConnection GetConnection() => new OracleConnection(_connectionString);
    }
}