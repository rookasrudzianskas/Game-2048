using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;

namespace ChatServer
{
    class CryptoClass
    {

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        #region HASH ALGORITHM
        public static string MD5String(string InText)
        {
            //Convert string to byte array
            byte[] InBytes = Encoding.UTF8.GetBytes(InText);
            //create MD5 coding object
            var md5 = new MD5CryptoServiceProvider();
            //encode byte array
            byte[] outBytes = md5.ComputeHash(InBytes);
            //convert outBytes to string
            StringBuilder outText= new StringBuilder();
            for (int i = 0; i < outBytes.Length; i++)
            {
                  outText.Append(outBytes[i].ToString("x2"));
            }
            return outText.ToString();
        }

        public static string MD5File(string InFile)
        {
            //Convert string to byte array
            byte[] InBytes = File.ReadAllBytes(InFile);
            //create MD5 coding object
            var md5 = new MD5CryptoServiceProvider();
            //encode byte array
            byte[] outBytes = md5.ComputeHash(InBytes);
            //convert outBytes to string
            StringBuilder outText = new StringBuilder();
            for (int i = 0; i < outBytes.Length; i++)
            {
                outText.Append(outBytes[i].ToString("x2"));
            }
            return outText.ToString();
        }
        #endregion

        #region Symetrical Algorithm
        //kodavimui reikia teksto ir slapto rakto
        public static string TDESencodeString(string inText,string SecretKey)
        {
            //konvertuojam teksta i byte masyva
            byte[] inBytes = Encoding.UTF8.GetBytes(inText);
            //kuriam kodavimo objekta
            var tdes = new TripleDESCryptoServiceProvider();
            //sukursim spec byte masyva slaptazodzio apsaugai
            byte[] salt = {12,54,75,12,222,65,78,12};
            //maisom slaptazodi su salt masyvu
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(SecretKey, salt,50);
            //algoritmo nustatymai
            tdes.Key = rfc.GetBytes(16);
            tdes.IV = rfc.GetBytes(8);
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;
            //kodavimas
            ICryptoTransform transform = tdes.CreateEncryptor();
            byte[] outBytes = transform.TransformFinalBlock(inBytes, 0, inBytes.Length);
            //convert outBytes to string
            //netinkamas variantas
            //StringBuilder outText = new StringBuilder();
            //for (int i = 0; i < outBytes.Length; i++)
            //{
            //    outText.Append(outBytes[i].ToString("x2"));
            //}
            //return outText.ToString();
            return Convert.ToBase64String(outBytes);
        }


        public static string TDESdecodeString(string inText, string SecretKey)
        {
            //konvertuojam uzkoduota teksta i byte masyva
            byte[] inBytes = Convert.FromBase64String(inText);
            //kuriam kodavimo objekta
            var tdes = new TripleDESCryptoServiceProvider();
            //sukursim spec byte masyva slaptazodzio apsaugai
            byte[] salt = { 12, 54, 75, 12, 222, 65, 78, 12 };
            //maisom slaptazodi su salt masyvu
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(SecretKey, salt, 50);
            //algoritmo nustatymai
            tdes.Key = rfc.GetBytes(16);
            tdes.IV = rfc.GetBytes(8);
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;
            //dekodavimas
            ICryptoTransform transform = tdes.CreateDecryptor();
            byte[] outBytes = transform.TransformFinalBlock(inBytes, 0, inBytes.Length);
            //convert outBytes to string
            //netinkamas variantas
            //StringBuilder outText = new StringBuilder();
            //for (int i = 0; i < outBytes.Length; i++)
            //{
            //    outText.Append(outBytes[i].ToString("x2"));
            //}
            //return outText.ToString();
            return Encoding.UTF8.GetString(outBytes);
        }
        
        public static void TDESencodeFile(string inFile,string SecretKey)
        {
            string outFile;//cia bus uzkoduotas failas
            string dir = Path.GetDirectoryName(inFile);//kelias iki failo
            string outName = Path.GetFileNameWithoutExtension(inFile) + "_ENCODED" + Path.GetExtension(inFile);
            outFile = dir + @"\" + outName;
            //reading stream -  skaitom failo turini
            FileStream inStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            //writeing stream - i kuroi faila rasysim
            FileStream outStream = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            //kodavimo algoritmo parametrai
            //kuriam kodavimo objekta
            var tdes = new TripleDESCryptoServiceProvider();
            //sukursim spec byte masyva slaptazodzio apsaugai
            byte[] salt = { 12, 54, 75, 12, 222, 65, 78, 12 };
            //maisom slaptazodi su salt masyvu
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(SecretKey, salt, 50);
            //algoritmo nustatymai
            tdes.Key = rfc.GetBytes(16);
            tdes.IV = rfc.GetBytes(8);
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;
            //dekodavimas
            ICryptoTransform transform = tdes.CreateEncryptor();
            //uzrasom uzkoduotus duomenis per outStream
            CryptoStream cryptStream = new CryptoStream(outStream, transform, CryptoStreamMode.Write);
            byte[] outByte = new byte[inStream.Length];//nustatom masyvo dydi pagal nuskaitoma faila
            //nuskaitom faila ir surasom i byte masyva
            inStream.Read(outByte, 0, outByte.Length);
            //koduojam duomenis ir irasom i faila
            cryptStream.Write(outByte, 0, outByte.Length);
            //uzdarom visus failus(srautus)
            cryptStream.Close();
            inStream.Close();
            outStream.Close();


        }


        public static void TDESdecodeFile(string inFile, string SecretKey)
        {
            string outFile;//cia bus uzkoduotas failas
            string dir = Path.GetDirectoryName(inFile);//kelias iki failo
            string outName = Path.GetFileNameWithoutExtension(inFile).Substring(0,Path.GetFileNameWithoutExtension(inFile).LastIndexOf('_')) + "_DECODED" + Path.GetExtension(inFile);
            outFile = dir + @"\" + outName;
            //reading stream -  skaitom failo turini
            FileStream inStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            //writeing stream - i kuroi faila rasysim
            FileStream outStream = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            //kodavimo algoritmo parametrai
            //kuriam kodavimo objekta
            var tdes = new TripleDESCryptoServiceProvider();
            //sukursim spec byte masyva slaptazodzio apsaugai
            byte[] salt = { 12, 54, 75, 12, 222, 65, 78, 12 };
            //maisom slaptazodi su salt masyvu
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(SecretKey, salt, 50);
            //algoritmo nustatymai
            tdes.Key = rfc.GetBytes(16);
            tdes.IV = rfc.GetBytes(8);
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;
            //dekodavimas
            ICryptoTransform transform = tdes.CreateDecryptor();
            //uzrasom uzkoduotus duomenis per outStream
            CryptoStream cryptStream = new CryptoStream(outStream, transform, CryptoStreamMode.Write);
            byte[] outByte = new byte[inStream.Length];//nustatom masyvo dydi pagal nuskaitoma faila
            //nuskaitom faila ir surasom i byte masyva
            inStream.Read(outByte, 0, outByte.Length);
            //koduojam duomenis ir irasom i faila
            cryptStream.Write(outByte, 0, outByte.Length);
            //uzdarom visus failus(srautus)
            cryptStream.Close();
            inStream.Close();
            outStream.Close();
        }
        #endregion

        #region Asymentric Algorithm
        private static string publicKey,privateKey;//kintamasis pasiekiamas tik sitoje klaseje

        public static void GenerateRSAkeys()
        {
            FileStream fStream;
            StreamWriter sWriter;
            var rsa =new RSACryptoServiceProvider();

            //public key -raktas bendram naudojimui
            publicKey = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\MyPublicKey.xml";
            fStream = new FileStream(publicKey, FileMode.Create, FileAccess.Write);
            sWriter = new StreamWriter(fStream);
            sWriter.Write(rsa.ToXmlString(false));
            sWriter.Close();
            fStream.Close();

            //privatus raktas
             privateKey = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\MyPrivateKey.xml";
            fStream = new FileStream(privateKey, FileMode.Create, FileAccess.Write);
            sWriter = new StreamWriter(fStream);
            sWriter.Write(rsa.ToXmlString(true));
            sWriter.Close();
            fStream.Close();


        }
        
        public static string RSAencodeString(string inText)
        {
            //convert string to byte array
            byte[] inBytes = Encoding.UTF8.GetBytes(inText);
            //rsa coding object
            var rsa = new RSACryptoServiceProvider();
            //import public key from file
           var sReader =  new StreamReader(publicKey);
           string keyData = sReader.ReadToEnd();
           sReader.Close();
            //assign key to rsa object
            rsa.FromXmlString(keyData);
            //encode
            byte[] outBytes = rsa.Encrypt(inBytes, true);
            return Convert.ToBase64String(outBytes);
        }

        public static string RSAencodeString(string inText, string publicKey)
        {
            //convert string to byte array
            byte[] inBytes = Encoding.UTF8.GetBytes(inText);
            //rsa coding object
            var rsa = new RSACryptoServiceProvider();
           
            //assign key to rsa object
            rsa.FromXmlString(publicKey);
            //encode
            byte[] outBytes = rsa.Encrypt(inBytes, true);
            return Convert.ToBase64String(outBytes);
        }

        public static string RSAdecodeString(string inText)
        {
            //convert string to byte array
            byte[] inBytes = Convert.FromBase64String(inText);
            //rsa coding object
            var rsa = new RSACryptoServiceProvider();
            //import pprivate key from file
            var sReader = new StreamReader(privateKey);
            string keyData = sReader.ReadToEnd();
            sReader.Close();
            //assign key to rsa object
            rsa.FromXmlString(keyData);
            //encode
            byte[] outBytes = rsa.Decrypt(inBytes, true);
            return Encoding.UTF8.GetString(outBytes);
        }
        #endregion

    }
}
