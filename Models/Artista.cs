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
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public Imagen Foto { get; set; }
        public class Imagen
        {
            public string Url { get; set; }
            public byte[] bytes { get; set; }
        }
        public static Imagen ByteToImage(byte[] array)
        {
            Imagen img = new Imagen();
            img.bytes = array;
            img.Url = "data:image;base64," + Convert.ToBase64String(array);
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
                    query = string.Format(@$"SELECT Art.cod_art , Art.nombre, Art.alias, Art.foto
                                                FROM artistas Art
                                                JOIN shows Sho ON Sho.cod_art = Art.cod_art 
                                                JOIN eventos Ev ON Ev.cod_ev = Sho.cod_ev
                                                WHERE Art.cod_art = { COD_ART }");
                else    //Seleccionar TODOS los Artistas del EVENTO
                    query = string.Format(@$"SELECT Art.cod_art , Art.nombre, Art.alias, Art.foto
                                                FROM artistas Art
                                                JOIN shows Sho ON Sho.cod_art = Art.cod_art
                                                JOIN eventos Ev ON Ev.cod_ev = Sho.cod_ev
                                                WHERE Ev.cod_ev = { COD_EV }");
                DataTable dtArt = await oConexion.ConsultaAsync(query);

                foreach (DataRow dr in dtArt.Rows)
                {
                    Artista oArtista = new Artista()
                    {
                        COD_ART = (int)dr["cod_art"],
                        Nombre = (string)dr["nombre"],
                        Alias = (string)dr["alias"],
                        Foto = dr["foto"] is DBNull? null : ByteToImage((byte[])dr["foto"])
                };
                lstReturn.Add(oArtista);
            }
            return lstReturn;
        }
    }
}
}