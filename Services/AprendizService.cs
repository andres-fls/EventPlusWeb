using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class AprendizService
    {
        // Obtener todos los aprendices con datos de usuario y ficha
        public List<Aprendiz> ObtenerTodos()
        {
            List<Aprendiz> aprendices = new List<Aprendiz>();

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"SELECT a.idAprendiz, a.Usuario_idUsuario, a.Ficha_idFicha, a.Cedula, a.Telefono, a.Edad, a.Genero,
                                 u.Nombre AS NombreUsuario, f.CodigoFicha
                                 FROM Aprendiz a
                                 INNER JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                 INNER JOIN Ficha f ON a.Ficha_idFicha = f.idFicha
                                 ORDER BY u.Nombre";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    aprendices.Add(MapearAprendiz(reader));
                }
            }

            return aprendices;
        }

        // Obtener aprendiz por ID
        public Aprendiz ObtenerPorId(int id)
        {
            Aprendiz aprendiz = null;

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"SELECT a.idAprendiz, a.Usuario_idUsuario, a.Ficha_idFicha, a.Cedula, a.Telefono, a.Edad, a.Genero,
                                 u.Nombre AS NombreUsuario, f.CodigoFicha
                                 FROM Aprendiz a
                                 INNER JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                 INNER JOIN Ficha f ON a.Ficha_idFicha = f.idFicha
                                 WHERE a.idAprendiz = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    aprendiz = MapearAprendiz(reader);
                }
            }

            return aprendiz;
        }

        // Obtener aprendiz por ID de usuario
        public Aprendiz ObtenerPorUsuarioId(int usuarioId)
        {
            Aprendiz aprendiz = null;

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"SELECT a.idAprendiz, a.Usuario_idUsuario, a.Ficha_idFicha, a.Cedula, a.Telefono, a.Edad, a.Genero,
                                 u.Nombre AS NombreUsuario, f.CodigoFicha
                                 FROM Aprendiz a
                                 INNER JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                 INNER JOIN Ficha f ON a.Ficha_idFicha = f.idFicha
                                 WHERE a.Usuario_idUsuario = @UsuarioId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    aprendiz = MapearAprendiz(reader);
                }
            }

            return aprendiz;
        }

        // Crear aprendiz
        public bool Crear(Aprendiz aprendiz)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"INSERT INTO Aprendiz (Usuario_idUsuario, Ficha_idFicha, Cedula, Telefono, Edad, Genero) 
                                 VALUES (@Usuario_idUsuario, @Ficha_idFicha, @Cedula, @Telefono, @Edad, @Genero)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Usuario_idUsuario", SqlDbType.Int).Value = aprendiz.Usuario_idUsuario;
                cmd.Parameters.Add("@Ficha_idFicha", SqlDbType.Int).Value = aprendiz.Ficha_idFicha;
                cmd.Parameters.Add("@Cedula", SqlDbType.VarChar, 20).Value = aprendiz.Cedula;
                cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object)aprendiz.Telefono ?? DBNull.Value;
                cmd.Parameters.Add("@Edad", SqlDbType.Int).Value = aprendiz.Edad.HasValue ? (object)aprendiz.Edad.Value : DBNull.Value;
                cmd.Parameters.Add("@Genero", SqlDbType.VarChar, 20).Value = (object)aprendiz.Genero ?? DBNull.Value;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Actualizar aprendiz
        public bool Actualizar(Aprendiz aprendiz)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = @"UPDATE Aprendiz SET Ficha_idFicha = @Ficha_idFicha, Cedula = @Cedula, 
                                 Telefono = @Telefono, Edad = @Edad, Genero = @Genero WHERE idAprendiz = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Ficha_idFicha", SqlDbType.Int).Value = aprendiz.Ficha_idFicha;
                cmd.Parameters.Add("@Cedula", SqlDbType.VarChar, 20).Value = aprendiz.Cedula;
                cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object)aprendiz.Telefono ?? DBNull.Value;
                cmd.Parameters.Add("@Edad", SqlDbType.Int).Value = aprendiz.Edad.HasValue ? (object)aprendiz.Edad.Value : DBNull.Value;
                cmd.Parameters.Add("@Genero", SqlDbType.VarChar, 20).Value = (object)aprendiz.Genero ?? DBNull.Value;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = aprendiz.IdAprendiz;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Eliminar aprendiz
        public bool Eliminar(int id)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "DELETE FROM Aprendiz WHERE idAprendiz = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                int resultado = cmd.ExecuteNonQuery();
                return resultado > 0;
            }
        }

        // Verificar si un usuario ya tiene perfil de aprendiz
        public bool ExistePerfilAprendiz(int usuarioId)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Aprendiz WHERE Usuario_idUsuario = @UsuarioId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                conn.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        // Mapear reader a entidad
        private Aprendiz MapearAprendiz(SqlDataReader reader)
        {
            Aprendiz aprendiz = new Aprendiz();
            aprendiz.IdAprendiz = Convert.ToInt32(reader["idAprendiz"]);
            aprendiz.Usuario_idUsuario = Convert.ToInt32(reader["Usuario_idUsuario"]);
            aprendiz.Ficha_idFicha = Convert.ToInt32(reader["Ficha_idFicha"]);
            aprendiz.Cedula = reader["Cedula"].ToString();
            aprendiz.Telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : null;
            aprendiz.Edad = reader["Edad"] != DBNull.Value ? Convert.ToInt32(reader["Edad"]) : (int?)null;
            aprendiz.Genero = reader["Genero"] != DBNull.Value ? reader["Genero"].ToString() : null;
            aprendiz.NombreUsuario = reader["NombreUsuario"].ToString();
            aprendiz.CodigoFicha = reader["CodigoFicha"].ToString();
            return aprendiz;
        }
    }
}