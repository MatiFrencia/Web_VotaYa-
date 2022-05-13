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
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<Show> Presentaciones { get; set; }
        public List<Participante> Participantes { get; set; }
        public Host Host { get; set; }

        public async Task<List<Show>> CalcularGanadores()
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
                        Voto oVoto = await clsVoto.CrearVoto(dr);

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