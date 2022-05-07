using System.Collections.Generic;

namespace VotaYa.Models
{
    public class Terna
    {
        private int COD_TER { get; set; }
        private string Nombre { get; set; }
        private string Descripcion { get; set; }

        public List<Terna> GetTerna(int COD_USER)
        {
            return new List<Terna>();
        }
    }
}