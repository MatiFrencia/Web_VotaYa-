using Services.Utils;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Show
    {
        public int COD_SHOW { get; set; }
        public string Descripcion { get; set; }
        public int cod_gen { get; set; }
        public int cod_art { get; set; }
        public int cod_ev { get; set; }
        public estadoShow Estado{ get; set; }

        public enum estadoShow
        {
            Proximo,
            Siguiente,
            En_curso,
            Finalizado,
            Cancelado
        }

        public async Task<List<Show>> GetShow(int COD_EV, int COD_SHOW = 0, int COD_ART = 0)
        {
            List<Show> lstReturn = new List<Show>();
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                if (COD_SHOW != 0)  //Seleccionar 1 SHOW del EVENTO
                    query = string.Format("Select * from shows where cod_show = {0}", COD_SHOW.ToString(), COD_EV.ToString());
                else   //Seleccionar TODOS los SHOWS del EVENTO
                    query = string.Format("Select * from shows where cod_ev = {0}", COD_EV.ToString());

                if (COD_ART != 0)
                    query += " AND cod_art = " + COD_ART;

                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Show oShow = new Show()
                    {
                        COD_SHOW = (int)dr["cod_show"],
                        cod_art = (int)dr["cod_art"],
                        cod_ev = (int)dr["cod_ev"],
                        Descripcion = (string)dr["descripcion"],
                        Estado = (estadoShow)dr["estado"],
                        cod_gen = (int)dr["cod_gen"]
                    };
                    lstReturn.Add(oShow);
                }
                return lstReturn;
            }
        }
    }
}