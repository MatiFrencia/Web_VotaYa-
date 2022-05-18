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
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<Show> Presentaciones { get; set; }
        public List<Participacion> Participantes { get; set; }
        public Participacion Host { get; set; }

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

            Evento oEvento = new Evento()
            {
                COD_EV = (int)dr["cod_ev"],
                Nombre = (string)dr["nombre"],
                FechaCreacion = (DateTime)dr["fecha_creacion"],
                FechaInicio = (DateTime)dr["fecha_inicio"],
                FechaFin = (DateTime)dr["fecha_fin"],
                Host = await clsParticipante.GetHost(COD_EV),
                Participantes = await clsParticipante.GetParticipantes(COD_EV),
                Presentaciones = await clsShows.GetShow(COD_EV)
            };
            return oEvento;
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
                        query = string.Format(@$"SELECT Ev.cod_ev, fecha_creacion, fecha_inicio, fecha_fin, Ev.host
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