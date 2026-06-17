using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Models;

namespace EventPlusWeb1.Services
{
    public class InscripcionService
    {
        public string UltimoError { get; private set; }
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

        // R3: indica si el aprendiz ya tiene una inscripcion Activa en el evento.
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

        // Alias semantico (R3) usado por la funcionalidad de grupos. Reutiliza YaEstaInscrito.
        public bool YaInscritoEnEvento(int aprendizId, int eventoId)
        {
            return YaEstaInscrito(aprendizId, eventoId);
        }



        public ResultadoInscripcion InscribirGrupal(int aprendizId, int eventoId, string codigoGrupo)
        {
            ResultadoInscripcion resultado = new ResultadoInscripcion();
            try
            {
                // R4: Si el aprendiz ya está inscrito en el evento (sea individual o grupal), devuelve error.
                if (YaInscritoEnEvento(aprendizId, eventoId))
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "Ya estás inscrito en este evento.";
                    return resultado;
                }

                GrupoService grupoService = new GrupoService();
                Grupo grupo = grupoService.ObtenerPorCodigo(eventoId, codigoGrupo);

                // R5: Si el código de grupo no existe para el evento, devuelve error.
                if (grupo == null)
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "El código de grupo no existe para este evento.";
                    return resultado;
                }

                // R6: Si el grupo ya está lleno (MaxIntegrantesGrupo), devuelve error.
                EventoService eventoService = new EventoService();
                Evento evento = eventoService.ObtenerPorId(eventoId);

                if (evento != null && evento.MaxIntegrantesGrupo.HasValue)
                {
                    int integrantesActuales = grupoService.ContarIntegrantes(grupo.IdGrupo);
                    if (integrantesActuales >= evento.MaxIntegrantesGrupo.Value)
                    {
                        resultado.Exito = false;
                        resultado.Mensaje = "El grupo ha alcanzado su número máximo de integrantes.";
                        return resultado;
                    }
                }

                // R7: Si el aprendiz es el líder del grupo, devuelve error.
                if (grupo.LiderAprendiz_idAprendiz.HasValue && grupo.LiderAprendiz_idAprendiz.Value == aprendizId)
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "Ya eres el líder de este grupo.";
                    return resultado;
                }

                // R8: Si todo ok, crea la inscripción (reutiliza CrearGrupal) y devuelve Exito=true, "Te uniste al grupo " + NombreGrupo.
                if (CrearGrupal(aprendizId, eventoId, grupo.IdGrupo))
                {
                    resultado.Exito = true;
                    resultado.Mensaje = "Te uniste al grupo \"" + grupo.NombreGrupo + "\".";
                }
                else
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "Error al unirse al grupo. Inténtalo de nuevo.";
                    if (!string.IsNullOrEmpty(UltimoError))
                    {
                        resultado.Mensaje += " " + UltimoError;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.InscribirGrupal] Error inesperado: {0}", ex.Message);
                resultado.Exito = false;
                resultado.Mensaje = "Error inesperado al intentar unirse al grupo.";
            }
            return resultado;
        }

        public ResultadoInscripcion InscribirIndividual(int aprendizId, int eventoId)
        {
            ResultadoInscripcion resultado = new ResultadoInscripcion();
            try
            {
                // R9: si ya inscrito -> error; si no, CrearIndividual y Exito.
                if (YaInscritoEnEvento(aprendizId, eventoId))
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "Ya estás inscrito en este evento.";
                    return resultado;
                }

                if (CrearIndividual(aprendizId, eventoId))
                {
                    resultado.Exito = true;
                    resultado.Mensaje = "Inscripción individual exitosa.";
                }
                else
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "Error al realizar la inscripción individual. Inténtalo de nuevo.";
                    if (!string.IsNullOrEmpty(UltimoError))
                    {
                        resultado.Mensaje += " " + UltimoError;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InscripcionService.InscribirIndividual] Error inesperado: {0}", ex.Message);
                resultado.Exito = false;
                resultado.Mensaje = "Error inesperado al intentar la inscripción individual.";
            }
            return resultado;
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