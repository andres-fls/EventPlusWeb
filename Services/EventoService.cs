using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class EventoService
    {
        public string UltimoError { get; private set; }
        public List<Evento> ObtenerTodos()
        {
            List<Evento> eventos = new List<Evento>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT e.idEvento, e.Usuario_idUsuario, e.Categoria_idCategoria, e.NombreEvento, 
                                     e.DescripcionEvento, e.LugarEvento, e.FechaInicioEvento, e.FechaFinEvento, 
                                     e.FechaInicioInscripcion, e.FechaFinInscripcion, e.CupoMaximo, e.TipoEvento, e.EstadoEvento,
                                     c.NombreCategoria, u.Nombre AS NombreCreador,
                                     (SELECT COUNT(*) FROM Inscripcion i WHERE i.Evento_idEvento = e.idEvento AND i.EstadoInscripcion = 'Activa') AS Inscritos
                                     FROM Evento e
                                     INNER JOIN Categoria c ON e.Categoria_idCategoria = c.idCategoria
                                     INNER JOIN Usuario u ON e.Usuario_idUsuario = u.idUsuario
                                     ORDER BY e.FechaInicioEvento DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        eventos.Add(MapearEvento(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[EventoService.ObtenerTodos] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EventoService.ObtenerTodos] Error inesperado: {0}", ex.Message);
            }
            return eventos;
        }

        public List<Evento> ObtenerActivos()
        {
            List<Evento> eventos = new List<Evento>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT e.idEvento, e.Usuario_idUsuario, e.Categoria_idCategoria, e.NombreEvento, 
                                     e.DescripcionEvento, e.LugarEvento, e.FechaInicioEvento, e.FechaFinEvento, 
                                     e.FechaInicioInscripcion, e.FechaFinInscripcion, e.CupoMaximo, e.TipoEvento, e.EstadoEvento,
                                     c.NombreCategoria, u.Nombre AS NombreCreador,
                                     (SELECT COUNT(*) FROM Inscripcion i WHERE i.Evento_idEvento = e.idEvento AND i.EstadoInscripcion = 'Activa') AS Inscritos
                                     FROM Evento e
                                     INNER JOIN Categoria c ON e.Categoria_idCategoria = c.idCategoria
                                     INNER JOIN Usuario u ON e.Usuario_idUsuario = u.idUsuario
                                     WHERE e.EstadoEvento = 'Activo'
                                     ORDER BY e.FechaInicioEvento ASC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        eventos.Add(MapearEvento(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[EventoService.ObtenerActivos] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EventoService.ObtenerActivos] Error inesperado: {0}", ex.Message);
            }
            return eventos;
        }

        public Evento ObtenerPorId(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT e.idEvento, e.Usuario_idUsuario, e.Categoria_idCategoria, e.NombreEvento, 
                                     e.DescripcionEvento, e.LugarEvento, e.FechaInicioEvento, e.FechaFinEvento, 
                                     e.FechaInicioInscripcion, e.FechaFinInscripcion, e.CupoMaximo, e.TipoEvento, e.EstadoEvento,
                                     c.NombreCategoria, u.Nombre AS NombreCreador,
                                     (SELECT COUNT(*) FROM Inscripcion i WHERE i.Evento_idEvento = e.idEvento AND i.EstadoInscripcion = 'Activa') AS Inscritos
                                     FROM Evento e
                                     INNER JOIN Categoria c ON e.Categoria_idCategoria = c.idCategoria
                                     INNER JOIN Usuario u ON e.Usuario_idUsuario = u.idUsuario
                                     WHERE e.idEvento = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        return MapearEvento(reader);
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[EventoService.ObtenerPorId] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EventoService.ObtenerPorId] Error inesperado: {0}", ex.Message);
            }
            return null;
        }

        public bool Crear(Evento evento)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"INSERT INTO Evento (Usuario_idUsuario, Categoria_idCategoria, NombreEvento, DescripcionEvento, 
                                     LugarEvento, FechaInicioEvento, FechaFinEvento, FechaInicioInscripcion, FechaFinInscripcion, 
                                     CupoMaximo, TipoEvento, EstadoEvento) 
                                     VALUES (@Usuario_idUsuario, @Categoria_idCategoria, @NombreEvento, @DescripcionEvento, 
                                     @LugarEvento, @FechaInicioEvento, @FechaFinEvento, @FechaInicioInscripcion, @FechaFinInscripcion, 
                                     @CupoMaximo, @TipoEvento, @EstadoEvento)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Usuario_idUsuario", SqlDbType.Int).Value = evento.Usuario_idUsuario;
                    cmd.Parameters.Add("@Categoria_idCategoria", SqlDbType.Int).Value = evento.Categoria_idCategoria;
                    cmd.Parameters.Add("@NombreEvento", SqlDbType.VarChar, 100).Value = evento.NombreEvento;
                    cmd.Parameters.Add("@DescripcionEvento", SqlDbType.VarChar, 500).Value = (object)evento.DescripcionEvento ?? DBNull.Value;
                    cmd.Parameters.Add("@LugarEvento", SqlDbType.VarChar, 100).Value = evento.LugarEvento;
                    cmd.Parameters.Add("@FechaInicioEvento", SqlDbType.DateTime).Value = evento.FechaInicioEvento;
                    cmd.Parameters.Add("@FechaFinEvento", SqlDbType.DateTime).Value = evento.FechaFinEvento;
                    cmd.Parameters.Add("@FechaInicioInscripcion", SqlDbType.DateTime).Value = evento.FechaInicioInscripcion;
                    cmd.Parameters.Add("@FechaFinInscripcion", SqlDbType.DateTime).Value = evento.FechaFinInscripcion;
                    cmd.Parameters.Add("@CupoMaximo", SqlDbType.Int).Value = evento.CupoMaximo;
                    cmd.Parameters.Add("@TipoEvento", SqlDbType.VarChar, 20).Value = evento.TipoEvento;
                    cmd.Parameters.Add("@EstadoEvento", SqlDbType.VarChar, 20).Value = evento.EstadoEvento;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[EventoService.Crear] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EventoService.Crear] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool Actualizar(Evento evento)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"UPDATE Evento SET Categoria_idCategoria = @Categoria_idCategoria, NombreEvento = @NombreEvento, 
                                     DescripcionEvento = @DescripcionEvento, LugarEvento = @LugarEvento, 
                                     FechaInicioEvento = @FechaInicioEvento, FechaFinEvento = @FechaFinEvento, 
                                     FechaInicioInscripcion = @FechaInicioInscripcion, FechaFinInscripcion = @FechaFinInscripcion, 
                                     CupoMaximo = @CupoMaximo, TipoEvento = @TipoEvento, EstadoEvento = @EstadoEvento 
                                     WHERE idEvento = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Categoria_idCategoria", SqlDbType.Int).Value = evento.Categoria_idCategoria;
                    cmd.Parameters.Add("@NombreEvento", SqlDbType.VarChar, 100).Value = evento.NombreEvento;
                    cmd.Parameters.Add("@DescripcionEvento", SqlDbType.VarChar, 500).Value = (object)evento.DescripcionEvento ?? DBNull.Value;
                    cmd.Parameters.Add("@LugarEvento", SqlDbType.VarChar, 100).Value = evento.LugarEvento;
                    cmd.Parameters.Add("@FechaInicioEvento", SqlDbType.DateTime).Value = evento.FechaInicioEvento;
                    cmd.Parameters.Add("@FechaFinEvento", SqlDbType.DateTime).Value = evento.FechaFinEvento;
                    cmd.Parameters.Add("@FechaInicioInscripcion", SqlDbType.DateTime).Value = evento.FechaInicioInscripcion;
                    cmd.Parameters.Add("@FechaFinInscripcion", SqlDbType.DateTime).Value = evento.FechaFinInscripcion;
                    cmd.Parameters.Add("@CupoMaximo", SqlDbType.Int).Value = evento.CupoMaximo;
                    cmd.Parameters.Add("@TipoEvento", SqlDbType.VarChar, 20).Value = evento.TipoEvento;
                    cmd.Parameters.Add("@EstadoEvento", SqlDbType.VarChar, 20).Value = evento.EstadoEvento;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = evento.IdEvento;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[EventoService.Actualizar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EventoService.Actualizar] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool Eliminar(int id)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdInscripciones = new SqlCommand("DELETE FROM Inscripcion WHERE Evento_idEvento = @Id", conn, transaction);
                    cmdInscripciones.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmdInscripciones.ExecuteNonQuery();

                    SqlCommand cmdGrupos = new SqlCommand("DELETE FROM Grupo WHERE Evento_idEvento = @Id", conn, transaction);
                    cmdGrupos.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmdGrupos.ExecuteNonQuery();

                    SqlCommand cmdEvento = new SqlCommand("DELETE FROM Evento WHERE idEvento = @Id", conn, transaction);
                    cmdEvento.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmdEvento.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    Trace.TraceError("[EventoService.Eliminar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Trace.TraceError("[EventoService.Eliminar] Error inesperado: {0}", ex.Message);
                    return false;
                }
            }
        }

        public List<Evento> ObtenerPorCategoria(int categoriaId)
        {
            List<Evento> eventos = new List<Evento>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT e.idEvento, e.Usuario_idUsuario, e.Categoria_idCategoria, e.NombreEvento, 
                                     e.DescripcionEvento, e.LugarEvento, e.FechaInicioEvento, e.FechaFinEvento, 
                                     e.FechaInicioInscripcion, e.FechaFinInscripcion, e.CupoMaximo, e.TipoEvento, e.EstadoEvento,
                                     c.NombreCategoria, u.Nombre AS NombreCreador,
                                     (SELECT COUNT(*) FROM Inscripcion i WHERE i.Evento_idEvento = e.idEvento AND i.EstadoInscripcion = 'Activa') AS Inscritos
                                     FROM Evento e
                                     INNER JOIN Categoria c ON e.Categoria_idCategoria = c.idCategoria
                                     INNER JOIN Usuario u ON e.Usuario_idUsuario = u.idUsuario
                                     WHERE e.Categoria_idCategoria = @CategoriaId
                                     ORDER BY e.FechaInicioEvento DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@CategoriaId", SqlDbType.Int).Value = categoriaId;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        eventos.Add(MapearEvento(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[EventoService.ObtenerPorCategoria] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EventoService.ObtenerPorCategoria] Error inesperado: {0}", ex.Message);
            }
            return eventos;
        }

        private Evento MapearEvento(SqlDataReader reader)
        {
            Evento evento = new Evento();
            evento.IdEvento = Convert.ToInt32(reader["idEvento"]);
            evento.Usuario_idUsuario = Convert.ToInt32(reader["Usuario_idUsuario"]);
            evento.Categoria_idCategoria = Convert.ToInt32(reader["Categoria_idCategoria"]);
            evento.NombreEvento = reader["NombreEvento"].ToString();
            evento.DescripcionEvento = reader["DescripcionEvento"] != DBNull.Value ? reader["DescripcionEvento"].ToString() : null;
            evento.LugarEvento = reader["LugarEvento"].ToString();
            evento.FechaInicioEvento = Convert.ToDateTime(reader["FechaInicioEvento"]);
            evento.FechaFinEvento = Convert.ToDateTime(reader["FechaFinEvento"]);
            evento.FechaInicioInscripcion = Convert.ToDateTime(reader["FechaInicioInscripcion"]);
            evento.FechaFinInscripcion = Convert.ToDateTime(reader["FechaFinInscripcion"]);
            evento.CupoMaximo = Convert.ToInt32(reader["CupoMaximo"]);
            evento.TipoEvento = reader["TipoEvento"].ToString();
            evento.EstadoEvento = reader["EstadoEvento"].ToString();
            evento.NombreCategoria = reader["NombreCategoria"].ToString();
            evento.NombreCreador = reader["NombreCreador"].ToString();
            evento.Inscritos = Convert.ToInt32(reader["Inscritos"]);
            return evento;
        }
    }
}