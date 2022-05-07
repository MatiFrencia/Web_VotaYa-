using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Voto
    {
        private int COD_VOT { get; set; }
        private Show oShow { get; set; }
        private Participante oParticipante { get; set; }
        private DateTime Fecha { get; set; }
        private Terna oTerna { get; set; }


        public Voto CrearVoto (DataRow dr)
        {
            Participante clsPart = new Participante();
            Show clsShow= new Show();
            Terna clsTerna = new Terna();

            Voto oVoto = new Voto()
            {
                COD_VOT = (int)dr["cod_vot"],
                Fecha = (System.DateTime)dr["fecha"],
                oParticipante = clsPart.GetParticipante((int)dr["cod_par"]).FirstOrDefault(),
                oShow = clsShow.GetShow((int)dr["cod_show"]).FirstOrDefault(),
                oTerna = clsTerna.GetTerna((int)dr["cod_ter"]).FirstOrDefault(),
            };
            return oVoto;
        }

    }
}