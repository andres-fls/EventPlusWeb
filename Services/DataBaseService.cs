using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EventPlusWeb1.Services
{
    public class DatabaseService
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["EventPlusDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(connectionString);
            }
            catch (ArgumentException ex)
            {
                Trace.TraceError("[DatabaseService] Cadena de conexión inválida: {0}", ex.Message);
                throw new ApplicationException("Error en la configuración de la base de datos.", ex);
            }
        }
    }
}