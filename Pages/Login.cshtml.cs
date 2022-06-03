using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VotaYa.Models;
using VotaYa.Util;

namespace VotaYa.Pages
{
    public class LoginModel : _PageModel
    {

        public async Task OnGet()
        {
            ViewData["MostrarLayout"] = false;
        }

        public async Task<JsonResult> OnGetLogin(string email, string pwd)
        {
            ViewData["bodyimg"] = false;
            email = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(email);
            pwd = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(pwd);
            try
            {
                if (string.IsNullOrEmpty(email))
                    throw new Exception("El campo 'Correo electrónico' no puede estar en blanco...");
                else if (string.IsNullOrEmpty(pwd))
                    throw new Exception("El campo 'Contraseña' no puede estar en blanco...");
                pwd = Nucleo.Crypto.Encrypt(pwd, "MaTiCpC2000");

                if (await ValidarUsuario(email, pwd))
                {
                    return new JsonResult(new ResultResponse() { Respuesta = "Inicio de Sesión existoso!", Resultado = true, strExtra = "" });
                }
                else
                {
                    throw new Exception("Hubo un problema al registrarse, por favor intente nuevamente...");
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultResponse() { Respuesta = ex.Message, Resultado = false, strExtra = "" });
            }
        }

        public async Task<JsonResult> OnGetRegistro(string nombre, string pwdConfirm, string newPWD, string email)
        {
            nombre = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(nombre);
            pwdConfirm = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(pwdConfirm);
            newPWD = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(newPWD);
            email = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(email);
            try
            {
                if (string.IsNullOrEmpty(nombre))
                    throw new Exception("El campo 'Nombre' no puede estar en blanco...");
                else if (string.IsNullOrEmpty(newPWD))
                    throw new Exception("El campo 'Contraseña' no puede estar en blanco...");
                else if (string.IsNullOrEmpty(email))
                    throw new Exception("El campo 'Correo electrónico' no puede estar en blanco...");
                else if (!new EmailAddressAttribute().IsValid(email))
                    throw new Exception("El campo 'Correo electrónico' no es válido...");
                else if (newPWD != pwdConfirm)
                    throw new Exception("Las contraseñas no coinciden...");

                newPWD = Nucleo.Crypto.Encrypt(newPWD, "MaTiCpC2000");
                if (await InsertarUsuario(email, newPWD, nombre))
                {
                    if (await ValidarUsuario(email, newPWD))
                    {
                        //if (await EnviarCorreo(email, newPWD))
                            return new JsonResult(new ResultResponse() { Respuesta = "Registro completado!!", Resultado = true, strExtra = "" });
                        //else
                        //    throw new Exception("Hubo un problema al registrarse, por favor intente nuevamente...");
                    }
                    else
                    {
                        throw new Exception("Hubo un problema al registrarse, por favor intente nuevamente...");
                    }
                }
                else
                {
                    throw new Exception("Hubo un problema al registrarse, por favor intente nuevamente...");
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultResponse() { Respuesta = ex.Message, Resultado = false, strExtra = "" });
            }
        }
    }
}
