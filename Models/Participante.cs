using Services.Utils;
using System.Collections.Generic;
using System.Data;

namespace VotaYa.Models
{
    public class Participante : Usuario
    {

        public List<Participante> GetParticipante(int COD_EV, int COD_USER = 0)
        {
            List<Participante> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                if (COD_USER != 0)  //Seleccionar 1 Participante del EVENTO
                    query = string.Format("Select * from usuarios where COD_USER = {0} AND COD_EV = {1}", COD_USER.ToString(), COD_EV.ToString());
                else    //Seleccionar TODOS los Participantes del EVENTO
                    query = string.Format("Select * from usuarios where COD_EV = {0}", COD_EV.ToString()); 
                DataTable dtPart = oConexion.Consulta(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Participante oParticipante = new Participante()
                    {
                        COD_USER = (int)dr["cod_user"],
                        Eventos = clsEvento.GetEventos((int)dr["cod_user"]),
                        Mail = (string)dr["mail"],
                        Nombre = (string)dr["nombre"],
                        Pwd = (string)dr["clave"],

                    };
                    lstReturn.Add(oParticipante);
                }
                return lstReturn;
            }
        }
    }
}