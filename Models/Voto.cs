using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VotaYa.Util;

namespace VotaYa.Models
{
    public class Voto
    {
        public int COD_VOT { get; set; }
        public int Cod_ter { get; set; }
        public int Cod_art { get; set; }
        public int Cod_part { get; set; }
        public int Cod_votac { get; set; }


        public async Task<Voto> CrearVoto(DataRow dr)
        {
            Participacion clsPart = new Participacion();
            Show clsShow = new Show();
            Terna clsTerna = new Terna();

            Voto oVoto = new Voto()
            {
                COD_VOT = (int)dr["cod_vot"],
                Cod_ter = (int)dr["Cod_ter"],
                Cod_art = (int)dr["Cod_art"],
                Cod_part = (int)dr["Cod_part"],
                Cod_votac = (int)dr["Cod_votac"]
            };
            return oVoto;
        }

        public async Task<List<Voto>> GetVotos(int COD_EV)
        {
            List<Voto> lstReturn = new List<Voto>();
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                    query = @$"Select * from votos Vot JOIN votacion Votac ON Vot.cod_votac = Votac.cod_votac where Votac.cod_ev = {COD_EV}";
                DataTable dtArt = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtArt.Rows)
                {
                    Voto oVoto = await CrearVoto(dr);
                    lstReturn.Add(oVoto);
                }
                return lstReturn;
            }
        }

        public async Task<bool> RegistrarVoto(string votos, string cod_ev, string cod_part, string cod_votac)
        {
            var lstVotos = votos.Split(",");
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    foreach (string voto in lstVotos)
                    {
                        if (!string.IsNullOrEmpty(voto))
                        {
                            var query = $"INSERT INTO votos (cod_ter, cod_art, cod_part, cod_votac) VALUES ({voto.Split("|")[0]},{voto.Split("|")[1]},{cod_part},{cod_votac})";
                            await oConexion.EjecutarcomandoAsync(query);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(cod_ev), new string[] { $"Error al registrar votos: {votos}" });
                return false;
            }
        }
    }
}