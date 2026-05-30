using EventPlusWeb1.Models.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EventPlusWeb1.Services
{
    public class CategoriaService
    {
        private readonly DatabaseService _db;

        public CategoriaService()
        {
            _db = new DatabaseService();
        }

        public List<Categoria> ObtenerTodas()
        {
            List<Categoria> categorias = new List<Categoria>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Nombre FROM Categorias";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString()
                            });
                        }
                    }
                }
            }
            return categorias;
        }
    }
}
