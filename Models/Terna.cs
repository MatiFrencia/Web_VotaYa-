using Services.Utils;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Terna
    {
        public int COD_TER { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public async Task<List<Terna>> GetTerna(int COD_EV, int COD_TER = 0)
        {
            List<Terna> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                if (COD_TER != 0)  //Seleccionar 1 Terna del EVENTO
                    query = string.Format("Select * from ternas where cod_ter = {0} AND cod_ev = {1}", COD_TER.ToString(), COD_EV.ToString());
                else    //Seleccionar TODOS las Ternas del EVENTO
                    query = string.Format("Select * from ternas where cod_ev = {0}", COD_EV.ToString());
                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Terna oTerna = new Terna()
                    {
                        COD_TER = (int)dr["cod_user"],
                        Nombre = (string)dr["nombre"],
                        Descripcion = (string)dr["clave"]
                    };
                    lstReturn.Add(oTerna);
                }
                return lstReturn;
            }
        }
    }
}