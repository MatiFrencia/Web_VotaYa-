using VotaYa.Datos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text;
using VotaYa.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace VotaYa.Models
{
    public class _PageModel : PageModel
    {
        protected readonly WebdbContext _db;

        public class ResultResponse
        {
            [JsonPropertyName("Respuesta")]
            public string Respuesta { get; set; }
            [JsonPropertyName("Resultado")]
            public bool Resultado { get; set; }
            [JsonPropertyName("strExtra")]
            public string strExtra { get; set; }
        }
        public List<Ganadores> oGanadores { get; set; }
        public List<Artista> oArtistas { get; set; }
        public List<Voto> oVotos { get; set; }
        public List<Usuario> oUsuarios { get; set; }
        public List<Terna> oTernas { get; set; }
        public List<Genero> oGeneros { get; set; }
        public List<Participacion> oParticipantes { get; set; }
        public List<Participacion> oParticipaciones { get; set; }
        public List<Votacion> oVotacion { get; set; }
        public List<Show> oShows { get; set; }
        public List<Evento> oEventos { get; set; }
        public Participacion oHost { get; set; }
        [BindProperty]
        public IFormFile Foto { get; set; }
        [BindProperty]
        public string Alias { get; set; }
        [BindProperty]
        public string NombreArt { get; set; }

        #region Cargar objetos
        public async Task<bool> GetVotacion(int cod_ev)
        {
            Votacion clsVotacion = new Votacion();

            oVotacion = await clsVotacion.GetVotaciones(cod_ev);

            if (oVotacion != null)
                return true;
            return false;
        }
        public async Task<bool> GetGeneros(int cod_ev)
        {
            Genero clsGeneros = new Genero();

            oGeneros = await clsGeneros.GetGeneros(cod_ev);

            if (oGeneros != null)
                return true;
            return false;
        }
        public async Task<bool> GetArtistas(int COD_EV, int COD_ART = 0)
        {
            Artista clsArtista = new Artista();

            oArtistas = await clsArtista.GetArtista(COD_EV, COD_ART);

            if (oArtistas != null)
                return true;
            return false;
        }
        public async Task<bool> GetVotos(int COD_EV)
        {
            Voto clsVotos = new Voto();
            oVotos = await clsVotos.GetVotos(COD_EV);

            if (oVotos != null)
                return true;
            return false;
        }
        public async Task<bool> GetUsuario(int COD_USER, int COD_EV = 0)
        {
            Usuario clsUsuario = new Usuario();

            oUsuarios = await clsUsuario.GetUsuario(COD_USER);

            if (oUsuarios != null)
                return true;
            return false;
        }
        public async Task<bool> GetTernas(int COD_EV)
        {
            Terna clsTerna = new Terna();

            oTernas = await clsTerna.GetTerna(COD_EV);

            if (oTernas != null)
                return true;
            return false;
        }
        public async Task<bool> GetParticipaciones(int COD_USER)
        {
            Participacion clsParticipante = new Participacion();
            if (oUsuarios != null && oUsuarios.Count != 0)
                oParticipaciones = await clsParticipante.GetParticipaciones(COD_USER);
            if (oParticipaciones != null)
                return true;
            return false;
        }
        public async Task<bool> GetParticipantes(int COD_EV)
        {
            Participacion clsParticipante = new Participacion();
            if (oUsuarios != null && oUsuarios.Count != 0)
                oParticipantes = await clsParticipante.GetParticipantes(COD_EV);
            if (oParticipantes != null)
                return true;
            return false;
        }
        public async Task<bool> GetShows(int COD_EV)
        {
            Show clsParticipante = new Show();

            oShows = await clsParticipante.GetShow(COD_EV);

            if (oShows != null)
                return true;
            return false;
        }
        public async Task<bool> RegistrarEvento(string nombre, string descripcion, string fechaInicio, string cod_user)
        {
            Evento clsEvento = new Evento();

            bool creado = await clsEvento.RegistrarEvento(nombre, descripcion, fechaInicio, cod_user);

            return creado;
        }
        public async Task<bool> RegistrarTematica(string nombre, string descripcion, string cod_ev)
        {
            Genero clsGenero = new Genero();

            bool creado = await clsGenero.RegistrarGenero(nombre, descripcion, cod_ev);

            return creado;
        }
        public async Task<bool> IngresarEvento(string codigo, string cod_user)
        {
            Evento clsEvento = new Evento();

            bool creado = await clsEvento.IngresarEvento(codigo, cod_user);

            return creado;
        }

        public async Task<int> RegistrarParticipacion(string cod_ev, string cod_user, bool host)
        {
            Participacion clsEvento = new Participacion();

            int creado = await clsEvento.RegistrarParticipacion(cod_ev, cod_user, host);

            return creado;
        }

        public async Task<bool> GetEventos(int COD_USER, int COD_EV = 0)
        {
            Evento clsEvento = new Evento();

            oEventos = await clsEvento.GetEventos(COD_USER);

            if (oEventos != null)
                return true;
            return false;
        }
        public async Task<bool> GetHost(int COD_EV)
        {
            Participacion clsParticipacion = new Participacion();
            oHost = await clsParticipacion.GetHost(COD_EV);
            if (oHost != null)
                return true;
            return false;
        }
        public async Task<bool> SetShow(string cod_show, string funcion)
        {
            Show clsShow = new Show();
            bool correcto = await clsShow.SetEstado(cod_show, funcion);
            return correcto;
        }



        public async Task<bool> ValidarUsuario(string email, string pwd)
        {
            Usuario clsUsuario = new Usuario();
            int cod_user = await clsUsuario.ValidarUsuario(email, pwd);
            if (cod_user != 0)
            {
                Response.Cookies.Append("pwd", pwd);
                Response.Cookies.Append("user", cod_user.ToString());
                return true;
            }
            return false;
        }
        public async Task<bool> InsertarUsuario(string email, string pwd, string nombre)
        {
            MysqlConection oConn = new MysqlConection();
            var query = $"SELECT email FROM usuarios WHERE email ='{email}'";
            if (await oConn.HayRegistros(query))
                throw new Exception("Ya existe una cuenta registrada con ese correo electrónico...");

            query = $"INSERT INTO usuarios (email, clave, nombre) VALUES ('{email}','{pwd}','{nombre}')";
            try
            {
                int user = await oConn.EjecutarcomandoAsync(query);
                if (user != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> RegistrarArtista(string cod_ev)
        {
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    using (var ms = new MemoryStream())
                    {
                        Foto.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        // act on the Base64 data

                        Artista oArtista = new Artista()
                        {
                            Nombre = NombreArt,
                            Alias = Alias,
                            Foto = new Artista.Imagen() { Base64 = s }
                        };
                        oConexion.Consulta($"INSERT INTO artistas (nombre,cod_ev,alias,foto) VALUES ('{oArtista.Nombre}', '{cod_ev}', '{oArtista.Alias}','{oArtista.Foto.Base64}')");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(NombreArt), new string[] { $"Error al registrar al artista {NombreArt}, {Alias}" });
                return false;
            }
        }
        public async Task<bool> RegistrarShow(string nombre, string cod_art, string cod_tem, string cod_ev)
        {
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    Show oShow = new Show()
                    {
                        cod_ev = Convert.ToInt32(cod_ev),
                        cod_art = Convert.ToInt32(cod_art),
                        cod_gen = Convert.ToInt32(cod_tem),
                        Descripcion = nombre,
                        Estado = Show.estadoShow.Proximamente
                    };
                    oConexion.Consulta($"INSERT INTO shows (descripcion, estado, cod_ev, cod_art, cod_gen) VALUES ('{oShow.Descripcion}','{(int)oShow.Estado}','{oShow.cod_ev}','{cod_art}','{cod_tem}')");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(cod_ev), new string[] { $"Error al registrar al show {nombre}, {cod_ev}" });
                return false;
            }
        }
        public async Task<bool> RegistrarTerna(string nombre, string descripcion, string cod_ev)
        {
            Terna clsGenero = new Terna();

            bool creado = await clsGenero.RegistrarTerna(nombre, descripcion, cod_ev);

            return creado;
        }
        public async Task<bool> IniciarVotacion(string cod_ev)
        {
            Votacion clsVotacion = new Votacion();

            bool iniciada = await clsVotacion.IniciarVotacion(cod_ev);

            return iniciada;
        }
        public async Task<bool> GetGanadores(string cod_ev)
        {
            List<Ganadores> rtrn = new List<Ganadores>();
            try
            {
                var ultimaVot = oVotacion.OrderBy(x => x.Fecha_inicio).FirstOrDefault(x => x.Estado == Votacion.estado.Cerrada && x.Cod_ev == Convert.ToInt32(cod_ev) && x.Fecha_cierre != null);
                if (ultimaVot != null)
                {
                    foreach (var terna in oTernas)
                    {
                        using (var oConexion = new MysqlConection())
                        {
                            var query = $"SELECT MAX(cantidad) as cant, cod_art FROM (SELECT count(*) as cantidad,cod_art FROM votaya.votos where cod_ter = {terna.COD_TER} group by cod_art)as T group by cod_art ";
                            DataTable DT = await oConexion.ConsultaAsync(query);
                            foreach (DataRow dr in DT.Rows)
                            {
                                Ganadores gan = new Ganadores()
                                {
                                    Cod_ter = terna.COD_TER,
                                    Cod_art = Convert.ToInt32(dr["cod_art"]),
                                    Cant_votos = Convert.ToInt32(dr["cant"])
                                };
                                rtrn.Add(gan);
                            }
                        }
                    }
                }
                oGanadores = rtrn;
                return true;
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Convert.ToInt32(cod_ev), new string[] { "Error al obtener ganadores" });
                return false;
            }
        }
        public async Task<bool> CerrarVotacion(string cod_ev)
        {
            Votacion clsVotacion = new Votacion();

            bool iniciada = await clsVotacion.CerrarVotacion(cod_ev);
            await GetVotacion(Convert.ToInt32(cod_ev));
            return iniciada;
        }
        public async Task<bool> RegistrarVoto(string votos, string cod_ev)
        {
            Voto clsVoto = new Voto();
            Participacion clsPart = new Participacion();
            var participante = await clsPart.GetParticipante(Convert.ToInt32(cod_ev), Convert.ToInt32(Request.Cookies["user"]));
            Votacion clsVotac = new Votacion();
            var votacion = await clsVotac.GetVotacion(Convert.ToInt32(cod_ev));
            if (oVotos.Where(x => x.Cod_votac == votacion && x.Cod_part == participante.Cod_par).Any())
                return false;

            bool registrados = await clsVoto.RegistrarVoto(votos, cod_ev, participante.Cod_par.ToString(), votacion.ToString());

            return registrados;
        }

        public async Task<bool> EnviarCorreo(string email, string newPWD)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("votayateam@gmail.com", "TEAM VotaYa", System.Text.Encoding.UTF8);//Correo de salida
                correo.To.Add(email); //Correo destino?
                correo.Subject = "Correo de prueba"; //Asunto
                correo.Body = "Este es un correo de prueba desde c#"; //Mensaje del correo
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com"; //Host del servidor de correo
                smtp.Port = 25; //Puerto de salida
                smtp.Credentials = new System.Net.NetworkCredential("votayateam@gmail.com", "123321VOTAYATEAM");//Cuenta de correo
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = true;//True si el servidor de correo permite ssl
                smtp.Send(correo);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
