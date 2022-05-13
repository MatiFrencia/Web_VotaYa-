using System;
using System.Collections.Generic;

namespace VotaYa.Models
{
    public class Host : Usuario
    {
        public Host CrearHost(int _COD_USER, string _Nombre, string _Mail, string _Pwd, List<Evento> _Eventos)
        {
            Host oHost = new Host()
            {
                COD_USER = _COD_USER,
                Eventos = _Eventos,
                Mail = _Mail,
                Nombre = _Nombre,
                Pwd = _Pwd,
                Host = true
            };
            return oHost;
        }
    }
}