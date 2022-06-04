using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VotaYa.Util;

namespace VotaYa.Models
{
    public class Terna
    {
        public int COD_TER { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int cod_ev { get; set; }

        public async Task<List<Terna>> GetTerna(int COD_EV, int COD_TER = 0)
        {
            try
            {
                List<Terna> lstReturn = new List<Terna> ();
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
                            COD_TER = (int)dr["cod_ter"],
                            Nombre = (string)dr["nombre"],
                            Descripcion = (string)dr["descripcion"],
                            cod_ev = (int)dr["cod_ev"]
                        };
                        lstReturn.Add(oTerna);
                    }
                    return lstReturn;
                }
            }
            catch(Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, COD_EV, new string[] { "Error consultar ternas" });
                return null;
            }
        }

        public async Task<bool> RegistrarTerna(string nombre, string descripcion, string cod_ev)
        {
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    await oConexion.ConsultaAsync($"INSERT INTO ternas (nombre, descripcion, cod_ev) VALUES ('{nombre}','{descripcion}','{cod_ev}')");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0, new string[] { $"Error al registrar el Genero {nombre}" });
                return false;
            }
        }
    }
}