using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Participacion
    {
        public int Cod_par { get; set; }
        public int Cod_user { get; set; }
        public int Cod_ev { get; set; }
        public bool Host { get; set; }

        public async Task<List<Participacion>> GetParticipantes(int COD_EV)
        {
            List<Participacion> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();   //Seleccionar TODOS los Participantes del EVENTO
                query = string.Format($"Select * from participaciones where cod_ev = {COD_EV}");
                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Participacion oParticipacion = CrearParticipante(dr);
                    lstReturn.Add(oParticipacion);
                }
                return lstReturn;
            }
        }
        public async Task<Participacion> GetParticipante(int COD_EV, int COD_USER)
        {
            Participacion oParticipante = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();   //Seleccionar TODOS los Participantes del EVENTO
                query = string.Format($"Select * from participaciones where cod_ev = {COD_EV} AND cod_user = {COD_USER}");
                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    oParticipante = CrearParticipante(dr);
                }
                return oParticipante;
            }
        }

        public async Task<List<Participacion>> GetParticipaciones(int COD_USER)
        {
            List<Participacion> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                query = string.Format($"Select * from participaciones where cod_user = {COD_USER}");
                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Participacion oParticipacion = CrearParticipante(dr);
                    lstReturn.Add(oParticipacion);
                }
                return lstReturn;
            }
        }


        public async Task<Participacion> GetHost(int COD_EV)
        {
            Participacion Host = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                Evento clsEvento = new Evento();
                query = string.Format($"Select * from participaciones where cod_ev = {COD_EV} AND  host = 1");
                DataTable dtPart = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtPart.Rows)
                {
                    Host = CrearParticipante(dr);
                }
                return Host;
            }
        }

        public Participacion CrearParticipante(DataRow dr)
        {
            Participacion oParticipacion = new Participacion()
            {
                Cod_par = (int)dr["cod_par"],
                Cod_user = (int)dr["cod_user"],
                Cod_ev = (int)dr["cod_ev"],
                Host = (int)dr["host"] == 1,
            };
            return oParticipacion;
        }
    }
}