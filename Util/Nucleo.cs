using Services.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VotaYa.Util
{
    public static class Nucleo
    {
        public static string PathUtiles = Directory.GetCurrentDirectory();
        public static string GetCarpetaSistema()
        {
            if (System.Reflection.Assembly.GetEntryAssembly() != null)
                return Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).FullName;
            else
                return AppDomain.CurrentDomain.BaseDirectory;
        }
        public static bool Guardar_Archivo(string path, string content)
        {
            StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(path);
                sw.Write(content.Trim(' '));

                return true;
            }
            catch (Exception ex)
            {
                GrabarExcepcion(ex, 0, new string[] { "Error al guardar Archivo" });
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
        public static string stringBetween(string Source, string Start, string End)
        {
            string result = "";
            if (Source.Contains(Start) && Source.Contains(End))
            {
                int StartIndex = Source.IndexOf(Start, 0) + Start.Length;
                int EndIndex = Source.IndexOf(End, StartIndex);
                result = Source.Substring(StartIndex, EndIndex - StartIndex);
                return result;
            }

            return result;
        }
        public static void GrabarExcepcion(Exception pEx, int Coop, string[] strAddInfo = null)
        {
            // string ret = "";
            try
            {


                string FileName = "LogProcoop";
                string Path = GetPathUtiles("");
                var ret = System.IO.Path.Combine(Path, FileName + ".txt");
                System.IO.FileStream fs = new System.IO.FileStream(ret, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                string strError = "";
                strError = "=========================================================================================================================================" + (char)13 + (char)10;
                strError += "    DateTime: " + DateTime.Now + (char)13 + (char)10;
                strError += "-----------------------------------------------------------------------------------------------------------------------------------------" + (char)13 + (char)10;
                strError += "       Error: " + pEx.Message + (char)13 + (char)10;
                strError += "-----------------------------------------------------------------------------------------------------------------------------------------" + (char)13 + (char)10;
                strError += pEx.Source + (char)13 + (char)10;
                strError += pEx.StackTrace + (char)13 + (char)10;
                strError += pEx.HelpLink + (char)13 + (char)10;
                strError += "** COOPERATIVA: " + Coop.ToString();
                if (strAddInfo != null && strAddInfo.Length != 0)
                {
                    strError += "**INFORMACION EXTRA**" + (char)13 + (char)10; ;
                    foreach (var item in strAddInfo)
                    {
                        strError += "   " + item + (char)13 + (char)10;
                    }
                }
                strError += "=========================================================================================================================================" + (char)13 + (char)10;
                sw.Write(strError);
                sw.Close();
                fs.Close();
                //  ret += " - Guardado OK";
            }
            catch (Exception ex)
            {
                //return ex.Message;
            }
            // return ret;
        }

        public static T DeserializeJSON<T>(string json)
        {
            T concreteObject = Activator.CreateInstance<T>();
            var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(concreteObject.GetType());

            concreteObject = (T)serializer.ReadObject(memoryStream);
            memoryStream.Close();
            memoryStream.Dispose();

            return concreteObject;
        }

        public static async Task RegistraLog(string Funcion, int? CodCoop, string PArametros)
        {
            try
            {
                using (var oConexion = GetConexionDispose())
                {
                    string strQuery = string.Format("INSERT INTO `AUDITORIA` (`FECHA`,`FUNCION`,`COD_COOP`,`PARAMETROS`)VALUES(now(),'{0}',{1},'{2}')", (string)Funcion.Replace("'", ""), CodCoop ?? 0, (string)PArametros.Replace("'", ""));
                    await oConexion.EjecutarcomandoAsync(strQuery);
                }
            }
            catch (Exception exception1)
            {
                GrabarExcepcion(exception1, CodCoop ?? 0);
            }
        }
        public static async Task RegistraLogMail(int CodCoop, string Destino, string Asunto, bool Enviado)
        {
            try
            {
                using (var oConexion = GetConexionDispose())
                {
                    string strQuery = string.Format("INSERT INTO `AUDITORIA_MAILS` (`COD_COOP`,`DESTINO`,`ASUNTO`,`FEC_ENVIO`,`FEC_REG`)VALUES({0},'{1}','{2}',{3},{4})", CodCoop, (string)Destino.Replace("'", "´"), (string)Asunto.Replace("'", "´"), Enviado ? "now()" : "null", "now()");
                    await oConexion.EjecutarcomandoAsync(strQuery);
                }
            }
            catch (Exception exception1)
            {
                GrabarExcepcion(exception1, CodCoop);
            }

        }
        public static void Auditoria(string ContextPath)
        {

            new System.Threading.Tasks.Task(delegate
            {
                string str;
                int? nullable1;
                string pArametros = "";
                int result = 0;
                try
                {
                    string[] strArray = (from x in ContextPath.Split("/", (StringSplitOptions)StringSplitOptions.None) select x).ToArray<string>();
                    str = strArray[0] + "/";
                    if (strArray.Length > 1)
                    {
                        str = str + strArray[1];
                    }
                    if ((strArray.Length > 2) && (!int.TryParse(strArray[2], out result) && (strArray.Length > 3)))
                    {
                        int.TryParse(strArray[3], out result);
                    }
                    if (strArray.Length > 2)
                    {
                        pArametros = ContextPath.Replace("/" + str + "/", "");
                    }
                }
                catch (Exception)
                {
                    str = "Error;";
                    pArametros = ContextPath;
                }
                if (result != 0)
                {
                    nullable1 = new int?(result);
                }
                else
                {
                    nullable1 = null;
                }
                RegistraLog(str, nullable1, pArametros);
            }).Start();

        }

        public static string GetPathUtiles(string subFolder)
        {
            return System.IO.Path.Combine(PathUtiles, subFolder);
        }
        public enum TipeEncript
        {
            ValidUser = 1,
            ResetPassword = 2
        }
        public static string GetEncryptedKey(int CodCoop, int CodUser, string Mail, long Tick, TipeEncript Tipo)
        {
            int Div1 = Tipo == TipeEncript.ValidUser ? CodCoop : (CodUser * 2);
            if (Div1 == 0)
                Div1 = 3;
            var resultado = Convert.ToInt64(Tick / Div1);
            int Div2 = Tipo == TipeEncript.ValidUser ? (CodUser * Mail.Length) : (CodUser / Tick.ToString().Length * Mail.Length);
            if (Div2 == 0)
                Div2 = 3;
            resultado = Convert.ToInt64(resultado / Div2);

            var strReturn = resultado.ToString();
            int Length = strReturn.Length;
            if (strReturn.Length >= 6)
            {
                if (Tipo == TipeEncript.ValidUser)
                    strReturn = strReturn.Substring(0, 1) + strReturn.Substring(3, 1) + strReturn.Substring(1, 1) + strReturn.Substring(4, 1) + strReturn.Substring(5, 1) + strReturn.Substring(2, 1);
                else
                    strReturn = strReturn.Substring(3, 1) + strReturn.Substring(4, 1) + strReturn.Substring(2, 1) + strReturn.Substring(5, 1) + strReturn.Substring(0, 1) + strReturn.Substring(1, 1);

                if (Length > 6)
                    strReturn = strReturn + resultado.ToString().Substring(6, Length - 6);
            }

            return strReturn;

        }
        public static string Decrypt(this string encryptedText, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key must have valid value.", nameof(key));
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentException("The encrypted text must have valid value.", nameof(encryptedText));

            var combined = Convert.FromBase64String(encryptedText);
            var buffer = new byte[combined.Length];
            var hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new ArgumentException("Parameter must not be null.", nameof(aes));

                aes.Key = aesKey;

                var iv = new byte[aes.IV.Length];
                var ciphertext = new byte[buffer.Length - iv.Length];

                Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
                Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(ciphertext))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    return Encoding.UTF8.GetString(resultStream.ToArray());
                }
            }
        }

        static internal class Crypto
        {
            // Define the secret salt value for encrypting data
            public static readonly byte[] salt = Encoding.ASCII.GetBytes("Mi#App#SeCuRiTy");

            /// <summary>
            /// Takes the given text string and encrypts it using the given password.
            /// </summary>
            /// <param name="textToEncrypt">Text to encrypt.</param>
            /// <param name="encryptionPassword">Encryption password.</param>
            internal static string Encrypt(string textToEncrypt, string encryptionPassword)
            {
                var algorithm = GetAlgorithm(encryptionPassword);

                //Anything to process?
                if (textToEncrypt == null || textToEncrypt == "") return "";

                byte[] encryptedBytes;
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
                {
                    byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);
                    encryptedBytes = InMemoryCrypt(bytesToEncrypt, encryptor);
                }
                return Convert.ToBase64String(encryptedBytes);
            }

            ///// <summary>
            ///// Takes the given encrypted text string and decrypts it using the given password
            ///// </summary>
            ///// <param name="encryptedText">Encrypted text.</param>
            ///// <param name="encryptionPassword">Encryption password.</param>
            //internal static string Decrypt(string encryptedText, string encryptionPassword)
            //{
            //    var algorithm = GetAlgorithm(encryptionPassword);

            //    //Anything to process?
            //    if (encryptedText == null || encryptedText == "") return "";

            //    byte[] descryptedBytes;
            //    using (ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            //    {
            //        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            //        descryptedBytes = InMemoryCrypt(encryptedBytes, decryptor);
            //    }
            //    return Encoding.UTF8.GetString(descryptedBytes);
            //}

            /// <summary>
            /// Performs an in-memory encrypt/decrypt transformation on a byte array.
            /// </summary>
            /// <returns>The memory crypt.</returns>
            /// <param name="data">Data.</param>
            /// <param name="transform">Transform.</param>
            public static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
            {
                MemoryStream memory = new MemoryStream();
                using (Stream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
                {
                    stream.Write(data, 0, data.Length);
                }
                return memory.ToArray();
            }

            /// <summary>
            /// Defines a RijndaelManaged algorithm and sets its key and Initialization Vector (IV) 
            /// values based on the encryptionPassword received.
            /// </summary>
            /// <returns>The algorithm.</returns>
            /// <param name="encryptionPassword">Encryption password.</param>
            public static RijndaelManaged GetAlgorithm(string encryptionPassword)
            {
                // Create an encryption key from the encryptionPassword and salt.
                var key = new Rfc2898DeriveBytes(encryptionPassword, salt);

                // Declare that we are going to use the Rijndael algorithm with the key that we've just got.
                var algorithm = new RijndaelManaged();
                int bytesForKey = algorithm.KeySize / 8;
                int bytesForIV = algorithm.BlockSize / 8;
                algorithm.Key = key.GetBytes(bytesForKey);
                algorithm.IV = key.GetBytes(bytesForIV);
                return algorithm;
            }

        }

        static internal class RandomNumberGenerator
        {
            // Generate a random number between two numbers    
            public static int RandomNumber(int min, int max)
            {
                Random random = new Random();
                return random.Next(min, max);
            }

            // Generate a random string with a given size and case.   
            // If second parameter is true, the return string is lowercase  
            public static string RandomString(int size, bool lowerCase)
            {
                StringBuilder builder = new StringBuilder();
                Random random = new Random();
                char ch;
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                if (lowerCase)
                    return builder.ToString().ToLower();
                return builder.ToString();
            }

            // Generate a random password 
            public static string RandomPassword()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(RandomString(2, true));
                builder.Append(RandomNumber(1000, 9999));
                builder.Append(RandomString(2, false));
                return builder.ToString();
            }
        }

        public static T Read<T>(string ruta, string Filename, bool Desencripta)
        {
            //  TVSettings settings = new MySettings(string path);
            System.IO.StreamReader stReader = null;
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(T));
                T objResult;
                string Path = ruta + "\\" + Filename;
                if (Desencripta)
                {

                    stReader = System.IO.File.OpenText(Path);
                    string Read = stReader.ReadToEnd();
                    string text = Desencriptar(Read);
                    var stream = new StringReader(text);
                    stReader.Close();
                    objResult = (T)x.Deserialize(stream);
                }
                else
                {
                    StreamReader reader = new StreamReader(Path);
                    objResult = (T)x.Deserialize(reader);

                }
                return objResult;
            }
            catch (Exception ex)
            {
                //GrabarExcepcion(ex);
                //MessageBox.Show("Error: " + ex.ToString());
                if (stReader != null)
                    stReader.Close();
                return default(T);
            }
        }

        public static String Desencriptar(string pPass)
        {
            String CLA_DES = "";
            int cant;
            Int32 valor;
            string caracter = "";
            for (cant = 0; cant < pPass.Length; cant++)
            {
                //1252 es la página de código que usa el vfp para encriptar las claves
                //debo utilizar la misma página de código para poder encriptar las claves
                //y que coincidan con las de la base de dato.
                //var aa =Encoding.GetEncodings();
                Encoding encoding = Encoding.GetEncoding(1200);
                byte[] ascii = encoding.GetBytes(pPass.Substring(cant, 1));
                if (ascii[0] >= 80 && ascii[0] <= 96)
                {
                    valor = ascii[0] + 160;
                    ascii[0] = (byte)valor;
                    caracter = encoding.GetString(ascii);
                    CLA_DES = CLA_DES + caracter;
                }
                else if (ascii[0] > 100)
                {
                    valor = ascii[0] - 16;
                    ascii[0] = (byte)valor;
                    caracter = encoding.GetString(ascii);
                    CLA_DES = CLA_DES + caracter;
                }
                else
                {
                    valor = ascii[0] + 30;
                    ascii[0] = (byte)valor;
                    caracter = encoding.GetString(ascii);
                    CLA_DES = CLA_DES + caracter;
                }


            }
            return CLA_DES;

        }
        public static MysqlConection GetConexionDispose()
        {
            return new MysqlConection("66.97.32.150", "procoop", "Prc0698!", "CoopOnline");
        }
    }
}
