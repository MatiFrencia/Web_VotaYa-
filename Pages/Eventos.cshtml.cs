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
            ViewData["Cod_ev"] = cod_ev;
            Response.Cookies.Append("cod_ev", cod_ev);
            ViewData["bodyimg"] = true;
            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
            {
                return Redirect("./Login");
            }
            
            await GetEventos(Convert.ToInt32(Request.Cookies["user"]),COD_EV: Convert.ToInt32(ViewData["Cod_ev"]));
            await GetArtistas(Convert.ToInt32(ViewData["Cod_ev"]));
            await GetShows(Convert.ToInt32(ViewData["Cod_ev"]));
            await GetGeneros(Convert.ToInt32(ViewData["Cod_ev"]));
            await GetParticipantes(Convert.ToInt32(ViewData["Cod_ev"]));
            ViewData["Host"] = oParticipantes.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Host;
            ViewData["Shows"] = true;
            ViewData["Eventos"] = false;
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            ViewData["MostrarLayout"] = true;
           
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
        public async Task<JsonResult> OnGetAgregarTematica(string nombre, string descripcion)
        {
            ResultResponse result;
            if (string.IsNullOrEmpty(nombre))
                return new JsonResult(new ResultResponse() { Respuesta = "Por favor ingrese un nombre.", Resultado = false, strExtra = "" });
            if (string.IsNullOrEmpty(descripcion))
                return new JsonResult(new ResultResponse() { Respuesta = "Por favor ingrese una descripcion.", Resultado = false, strExtra = "" });

            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
                Response.Redirect("./Login");
            bool creado = await RegistrarTematica(nombre, descripcion, Request.Cookies["cod_ev"].ToString());
            if (creado)
            {
                result = new ResultResponse() { Respuesta = "Tematica Registrada", Resultado = true, strExtra = "" };
            }
            else
                result = new ResultResponse() { Respuesta = "Hubo un problema al intentar registrar la tematica", Resultado = false, strExtra = "" };

            return new JsonResult(result);
        }
        public async Task<IActionResult> OnPostAgregarArtista()
        {
            ResultResponse result;
            //if (string.IsNullOrEmpty(Alias))
            //    return new JsonResult(new ResultResponse() { Respuesta = "Por favor ingrese un alias.", Resultado = false, strExtra = "" });
            //if (string.IsNullOrEmpty(NombreArt))
            //    return new JsonResult(new ResultResponse() { Respuesta = "Por favor ingrese un nombre.", Resultado = false, strExtra = "" });
           

            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
                Response.Redirect("./Login");
            ViewData["Shows"] = false;
            ViewData["Eventos"] = true;
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            ViewData["MostrarLayout"] = true;
            bool creado = await RegistrarArtista();
            if (creado)
            {
                result = new ResultResponse() { Respuesta = "Artista Registrado.", Resultado = true, strExtra = "" };
            }
            else
                result = new ResultResponse() { Respuesta = "Hubo un problema al intentar registrar el artista.", Resultado = false, strExtra = "" };

            return Redirect($"./Eventos?cod_ev={ViewData["Cod_ev"]}");
        }
    }
}
