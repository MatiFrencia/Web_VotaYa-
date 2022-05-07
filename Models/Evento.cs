using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using VotaYa.Util;

namespace VotaYa.Models
{
    public class Evento
    {
        private int COD_EV { get; set; }
        private DateTime FechaCreacion { get; set; }
        private DateTime FechaInicio { get; set; }
        private DateTime FechaFin { get; set; }
        private List<Show> Presentaciones { get; set; }
        private List<Participante> Participantes { get; set; }
        private Host Host { get; set; }

        public List<Show> CalcularGanadores()
        {
            List<Show> lstGanadores = null;
            List<Voto> lstVotos = null;
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = string.Format("Select * from votaciones where ID_EVENTO = {0}", COD_EV.ToString());
                    DataTable votos = oConexion.Consulta(query);
                    Voto clsVoto = new Voto();

                    foreach (System.Data.DataRow dr in votos.Rows)
                    {
                        Voto oVoto = clsVoto.CrearVoto(dr);

                        lstVotos.Add(oVoto);
                    }
                }
            }
            catch(Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, COD_EV, new string[] { "Error al calcular ganadores" });
            }

                return lstGanadores;
        }
        public List<Evento> GetEventos(int COD_USER)
        {
            return new List<Evento>();
        }
    }
}