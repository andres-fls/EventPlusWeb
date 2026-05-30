using System.Collections.Generic;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class UsuarioService
    {
        private readonly DatabaseService _db;

        public UsuarioService()
        {
            _db = new DatabaseService();
        }

        public Usuario Login(string correo, string contrasena)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Nombre, Correo, Rol FROM Usuarios WHERE Correo = @correo AND Contrasena = @contrasena";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Rol = reader["Rol"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool Registrar(Usuario usuario)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Usuarios (Nombre, Correo, Contrasena, Rol) VALUES (@nombre, @correo, @contrasena, @rol)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@contrasena", usuario.Contrasena);
                    cmd.Parameters.AddWithValue("@rol", "Usuario");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> usuarios = new List<Usuario>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Nombre, Correo, Rol FROM Usuarios";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Rol = reader["Rol"].ToString()
                            });
                        }
                    }
                }
            }
            return usuarios;
        }
    }
}