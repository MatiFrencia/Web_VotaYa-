using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.Utils;

namespace CoopOnlineWeb.MemoryData
{
    public class WebdbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        public MysqlConection oConexion { get; set; }
        public WebdbContext(DbContextOptions options)
            : base(options)
        {
            //CargarUsuarios().Wait();
        }
    }
}
//        public async Task CargarUsuarios(bool forzar = false)
//        {
//            oConexion = new MysqlConection("192.168.100.68","VOTAYA","123321asd.","votaya");
//            if (!Usuarios.Any() || forzar == true)
//            {
//                Usuarios.RemoveRange(Usuarios);
//                foreach (var item in oConexion.GetListResultAsync<Model.UsersModel>("Usuario", new object[1] { "WEB" }, "").Result)
//                {
//                    Usuarios.Add(new CoopOnlineWeb.Model.UsersModel() { Cod_User = item.Cod_User, nombre = item.nombre, apellido = item.apellido, Mail = item.Mail});
//                }
//                SaveChanges();
//            }
//        }
//        public DbSet<Model.UsersModel> Usuarios { get; set; }
//        //   public DbSet<Model.DatosCooperativa> DatosCooperativas { get; set; }

//    }
//}