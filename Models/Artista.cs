using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Artista
    {
        public int COD_ART { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }


        public async Task<List<Artista>> GetArtista(int COD_EV, int COD_ART = 0)
        {
            List<Artista> lstReturn = null;
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                if (COD_ART != 0)  //Seleccionar 1 Artista del EVENTO
                    query = string.Format("Select * from artistas where cod_art = {0} AND cod_ev = {1}", COD_ART.ToString(), COD_EV.ToString());
                else    //Seleccionar TODOS los Artistas del EVENTO
                    query = string.Format("Select * from artistas where cod_ev = {0}", COD_EV.ToString());
                DataTable dtArt = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtArt.Rows)
                {
                    Artista oArtista = new Artista()
                    {
                        COD_ART = (int)dr["cod_art"],
                        Nombre = (string)dr["nombre"],
                        Alias = (string)dr["alias"]

                    };
                    lstReturn.Add(oArtista);
                }
                return lstReturn;
            }
        }
    }
}