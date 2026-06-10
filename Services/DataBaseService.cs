using System;
using System.Configuration;
using System.Data.SqlClient;

namespace EventPlusWeb1.Services
{
    public class DatabaseService
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["EventPlusDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}