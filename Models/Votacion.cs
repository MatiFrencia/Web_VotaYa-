using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VotaYa.Util;

namespace VotaYa.Models
{
    public class Votacion
    {
        public int Cod_votac { get; set; }
        public estado Estado { get; set; }

        public enum estado
        {
            Abierta,
            Cerrada
        }
        public int Cod_ev { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime? Fecha_cierre { get; set; }

        public async Task<List<Votacion>> GetVotaciones(int COD_EV)
        {
            List<Votacion> lstReturn = new List<Votacion>();
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                query = string.Format("Select * from votacion where cod_ev = {0}", COD_EV.ToString());

                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Votacion oVotacion = new Votacion()
                    {
                        Cod_votac = (int)dr["cod_votac"],
                        Estado = (estado)dr["estado"],
                        Cod_ev = (int)dr["cod_ev"],
                        Fecha_inicio = (DateTime)dr["fecha_inicio"],
                    };
                    if (dr["fecha_cierre"] is DBNull)
                    {
                        oVotacion.Fecha_cierre = null;
                    }
                    else
                    {
                        oVotacion.Fecha_cierre = (DateTime)dr["fecha_cierre"];
                    }
                    lstReturn.Add(oVotacion);
                }
                return lstReturn;
            }
        }
        public async Task<int> GetVotacion(int COD_EV)
        {
            var cod_votac = 0;
            using (var oConexion = new MysqlConection())
            {
                string query = $"SELECT cod_votac FROM votacion where cod_ev = {COD_EV} AND estado = 0 AND fecha_cierre is null";

                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    cod_votac = (int)dr["cod_votac"];
                }
                return cod_votac;
            }
        }

        public async Task<bool> IniciarVotacion(string cod_ev)
        {
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = "";
                    query = $"INSERT INTO votacion (estado, cod_ev, fecha_inicio) VALUES ('0','{cod_ev}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}')";
                    var resp = await oConexion.EjecutarcomandoAsync(query);
                    if (resp != 0)
                        return true;
                    return false;
                }
            }
            catch(Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(cod_ev), new string[] { "Error al abrir votación." });
                return false;
            }
        }
        public async Task<bool> CerrarVotacion(string cod_ev)
        {
            using (var oConexion = new MysqlConection())
            {
                string query = $"SELECT cod_votac FROM votacion where cod_ev = {cod_ev} AND estado = 0 AND fecha_cierre is null";
                var cod_votac = 0;
                DataTable dtPart = await oConexion.ConsultaAsync(query);
                foreach (DataRow dr in dtPart.Rows)
                {
                    cod_votac = (int)dr["cod_votac"];
                }

                if (cod_votac != 0)
                {
                    query = $"UPDATE votacion SET estado = 1, fecha_cierre = '{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}' WHERE cod_votac = {cod_votac}";
                    var resp = await oConexion.EjecutarcomandoAsync(query);
                    if (resp != 0)
                        return true;
                    return false;
                }
                return false;
            }
        }
    }
}
