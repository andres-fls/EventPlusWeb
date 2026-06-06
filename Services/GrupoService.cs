using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EventPlusWeb1.Models.Entities;

namespace EventPlusWeb1.Services
{
    public class GrupoService
    {
        // Obtener todos los grupos con nombre del evento
        public List<Grupo> ObtenerTodos()
        {
            List<Grupo> grupos = new List<Grupo>();

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
                {
                    grupos.Add(MapearGrupo(reader));
                }
            }

            return grupos;
        }

        // Obtener grupos por evento
        public List<Grupo> ObtenerPorEvento(int eventoId)
        {
            List<Grupo> grupos = new List<Grupo>();

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
                {
                    grupos.Add(MapearGrupo(reader));
                }
            }

            return grupos;
        }

        // Obtener grupo por ID
        public Grupo ObtenerPorId(int id)
        {
            Grupo grupo = null;

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
                {
                    grupo = MapearGrupo(reader);
                }
            }

            return grupo;
        }

        // Crear grupo
        public bool Crear(Grupo grupo)
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

        // Actualizar grupo
        public bool Actualizar(Grupo grupo)
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

        // Eliminar grupo (elimina inscripciones asociadas primero)
        public bool Eliminar(int id)
        {
            using (SqlConnection conn = DatabaseService.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Eliminar inscripciones del grupo
                    SqlCommand cmdInscripciones = new SqlCommand("DELETE FROM Inscripcion WHERE Grupo_idGrupo = @Id", conn, transaction);
                    cmdInscripciones.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmdInscripciones.ExecuteNonQuery();

                    // Eliminar el grupo
                    SqlCommand cmdGrupo = new SqlCommand("DELETE FROM Grupo WHERE idGrupo = @Id", conn, transaction);
                    cmdGrupo.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmdGrupo.ExecuteNonQuery();

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

        // Mapear reader a entidad
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