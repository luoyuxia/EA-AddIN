using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MyAddin_Test;
using MyAddin_Test.utils;

namespace MyAddin_Test
{
    class Program
    {
        private static Rijndael RijndaelAlg = Rijndael.Create();
        static void Main(string[] args)
        {

              string xmiNameSpaceUrl = "http://www.omg.org/XMI";
         string faceNameSpaceUrl = "http://www.opengroup.us/face/2.1";
         XNamespace faceNameSpace = "http://www.opengroup.us/face/2.1";
         XNamespace xmiNameSapce = "http://www.omg.org/XMI";
         string nameSpace = "face";


        XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null));

          

            string path = @"E:\交大软院实验室材料\EA+13CN+13.5EN\ttt.face";
            FaceFileParser f = new FaceFileParser(path);
            EAPackage p = f.parse("KR1E9NVPJW");
            var k = 10;


            //string targetPath = @"C:\Users\yuxia\Desktop\faceTestRecovery.face";
            //FaceFileExporter faceFileExporter = new FaceFileExporter();
            //faceFileExporter.export(package, targetPath);
            //bool flag = EnumUtil.checkInEnumValues("Dousble", typeof(PlatformDataTypeEnum));
            //Console.WriteLine(flag);
        }
    }
}
