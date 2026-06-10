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
        public List<Grupo> ObtenerTodos()
        {
            List<Grupo> grupos = new List<Grupo>();
            try
            {
                using (SqlConnection conn = DatabaseService.GetConnection())
                {
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, e.NombreEvento
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
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, e.NombreEvento
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
                    string query = @"SELECT g.idGrupo, g.Evento_idEvento, g.NombreGrupo, e.NombreEvento
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
                    string query = "INSERT INTO Grupo (Evento_idEvento, NombreGrupo) VALUES (@Evento_idEvento, @NombreGrupo)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = grupo.Evento_idEvento;
                    cmd.Parameters.Add("@NombreGrupo", SqlDbType.VarChar, 45).Value = grupo.NombreGrupo;
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
                    string query = "UPDATE Grupo SET Evento_idEvento = @Evento_idEvento, NombreGrupo = @NombreGrupo WHERE idGrupo = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Evento_idEvento", SqlDbType.Int).Value = grupo.Evento_idEvento;
                    cmd.Parameters.Add("@NombreGrupo", SqlDbType.VarChar, 45).Value = grupo.NombreGrupo;
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
            return grupo;
        }
    }
}