using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VotaYa.Models;

namespace VotaYa.Pages
{
    public class EventosModel : _PageModel
    {
        public async Task<IActionResult> OnGet(string cod_ev)
        {
            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
            {
                return Redirect("./Login");
            }
            ViewData["Cod_ev"] = cod_ev;
            ViewData["MostrarLayout"] = true;
            await GetEventos(Convert.ToInt32(Request.Cookies["user"]),COD_EV: Convert.ToInt32(cod_ev));
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x=> x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            await GetArtistas(Convert.ToInt32(cod_ev));
            await GetShows(Convert.ToInt32(cod_ev));
            return Page();
        }
    }
}
