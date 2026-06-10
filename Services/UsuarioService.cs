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
                    string query = "SELECT idUsuario, Nombre, Correo, ContrasenaHash, Rol, Estado FROM Usuario";
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

        public Usuario ObtenerPorId(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "SELECT idUsuario, Nombre, Correo, ContrasenaHash, Rol, Estado FROM Usuario WHERE idUsuario = @Id";
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
                    string query = "SELECT idUsuario, Nombre, Correo, ContrasenaHash, Rol, Estado FROM Usuario WHERE Correo = @Correo";
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
                    string query = @"INSERT INTO Usuario (Nombre, Correo, ContrasenaHash, Rol, Estado) 
                                     VALUES (@Nombre, @Correo, @ContrasenaHash, @Rol, @Estado)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = usuario.Nombre;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = usuario.Correo;
                    cmd.Parameters.Add("@ContrasenaHash", SqlDbType.VarChar, 255).Value = hash;
                    cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 20).Value = usuario.Rol;
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = true;
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
            return usuario;
        }
    }
}