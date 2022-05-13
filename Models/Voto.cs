using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Voto
    {
        public int COD_VOT { get; set; }
        public Show oShow { get; set; }
        public Participante oParticipante { get; set; }
        public DateTime Fecha { get; set; }
        public Terna oTerna { get; set; }


        public async Task<Voto> CrearVoto (DataRow dr)
        {
            Participante clsPart = new Participante();
            Show clsShow= new Show();
            Terna clsTerna = new Terna();

            Voto oVoto = new Voto()
            {
                COD_VOT = (int)dr["cod_vot"],
                Fecha = (System.DateTime)dr["fecha"],
                oParticipante = (await clsPart.GetParticipante((int)dr["cod_par"])).FirstOrDefault(),
                oShow = (await clsShow.GetShow((int)dr["cod_show"])).FirstOrDefault(),
                oTerna = (await clsTerna.GetTerna((int)dr["cod_ter"])).FirstOrDefault()
            };
            return oVoto;
        }

        public async Task<List<Voto>> GetVoto(int COD_EV, int COD_SHOW = 0)
        {
            List<Voto> lstReturn = null;
            Show clsShow = new Show();
            Participante clsParticipante = new Participante();
            Terna clsTerna = new Terna();
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                if (COD_SHOW != 0)  //Seleccionar 1 Voto del EVENTO
                    query = string.Format("Select * from votos where cod_show = {0} AND cod_ev = {1}", COD_SHOW.ToString(), COD_EV.ToString());
                else    //Seleccionar TODOS los Votos del EVENTO
                    query = string.Format("Select * from votos where cod_ev = {0}", COD_EV.ToString());
                DataTable dtArt = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtArt.Rows)
                {
                    Voto oVoto = new Voto()
                    {
                        COD_VOT = (int)dr["cod_vot"],
                        oShow = (await clsShow.GetShow((int)dr["cod_show"])).FirstOrDefault(),
                        oParticipante = (await clsParticipante.GetParticipante((int)dr["cod_user"])).FirstOrDefault(),
                        Fecha = (DateTime)dr["fecha"],
                        oTerna = (await clsTerna.GetTerna((int)dr["cod_ter"])).FirstOrDefault()
                    };
                    lstReturn.Add(oVoto);
                }
                return lstReturn;
            }
        }
    }
}