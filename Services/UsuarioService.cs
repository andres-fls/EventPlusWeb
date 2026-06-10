using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class UsuarioService
    {
        // Obtener todos los usuarios
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT idUsuario, Nombre, Correo, ContrasenaHash, Rol, Estado FROM Usuario";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    usuarios.Add(MapearUsuario(reader));
                }
            }

            return usuarios;
        }

        // Obtener usuario por ID
        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario = null;

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT idUsuario, Nombre, Correo, ContrasenaHash, Rol, Estado FROM Usuario WHERE idUsuario = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    usuario = MapearUsuario(reader);
                }
            }

            return usuario;
        }

        // Obtener usuario por correo (para login)
        public Usuario ObtenerPorCorreo(string correo)
        {
            Usuario usuario = null;

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                string query = "SELECT idUsuario, Nombre, Correo, ContrasenaHash, Rol, Estado FROM Usuario WHERE Correo = @Correo";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = correo;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    usuario = MapearUsuario(reader);
                }
            }

            return usuario;
        }

        // Login con BCrypt
        public Usuario Login(string correo, string contrasena)
        {
            Usuario usuario = ObtenerPorCorreo(correo);

            if (usuario == null)
                return null;

            if (!usuario.Estado)
                return null;

            bool contrasenaValida = BCrypt.Net.BCrypt.Verify(contrasena, usuario.ContrasenaHash);

            if (!contrasenaValida)
                return null;

            return usuario;
        }

        // Registrar nuevo usuario
        public bool Registrar(Usuario usuario, string contrasena)
        {
            // Verificar si el correo ya existe
            Usuario existente = ObtenerPorCorreo(usuario.Correo);
            if (existente != null)
                return false;

            // Hashear contraseña con BCrypt
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

        // Actualizar usuario
        public bool Actualizar(Usuario usuario)
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

        // Cambiar estado (activar/desactivar)
        public bool CambiarEstado(int id, bool estado)
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

        // Mapear reader a entidad
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