using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Threading.Tasks;
using System.IO.Compression;
using VotaYa.Util;
using VotaYa.Models;

namespace Services.Utils
{
    public class MysqlConection : IDisposable
    {
        private static string strConnection = "server=192.168.100.68;" +
                                       "uid=VOTAYA;" +
                                       "pwd=123321asd.;" +
                                       " database=votaya;" +
                                       "persistsecurityinfo=True";

        public MySqlConnection oConexionGral;

        /// <summary>
        /// Constructor. Crea la conexion con los parametros correspondientes
        /// </summary>
        /// <param name="Server">Server Variable 808</param>
        /// <param name="User">Usuario Variable 809</param>
        /// <param name="Pass">Password Variable 810</param>
        /// <param name="Base">Base Variable 811</param>
        public MysqlConection()
        {
            CrearConexion();
        }
        public MysqlConection(string Server, string User, string Pass, string Base)
        {
            if (string.IsNullOrEmpty(Server))
                Server = "192.168.100.68";
            strConnection = "server=" + Server + ";uid=" + User + ";pwd=" + Pass + "; database=" + Base + ";persistsecurityinfo=True";
            CrearConexion();
        }
        public MysqlConection(Conexion oConn)
            : this(oConn.GetMysqlServer(), oConn.GetMySQL_User(), oConn.GetMySQL_Password(), oConn.GetMySQL_Database())
        {

        }

        public async Task<bool> ExisteTabla(string Tabla)
        {
            string Query = $@"SELECT * FROM information_schema.tables WHERE table_name = '{Tabla}' LIMIT 1;";
            return await HayRegistros(Query);
        }
        public void CrearConexion()
        {
            oConexionGral = new MySql.Data.MySqlClient.MySqlConnection(strConnection);
            oConexionGral.Open();
        }
        public void CerrarConexion()
        {
            if (oConexionGral != null)
            {
                oConexionGral.Close();
                oConexionGral.Dispose();
                oConexionGral.ClearAllPoolsAsync();
            }
            oConexionGral = null;
        }

        public void ImportarTXT_toMySQL(string campos, string tabla, MySql.Data.MySqlClient.MySqlConnection oConexion, int Coop)
        {
            try
            {
                string query = "LOAD DATA INFILE '/var/lib/mysql-files/script_Sincronizacion.txt' REPLACE INTO TABLE " + tabla + " FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n' "
                    + campos;
                using (MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(query))
                {
                    comando.Connection = oConexion;
                    comando.CommandTimeout = 120;
                    comando.CommandType = CommandType.Text;
                    comando.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, Coop, new string[] { "Error al ejecutar el comando de importación" });
            }
        }

        /// <summary>
        /// Genera una consunlta en my sql
        /// </summary>
        /// <param name="pQuery"></param>
        /// <param name="pCadena"></param>
        /// <returns></returns>
        public DataTable Consulta(String pQuery, MySql.Data.MySqlClient.MySqlConnection oConexion)
        {
            if (oConexion == null)
                return Consulta(pQuery);
            DataSet _ds = new DataSet();
            using (MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand())
            {
                using (MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter())
                {
                    comando.Connection = oConexion;
                    comando.CommandTimeout = 120;
                    comando.CommandType = CommandType.Text;
                    comando.CommandText = pQuery;
                    da.SelectCommand = comando;
                    da.Fill(_ds);

                    if (_ds.Tables.Count != 0)
                        return _ds.Tables[0];
                    return null;
                }
            }
        }
        public async Task<DataTable> ConsultaAsync(String pQuery, MySql.Data.MySqlClient.MySqlConnection oConexion)
        {
            try
            {
                if (oConexion == null)
                    return await ConsultaAsync(pQuery);
                DataSet _ds = new DataSet();
                using (MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand())
                {
                    using (MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter())
                    {
                        comando.Connection = oConexion;
                        comando.CommandTimeout = 120;
                        comando.CommandType = CommandType.Text;
                        comando.CommandText = pQuery;
                        da.SelectCommand = comando;
                        await da.FillAsync(_ds);

                        if (_ds.Tables.Count != 0)
                            return _ds.Tables[0];
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0, new string[2] { strConnection, pQuery });
            }
            return null;
        }
        public DataTable Consulta(String pQuery)
        {
            var Return = Consulta(pQuery, oConexionGral);
            return Return;
        }
        public async Task<DataTable> ConsultaAsync(String pQuery)
        {
            DataTable Return = null;
            Return = await ConsultaAsync(pQuery, oConexionGral);
            return Return;

        }
        /// <summary>
        /// Valida que una consulta no devuelva resultados
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public async Task<bool> HayRegistros(string Query)
        {
            var dt = await ConsultaAsync(Query, this.oConexionGral);
            if (dt == null || dt.Rows.Count == 0)
                return false;
            else
                return true;
        }

        public async Task<bool> ValidarBaseDeDatos()
        {

            var query = @"CREATE TABLE `WEB_colores` (
  `id` int NOT NULL AUTO_INCREMENT,
  `elemento` varchar(45) NOT NULL,
  `color` int NOT NULL DEFAULT '0',
  `hexadecimal` varchar(45) DEFAULT NULL,
  PRIMARY KEY(`id`)
) ENGINE = InnoDB AUTO_INCREMENT = 6 ";
            await EjecutarcomandoAsync(query);
            
            return true;
        }
        private async Task ChequearCampo(string Tabla, string Campo, string Tipo = "INT NULL DEFAULT '0'")
        {
            if (!await ExisteElCampo(Tabla, Campo))
            {
                string Query = "ALTER TABLE `" + Tabla + "` ADD COLUMN `" + Campo + "` " + Tipo + ";";
                await EjecutarcomandoAsync(Query);
            }
        }
        private async Task<bool> ExisteElCampo(string Tabla, string Campo)
        {
            string Query = "SHOW COLUMNS FROM `" + Tabla + "` LIKE '" + Campo + "';";
            return await HayRegistros(Query);
        }

        public event EventHandler<EventArgs> AvanzarEstado;
        public int BulkUpdate<T>(List<T> lstObjetos, PropertyInfo objPkProperty)
        {
            int Result = 0;
            try
            {
                if (lstObjetos != null && lstObjetos.Any())
                {

                    PropertyInfo[] arrProperties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    List<PropertyInfo> lstPropie = new List<PropertyInfo>();
                    string Fields = "";

                    foreach (var item in arrProperties)
                    {
                        if (item.Name != objPkProperty.Name)
                        {
                            object[] objAtributos = item.GetCustomAttributes(true);
                            if (item.CanWrite && objAtributos.Length > 0 && (objAtributos.Any(y => y.GetType().Name == "EdmScalarPropertyAttribute") || objAtributos.Length == 0))
                            {
                                lstPropie.Add(item);
                                if (Fields != "")
                                    Fields += ",";
                                Fields += item.Name;
                            }
                        }
                    }

                    string Start = "UPDATE " + typeof(T).Name + " Set ";
                    List<string> lstString = new List<string>();
                    string values = "";
                    foreach (var item in lstObjetos)
                    {
                        values = "";
                        foreach (var prop in lstPropie)
                        {
                            if (values != "")
                                values += ",";

                            values += prop.Name + "=" + GetValuesToQuery(prop, prop.GetValue(item, null));

                        }
                        values += " Where " + objPkProperty.Name + "=" + GetValuesToQuery(objPkProperty, objPkProperty.GetValue(item, null));
                        lstString.Add(Start + values + ";");
                        if (lstString.Count == 100)
                        {
                            int Cantidad = Ejecutarcomando(lstString);
                            Result += Cantidad;
                            if (AvanzarEstado != null)
                                AvanzarEstado(Cantidad, null);
                            lstString.Clear();
                        }

                    }
                    if (lstString.Count != 0)
                    {
                        int Cantidad = Ejecutarcomando(lstString);
                        Result += Cantidad;
                        lstString.Clear();
                    }

                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0, new string[1] { strConnection });

            }
            return Result;
        }

        public string GetValuesToQuery(PropertyInfo prop, object Value)
        {
            string Ret;
            if (Value == null)
                Ret = "NULL";
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                Ret = "'" + MySqlHelper.EscapeString(((DateTime)Value).ToString("yyyy-MM-dd")) + "'";
            else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                Ret = "'" + MySqlHelper.EscapeString(((decimal)Value).ToString().Replace(",", ".")) + "'";
            else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                Ret = "'" + MySqlHelper.EscapeString(((double)Value).ToString().Replace(",", ".")) + "'";
            else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                if ((bool)Value)
                    Ret = "'1'";
                else
                    Ret = "'0'";
            }
            else
                Ret = "'" + MySqlHelper.EscapeString(Value.ToString()) + "'";
            return Ret;
        }
        public int BulkInsert<T>(List<T> lstObjetos, int CantidadInsert = 100)
        {
            int Result = 0;
            try
            {
                if (lstObjetos != null && lstObjetos.Any())
                {

                    PropertyInfo[] arrProperties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    List<PropertyInfo> lstPropie = new List<PropertyInfo>();
                    string Fields = "";
                    foreach (var item in arrProperties)
                    {
                        object[] objAtributos = item.GetCustomAttributes(true);
                        if (item.CanWrite && objAtributos.Length > 0 && (objAtributos.Any(y => y.GetType().Name == "EdmScalarPropertyAttribute") || objAtributos.Length == 0))
                        {
                            lstPropie.Add(item);
                            if (Fields != "")
                                Fields += ",";
                            Fields += item.Name;
                        }
                    }

                    string Start = "INSERT INTO " + typeof(T).Name + " (" + Fields + ") VALUES ";
                    List<string> lstString = new List<string>();
                    string values = "";
                    foreach (var item in lstObjetos)
                    {
                        values = "";
                        foreach (var prop in lstPropie)
                        {
                            if (values != "")
                                values += ",";

                            values += GetValuesToQuery(prop, prop.GetValue(item, null));

                        }
                        lstString.Add(Start + "(" + values + ");");
                        if (lstString.Count == CantidadInsert)
                        {
                            int Cantidad = Ejecutarcomando(lstString, CantidadInsert);
                            Result += Cantidad;
                            if (AvanzarEstado != null)
                                AvanzarEstado(Cantidad, null);
                            lstString.Clear();
                        }

                    }
                    if (lstString.Count != 0)
                    {
                        int Cantidad = Ejecutarcomando(lstString, CantidadInsert);
                        Result += Cantidad;
                        lstString.Clear();
                    }

                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0, new string[1] { strConnection });


            }
            return Result;
        }
        public int Ejecutarcomando(List<string> lstString, int CantidadInsert = 50)
        {

            int Cantidad = 0;
            while (Cantidad != lstString.Count)
            {
                var aTomar = lstString.Skip(Cantidad).Take(CantidadInsert);
                string Query = aTomar.Aggregate((x, y) => (x + y));

                Cantidad += Ejecutarcomando(Query);
            }

            return Cantidad;
        }
        public int Ejecutarcomando(string String)
        {
            try
            {
                using (MySqlCommand myCmd = new MySqlCommand(String, oConexionGral))
                {
                    myCmd.CommandType = System.Data.CommandType.Text;
                    myCmd.CommandTimeout = 120;
                    return myCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0, new string[1] { strConnection });
            }
            return 0;
        }
        public async Task<int> EjecutarcomandoAsync(string Query)
        {
            if (oConexionGral != null)
                return await EjecutarcomandoAsync(Query, oConexionGral);
            try
            {
                return await EjecutarcomandoAsync(Query, oConexionGral);
            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0, new string[1] { strConnection });
            }
            return 0;
        }
        public async Task<int> EjecutarcomandoAsync(string Query, MySqlConnection mConnection)
        {
            try
            {
                using (MySqlCommand myCmd = new MySqlCommand(Query, mConnection))
                {
                    myCmd.CommandType = System.Data.CommandType.Text;
                    myCmd.CommandTimeout = 120;
                    return await myCmd.ExecuteNonQueryAsync();
                }

            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0, new string[1] { Query });

            }
            return 0;
        }
        public string Queryinsert<T>(T objPresu)
        {
            string Start = "";
            try
            {
                if (objPresu != null)
                {

                    System.Reflection.PropertyInfo[] arrProperties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    List<System.Reflection.PropertyInfo> lstPropie = new List<System.Reflection.PropertyInfo>();
                    string Fields = "";
                    foreach (var item in arrProperties)
                    {
                        object[] objAtributos = item.GetCustomAttributes(true);
                        if (item.CanWrite && objAtributos.Length > 0 && (objAtributos.Any(y => y.GetType().Name == "EdmScalarPropertyAttribute") || objAtributos.Length == 0))
                        {
                            lstPropie.Add(item);
                            if (Fields != "")
                                Fields += ",";
                            Fields += item.Name;
                        }
                    }

                    Start = "INSERT INTO " + typeof(T).Name + " (" + Fields + ") VALUES ";
                    string values = "";

                    foreach (var prop in lstPropie)
                    {
                        if (values != "")
                            values += ",";

                        values += GetValuesToQuery(prop, prop.GetValue(objPresu, null));

                    }

                    Start = Start + "(" + values + " " + ")";
                }


            }
            catch (Exception ex)
            {
                Nucleo.GrabarExcepcion(ex, 0);

            }

            return Start;
        }

        public async Task<int> InsertGetIdentityAsync(string String)
        {
            using (MySqlCommand myCmd = new MySqlCommand(String, oConexionGral))
            {
                myCmd.CommandType = System.Data.CommandType.Text;
                myCmd.CommandTimeout = 120;
                try
                {
                    var respuesta = await myCmd.ExecuteNonQueryAsync();
                    if (respuesta != 0)
                    {
                        using (MySqlDataAdapter sqlDa = new MySqlDataAdapter("SELECT LAST_INSERT_ID();", oConexionGral))
                        {
                            System.Data.DataSet ds = new System.Data.DataSet();
                            await sqlDa.FillAsync(ds);
                            if (ds != null && ds.Tables.Count != 0)
                                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Nucleo.GrabarExcepcion(ex, 0, new string[1] { String });
                }
            }
            return 0;
        }

        public void Dispose()
        {
            CerrarConexion();
        }

    }
}