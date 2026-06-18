using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class GrupoService
    {
        public string UltimoError { get; private set; }

        public List<Grupo> ObtenerTodos()
        {
            List<Grupo> grupos = new List<Grupo>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, g.CodigoGrupo, g.LiderAprendiz_idAprendiz, g.FechaCreacion, e.NombreEvento
                                     FROM Grupo g
                                     INNER JOIN Evento e ON g.Evento_idEvento = e.idEvento
                                     ORDER BY e.NombreEvento, g.NombreGrupo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        grupos.Add(MapearGrupo(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.ObtenerTodos] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.ObtenerTodos] Error inesperado: {0}", ex.Message);
            }
            return grupos;
        }

        public List<Grupo> ObtenerPorEvento(int eventoId)
        {
            List<Grupo> grupos = new List<Grupo>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, g.CodigoGrupo, g.LiderAprendiz_idAprendiz, g.FechaCreacion, e.NombreEvento
                                     FROM Grupo g
                                     INNER JOIN Evento e ON g.Evento_idEvento = e.idEvento
                                     WHERE g.Evento_idEvento = @EventoId
                                     ORDER BY g.NombreGrupo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        grupos.Add(MapearGrupo(reader));
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.ObtenerPorEvento] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.ObtenerPorEvento] Error inesperado: {0}", ex.Message);
            }
            return grupos;
        }

        public Grupo ObtenerPorId(int id)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, g.CodigoGrupo, g.LiderAprendiz_idAprendiz, g.FechaCreacion, e.NombreEvento
                                     FROM Grupo g
                                     INNER JOIN Evento e ON g.Evento_idEvento = e.idEvento
                                     WHERE g.idGrupo = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        return MapearGrupo(reader);
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.ObtenerPorId] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.ObtenerPorId] Error inesperado: {0}", ex.Message);
            }
            return null;
        }

        public bool Crear(Grupo grupo)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "INSERT INTO Grupo (Evento_idEvento, NombreGrupo, CodigoGrupo, LiderAprendiz_idAprendiz, FechaCreacion) VALUES (@Evento_idEvento, @NombreGrupo, @CodigoGrupo, @LiderAprendiz_idAprendiz, GETDATE())";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = grupo.Evento_idEvento;
                    cmd.Parameters.Add("@NombreGrupo", SqlDbType.VarChar, 45).Value = grupo.NombreGrupo;
                    cmd.Parameters.Add("@CodigoGrupo", SqlDbType.VarChar, 20).Value = (object)grupo.CodigoGrupo ?? DBNull.Value;
                    cmd.Parameters.Add("@LiderAprendiz_idAprendiz", SqlDbType.Int).Value = (object)grupo.LiderAprendiz_idAprendiz ?? DBNull.Value;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.Crear] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.Crear] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        public bool Actualizar(Grupo grupo)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "UPDATE Grupo SET Evento_idEvento = @Evento_idEvento, NombreGrupo = @NombreGrupo, CodigoGrupo = @CodigoGrupo, LiderAprendiz_idAprendiz = @LiderAprendiz_idAprendiz, FechaCreacion = @FechaCreacion WHERE idGrupo = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = grupo.Evento_idEvento;
                    cmd.Parameters.Add("@NombreGrupo", SqlDbType.VarChar, 45).Value = grupo.NombreGrupo;
                    cmd.Parameters.Add("@CodigoGrupo", SqlDbType.VarChar, 20).Value = (object)grupo.CodigoGrupo ?? DBNull.Value;
                    cmd.Parameters.Add("@LiderAprendiz_idAprendiz", SqlDbType.Int).Value = (object)grupo.LiderAprendiz_idAprendiz ?? DBNull.Value;
                    cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = (object)grupo.FechaCreacion ?? DBNull.Value;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = grupo.IdGrupo;
                    conn.Open();
                    int resultado = cmd.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.Actualizar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.Actualizar] Error inesperado: {0}", ex.Message);
                return false;
            }
        }

        // Caracteres permitidos para el código de grupo
        private const string ALFABETO_CODIGO = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
        private const int LONGITUD_CODIGO = 4;

        public string GenerarCodigoUnico()
        {
            string codigo;
            Random random = new Random();
            do
            {
                char[] chars = new char[LONGITUD_CODIGO];
                for (int i = 0; i < LONGITUD_CODIGO; i++)
                {
                    chars[i] = ALFABETO_CODIGO[random.Next(ALFABETO_CODIGO.Length)];
                }
                codigo = "GRP-" + new string(chars);
            } while (CodigoYaExiste(codigo));
            return codigo;
        }

        private bool CodigoYaExiste(string codigo)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "SELECT COUNT(*) FROM Grupo WHERE CodigoGrupo = @Codigo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.CodigoYaExiste] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return true; // Asumir que existe para evitar duplicados en caso de error
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.CodigoYaExiste] Error inesperado: {0}", ex.Message);
                return true; // Asumir que existe para evitar duplicados en caso de error
            }
        }

        public Grupo ObtenerPorCodigo(int eventoId, string codigo)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, g.CodigoGrupo, g.LiderAprendiz_idAprendiz, g.FechaCreacion, e.NombreEvento
                                     FROM Grupo g
                                     INNER JOIN Evento e ON g.Evento_idEvento = e.idEvento
                                     WHERE g.Evento_idEvento = @EventoId AND g.CodigoGrupo = @Codigo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                    cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        return MapearGrupo(reader);
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.ObtenerPorCodigo] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.ObtenerPorCodigo] Error inesperado: {0}", ex.Message);
            }
            return null;
        }

        public int ContarIntegrantes(int grupoId)
        {
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = "SELECT COUNT(*) FROM Inscripcion WHERE Grupo_idGrupo = @GrupoId AND EstadoInscripcion = 'Activa'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@GrupoId", SqlDbType.Int).Value = grupoId;
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.ContarIntegrantes] Error SQL ({0}): {1}", ex.Number, ex.Message);
                return 0;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.ContarIntegrantes] Error inesperado: {0}", ex.Message);
                return 0;
            }
        }

        public List<Grupo> ObtenerGruposPorEvento(int eventoId)
        {
            List<Grupo> grupos = new List<Grupo>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, g.CodigoGrupo, g.LiderAprendiz_idAprendiz, g.FechaCreacion, 
                                     e.NombreEvento, e.MaxIntegrantesGrupo, u.Nombre AS NombreLider
                                     FROM Grupo g
                                     INNER JOIN Evento e ON g.Evento_idEvento = e.idEvento
                                     LEFT JOIN Aprendiz a ON g.LiderAprendiz_idAprendiz = a.idAprendiz
                                     LEFT JOIN Usuario u ON a.Usuario_idUsuario = u.idUsuario
                                     WHERE g.Evento_idEvento = @EventoId
                                     ORDER BY g.NombreGrupo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Grupo grupo = MapearGrupo(reader);
                        grupo.IntegrantesActuales = ContarIntegrantes(grupo.IdGrupo);
                        grupo.NombreLider = reader["NombreLider"] != DBNull.Value ? reader["NombreLider"].ToString() : null;
                        int? maxIntegrantes = reader["MaxIntegrantesGrupo"] != DBNull.Value ? Convert.ToInt32(reader["MaxIntegrantesGrupo"]) : (int?)null;
                        grupo.TieneCupo = maxIntegrantes.HasValue ? (grupo.IntegrantesActuales < maxIntegrantes.Value) : true; // Si no hay max, siempre hay cupo
                        grupos.Add(grupo);
                    }
                }
            }
            catch (SqlException ex)
            {
                Trace.TraceError("[GrupoService.ObtenerGruposPorEvento] Error SQL ({0}): {1}", ex.Number, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GrupoService.ObtenerGruposPorEvento] Error inesperado: {0}", ex.Message);
            }
            return grupos;
        }

        public string CrearGrupoConLider(int eventoId, string nombreGrupo, int aprendizId, int maxIntegrantes)
        {
            string codigoGenerado = GenerarCodigoUnico(); // Generar código antes de la transacción

            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // (a) Valida R3: si el aprendiz ya tiene inscripción Activa en el evento, NO crea y devuelve null;
                    InscripcionService inscripcionService = new InscripcionService();
                    if (inscripcionService.YaInscritoEnEvento(aprendizId, eventoId))
                    {
                        transaction.Rollback();
                        UltimoError = "El aprendiz ya está inscrito en este evento.";
                        return null;
                    }

                    // (c) INSERT Grupo
                    string queryGrupo = @"INSERT INTO Grupo (Evento_idEvento, NombreGrupo, CodigoGrupo, LiderAprendiz_idAprendiz, FechaCreacion) 
                                          VALUES (@Evento_idEvento, @NombreGrupo, @CodigoGrupo, @LiderAprendiz_idAprendiz, GETDATE());
                                          SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdGrupo = new SqlCommand(queryGrupo, conn, transaction);
                    cmdGrupo.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = eventoId;
                    cmdGrupo.Parameters.Add("@NombreGrupo", SqlDbType.VarChar, 45).Value = nombreGrupo;
                    cmdGrupo.Parameters.Add("@CodigoGrupo", SqlDbType.VarChar, 20).Value = codigoGenerado;
                    cmdGrupo.Parameters.Add("@LiderAprendiz_idAprendiz", SqlDbType.Int).Value = aprendizId;
                    int idGrupo = Convert.ToInt32(cmdGrupo.ExecuteScalar());

                    // (d) INSERT primera Inscripcion
                    string queryInscripcion = @"INSERT INTO Inscripcion (Grupo_idGrupo, Aprendiz_idAprendiz, Evento_idEvento, FechaInscripcion, EstadoInscripcion) 
                                                VALUES (@Grupo_idGrupo, @Aprendiz_idAprendiz, @Evento_idEvento, GETDATE(), 'Activa');";
                    SqlCommand cmdInscripcion = new SqlCommand(queryInscripcion, conn, transaction);
                    cmdInscripcion.Parameters.Add("@Grupo_idGrupo", SqlDbType.Int).Value = idGrupo;
                    cmdInscripcion.Parameters.Add("@Aprendiz_idAprendiz", SqlDbType.Int).Value = aprendizId;
                    cmdInscripcion.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = eventoId;
                    cmdInscripcion.ExecuteNonQuery();

                    transaction.Commit();
                    return codigoGenerado;
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    Trace.TraceError("[GrupoService.CrearGrupoConLider] Error SQL ({0}): {1}", ex.Number, ex.Message);
                    UltimoError = "Error al crear el grupo y la inscripción: " + ex.Message;
                    return null;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Trace.TraceError("[GrupoService.CrearGrupoConLider] Error inesperado: {0}", ex.Message);
                    UltimoError = "Error inesperado al crear el grupo y la inscripción: " + ex.Message;
                    return null;
                }
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
                    SqlCommand cmdInscripciones = new SqlCommand("DELETE FROM Inscripcion WHERE Grupo_idGrupo = @Id", conn, transaction);
                    cmdInscripciones.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmdInscripciones.ExecuteNonQuery();

                    SqlCommand cmdGrupo = new SqlCommand("DELETE FROM Grupo WHERE idGrupo = @Id", conn, transaction);
                    cmdGrupo.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmdGrupo.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    Trace.TraceError("[GrupoService.Eliminar] Error SQL ({0}): {1}", ex.Number, ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Trace.TraceError("[GrupoService.Eliminar] Error inesperado: {0}", ex.Message);
                    return false;
                }
            }
        }

        private Grupo MapearGrupo(SqlDataReader reader)
        {
            Grupo grupo = new Grupo();
            grupo.IdGrupo = Convert.ToInt32(reader["idGrupo"]);
            grupo.Evento_idEvento = Convert.ToInt32(reader["Evento_idEvento"]);
            grupo.NombreGrupo = reader["NombreGrupo"].ToString();
            grupo.NombreEvento = reader["NombreEvento"].ToString();
            grupo.CodigoGrupo = reader["CodigoGrupo"] != DBNull.Value ? reader["CodigoGrupo"].ToString() : null;
            grupo.LiderAprendiz_idAprendiz = reader["LiderAprendiz_idAprendiz"] != DBNull.Value ? Convert.ToInt32(reader["LiderAprendiz_idAprendiz"]) : (int?)null;
            grupo.FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : (DateTime?)null;
            return grupo;
        }
    }
}