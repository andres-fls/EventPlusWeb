using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class FichaService
    {
        // Obtener todas las fichas con nombre del programa
        public List<Ficha> ObtenerTodas()
        {
            List<Ficha> fichas = new List<Ficha>();

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"SELECT f.idFicha, f.Programa_idPrograma, f.CodigoFicha, f.FechaInicio, f.FechaFin, f.Estado,
                                 p.NombrePrograma
                                 FROM Ficha f
                                 INNER JOIN Programa p ON f.Programa_idPrograma = p.idPrograma
                                 ORDER BY f.CodigoFicha";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    fichas.Add(MapearFicha(reader));
                }
            }

            return fichas;
        }

        // Obtener ficha por ID
        public Ficha ObtenerPorId(int id)
        {
            Ficha ficha = null;

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"SELECT f.idFicha, f.Programa_idPrograma, f.CodigoFicha, f.FechaInicio, f.FechaFin, f.Estado,
                                 p.NombrePrograma
                                 FROM Ficha f
                                 INNER JOIN Programa p ON f.Programa_idPrograma = p.idPrograma
                                 WHERE f.idFicha = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ficha = MapearFicha(reader);
                }
            }

            return ficha;
        }

        // Obtener fichas activas (para dropdowns)
        public List<Ficha> ObtenerActivas()
        {
            List<Ficha> fichas = new List<Ficha>();

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"SELECT f.idFicha, f.Programa_idPrograma, f.CodigoFicha, f.FechaInicio, f.FechaFin, f.Estado,
                                 p.NombrePrograma
                                 FROM Ficha f
                                 INNER JOIN Programa p ON f.Programa_idPrograma = p.idPrograma
                                 WHERE f.Estado = 'Activa'
                                 ORDER BY f.CodigoFicha";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    fichas.Add(MapearFicha(reader));
                }
            }

            return fichas;
        }

        // Crear ficha
        public bool Crear(Ficha ficha)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"INSERT INTO Ficha (Programa_idPrograma, CodigoFicha, FechaInicio, FechaFin, Estado) 
                                 VALUES (@Programa_idPrograma, @CodigoFicha, @FechaInicio, @FechaFin, @Estado)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Programa_idPrograma", SqlDbType.Int).Value = ficha.Programa_idPrograma;
                cmd.Parameters.Add("@CodigoFicha", SqlDbType.VarChar, 20).Value = ficha.CodigoFicha;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = ficha.FechaInicio;
                cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = ficha.FechaFin;
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = ficha.Estado;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Actualizar ficha
        public bool Actualizar(Ficha ficha)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"UPDATE Ficha SET Programa_idPrograma = @Programa_idPrograma, CodigoFicha = @CodigoFicha, 
                                 FechaInicio = @FechaInicio, FechaFin = @FechaFin, Estado = @Estado WHERE idFicha = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Programa_idPrograma", SqlDbType.Int).Value = ficha.Programa_idPrograma;
                cmd.Parameters.Add("@CodigoFicha", SqlDbType.VarChar, 20).Value = ficha.CodigoFicha;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = ficha.FechaInicio;
                cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = ficha.FechaFin;
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = ficha.Estado;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = ficha.IdFicha;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Eliminar ficha
        public bool Eliminar(int id)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "DELETE FROM Ficha WHERE idFicha = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Mapear reader a entidad
        private Ficha MapearFicha(SqlDataReader reader)
        {
            Ficha ficha = new Ficha();
            ficha.IdFicha = Convert.ToInt32(reader["idFicha"]);
            ficha.Programa_idPrograma = Convert.ToInt32(reader["Programa_idPrograma"]);
            ficha.CodigoFicha = reader["CodigoFicha"].ToString();
            ficha.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
            ficha.FechaFin = Convert.ToDateTime(reader["FechaFin"]);
            ficha.Estado = reader["Estado"].ToString();
            ficha.NombrePrograma = reader["NombrePrograma"].ToString();
            return ficha;
        }
    }
}