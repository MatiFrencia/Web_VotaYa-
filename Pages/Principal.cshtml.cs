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
            ViewData["bodyimg"] = true;
            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
                return Redirect("./Login");
            if (!await GetEventos(Convert.ToInt32(Request.Cookies["user"])))
                return Redirect("./Login");
            ViewData["Usuario"] = oUsuarios.Any() ? oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre : "";
            ViewData["MostrarLayout"] = true;
            ViewData["Eventos"] = true;
            var ev = oEventos;
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

            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
                Response.Redirect("./Login");
            ViewData["Shows"] = false;
            ViewData["Eventos"] = true;
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            ViewData["MostrarLayout"] = true;
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
        public async Task<JsonResult> OnGetIngresarEvento(string Codigo)
        {
            ResultResponse result;
            if (string.IsNullOrEmpty(Codigo))
                return new JsonResult(new ResultResponse() { Respuesta = "Por favor ingrese un código de evento, este mismo puede pedirselo al creador del evento", Resultado = false, strExtra = "" });
            if (!await GetUsuario(Convert.ToInt32(Request.Cookies["user"])))
                Response.Redirect("./Login");
            ViewData["Shows"] = false;
            ViewData["Eventos"] = true;
            ViewData["Usuario"] = oUsuarios.FirstOrDefault(x => x.Cod_user == Convert.ToInt32(Request.Cookies["user"])).Nombre;
            ViewData["MostrarLayout"] = true;
            bool CodigoCorrecto = await IngresarEvento(Codigo, Request.Cookies["user"]);
            if (CodigoCorrecto)
            {
                result = new ResultResponse() { Respuesta = "Nuevo evento agregado.", Resultado = true, strExtra = "" };
            }
            else
                result = new ResultResponse() { Respuesta = "No se encontró un evento con el código ingresado", Resultado = false, strExtra = "" };
            if (!await GetEventos(Convert.ToInt32(Request.Cookies["user"])))
                Response.Redirect("./Login");

            return new JsonResult(result);
        }
    }
}
