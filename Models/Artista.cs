using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Artista
    {
        public int COD_ART { get; set; }
        public int cod_ev { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public Imagen Foto { get; set; }
        public class Imagen
        {
            public string Url { get; set; }
            public string Base64 { get; set; }
        }
        public static Imagen ByteToImage(string Base64)
        {
            Imagen img = new Imagen();
            img.Base64 = Base64;
            img.Url = "data:image/jpg;base64," + Base64;
            return img;
        }

        public static byte[] ImageToByte(Imagen img)
        {
            var array = Encoding.ASCII.GetBytes(img.Url.Replace("data:image;base64,", ""));
            return array;
        }



        public async Task<List<Artista>> GetArtista(int COD_EV, int COD_ART = 0)
        {
            List<Artista> lstReturn = new List<Artista>();
            using (var oConexion = new MysqlConection())
            {
                string query = "";
                if (COD_ART != 0)  //Seleccionar 1 Artista del EVENTO
                    query = string.Format(@$"SELECT *
                                                FROM artistas Art
                                                WHERE cod_art = { COD_ART }");
                else    //Seleccionar TODOS los Artistas del EVENTO
                    query = string.Format(@$"SELECT Art.cod_art, Art.nombre, Art.cod_ev, Art.alias, Art.foto
                                                FROM artistas Art
                                                JOIN eventos Ev ON Ev.cod_ev = Art.cod_ev
                                                WHERE Ev.cod_ev = { COD_EV }");
                DataTable dtArt = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtArt.Rows)
                {
                    Artista oArtista = new Artista()
                    {
                        COD_ART = (int)dr["cod_art"],
                        Nombre = (string)dr["nombre"],
                        cod_ev = (int)dr["cod_ev"],
                        Alias = (string)dr["alias"],
                        Foto = dr["foto"] is DBNull? null : ByteToImage((string)dr["foto"])
                };
                lstReturn.Add(oArtista);
            }
            return lstReturn;
        }
    }
}
}