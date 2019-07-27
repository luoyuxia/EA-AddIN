using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MyAddin.utils
{
    class AESHelper
    {

        private static Rijndael RijndaelAlg = Rijndael.Create();
        private static Random random = new Random();
        private static Byte[] IV = new Byte[16] { 33, 241, 14, 16, 103, 18, 14, 248, 4, 54, 18, 5, 60, 76, 16, 191 }; // magic number
        public static String generateKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void encryptXML(XDocument xDocument, String filePath)
        {
         
            // Encrypt
            FileStream writeStream = File.Open(filePath, FileMode.Create);
            String key = generateKey(10);
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            CryptoStream cStream = new CryptoStream(writeStream,
                 RijndaelAlg.CreateEncryptor(bKey, IV),
                 CryptoStreamMode.Write);
            XmlTextWriter writer = new XmlTextWriter(cStream, Encoding.UTF8);
            xDocument.Save(writer);
            writer.Flush();
            writer.Close();
            String keyPath = filePath + ".key";
            File.WriteAllText(keyPath, key);
        }

        public static XmlDocument decryptXML(String filePath, String key)
        {
            
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            FileStream readStream = File.OpenRead(filePath);
            CryptoStream cStream = new CryptoStream(readStream,
            RijndaelAlg.CreateDecryptor(bKey, IV),
                CryptoStreamMode.Read);
            XmlTextReader reader = new XmlTextReader(cStream);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);
            return xmlDoc;
        }
    }
}
