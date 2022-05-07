using Services.Utils;
using System.Collections.Generic;
using System.Data;

namespace VotaYa.Models
{
    public class Show
    {
        private int COD_SHOW { get; set; }

        public List<Show> GetParticipante(int COD_EV, int COD_ART = 0)
        {
            List<Show> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                if (COD_ART != 0)
                    query = string.Format("Select * from shows where COD_USER = {0} AND COD_EV = {1}", COD_ART.ToString(), COD_EV.ToString());
                else
                    query = string.Format("Select * from shows where COD_EV = {0}", COD_EV.ToString());
                DataTable dtPart = oConexion.Consulta(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Show oShow = new Show()
                    {
                        COD_SHOW = (int)dr["cod_show"]
                    };
                    lstReturn.Add(oShow);
                }
                return lstReturn;
            }
        }
        public List<Show> GetShow(int COD_EV, int COD_SHOW = 0)
        {
            List<Show> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                if (COD_SHOW != 0)  //Seleccionar 1 SHOW del EVENTO
                    query = string.Format("Select * from shows where cod_show = {0} AND cod_ev = {1}", COD_SHOW.ToString(), COD_EV.ToString());
                else   //Seleccionar TODOS los SHOWS del EVENTO
                    query = string.Format("Select * from shows where cod_ev = {0}", COD_EV.ToString());
                DataTable dtPart = oConexion.Consulta(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Show oShow = new Show()
                    {
                        COD_SHOW = (int)dr["cod_show"]
                    };
                    lstReturn.Add(oShow);
                }
                return lstReturn;
            }
        }
    }
}