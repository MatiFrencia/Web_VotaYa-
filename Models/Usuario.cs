using Services.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VotaYa.Util;

namespace VotaYa.Models
{
    public class Usuario
    {
        public int Cod_user { get; set; }
        public string Mail { get; set; }
        public string Pwd { get; set; }
        public string Nombre { get; set; }


        public async Task<int> ValidarUsuario(string email, string pwd)
        {
            MysqlConection oConn = new MysqlConection();
            var query = $"SELECT cod_user,clave FROM usuarios where email = '{email}'";

            DataTable user = await oConn.ConsultaAsync(query);
            if (user != null && user.Rows.Count != 0)
            {
                bool pwdCorrecta = user.Rows[0]["clave"].ToString() == pwd;
                if (pwdCorrecta)
                {
                    return Convert.ToInt32(user.Rows[0]["cod_user"]);
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

        public Usuario CrearUsuario(DataRow dr)
        {
            Usuario oParticipacion = new Usuario()
            {
                Cod_user = (int)dr["cod_user"],
                Mail = (string)dr["email"],
                Pwd = (string)dr["clave"],
                Nombre = (string)dr["nombre"]
            };
            return oParticipacion;
        }

        public async Task<List<Usuario>> GetUsuarios(int COD_EV)
        {
            List<Usuario> lstReturn = new List<Usuario>();
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = "";
                    query = string.Format($"Select * from usuarios where cod_ev = {COD_EV}");
                    DataTable dtPart = await oConexion.ConsultaAsync(query);

                    foreach (DataRow dr in dtPart.Rows)
                    {
                        Usuario oUser = CrearUsuario(dr);
                        lstReturn.Add(oUser);
                    }
                    return lstReturn;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, COD_EV, new string[] { "error al consultar usuarios" });
                return lstReturn;
            }
        }

        public async Task<List<Usuario>> GetUsuario(int COD_USER)
        {
            List<Usuario> lstReturn = new List<Usuario>();
            try
            {
                using (var oConexion = new MysqlConection())
                {
                    string query = "";
                    query = string.Format($"Select * from usuarios where cod_user = {COD_USER}");
                    DataTable dtPart = await oConexion.ConsultaAsync(query);

                    foreach (DataRow dr in dtPart.Rows)
                    {
                        Usuario oUser = CrearUsuario(dr);
                        lstReturn.Add(oUser);
                    }
                    return lstReturn;
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, COD_USER, new string[] { "error al consultar usuario" });
                return lstReturn;
            }
        }
    }
}
