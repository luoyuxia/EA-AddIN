using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MyAddin.utils;
using System.Windows.Forms;

namespace MyAddin
{
    class FaceFileParser
    {
        private string filePath;
        private string defaultRootPackageSterotype = "FACEDataModel";
        private string typeRepresentStr = "xmi:type";
        private string attributeStr = "rolename";
        public FaceFileParser(String filePath)
        {
            this.filePath = filePath;
        }
        public EAPackage parse(string key)
        {
            XmlDocument xmlDoc = null;
            try
            {
                xmlDoc = AESHelper.decryptXML(filePath, key);
            } catch(Exception e)
            {
                MessageBox.Show("无法读取文件，请确保秘钥是否正确！");
                return null;
            }
            XmlNode faceModelNode = xmlDoc.LastChild;
            XmlElement faceModelElement = faceModelNode as XmlElement;
            String name = faceModelElement.GetAttribute("name");
            EAPackage rootPackage = new EAPackage(name, defaultRootPackageSterotype);
            generatePackageFromXMLNode(rootPackage, faceModelNode);
            return rootPackage;
        }

        public void generatePackageFromXMLNode(EAPackage parentPackage, XmlNode xmlNode)
        {
            foreach(XmlNode n in xmlNode.ChildNodes)
            {
                if(isPackage(n))
                {
                    EAPackage package = createEAPackageFromXMLNode(n);
                    parentPackage.addPackage(package);
                    generatePackageFromXMLNode(package, n);
                } else if (isClass(n))
                {
                    parentPackage.addClass(createEAClassFromXMLNode(n));
                }
            }
        }

        public bool isPackage(XmlNode xmlNode)
        {
            string faceType = getRealType((xmlNode as XmlElement).GetAttribute(typeRepresentStr));
            return isPackage(faceType);
        }

        public EAClass createEAClassFromXMLNode(XmlNode xmlNode)
        {
            XmlElement xmlElement = xmlNode as XmlElement;
            string name = xmlElement.GetAttribute("name");
            string id = xmlElement.GetAttribute("xmi:id");
            string stereoType = getFaceType(xmlNode);
            EAClass eaClass = new EAClass(name, stereoType, id);
            string realizeId = xmlElement.GetAttribute("realizes");
            if (realizeId != "")
            {
                eaClass.RealizeClassID = realizeId;
            }
            string measurementAxis = xmlElement.GetAttribute("measurementAxis");
            if(measurementAxis != "")
            {
               List<string> measurementList =  measurementAxis.Split(' ').ToList();
               eaClass.MeasurementAxisList = measurementList;
            }
            foreach (XmlNode n in xmlNode.ChildNodes)
            {
                XmlElement element = n as XmlElement;
                EAAttribute eAAttribute = null;
                if (isAttribute(n))
                    eAAttribute  = new EAAttribute(element.GetAttribute(attributeStr), getFaceType(n), element.GetAttribute("xmi:id"));
                if (eAAttribute != null && isAttributeClass(n))
                {
                    eAAttribute.Name = element.GetAttribute("name");
                    eAAttribute.IsEAClass = true;
                }
                    
                if (eAAttribute != null)
                    eaClass.addAttribute(eAAttribute);
            }
            return eaClass;
        }

        public string getFaceType(XmlNode xmlNode)
        {
            XmlElement element = xmlNode as XmlElement;
            return getRealType(element.GetAttribute(typeRepresentStr));
        }

        public bool isAttribute(XmlNode xmlNode)
        {
            string faceType = getFaceType(xmlNode);
            return isAttibuteByTag(xmlNode) || isAttribute(faceType) || isAttributeClass(xmlNode);
        }

        public bool isAttributeClass(XmlNode xmlNode)
        {
            return xmlNode.Name == "port" || getFaceType(xmlNode) == FaceTypeEnum.MessagePort.ToString();
        }

        public bool isAttribute(string faceType)
        {
            return faceType == FaceTypeEnum.Composition.ToString();
        }

        public bool isAttibuteByTag(XmlNode xmlNode)
        {
            return xmlNode.Name == "composition" || xmlNode.Name == "characteristic";
        }

        public bool isPackage(string faceType)
        {
            return faceType == FaceTypeEnum.ConceptualDataModel.ToString() || faceType == FaceTypeEnum.LogicalDataModel.ToString()
                || faceType == FaceTypeEnum.PlatformDataModel.ToString() || faceType == FaceTypeEnum.UoPModel.ToString();
        }


        public EAPackage createEAPackageFromXMLNode(XmlNode xmlNode)
        {
            return new EAPackage((xmlNode as XmlElement).GetAttribute("name"), getFaceType(xmlNode));
        }

        public bool isClass(XmlNode xmlNode)
        {
            string faceType  = getFaceType(xmlNode);
            return xmlNode.Name == "element" || xmlNode.Name == "port" || isClass(faceType);
        }

        public bool isClass(string faceType)
        {
            return faceType == FaceTypeEnum.Entity.ToString() || faceType == FaceTypeEnum.MessagePort.ToString()
                || faceType == FaceTypeEnum.Observable.ToString();
        }

        public string getRealType(string faceType)
        {
            string[] s = faceType.Split(':');
            return s.Length >= 2 ? s[1] : s[0];
        }
    }
}
