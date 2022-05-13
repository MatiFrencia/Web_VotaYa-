using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Participante : Usuario
    {

        public async Task<List<Participante>> GetParticipante(int COD_EV, int COD_USER = 0)
        {
            List<Participante> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                if (COD_USER != 0)  //Seleccionar 1 Participante del EVENTO
                    query = string.Format("Select * from usuarios where cod_user = {0} AND cod_ev = {1}", COD_USER.ToString(), COD_EV.ToString());
                else    //Seleccionar TODOS los Participantes del EVENTO
                    query = string.Format("Select * from usuarios where cod_ev = {0}", COD_EV.ToString()); 
                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Participante oParticipante = new Participante()
                    {
                        COD_USER = (int)dr["cod_user"],
                        Eventos = clsEvento.GetEventos((int)dr["cod_user"]),
                        Mail = (string)dr["mail"],
                        Nombre = (string)dr["nombre"],
                        Pwd = (string)dr["clave"],
                        Host = false
                    };
                    lstReturn.Add(oParticipante);
                }
                return lstReturn;
            }
        }

        public Participante CrearParticipante(int _COD_USER, string _Nombre, string _Mail, string _Pwd, List<Evento> _Eventos)
        {
            Participante oParticipante = new Participante()
            {
                COD_USER = _COD_USER,
                Eventos = _Eventos,
                Mail = _Mail,
                Nombre = _Nombre,
                Pwd = _Pwd,
                Host = false
            };
            return oParticipante;
        }
    }
}