using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VotaYa.Util;

namespace VotaYa.Models
{
    public class Show
    {
        public int COD_SHOW { get; set; }
        public string Descripcion { get; set; }
        public int cod_gen { get; set; }
        public int cod_art { get; set; }
        public int cod_ev { get; set; }
        public estadoShow Estado { get; set; }

        public enum estadoShow
        {
            Proximamente,
            Siguiente,
            En_curso,
            Finalizado,
            Cancelado
        }

        public async Task<bool> SetEstado(string cod_show, string funcion)
        {
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = "";
                    switch (funcion)
                    {
                        case "DeleteShow":
                            query = $"DELETE FROM shows where cod_show = {cod_show}";
                            break;
                        case "SetEnCurso":
                            query = $"UPDATE shows SET estado = 2 where cod_show = {cod_show}";
                            break;
                        case "SetSiguiente":
                            query = $"UPDATE shows SET estado = 1 where cod_show = {cod_show}";
                            break;
                        case "SetCancelado":
                            query = $"UPDATE shows SET estado = 4 where cod_show = {cod_show}";
                            break;
                        case "SetFinalizado":
                            query = $"UPDATE shows SET estado = 3 where cod_show = {cod_show}";
                            break;
                        case "SetProximo":
                            query = $"UPDATE shows SET estado = 0 where cod_show = {cod_show}";
                            break;
                    }
                    var resp = await oConexion.EjecutarcomandoAsync(query);
                    if (resp != 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, cod_ev, new string[] { funcion });
                return false;
            }
        }
        public async Task<List<Show>> GetShow(int COD_EV, int COD_SHOW = 0, int COD_ART = 0)
        {
            List<Show> lstReturn = new List<Show>();
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                if (COD_SHOW != 0)  //Seleccionar 1 SHOW del EVENTO
                    query = string.Format("Select * from shows where cod_show = {0}", COD_SHOW.ToString(), COD_EV.ToString());
                else   //Seleccionar TODOS los SHOWS del EVENTO
                    query = string.Format("Select * from shows where cod_ev = {0}", COD_EV.ToString());

                if (COD_ART != 0)
                    query += " AND cod_art = " + COD_ART;

                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Show oShow = new Show()
                    {
                        COD_SHOW = (int)dr["cod_show"],
                        cod_art = (int)dr["cod_art"],
                        cod_ev = (int)dr["cod_ev"],
                        Descripcion = (string)dr["descripcion"],
                        Estado = (estadoShow)dr["estado"],
                        cod_gen = (int)dr["cod_gen"]
                    };
                    lstReturn.Add(oShow);
                }
                return lstReturn;
            }
        }
    }
}