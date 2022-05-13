using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class _PageModel : PageModel
    {
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
        #endregion
    }
}
