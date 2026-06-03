using System.Data.SqlClient;

namespace EventPlusWeb1.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EventPlusDB"].ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}