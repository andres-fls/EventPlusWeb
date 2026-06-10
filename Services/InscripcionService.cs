using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class InscripcionService
    {
        public List<Inscripcion> ObtenerTodas()
        {
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT i.idInscripcion, i.Grupo_idGrupo, i.Aprendiz_idAprendiz, i.Evento_idEvento, 
                                     i.FechaInscripcion, i.EstadoInscripcion,
                                     u.Nombre AS NombreAprendiz, e.NombreEvento, g.NombreGrupo
                                     FROM Inscripcion i
                                     INNER JOIN Aprendiz a ON i.Aprendiz_idAprendiz = a.idAprendiz
                                     INNER JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                     INNER JOIN Evento e ON i.Evento_idEvento = e.idEvento
                                     LEFT JOIN Grupo g ON i.Grupo_idGrupo = g.idGrupo
                                     ORDER BY i.FechaInscripcion DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        inscripciones.Add(MapearInscripcion(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerTodas] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerTodas] Error inesperado: {0}", ex.Message);
            }
            return inscripciones;
        }

        public List<Inscripcion> ObtenerPorEvento(int eventoId)
        {
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT i.idInscripcion, i.Grupo_idGrupo, i.Aprendiz_idAprendiz, i.Evento_idEvento, 
                                     i.FechaInscripcion, i.EstadoInscripcion,
                                     u.Nombre AS NombreAprendiz, e.NombreEvento, g.NombreGrupo
                                     FROM Inscripcion i
                                     INNER JOIN Aprendiz a ON i.Aprendiz_idAprendiz = a.idAprendiz
                                     INNER JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                     INNER JOIN Evento e ON i.Evento_idEvento = e.idEvento
                                     LEFT JOIN Grupo g ON i.Grupo_idGrupo = g.idGrupo
                                     WHERE i.Evento_idEvento = @EventoId
                                     ORDER BY i.FechaInscripcion DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        inscripciones.Add(MapearInscripcion(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerPorEvento] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerPorEvento] Error inesperado: {0}", ex.Message);
            }
            return inscripciones;
        }

        public List<Inscripcion> ObtenerPorAprendiz(int aprendizId)
        {
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT i.idInscripcion, i.Grupo_idGrupo, i.Aprendiz_idAprendiz, i.Evento_idEvento, 
                                     i.FechaInscripcion, i.EstadoInscripcion,
                                     u.Nombre AS NombreAprendiz, e.NombreEvento, g.NombreGrupo
                                     FROM Inscripcion i
                                     INNER JOIN Aprendiz a ON i.Aprendiz_idAprendiz = a.idAprendiz
                                     INNER JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                     INNER JOIN Evento e ON i.Evento_idEvento = e.idEvento
                                     LEFT JOIN Grupo g ON i.Grupo_idGrupo = g.idGrupo
                                     WHERE i.Aprendiz_idAprendiz = @AprendizId
                                     ORDER BY i.FechaInscripcion DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@AprendizId", SqlDbType.Int).Value = aprendizId;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        inscripciones.Add(MapearInscripcion(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerPorAprendiz] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerPorAprendiz] Error inesperado: {0}", ex.Message);
            }
            return inscripciones;
        }

        public Inscripcion ObtenerPorId(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT i.idInscripcion, i.Grupo_idGrupo, i.Aprendiz_idAprendiz, i.Evento_idEvento, 
                                     i.FechaInscripcion, i.EstadoInscripcion,
                                     u.Nombre AS NombreAprendiz, e.NombreEvento, g.NombreGrupo
                                     FROM Inscripcion i
                                     INNER JOIN Aprendiz a ON i.Aprendiz_idAprendiz = a.idAprendiz
                                     INNER JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                     INNER JOIN Evento e ON i.Evento_idEvento = e.idEvento
                                     LEFT JOIN Grupo g ON i.Grupo_idGrupo = g.idGrupo
                                     WHERE i.idInscripcion = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        return MapearInscripcion(reader);
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerPorId] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.ObtenerPorId] Error inesperado: {0}", ex.Message);
            }
            return null;
        }

        public bool CrearIndividual(int aprendizId, int eventoId)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"INSERT INTO Inscripcion (Grupo_idGrupo, Aprendiz_idAprendiz, Evento_idEvento, FechaInscripcion, EstadoInscripcion) 
                                     VALUES (NULL, @Aprendiz_idAprendiz, @Evento_idEvento, GETDATE(), 'Activa')";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Aprendiz_idAprendiz", SqlDbType.Int).Value = aprendizId;
                    cmd.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = eventoId;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.CrearIndividual] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.CrearIndividual] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool CrearGrupal(int aprendizId, int eventoId, int grupoId)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"INSERT INTO Inscripcion (Grupo_idGrupo, Aprendiz_idAprendiz, Evento_idEvento, FechaInscripcion, EstadoInscripcion) 
                                     VALUES (@Grupo_idGrupo, @Aprendiz_idAprendiz, @Evento_idEvento, GETDATE(), 'Activa')";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Grupo_idGrupo", SqlDbType.Int).Value = grupoId;
                    cmd.Parameters.Add("@Aprendiz_idAprendiz", SqlDbType.Int).Value = aprendizId;
                    cmd.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = eventoId;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.CrearGrupal] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.CrearGrupal] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool Cancelar(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "UPDATE Inscripcion SET EstadoInscripcion = 'Cancelada' WHERE idInscripcion = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.Cancelar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.Cancelar] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool Eliminar(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "DELETE FROM Inscripcion WHERE idInscripcion = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.Eliminar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.Eliminar] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool YaEstaInscrito(int aprendizId, int eventoId)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT COUNT(*) FROM Inscripcion 
                                     WHERE Aprendiz_idAprendiz = @AprendizId 
                                     AND Evento_idEvento = @EventoId 
                                     AND EstadoInscripcion = 'Activa'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@AprendizId", SqlDbType.Int).Value = aprendizId;
                    cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.YaEstaInscrito] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.YaEstaInscrito] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public int ContarInscritosPorEvento(int eventoId)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "SELECT COUNT(*) FROM Inscripcion WHERE Evento_idEvento = @EventoId AND EstadoInscripcion = 'Activa'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[InscripcionService.ContarInscritosPorEvento] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return 0;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.ContarInscritosPorEvento] Error inesperado: {0}", ex.Message);
                return 0;
            }
        }

        private Inscripcion MapearInscripcion(SqlDataReader reader)
        {
            Inscripcion inscripcion = new Inscripcion();
            inscripcion.IdInscripcion = Convert.ToInt32(reader["idInscripcion"]);
            inscripcion.Grupo_idGrupo = reader["Grupo_idGrupo"] != DBNull.Value ? Convert.ToInt32(reader["Grupo_idGrupo"]) : (int?)null;
            inscripcion.Aprendiz_idAprendiz = Convert.ToInt32(reader["Aprendiz_idAprendiz"]);
            inscripcion.Evento_idEvento = Convert.ToInt32(reader["Evento_idEvento"]);
            inscripcion.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
            inscripcion.EstadoInscripcion = reader["EstadoInscripcion"].ToString();
            inscripcion.NombreAprendiz = reader["NombreAprendiz"].ToString();
            inscripcion.NombreEvento = reader["NombreEvento"].ToString();
            inscripcion.NombreGrupo = reader["NombreGrupo"] != DBNull.Value ? reader["NombreGrupo"].ToString() : null;
            return inscripcion;
        }
    }
}