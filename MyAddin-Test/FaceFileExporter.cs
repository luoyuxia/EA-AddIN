using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAddin_Test
{
    class FaceFileExporter
    {
        private FaceTypeEnumTool faceTypeEnumTool;
        private string xmiNameSpaceUrl = "http://www.omg.org/XMI";
        private string faceNameSpaceUrl = "http://www.opengroup.us/face/2.1";
        private XNamespace faceNameSpace = "http://www.opengroup.us/face/2.1";
        private XNamespace xmiNameSapce = "http://www.omg.org/XMI";
        private string nameSpace = "face";

        public FaceFileExporter()
        {
            faceTypeEnumTool = new FaceTypeEnumTool();
        }
        public void export(EAPackage rootPackage, string filePath)
        {
            // create document with declaration
            XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null));

            // add element
            XElement rootElment = createRootElement();
            xDocument.Add(rootElment);

            initDocumentWithPackage(rootElment, rootPackage);

            // save element
            xDocument.Save(filePath);
        }

        private void initDocumentWithPackage(XElement rootElement, EAPackage rootPackage)
        {
            foreach (EAPackage p in rootPackage.EAPackages)
            {
                XElement element = new XElement(faceTypeEnumTool.getPacakgeTag(p.StereoType), 
                    new XAttribute(this.xmiNameSapce + "type", String.Format("face:{0}", p.StereoType)),
                    new XAttribute("name", p.Name));
                rootElement.Add(element);
                foreach(EAClass c in p.EAClass)
                {
                    XElement classElement = new XElement(faceTypeEnumTool.getElementTag(c.StereoType),
                        new XAttribute(this.xmiNameSapce + "type", String.Format("{0}:{1}", faceTypeEnumTool.getPackageChildrenElementTypePrefix(p.StereoType), c.StereoType)),
                        new XAttribute("name", c.Name)
                        );
                    element.Add(classElement);
                    foreach (EAAttribute attribute in c.Attributes)
                    {
                        XElement xAttributeElement = new XElement(faceTypeEnumTool.getElementAttributeTag(attribute.Type),
                            new XAttribute(this.xmiNameSapce + "type", String.Format("{0}:{1}", faceTypeEnumTool.getPackageChildrenElementTypePrefix(p.StereoType), attribute.Type)),
                            new XAttribute("rolename", attribute.Name)
                            );
                        classElement.Add(xAttributeElement);
                    }
                }
                initDocumentWithPackage(element, p);
            }
        }

        private XElement createRootElement()
        {
           
            XElement root = new XElement(this.faceNameSpace+ "DataModel", 
                new XAttribute(XNamespace.Xmlns + "conceptual", "http://www.opengroup.us/face/conceptual/2.1"),
                new XAttribute(XNamespace.Xmlns + "logical", "http://www.opengroup.us/face/logical/2.1"),
                new XAttribute(XNamespace.Xmlns + "platform", "http://www.opengroup.us/face/platform/2.1"),
                new XAttribute(XNamespace.Xmlns + "uop", "http://www.opengroup.us/face/uop/2.1"),
                new XAttribute(XNamespace.Xmlns + "xmi", this.xmiNameSpaceUrl),
                new XAttribute(this.xmiNameSapce + "id", "0fd0fac7-315d-4755-a378-469310ff68c6"),
                new XAttribute(XNamespace.Xmlns + this.nameSpace, this.faceNameSpaceUrl));
            return root;
        }
    }
}
