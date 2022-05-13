using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotaYa.Models
{
    public class Conexion
    {
        public string MySQL_Server { get; set; }
        public string MySQL_User { get; set; }
        public string MySQL_Password { get; set; }
        public string MySQL_Database { get; set; }


        public string GetMysqlServer()
        {
            return MySQL_Server;
        }
        public string GetMySQL_User()
        {
            return MySQL_User;
        }
        public string GetMySQL_Password()
        {
            return MySQL_Password;
        }
        public string GetMySQL_Database()
        {
            return MySQL_Database;
        }

        public void SetMysqlServer(string server)
        {
            MySQL_Server = server;
        }
        public void SetMySQL_User(string user)
        {
            MySQL_User = user;
        }
        public void SetMySQL_Password(string password)
        {
            MySQL_Password = password;
        }
        public void SetMySQL_Database(string database)
        {
            MySQL_Database = database;
        }
    }
}
