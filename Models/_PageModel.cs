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

namespace VotaYa.Models
{
    public class _PageModel : PageModel
    {
        public class ResultResponse
        {
            [JsonPropertyName("Respuesta")]
            public string Respuesta { get; set; }
            [JsonPropertyName("Resultado")]
            public bool Resultado { get; set; }
            [JsonPropertyName("strExtra")]
            public string strExtra { get; set; }
        }

        public List<Artista> oArtistas { get; set; }
        public List<Voto> oVotos { get; set; }
        public List<Usuario> oUsuarios { get; set; }
        public List<Terna> oTernas { get; set; }
        public List<Participacion> oParticipantes { get; set; }
        public List<Participacion> oParticipaciones { get; set; }
        public List<Show> oShows { get; set; }
        public List<Evento> oEventos { get; set; }
        public Participacion oHost { get; set; }


        #region Cargar objetos
        public async Task<bool> GetArtistas(int COD_EV, int COD_ART = 0)
        {
            Artista clsArtista = new Artista();

            oArtistas = await clsArtista.GetArtista(COD_EV, COD_ART);

            if (oArtistas != null)
                return true;
            return false;
        }
        public async Task<bool> GetVotos(int COD_EV, int COD_SHOW = 0)
        {
            Voto clsVotos = new Voto();

            oVotos = await clsVotos.GetVotos(COD_EV, COD_SHOW);

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
            Participacion clsParticipante = new Participacion();

            oParticipantes = await clsParticipante.GetParticipaciones(COD_EV);

            if (oParticipantes != null)
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
