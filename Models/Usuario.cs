using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Usuario
    {
        protected int COD_USER { get; set; }
        protected string Mail { get; set; }
        protected string Pwd { get; set; }
        protected string Nombre { get; set; }
        protected List<Evento> Eventos { get; set; }
    }
}
