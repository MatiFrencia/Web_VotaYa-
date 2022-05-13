using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Usuario
    {
        public int COD_USER { get; set; }
        public string Mail { get; set; }
        public string Pwd { get; set; }
        public string Nombre { get; set; }
        public List<Evento> Eventos { get; set; }
        public bool Host { get; set; }

        public async Task<List<Usuario>> GetUsuario(int COD_EV, int COD_USER = 0)
        {
            List<Usuario> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                if (COD_USER != 0)  //Seleccionar 1 Usuario del EVENTO
                    query = string.Format("Select * from usuarios where cod_user = {0} AND cod_ev = {1}", COD_USER.ToString(), COD_EV.ToString());
                else    //Seleccionar TODOS los Usuarios del EVENTO
                    query = string.Format("Select * from usuarios where cod_ev = {0}", COD_EV.ToString());
                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Usuario oUser = new Usuario()
                    {
                        COD_USER = (int)dr["cod_user"],
                        Eventos = clsEvento.GetEventos((int)dr["cod_user"]),
                        Mail = (string)dr["mail"],
                        Nombre = (string)dr["nombre"],
                        Pwd = (string)dr["clave"],
                        Host = (bool)dr["host"]
                    };
                    lstReturn.Add(oUser);
                }
                return lstReturn;
            }
        }
    }
}
