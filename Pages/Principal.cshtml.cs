using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VotaYa.Models;

namespace VotaYa.Pages
{
    public class PrincipalModel : _PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
                return Redirect("./Login");
            if (!await GetEventos(Convert.ToInt32(Request.Cookies["user"])))
                return Redirect("./Login");
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            ViewData["MostrarLayout"] = true;
            return null;
        }
        public async Task<JsonResult> OnGetRegistroEvento(string nombre, string FechaInicio)
        {
            return new JsonResult("asd");
        }
    }
}
