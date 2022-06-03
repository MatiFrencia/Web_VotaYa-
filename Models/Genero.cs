using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VotaYa.Util;

namespace VotaYa.Models
{
    public class Genero
    {
        public int COD_GEN { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }


        public Genero CrearGenero(DataRow dr)
        {

            Genero oGenero = new Genero()
            {
                COD_GEN = (int)dr["cod_gen"],
                Nombre = (string)dr["nombre"],
                Descripcion = (string)dr["descripcion"]
            };
            return oGenero;
        }

        public async Task<bool> RegistrarGenero(string nombre, string descripcion, string cod_user)
        {
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    oConexion.Consulta($"INSERT INTO generos (nombre, descripcion) VALUES ({nombre},{descripcion})");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(cod_user), new string[] { $"Error al registrar el Genero {nombre}" });
                return false;
            }
        }

        public async Task<List<Genero>> GetGeneros(int cod_ev)
        {
            List<Genero> lstReturn = new List<Genero>();
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = "";
                    if (cod_ev != 0)   //Seleccionar TODOS los Usuarios del EVENTO
                        query = string.Format(@$"SELECT * FROM generos WHERE cod_ev = {cod_ev}");
                    DataTable dtPart = await oConexion.ConsultaAsync(query);

                    foreach (DataRow dr in dtPart.Rows)
                    {
                        Genero oGenero = CrearGenero(dr);
                        lstReturn.Add(oGenero);
                    }
                    return lstReturn;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, cod_ev, new string[] { "error al consultar generos" });
                return lstReturn;
            }
        }
    }
}