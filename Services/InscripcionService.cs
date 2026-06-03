using EventPlusWeb1.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EventPlusWeb1.Services
{
    public class InscripcionService
    {
        private readonly DatabaseService _db;

        public InscripcionService()
        {
            _db = new DatabaseService();
        }

        public List<Inscripcion> ObtenerPorEvento(int eventoId)
        {
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT i.Id, i.UsuarioId, i.EventoId, i.FechaInscripcion, 
                                 u.Nombre AS UsuarioNombre, e.Titulo AS EventoTitulo
                                 FROM Inscripciones i
                                 INNER JOIN Usuarios u ON i.UsuarioId = u.Id

                                 INNER JOIN Eventos e ON i.EventoId = e.Id
                                 WHERE i.EventoId = @eventoId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@eventoId", eventoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inscripciones.Add(new Inscripcion
                            {
                                Id = (int)reader["Id"],
                                UsuarioId = (int)reader["UsuarioId"],
                                EventoId = (int)reader["EventoId"],
                                FechaInscripcion = (DateTime)reader["FechaInscripcion"],
                                UsuarioNombre = reader["UsuarioNombre"].ToString(),
                                EventoTitulo = reader["EventoTitulo"].ToString()
                            });
                        }
                    }
                }
            }
            return inscripciones;
        }

        public bool Inscribir(int usuarioId, int eventoId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                // Verificar si ya está inscrito
                string queryVerificar = "SELECT COUNT(*) FROM Inscripciones WHERE UsuarioId = @usuarioId AND EventoId = @eventoId";
                using (SqlCommand cmd = new SqlCommand(queryVerificar, conn))
                {
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@eventoId", eventoId);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0) return false; // Ya está inscrito
                }

                string query = "INSERT INTO Inscripciones (UsuarioId, EventoId, FechaInscripcion) VALUES (@usuarioId, @eventoId, @fecha)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@eventoId", eventoId);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool CancelarInscripcion(int usuarioId, int eventoId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Inscripciones WHERE UsuarioId = @usuarioId AND EventoId = @eventoId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@eventoId", eventoId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
