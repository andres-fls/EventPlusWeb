using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class CategoriaService
    {
        // Obtener todas las categorías
        public List<Categoria> ObtenerTodas()
        {
            List<Categoria> categorias = new List<Categoria>();

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT idCategoria, NombreCategoria FROM Categoria ORDER BY NombreCategoria";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    categorias.Add(MapearCategoria(reader));
                }
            }

            return categorias;
        }

        // Obtener categoría por ID
        public Categoria ObtenerPorId(int id)
        {
            Categoria categoria = null;

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT idCategoria, NombreCategoria FROM Categoria WHERE idCategoria = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    categoria = MapearCategoria(reader);
                }
            }

            return categoria;
        }

        // Crear categoría
        public bool Crear(Categoria categoria)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "INSERT INTO Categoria (NombreCategoria) VALUES (@NombreCategoria)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@NombreCategoria", SqlDbType.VarChar, 45).Value = categoria.NombreCategoria;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Actualizar categoría
        public bool Actualizar(Categoria categoria)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "UPDATE Categoria SET NombreCategoria = @NombreCategoria WHERE idCategoria = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@NombreCategoria", SqlDbType.VarChar, 45).Value = categoria.NombreCategoria;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = categoria.IdCategoria;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Eliminar categoría
        public bool Eliminar(int id)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "DELETE FROM Categoria WHERE idCategoria = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Mapear reader a entidad
        private Categoria MapearCategoria(SqlDataReader reader)
        {
            Categoria categoria = new Categoria();
            categoria.IdCategoria = Convert.ToInt32(reader["idCategoria"]);
            categoria.NombreCategoria = reader["NombreCategoria"].ToString();
            return categoria;
        }
    }
}