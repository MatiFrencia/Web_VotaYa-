using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VotaYa.Util;


namespace VotaYa.Models
{
    public class Evento
    {
        public int COD_EV { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public estadoEvento Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public List<Show> Presentaciones { get; set; }
        public List<Participacion> Participantes { get; set; }
        public Participacion Host { get; set; }

        public enum estadoEvento
        {
            Proximamente,
            En_curso,
            Finalizado
        }

        public async Task<bool> RegistrarEvento(string nombre, string descripcion, string fechaInicio, string cod_user)
        {
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    Evento NuevoEvento = new Evento()
                    {
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Estado = estadoEvento.Proximamente,
                        FechaCreacion = DateTime.Now,
                        FechaInicio = Convert.ToDateTime(fechaInicio),
                        Host = new Participacion()
                        {
                            Cod_ev = oConexion.SelectMax("eventos", "cod_ev") + 1,
                            Cod_user = Convert.ToInt32(cod_user),
                            Host = true
                        }
                    };
                    oConexion.Consulta("LOCK TABLES eventos AS eventos_write WRITE , eventos AS eventos_read READ, participaciones WRITE");
                    oConexion.Consulta("SET FOREIGN_KEY_CHECKS = 0;");
                    bool creado = oConexion.Transaccion(NuevoEvento);
                    oConexion.Consulta("SET FOREIGN_KEY_CHECKS = 1;");
                    oConexion.Consulta("UNLOCK TABLES");
                    return creado;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(cod_user), new string[] { $"Error al registrar el evento {nombre}, {fechaInicio}, {cod_user}" });
                using (var oConexion = new MysqlConection())
                {
                    oConexion.Consulta("UNLOCK TABLES");
                    oConexion.Consulta("SET FOREIGN_KEY_CHECKS = 1;");
                }
                return false;
            }
        }
        public async Task<List<Show>> CalcularGanadores()
        {
            List<Show> lstGanadores = null;
            List<Voto> lstVotos = null;
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = string.Format($"Select * from votaciones where cod_ev = {COD_EV}");
                    DataTable votos = oConexion.Consulta(query);
                    Voto clsVoto = new Voto();

                    foreach (System.Data.DataRow dr in votos.Rows)
                    {
                        Voto oVoto = await clsVoto.CrearVoto(dr);

                        lstVotos.Add(oVoto);
                    }
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, COD_EV, new string[] { "Error al calcular ganadores" });
            }

            return lstGanadores;
        }

        public async Task<Evento> CrearEvento(DataRow dr)
        {
            Participacion clsParticipante = new Participacion();
            Show clsShows = new Show();
            DateTime? FechaDeFin;
            if (Nucleo.IsDbNull(dr["fecha_fin"]))
            {
                FechaDeFin = null;
            }
            else
            {
                FechaDeFin = (DateTime)dr["fecha_fin"];
            }

            Evento oEvento = new Evento()
            {
                COD_EV = (int)dr["cod_ev"],
                Nombre = (string)dr["nombre"],
                Descripcion = (string)dr["descripcion"],
                Estado = Convert.ToInt32(dr["estado"]) == 0 ? estadoEvento.Proximamente : Convert.ToInt32(dr["estado"]) == 1 ? estadoEvento.En_curso : estadoEvento.Finalizado,
                FechaCreacion = (DateTime)dr["fecha_creacion"],
                FechaInicio = (DateTime)dr["fecha_inicio"],
                FechaFin = FechaDeFin,
                Host = await clsParticipante.GetHost(COD_EV),
                Participantes = await clsParticipante.GetParticipantes(COD_EV),
                Presentaciones = await clsShows.GetShow(COD_EV)
            };
            return oEvento;
        }

        public async Task<bool> IngresarEvento(string codigo, string cod_user)
        {
            Participacion clsParticipante = new Participacion();
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    var query = $"Select cod_ev from eventos where codigo = '{codigo}'";
                    DataTable dtEvento = await oConexion.ConsultaAsync(query);
                    if (dtEvento.Rows.Count != 0)
                    {
                        if (await clsParticipante.RegistrarParticipacion(cod_user, Convert.ToInt32(dtEvento.Rows[0]["cod_ev"]).ToString(), false) != 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(cod_user), new string[] { $"Error al agregar el evento con código = {codigo}, usuario = {cod_user}" });
                return false;
            }
        }

        public async Task<List<Evento>> GetEventos(int COD_USER)
        {
            List<Evento> lstReturn = new List<Evento>();
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = "";
                    Evento clsEvento = new Evento();

                    Host clsHost = new Host();
                    if (COD_USER != 0)   //Seleccionar TODOS los Usuarios del EVENTO
                        query = string.Format(@$"SELECT Ev.cod_ev, nombre, descripcion, estado, fecha_creacion, fecha_inicio, fecha_fin, Ev.host
                                                FROM eventos Ev
                                                JOIN participaciones Par
                                                ON Ev.cod_ev = Par.cod_ev WHERE Par.cod_user = {COD_USER}");
                    DataTable dtPart = await oConexion.ConsultaAsync(query);

                    foreach (DataRow dr in dtPart.Rows)
                    {
                        Evento oEvento = await CrearEvento(dr);
                        lstReturn.Add(oEvento);
                    }
                    return lstReturn;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, COD_USER, new string[] { "error al consultar usuario" });
                return lstReturn;
            }
        }
    }
}