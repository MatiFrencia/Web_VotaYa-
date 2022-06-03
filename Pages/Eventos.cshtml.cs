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
            ViewData["bodyimg"] = true;
            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
            {
                return Redirect("./Login");
            }
            
            await GetEventos(Convert.ToInt32(Request.Cookies["user"]),COD_EV: Convert.ToInt32(cod_ev));
            await GetArtistas(Convert.ToInt32(cod_ev));
            await GetShows(Convert.ToInt32(cod_ev));
            await GetGeneros(Convert.ToInt32(cod_ev));
            ViewData["Shows"] = true;
            ViewData["Eventos"] = false;
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            ViewData["MostrarLayout"] = true;
            ViewData["Cod_ev"] = cod_ev;
            return Page();
        }
        public async Task<JsonResult> OnGetRegistroEvento(string nombre, string descripcion, string FechaInicio)
        {
            ResultResponse result;
            if (string.IsNullOrEmpty(FechaInicio))
                return new JsonResult(new ResultResponse() { Respuesta = "Por favor ingrese Fecha y Hora estimada de inicio", Resultado = false, strExtra = "" });
            if (string.IsNullOrEmpty(nombre))
                return new JsonResult(new ResultResponse() { Respuesta = "Por favor ingrese un nombre del evento", Resultado = false, strExtra = "" });
            if (FechaInicio.Contains("vacio"))
            {
                return new JsonResult(new ResultResponse() { Respuesta = "Por favor seleccione Fecha y Hora estimada de inicio", Resultado = false, strExtra = "" });
            }

            ViewData["Shows"] = true;
            ViewData["Eventos"] = false;
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            ViewData["MostrarLayout"] = true;

            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
                Response.Redirect("./Login");
            bool creado = await RegistrarEvento(nombre, descripcion, FechaInicio, Request.Cookies["user"]);
            if (creado)
            {
                result = new ResultResponse() { Respuesta = "Evento Registrado", Resultado = true, strExtra = "" };
            }
            else
                result = new ResultResponse() { Respuesta = "Hubo un problema al intentar registrar el evento", Resultado = false, strExtra = "" };
            if (!await GetEventos(Convert.ToInt32(Request.Cookies["user"])))
                Response.Redirect("./Login");

            return new JsonResult(result);
        }
    }
}
