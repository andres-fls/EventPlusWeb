using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class UsuarioService
    {
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT u.idUsuario, u.Nombre, u.Correo, u.ContrasenaHash, u.Rol, u.Estado,
                                     u.CreadoPor, u.FechaCreacion,
                                     c.Nombre AS NombreCreador
                                     FROM Usuario u
                                     LEFT JOIN Usuario c ON u.CreadoPor = c.idUsuario";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        usuarios.Add(MapearUsuario(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerTodos] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerTodos] Error inesperado: {0}", ex.Message);
            }
            return usuarios;
        }

        public List<Usuario> ObtenerAdministradores()
        {
            List<Usuario> admins = new List<Usuario>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT u.idUsuario, u.Nombre, u.Correo, u.ContrasenaHash, u.Rol, u.Estado,
                                     u.CreadoPor, u.FechaCreacion,
                                     c.Nombre AS NombreCreador
                                     FROM Usuario u
                                     LEFT JOIN Usuario c ON u.CreadoPor = c.idUsuario
                                     WHERE u.Rol = 'Admin'
                                     ORDER BY u.FechaCreacion DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        admins.Add(MapearUsuario(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerAdministradores] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerAdministradores] Error inesperado: {0}", ex.Message);
            }
            return admins;
        }

        public List<Usuario> ObtenerAprendices(int? programaId = null, int? fichaId = null)
        {
            List<Usuario> aprendices = new List<Usuario>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT u.idUsuario, u.Nombre, u.Correo, u.ContrasenaHash, u.Rol, u.Estado,
                                     u.CreadoPor, u.FechaCreacion,
                                     NULL AS NombreCreador
                                     FROM Usuario u
                                     INNER JOIN Aprendiz a ON u.idUsuario = a.Usuario_idUsuario
                                     INNER JOIN Ficha f ON a.Ficha_idFicha = f.idFicha
                                     INNER JOIN Programa p ON f.Programa_idPrograma = p.idPrograma
                                     WHERE u.Rol = 'Usuario'";

                    if (programaId.HasValue)
                        query += " AND p.idPrograma = @ProgramaId";

                    if (fichaId.HasValue)
                        query += " AND f.idFicha = @FichaId";

                    query += " ORDER BY u.Nombre";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (programaId.HasValue)
                        cmd.Parameters.Add("@ProgramaId", SqlDbType.Int).Value = programaId.Value;

                    if (fichaId.HasValue)
                        cmd.Parameters.Add("@FichaId", SqlDbType.Int).Value = fichaId.Value;

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        aprendices.Add(MapearUsuario(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerAprendices] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerAprendices] Error inesperado: {0}", ex.Message);
            }
            return aprendices;
        }

        public bool RegistrarAdministrador(Usuario usuario, string contrasena, int creadoPorId)
        {
            try
            {
                Usuario existente = ObtenerPorCorreo(usuario.Correo);
                if (existente != null) return false;

                string hash = BCrypt.Net.BCrypt.HashPassword(contrasena);

                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"INSERT INTO Usuario (Nombre, Correo, ContrasenaHash, Rol, Estado, CreadoPor, FechaCreacion) 
                                     VALUES (@Nombre, @Correo, @ContrasenaHash, 'Admin', 1, @CreadoPor, @FechaCreacion)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = usuario.Nombre;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = usuario.Correo;
                    cmd.Parameters.Add("@ContrasenaHash", SqlDbType.VarChar, 255).Value = hash;
                    cmd.Parameters.Add("@CreadoPor", SqlDbType.Int).Value = creadoPorId;
                    cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value =
                        EventPlusWeb1.Helpers.DateTimeHelper.AhoraEnColombia();
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.RegistrarAdministrador] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.RegistrarAdministrador] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public Usuario ObtenerPorId(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT u.idUsuario, u.Nombre, u.Correo, u.ContrasenaHash, u.Rol, u.Estado,
                                     u.CreadoPor, u.FechaCreacion,
                                     c.Nombre AS NombreCreador
                                     FROM Usuario u
                                     LEFT JOIN Usuario c ON u.CreadoPor = c.idUsuario
                                     WHERE u.idUsuario = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        return MapearUsuario(reader);
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerPorId] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerPorId] Error inesperado: {0}", ex.Message);
            }
            return null;
        }

        public Usuario ObtenerPorCorreo(string correo)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT u.idUsuario, u.Nombre, u.Correo, u.ContrasenaHash, u.Rol, u.Estado,
                                     u.CreadoPor, u.FechaCreacion,
                                     c.Nombre AS NombreCreador
                                     FROM Usuario u
                                     LEFT JOIN Usuario c ON u.CreadoPor = c.idUsuario
                                     WHERE u.Correo = @Correo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = correo;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        return MapearUsuario(reader);
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerPorCorreo] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.ObtenerPorCorreo] Error inesperado: {0}", ex.Message);
            }
            return null;
        }

        public Usuario Login(string correo, string contrasena)
        {
            try
            {
                Usuario usuario = ObtenerPorCorreo(correo);
                if (usuario == null) return null;
                if (!usuario.Estado) return null;
                bool contrasenaValida = BCrypt.Net.BCrypt.Verify(contrasena, usuario.ContrasenaHash);
                if (!contrasenaValida) return null;
                return usuario;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.Login] Error inesperado: {0}", ex.Message);
                return null;
            }
        }

        public bool Registrar(Usuario usuario, string contrasena)
        {
            try
            {
                Usuario existente = ObtenerPorCorreo(usuario.Correo);
                if (existente != null) return false;

                string hash = BCrypt.Net.BCrypt.HashPassword(contrasena);

                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"INSERT INTO Usuario (Nombre, Correo, ContrasenaHash, Rol, Estado, FechaCreacion) 
                                     VALUES (@Nombre, @Correo, @ContrasenaHash, 'Usuario', 1, @FechaCreacion)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = usuario.Nombre;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = usuario.Correo;
                    cmd.Parameters.Add("@ContrasenaHash", SqlDbType.VarChar, 255).Value = hash;
                    cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value =
                        EventPlusWeb1.Helpers.DateTimeHelper.AhoraEnColombia();
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.Registrar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.Registrar] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool Actualizar(Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"UPDATE Usuario SET Nombre = @Nombre, Correo = @Correo, 
                                     Rol = @Rol, Estado = @Estado WHERE idUsuario = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = usuario.Nombre;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = usuario.Correo;
                    cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 20).Value = usuario.Rol;
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = usuario.Estado;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = usuario.IdUsuario;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.Actualizar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.Actualizar] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool CambiarEstado(int id, bool estado)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "UPDATE Usuario SET Estado = @Estado WHERE idUsuario = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = estado;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[UsuarioService.CambiarEstado] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[UsuarioService.CambiarEstado] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            Usuario usuario = new Usuario();
            usuario.IdUsuario = Convert.ToInt32(reader["idUsuario"]);
            usuario.Nombre = reader["Nombre"].ToString();
            usuario.Correo = reader["Correo"].ToString();
            usuario.ContrasenaHash = reader["ContrasenaHash"].ToString();
            usuario.Rol = reader["Rol"].ToString();
            usuario.Estado = Convert.ToBoolean(reader["Estado"]);
            usuario.CreadoPor = reader["CreadoPor"] != DBNull.Value ?
                Convert.ToInt32(reader["CreadoPor"]) : (int?)null;
            usuario.FechaCreacion = reader["FechaCreacion"] != DBNull.Value ?
                Convert.ToDateTime(reader["FechaCreacion"]) : (DateTime?)null;
            usuario.NombreCreador = reader["NombreCreador"] != DBNull.Value ?
                reader["NombreCreador"].ToString() : null;
            return usuario;
        }
    }
}