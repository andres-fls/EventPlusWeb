using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class ProgramaService
    {
        // Obtener todos los programas
        public List<Programa> ObtenerTodos()
        {
            List<Programa> programas = new List<Programa>();

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT idPrograma, CodigoPrograma, NombrePrograma, DuracionMeses, NivelPrograma FROM Programa ORDER BY NombrePrograma";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    programas.Add(MapearPrograma(reader));
                }
            }

            return programas;
        }

        // Obtener programa por ID
        public Programa ObtenerPorId(int id)
        {
            Programa programa = null;

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT idPrograma, CodigoPrograma, NombrePrograma, DuracionMeses, NivelPrograma FROM Programa WHERE idPrograma = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    programa = MapearPrograma(reader);
                }
            }

            return programa;
        }

        // Crear programa
        public bool Crear(Programa programa)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"INSERT INTO Programa (CodigoPrograma, NombrePrograma, DuracionMeses, NivelPrograma) 
                                 VALUES (@CodigoPrograma, @NombrePrograma, @DuracionMeses, @NivelPrograma)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@CodigoPrograma", SqlDbType.VarChar, 20).Value = programa.CodigoPrograma;
                cmd.Parameters.Add("@NombrePrograma", SqlDbType.VarChar, 50).Value = programa.NombrePrograma;
                cmd.Parameters.Add("@DuracionMeses", SqlDbType.Int).Value = programa.DuracionMeses;
                cmd.Parameters.Add("@NivelPrograma", SqlDbType.VarChar, 45).Value = programa.NivelPrograma;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Actualizar programa
        public bool Actualizar(Programa programa)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"UPDATE Programa SET CodigoPrograma = @CodigoPrograma, NombrePrograma = @NombrePrograma, 
                                 DuracionMeses = @DuracionMeses, NivelPrograma = @NivelPrograma WHERE idPrograma = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@CodigoPrograma", SqlDbType.VarChar, 20).Value = programa.CodigoPrograma;
                cmd.Parameters.Add("@NombrePrograma", SqlDbType.VarChar, 50).Value = programa.NombrePrograma;
                cmd.Parameters.Add("@DuracionMeses", SqlDbType.Int).Value = programa.DuracionMeses;
                cmd.Parameters.Add("@NivelPrograma", SqlDbType.VarChar, 45).Value = programa.NivelPrograma;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = programa.IdPrograma;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Eliminar programa
        public bool Eliminar(int id)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "DELETE FROM Programa WHERE idPrograma = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Mapear reader a entidad
        private Programa MapearPrograma(SqlDataReader reader)
        {
            Programa programa = new Programa();
            programa.IdPrograma = Convert.ToInt32(reader["idPrograma"]);
            programa.CodigoPrograma = reader["CodigoPrograma"].ToString();
            programa.NombrePrograma = reader["NombrePrograma"].ToString();
            programa.DuracionMeses = Convert.ToInt32(reader["DuracionMeses"]);
            programa.NivelPrograma = reader["NivelPrograma"].ToString();
            return programa;
        }
    }
}