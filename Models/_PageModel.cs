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
        public List<Participante> oParticipantes { get; set; }
        public List<Show> oShows { get; set; }
        public List<Evento> oEventos { get; set; }
        public List<Host> oHosts { get; set; }


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

            oVotos = await clsVotos.GetVoto(COD_EV, COD_SHOW);

            if (oVotos != null)
                return true;
            return false;
        }
        public async Task<bool> GetUsuarios(int COD_EV, int COD_USER = 0)
        {
            Usuario clsUsuario = new Usuario();

            oUsuarios = await clsUsuario.GetUsuario(COD_EV, COD_USER);

            if (oUsuarios != null)
                return true;
            return false;
        }
        public async Task<bool> GetTernas(int COD_EV)
        {
            Participante clsParticipante = new Participante();

            oParticipantes = await clsParticipante.GetParticipante(COD_EV);

            if (oParticipantes != null)
                return true;
            return false;
        }
        public bool GetParticipantes(int COD_EV, int COD_PART)
        {
            Participante clsParticipante = new Participante();
            if (oUsuarios != null && oUsuarios.Count != 0)
                oParticipantes = oUsuarios.Where(x => !x.Host && x.Eventos.FirstOrDefault(z => z.COD_EV == COD_EV) != null)
                    .Select(x => clsParticipante.CrearParticipante(
                   x.COD_USER,
                   x.Nombre,
                   x.Mail,
                   x.Pwd,
                   x.Eventos
                   )).ToList();
            if (oParticipantes != null)
                return true;
            return false;
        }
        public async Task<bool> GetShows(int COD_EV)
        {
            Participante clsParticipante = new Participante();

            oParticipantes = await clsParticipante.GetParticipante(COD_EV);

            if (oParticipantes != null)
                return true;
            return false;
        }
        public async Task<bool> GetEventos(int COD_EV)
        {
            Participante clsParticipante = new Participante();

            oParticipantes = await clsParticipante.GetParticipante(COD_EV);

            if (oParticipantes != null)
                return true;
            return false;
        }
        public bool GetHosts(int COD_EV)
        {
            Host clsHost = new Host();
            if (oUsuarios != null && oUsuarios.Count != 0)
                oHosts = oUsuarios.Where(x => x.Host && x.Eventos.FirstOrDefault(z => z.COD_EV == COD_EV) != null)
                    .Select(x => clsHost.CrearHost(
                   x.COD_USER,
                   x.Nombre,
                   x.Mail,
                   x.Pwd,
                   x.Eventos
                   )).ToList();
            if (oHosts != null)
                return true;
            return false;
        }



        public async Task<bool> ValidarUsuario(string email, string pwd)
        {
            MysqlConection oConn = new MysqlConection();
            var query = $"SELECT cod_user,clave FROM usuarios where email = '{email}'";

            DataTable user = await oConn.ConsultaAsync(query);
            if (user != null && user.Rows.Count != 0)
            {
                bool pwdCorrecta = user.Rows[0]["clave"].ToString() == pwd;
                if (pwdCorrecta)
                {
                    Response.Cookies.Append("pwd", pwd);
                    Response.Cookies.Append("user", user.Rows[0]["cod_user"].ToString());
                    return true;
                }
                else
                    throw new Exception("Contraseña incorrecta...");
            }
            else
            {
                throw new Exception("Correo electrónico no registrado...");
            }
        }
        public async Task<bool> InsertarUsuario(string email, string pwd, string nombre)
        {
            MysqlConection oConn = new MysqlConection();
            var query = $"SELECT email FROM usuarios WHERE email ='{email}'";
            if (await oConn.HayRegistros(query))
                throw new Exception("Ya existe una cuenta registrada con ese correo electrónico...");

            query = $"INSERT INTO usuarios (email, clave, nombre, host) VALUES ('{email}','{pwd}','{nombre}','0')";
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
