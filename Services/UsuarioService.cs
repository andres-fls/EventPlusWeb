using System.Collections.Generic;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;
using BCrypt.Net;

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
                string query = "SELECT * FROM Usuarios WHERE Correo = @Correo";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Correo", correo);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string hashAlmacenado = reader["Contrasena"].ToString();

                    // Verificar si la contraseña coincide con el hash
                    if (BCrypt.Net.BCrypt.Verify(contrasena, hashAlmacenado))
                    {
                        return new Usuario
                        {
                            Id = (int)reader["Id"],
                            Nombre = reader["Nombre"].ToString(),
                            Correo = reader["Correo"].ToString(),
                            Contrasena = hashAlmacenado,
                            Rol = reader["Rol"].ToString()
                        };
                    }
                }
                return null;
            }
        }

        public bool Registrar(Usuario usuario)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                // Verificar si el correo ya existe
                string queryVerificar = "SELECT COUNT(*) FROM Usuarios WHERE Correo = @correo";
                using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, conn))
                {
                    cmdVerificar.Parameters.AddWithValue("@correo", usuario.Correo);
                    int existe = (int)cmdVerificar.ExecuteScalar();
                    if (existe > 0)
                    {
                        return false;
                    }
                }

                string query = "INSERT INTO Usuarios (Nombre, Correo, Contrasena, Rol) VALUES (@nombre, @correo, @contrasena, @rol)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                    string hashContrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
                    cmd.Parameters.AddWithValue("@Contrasena", hashContrasena);
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