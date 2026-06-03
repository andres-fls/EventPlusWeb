using EventPlusWeb1.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EventPlusWeb1.Services
{
    public class EventoService
    {
        private readonly DatabaseService _db;

        public EventoService()
        {
            _db = new DatabaseService();
        }

        public List<Evento> ObtenerTodos()
        {
            List<Evento> eventos = new List<Evento>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT e.Id, e.Titulo, e.Descripcion, e.FechaEvento, e.Ubicacion, 
                                 e.CategoriaId, e.UsuarioCreadorId, c.Nombre AS CategoriaNombre, 
                                 u.Nombre AS CreadorNombre
                                 FROM Eventos e
                                 INNER JOIN Categorias c ON e.CategoriaId = c.Id
                                 INNER JOIN Usuarios u ON e.UsuarioCreadorId = u.Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventos.Add(new Evento
                            {
                                Id = (int)reader["Id"],
                                Titulo = reader["Titulo"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                FechaEvento = (DateTime)reader["FechaEvento"],
                                Ubicacion = reader["Ubicacion"].ToString(),
                                CategoriaId = (int)reader["CategoriaId"],
                                UsuarioCreadorId = (int)reader["UsuarioCreadorId"],
                                CategoriaNombre = reader["CategoriaNombre"].ToString(),
                                CreadorNombre = reader["CreadorNombre"].ToString()
                            });
                        }
                    }
                }
            }
            return eventos;
        }

        public Evento ObtenerPorId(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT e.Id, e.Titulo, e.Descripcion, e.FechaEvento, e.Ubicacion, 
                                 e.CategoriaId, e.UsuarioCreadorId, c.Nombre AS CategoriaNombre, 
                                 u.Nombre AS CreadorNombre
                                 FROM Eventos e
                                 INNER JOIN Categorias c ON e.CategoriaId = c.Id
                                 INNER JOIN Usuarios u ON e.UsuarioCreadorId = u.Id
                                 WHERE e.Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Evento
                            {
                                Id = (int)reader["Id"],
                                Titulo = reader["Titulo"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                FechaEvento = (DateTime)reader["FechaEvento"],
                                Ubicacion = reader["Ubicacion"].ToString(),
                                CategoriaId = (int)reader["CategoriaId"],
                                UsuarioCreadorId = (int)reader["UsuarioCreadorId"],
                                CategoriaNombre = reader["CategoriaNombre"].ToString(),
                                CreadorNombre = reader["CreadorNombre"].ToString()
                            };

                        }
                    }
                }
            }
            return null;
        }

        public bool Crear(Evento evento)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Eventos (Titulo, Descripcion, FechaEvento, Ubicacion, CategoriaId, UsuarioCreadorId) 
                                 VALUES (@titulo, @descripcion, @fecha, @ubicacion, @categoriaId, @creadorId)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@titulo", evento.Titulo);
                    cmd.Parameters.AddWithValue("@descripcion", evento.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("@fecha", evento.FechaEvento);
                    cmd.Parameters.AddWithValue("@ubicacion", evento.Ubicacion ?? "");
                    cmd.Parameters.AddWithValue("@categoriaId", evento.CategoriaId);
                    cmd.Parameters.AddWithValue("@creadorId", evento.UsuarioCreadorId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Editar(Evento evento)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Eventos SET Titulo = @titulo, Descripcion = @descripcion, 
                                 FechaEvento = @fecha, Ubicacion = @ubicacion, CategoriaId = @categoriaId
                                 WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@titulo", evento.Titulo);
                    cmd.Parameters.AddWithValue("@descripcion", evento.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("@fecha", evento.FechaEvento);
                    cmd.Parameters.AddWithValue("@ubicacion", evento.Ubicacion ?? "");
                    cmd.Parameters.AddWithValue("@categoriaId", evento.CategoriaId);
                    cmd.Parameters.AddWithValue("@id", evento.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // Primero eliminar inscripciones relacionadas
                    string queryInsc = "DELETE FROM Inscripciones WHERE EventoId = @id";
                    using (SqlCommand cmd = new SqlCommand(queryInsc, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    // Luego eliminar el evento
                    string query = "DELETE FROM Eventos WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int resultado = cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}