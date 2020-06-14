using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyAddin.utils;
using System.Windows.Forms;
namespace MyAddin
{
    class FaceFileExporter
    {
        private XNamespace xmiNameSapce = "http://www.omg.org/spec/XMI/20110701";
        private XNamespace umlNameSapce = "http://www.omg.org/spec/UML/20110701";
        private EA.Repository repository;
        HashSet<string> connectorSet = new HashSet<string>();
        HashSet<string> depSet = new HashSet<string>();

        public FaceFileExporter(EA.Repository repository)
        {
            this.repository = repository;
        }

        public void export(EAPackage rootPackage, string filePath)
        {
            connectorSet.Clear();
            depSet.Clear();
            // create document with declaration
            XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null));

            // add element
            XElement rootElment = createRootElement();
            XElement element = new XElement(this.xmiNameSapce + "Documentation",
               new XAttribute("exporter", "Enterprise Architect"), new XAttribute("exporterVersion", "6.5"));
            xDocument.Add(rootElment);
            rootElment.Add(element);
            XElement modelElement = new XElement(this.umlNameSapce + "Model",
                new XAttribute(this.xmiNameSapce + "type", "Enterprise Architect"), new XAttribute("name", "EA_Model"));
            rootElment.Add(modelElement);

            initDocumentWithPackage(modelElement, rootPackage);

            XElement extendElement = new XElement(this.xmiNameSapce + "Extension",
                new XAttribute("extender", "Enterprise Architect"), new XAttribute("extenderID", "6.5"));
            rootElment.Add(extendElement);


            XElement xmlEles = new XElement("elements");
            XElement xmlCon = new XElement("connectors");
            XElement xmlPrim = new XElement("primitivetypes");
            XElement xmlDia = new XElement("diagrams");
            extendElement.Add(xmlEles);
            extendElement.Add(xmlCon);
            extendElement.Add(xmlPrim);
            extendElement.Add(xmlDia);
            addExtension(xmlEles, xmlCon, xmlPrim, xmlDia, rootPackage.EaPackage);

            // Encrypt and save
            AESHelper.encryptXML(xDocument, filePath);
            // save element
            //    xDocument.Save(filePath);
        }
        private void addExtension(XElement xmlElements, XElement xmlConnectors, XElement xmlPrimitivetypes,
            XElement xmldiagrams,
            EA.Package package)
        {
            foreach(EA.Package p in package.Packages)
            {
                XElement e = new XElement("element", new XAttribute(this.xmiNameSapce + "idref", handleGUID(p.PackageGUID)),
                    new XAttribute(this.xmiNameSapce + "type", "uml:Package"),
                    new XAttribute("name", p.Name), new XAttribute("scope", p.IsProtected ? "protect" : "public"));
                addExtensionPackage(p, e);
                xmlElements.Add(e);
                foreach(EA.Element ele in p.Elements)
                {
                    e = new XElement("element", new XAttribute(this.xmiNameSapce + "idref", handleGUID(ele.ElementGUID)),
                    new XAttribute(this.xmiNameSapce + "type", "uml:Class"),
                    new XAttribute("name", ele.Name), new XAttribute("scope", ele.Visibility.ToLower()));
                    addExtensionClass(ele, p, e);
                    xmlElements.Add(e);
                    addConnecter(xmlConnectors, ele);
                }
                foreach(EA.Element ele in p.Elements)
                {
                    foreach (EA.Element ee in ele.EmbeddedElements)
                    {
                        e = new XElement("element", new XAttribute(this.xmiNameSapce + "idref", handleGUID(ee.ElementGUID)),
                    new XAttribute(this.xmiNameSapce + "type", "uml:" + ee.Type),
                    new XAttribute("name", ee.Name), new XAttribute("scope", ee.Visibility.ToLower()));
                        addExtensionClass(ee, p, e);
                        xmlElements.Add(e);
                        addConnecter(xmlConnectors, ee);
                    }
                }
                addDiagram(p, xmldiagrams);
                addExtension(xmlElements, xmlConnectors, xmlPrimitivetypes, xmldiagrams, p);
            }
        }

        private void addDiagram(EA.Package package, XElement xmlDiagram)
        {
            foreach(EA.Diagram diagram in package.Diagrams)
            {
                XElement diag = new XElement("diagram", new XAttribute(this.xmiNameSapce + "id", handleGUID(diagram.DiagramGUID)));
                diag.Add(new XElement("model",
                new XAttribute("package",handleGUID(package.PackageGUID)), new XAttribute("localID", diagram.DiagramID),
                new XAttribute("owner", handleGUID(package.PackageGUID))));
                diag.Add(new XElement("properties", new XAttribute("name", diagram.Name),
                    new XAttribute("type", diagram.Type)));
                diag.Add(new XElement("project", new XAttribute("author", diagram.Author),
                    new XAttribute("version", "1.0"), new XAttribute("created", diagram.CreatedDate),
                    new XAttribute("modified", diagram.ModifiedDate)));
                diag.Add(new XElement("style1", new XAttribute("value",
                    "ShowPrivate=1;ShowProtected=1;ShowPublic=1;HideRelationships=0;Locked=0;Border=1;HighlightForeign=1;PackageContents=1;SequenceNotes=0;ScalePrintImage=0;PPgs.cx=0;PPgs.cy=0;DocSize.cx=827;DocSize.cy=1169;ShowDetails=0;Orientation=P;Zoom=100;ShowTags=0;OpParams=1;VisibleAttributeDetail=0;ShowOpRetType=1;ShowIcons=1;CollabNums=0;HideProps=0;ShowReqs=0;ShowCons=0;PaperSize=9;HideParents=0;UseAlias=0;HideAtts=0;HideOps=0;HideStereo=0;HideElemStereo=0;ShowTests=0;ShowMaint=0;ConnectorNotation=UML 2.1;ExplicitNavigability=0;ShowShape=1;AdvancedElementProps=1;AdvancedFeatureProps=1;AdvancedConnectorProps=1;m_bElementClassifier=1;ShowNotes=0;SuppressBrackets=0;SuppConnectorLabels=0;PrintPageHeadFoot=0;ShowAsList=0;")));
                diag.Add(new XElement("style2", new XAttribute("value", "ExcludeRTF=0;DocAll=0;HideQuals=0;AttPkg=1;ShowTests=0;ShowMaint=0;SuppressFOC=1;MatrixActive=0;SwimlanesActive=1;KanbanActive=0;MatrixLineWidth=1;MatrixLineClr=0;MatrixLocked=0;TConnectorNotation=UML 2.1;TExplicitNavigability=0;AdvancedElementProps=1;AdvancedFeatureProps=1;AdvancedConnectorProps=1;m_bElementClassifier=1;SPT=1;MDGDgm=;STBLDgm=;ShowNotes=0;VisibleAttributeDetail=0;ShowOpRetType=1;SuppressBrackets=0;SuppConnectorLabels=0;PrintPageHeadFoot=0;ShowAsList=0;SuppressedCompartments=;Theme=:119;SaveTag=88453DE1;")));
                diag.Add(new XElement("swimlanes", new XAttribute("value", "locked=false;orientation=0;width=0;inbar=false;names=false;color=-1;bold=false;fcol=0;tcol=-1;ofCol=-1;ufCol=-1;hl=0;ufh=0;cls=0;")));
                diag.Add(new XElement("matrixitems", new XAttribute("value", "locked=false;matrixactive=false;swimlanesactive=true;kanbanactive=false;width=1;clrLine=0;")));
                diag.Add(new XElement("extendedProperties"));
                XElement elements = new XElement("elements");
                foreach(EA.DiagramObject diagramObject in diagram.DiagramObjects)
                {
                   
                    string geometry = String.Format("Left={0};Top={1};Right={2};Bottom={3};",
                       Math.Abs(diagramObject.left),
                       Math.Abs(diagramObject.top),
                       Math.Abs(diagramObject.right),
                       Math.Abs(diagramObject.bottom));
                    XElement diagramObj = new XElement("element", new XAttribute("geometry",geometry),
                        new XAttribute("subject", handleGUID(this.repository.GetElementByID(diagramObject.ElementID).ElementGUID)),
                        new XAttribute("seqno", diagramObject.Sequence),
                        new XAttribute("style", diagramObject.Style));
                    elements.Add(diagramObj);
                }
                foreach(EA.DiagramLink diagramLink in diagram.DiagramLinks)
                { 
                    XElement diagramEle = new XElement("element", new XAttribute("geometry", diagramLink.Geometry),
                        new XAttribute("subject", 
                       handleGUID(this.repository.GetElementByID(this.repository.GetConnectorByID(diagramLink.ConnectorID).ClientID).ElementGUID)),
                        new XAttribute("style", diagramLink.Style));
                    elements.Add(diagramEle);
                }
                
                diag.Add(elements);
                xmlDiagram.Add(diag);

            }
        }

        private void addExtensionPackage(EA.Package p, XElement e)
        {
            e.Add(new XElement("model", new XAttribute("package", handleGUID(p.PackageGUID)),
            new XAttribute("ea_eleType", "package")));
            e.Add(new XElement("properties", new XAttribute("isSpecification", "false"),
                new XAttribute("sType", "Package"), new XAttribute("nType", "0"),
                new XAttribute("stereotype", p.Element.Stereotype),
                new XAttribute("scope", p.IsProtected ? "protect" : "public")));
            e.Add(new XElement("project", new XAttribute("author", p.Owner),
                new XAttribute("version", "1.0"), new XAttribute("phase", "1.0"),
                new XAttribute("created", p.Created.ToString()), new XAttribute("modified", p.Modified.ToString()),
                new XAttribute("complexity", "1"), new XAttribute("status", "Proposed")));
            e.Add(new XElement("code", new XAttribute("gentype", "Java")));
            e.Add(new XElement("style", new XAttribute("appearance", "BackColor=-1;BorderColor=-1;BorderWidth=-1;FontColor=-1;VSwimLanes=1;HSwimLanes=1;BorderStyle=0;")));
            e.Add(new XElement("tags"));
            e.Add(new XElement("xrefs"));
            e.Add(new XElement("extendedProperties", new XAttribute("tagged", "0"),
                new XAttribute("package_name", this.repository.GetPackageByID(p.ParentID).Name)));
            e.Add(new XElement("packageproperties", new XAttribute("version", "1.0")));
            e.Add(new XElement("paths"));
            e.Add(new XElement("times", new XAttribute("created", p.Created.ToString()), new XAttribute("modified", p.Modified.ToString())));
            e.Add(new XElement("flags", new XAttribute("iscontrolled", p.IsControlled), 
                new XAttribute("batchsave", p.BatchSave), new XAttribute("batchload", p.BatchLoad),
                new XAttribute("isprotected", p.IsProtected), new XAttribute("usedtd", "FALSE"),
                new XAttribute("logxml", "FALSE"), new XAttribute("packageFlags", "isModel=1;VICON=3;")));
        }
        private void addExtensionClass(EA.Element ele, EA.Package p, XElement e)
        {
            e.Add(new XElement("model", new XAttribute("package", handleGUID(p.PackageGUID)),
           new XAttribute("tpos", ele.TreePos), new XAttribute("ea_localid", ele.ElementID),
           new XAttribute("ea_eleType", ele.Type)));
            e.Add(new XElement("properties", new XAttribute("isSpecification", "false"),
                new XAttribute("sType", "Class"), new XAttribute("nType", "0"), new XAttribute("scope", ele.Visibility.ToLower()),
                new XAttribute("isRoot", "false"), new XAttribute("isLeaf", ele.IsLeaf), new XAttribute("isAbstract", ele.Abstract),
                new XAttribute("stereotype", ele.Stereotype),
                new XAttribute("isActive", ele.IsActive)));
            e.Add(new XElement("project", new XAttribute("author", p.Owner),
                new XAttribute("version", "1.0"), new XAttribute("phase", "1.0"),
                new XAttribute("created", p.Created.ToString()), new XAttribute("modified", p.Modified.ToString()),
                new XAttribute("complexity", "1"), new XAttribute("status", "Proposed")));
            e.Add(new XElement("code", new XAttribute("gentype", "Java")));
            e.Add(new XElement("style", 
                new XAttribute("appearance", "BackColor=-1;BorderColor=-1;BorderWidth=-1;FontColor=-1;VSwimLanes=1;HSwimLanes=1;BorderStyle=0;")));
            XElement tags = new XElement("tags");
            foreach(EA.TaggedValue taggedValue in ele.TaggedValues)
            {
                string value = taggedValue.Value + "#NOTES#" +  taggedValue.Notes;
                tags.Add(new XElement("tag", new XAttribute("name", taggedValue.Name),
                    new XAttribute("value", value),
                    new XAttribute("modelElement", handleGUID(ele.ElementGUID)),
                    new XAttribute(this.xmiNameSapce + "id", handleGUID(taggedValue.PropertyGUID))));
            }
            e.Add(tags);
            e.Add(new XElement("xrefs"));
            e.Add(new XElement("extendedProperties", new XAttribute("tagged", ele.TaggedValues.Count),
                new XAttribute("package_name", p.Name)));
            addAttribute(e, ele);
            XElement links = new XElement("links");
            e.Add(links);
            foreach(EA.Connector con in ele.Connectors)
            {
                links.Add(new XElement("Association", new XAttribute(this.xmiNameSapce + "id", handleGUID(con.ConnectorGUID)),
                    new XAttribute("start", handleGUID(this.repository.GetElementByID(con.ClientID).ElementGUID)),
                    new XAttribute("end", handleGUID(this.repository.GetElementByID(con.SupplierID).ElementGUID))));
            }
        }
        private void addAttribute(XElement e, EA.Element ele)
        {
            if (ele.Attributes.Count == 0)
                return;
            XElement attr = new XElement("attributes");
            e.Add(attr);
            foreach(EA.Attribute attribute in ele.Attributes)
            {
                XElement att = new XElement("attribute", new XAttribute(this.xmiNameSapce + "idref", handleGUID(attribute.AttributeGUID)),
                    new XAttribute("name", attribute.Name), new XAttribute("scope", attribute.Visibility.ToLower()));
                attr.Add(att);
                att.Add(new XElement("initial"));
                att.Add(new XElement("documentation"));
                att.Add(new XElement("model", new XAttribute("ea_localid", ele.ElementID),
                    new XAttribute("ea_guid", handleGUID(ele.ElementGUID))));
                att.Add(new XElement("properties", new XAttribute("type", attribute.Type),
                    new XAttribute("collection", attribute.IsCollection),
                    new XAttribute("duplicates", attribute.AllowDuplicates ? 1 : 0), 
                    new XAttribute("derived", attribute.IsDerived ? 1 : 0),
                    new XAttribute("changeability", attribute.IsConst ? "unchangeable" : "changeable")));
                att.Add(new XElement("coords", new XAttribute("ordered", attribute.IsOrdered)));
                att.Add(new XElement("containment", new XAttribute("containment", attribute.Containment),
                    new XAttribute("position", attribute.Pos)));
                att.Add(new XElement("stereotype"));
                att.Add(new XElement("bounds", new XAttribute("lower", attribute.LowerBound), 
                    new XAttribute("upper", attribute.UpperBound)));
                att.Add(new XElement("options"));
                att.Add(new XElement("style"));
                att.Add(new XElement("styleex", new XAttribute("value", ele.StyleEx)));
                att.Add(new XElement("tags"));
                att.Add(new XElement("xrefs"));
            }

        }
        private void addConnecter(XElement e, EA.Element eaEle)
        {
            foreach(EA.Connector con in eaEle.Connectors)
            {
                if (con.ClientID == eaEle.ElementID)
                {
                    XElement xmlCon = new XElement("connector", new XAttribute(this.xmiNameSapce + "idref", handleGUID(con.ConnectorGUID)));
                    e.Add(xmlCon);
                    XElement source = new XElement("source", new XAttribute(this.xmiNameSapce + "idref", handleGUID(eaEle.ElementGUID)));
                    xmlCon.Add(source);
                    addConnectST(eaEle, source, con.ClientEnd);
                    EA.Element tar = this.repository.GetElementByID(con.SupplierID);
                    XElement target = new XElement("target", new XAttribute(this.xmiNameSapce + "idref",
                        handleGUID(tar.ElementGUID)));
                    xmlCon.Add(target);
                    addConnectST(tar, target, con.SupplierEnd);
                    xmlCon.Add(new XElement("model", new XAttribute("ea_localid", eaEle.PackageID)));
                    xmlCon.Add(new XElement("properties", new XAttribute("ea_type", "Association"),
                        new XAttribute("direction", con.Direction)));
                    xmlCon.Add(new XElement("modifiers", new XAttribute("isRoot", "flase"),
                        new XAttribute("isLeaf", con.IsLeaf)));
                    xmlCon.Add(new XElement("parameterSubstitutions"));
                    xmlCon.Add(new XElement("documentation"));
                    xmlCon.Add(new XElement("appearance", new XAttribute("linemode", "3"),
                        new XAttribute("linecolor", "-1"), new XAttribute("linewidth", "0"), new XAttribute("seqno", "0"),
                        new XAttribute("headStyle", "0"), new XAttribute("lineStyle", "0")
                        ));
                    xmlCon.Add(new XElement("labels"));
                    xmlCon.Add(new XElement("extendedProperties", new XAttribute("extendedProperties", 0)));
                    xmlCon.Add(new XElement("style"));
                    xmlCon.Add(new XElement("xrefs"));
                    xmlCon.Add(new XElement("tags"));
                }
            }
        }
        private void addConnectST(EA.Element ele, XElement e, EA.ConnectorEnd connectorEnd)
        {
            e.Add(new XElement("model", new XAttribute("ea_localid", ele.ElementID),
                new XAttribute("type", "Class"), new XAttribute("name", ele.Name)));
            e.Add(new XElement("role", new XAttribute("visibility", connectorEnd.Visibility.ToLower())),
                new XAttribute("targetScope", "instance"));
            e.Add(new XElement("type", new XAttribute("aggregation", connectorEnd.Aggregation),
                new XAttribute("multiplicity", connectorEnd.Cardinality),
                new XAttribute("containment", connectorEnd.Containment)));
            e.Add(new XElement("constraints"));
            e.Add(new XElement("modifiers", new XAttribute("isOrdered", connectorEnd.Ordering != 0), 
                new XAttribute("changeable", connectorEnd.IsChangeable),
                new XAttribute("isNavigable", connectorEnd.IsNavigable)));
            e.Add(new XElement("style", new XAttribute("value", "Union=0;Derived=0;AllowDuplicates=0;Owned=0;Navigable=Unspecified;")));
            e.Add(new XElement("documentation"));
            e.Add(new XElement("xrefs"));
            e.Add(new XElement("tags"));
        }
        private void initDocumentWithPackage(XElement rootElement, EAPackage rootPackage)
        {
            
            foreach (EAPackage p in rootPackage.EAPackages)
            {
                XElement element = new XElement("packagedElement",
                    new XAttribute(this.xmiNameSapce + "type",  "uml:Package"),
                    new XAttribute(this.xmiNameSapce + "id", handleGUID(p.StereoType)),
                    new XAttribute("name", p.Name));
                rootElement.Add(element);
                
                foreach (EAClass c in p.EAClass)
                {
                    XElement classElement = new XElement("packagedElement",
                    new XAttribute(this.xmiNameSapce + "type", "uml:Class"),
                    new XAttribute(this.xmiNameSapce + "id", handleGUID(c.StereoType)),
                    new XAttribute("name", c.Name));
                    element.Add(classElement);
                    foreach (EA.Attribute att in c.Element.Attributes)
                    {

                        XElement xAttributeElement = new XElement("ownedAttribute",
                        new XAttribute(this.xmiNameSapce + "type", "uml:Property"),
                        new XAttribute("name", att.Name),
                         new XAttribute(this.xmiNameSapce + "id", handleGUID(att.AttributeGUID)),
                        new XAttribute("visibility",
                        att.Visibility.ToLower()));
                        classElement.Add(xAttributeElement);
                        xAttributeElement.Add(new XElement("lowerValue", new XAttribute("value", att.LowerBound)));
                        xAttributeElement.Add(new XElement("upperValue", new XAttribute("value", att.UpperBound)));
                        try
                        {
                            xAttributeElement.Add(new XElement("type", new XAttribute(this.xmiNameSapce + "idref",
                          handleGUID(this.repository.GetElementByID(att.ClassifierID).ElementGUID)
                           )));
                        }
                         catch
                        {
                            xAttributeElement.Add(new XElement("type", new XAttribute(this.xmiNameSapce + "idref",
                                att.Type
                         )));
                        }
                       
                    }
                    foreach(EA.Element e in c.Element.EmbeddedElements)
                    {
                        XElement xElement = new XElement("ownedAttribute",
                            new XAttribute(this.xmiNameSapce + "type", "uml:" + e.Type),
                            new XAttribute(this.xmiNameSapce + "id", handleGUID(e.ElementGUID)),
                            new XAttribute("name", e.Name), new XAttribute("aggregation", "composite"));
                        classElement.Add(xElement);
                        foreach(EA.Connector connector in e.Connectors)
                        {
                            if (connectorSet.Contains(connector.ConnectorGUID))
                                continue;
                            if (connector.Type == "InformationFlow")
                            {
                                XElement connectorElement = new XElement("packagedElement",
                                   new XAttribute(this.xmiNameSapce + "type", "uml:" + connector.Type),
                                   new XAttribute(this.xmiNameSapce + "id", handleGUID(connector.ConnectorGUID)),
                                   new XAttribute("informationTarget", handleGUID(this.repository.GetElementByID(connector.SupplierID).ElementGUID)),
                                   new XAttribute("informationSource", handleGUID(this.repository.GetElementByID(connector.ClientID).ElementGUID)));
                                element.Add(connectorElement);
                                connectorSet.Add(connector.ConnectorGUID);
                            }
                        }
                    }
                    foreach(EA.Connector connector in c.Element.Connectors)
                    {
                        if (connectorSet.Contains(connector.ConnectorGUID))
                            continue;
                        if(connector.Type == "Realisation" || connector.Type == "Realization")
                        {
                            XElement connectorElement = new XElement("packagedElement", 
                                new XAttribute(this.xmiNameSapce + "type", "uml:" + connector.Type),
                                new XAttribute(this.xmiNameSapce + "id", handleGUID(connector.ConnectorGUID)),
                                new XAttribute("supplier", handleGUID(this.repository.GetElementByID(connector.SupplierID).ElementGUID)),
                                new XAttribute("client", handleGUID(this.repository.GetElementByID(connector.ClientID).ElementGUID)));
                            element.Add(connectorElement);
                            connectorSet.Add(connector.ConnectorGUID);
                        } else if(connector.Type == "Association")
                        {
                            XElement connectorElement = new XElement("packagedElement",
                            new XAttribute(this.xmiNameSapce + "type", "uml:" + connector.Type),
                            new XAttribute("name", connector.Name),
                            new XAttribute(this.xmiNameSapce + "id", handleGUID(connector.ConnectorGUID)));
                            element.Add(connectorElement);
                            String clientID = "EAID_src" + Guid.NewGuid().ToString();
                            String supplieID = "EAID_tgt" + Guid.NewGuid().ToString();
                            XElement s = new XElement("memberEnd",
                                new XAttribute(this.xmiNameSapce + "idref", clientID));
                            XElement t = new XElement("memberEnd",
                               new XAttribute(this.xmiNameSapce + "idref", supplieID));
                            connectorElement.Add(s);
                            connectorElement.Add(t);
                            XElement sEnd = new XElement("ownedEnd",
                               new XAttribute(this.xmiNameSapce + "type", "uml:Property"),
                               new XAttribute(this.xmiNameSapce + "id", clientID), new XAttribute("association", handleGUID(connector.ConnectorGUID)));
                            sEnd.Add(new XElement("type",
                                 new XAttribute(this.xmiNameSapce + "idref", handleGUID(this.repository.GetElementByID(connector.ClientID).ElementGUID))));

                            XElement oEnd = new XElement("ownedEnd",
                               new XAttribute(this.xmiNameSapce + "type", "uml:Property"),
                               new XAttribute(this.xmiNameSapce + "id", supplieID), new XAttribute("association", handleGUID(connector.ConnectorGUID)));
                            oEnd.Add(new XElement("type",
                                 new XAttribute(this.xmiNameSapce + "idref",
                                handleGUID(this.repository.GetElementByID(connector.SupplierID).ElementGUID))));
                            connectorElement.Add(sEnd);
                            connectorElement.Add(oEnd);
                            connectorSet.Add(connector.ConnectorGUID);
                        }
                        else if(connector.Type == "InformationFlow")
                        {
                            XElement connectorElement = new XElement("packagedElement",
                               new XAttribute(this.xmiNameSapce + "type", "uml:" + connector.Type),
                               new XAttribute(this.xmiNameSapce + "id", handleGUID(connector.ConnectorGUID)),
                               new XAttribute("informationTarget", handleGUID(this.repository.GetElementByID(connector.SupplierID).ElementGUID)),
                               new XAttribute("informationSource", handleGUID(this.repository.GetElementByID(connector.ClientID).ElementGUID)));
                            element.Add(connectorElement);
                            connectorSet.Add(connector.ConnectorGUID);
                        } 
                    }                   
                }
                foreach (EA.Connector connector in p.EaPackage.Connectors)
                {
                    if (connectorSet.Contains(connector.ConnectorGUID))
                        continue;
                    if (connector.Type == "Dependency")
                    {
                        string sourceId = handleGUID(this.repository.GetElementByID(connector.ClientID).ElementGUID);
                        string tgtId = handleGUID(this.repository.GetElementByID(connector.SupplierID).ElementGUID);
                        
                        if (depSet.Contains(sourceId + "_" + tgtId))
                            continue;
                        depSet.Add(sourceId + "_" + tgtId);

                        XElement dependency = new XElement("packagedElement",
                            new XAttribute(this.xmiNameSapce + "type", "uml:" + connector.Type),
                            new XAttribute(this.xmiNameSapce + "id", handleGUID(connector.ConnectorGUID)),
                            new XAttribute("supplier", tgtId
                           ),
                            new XAttribute("client", sourceId)
                           );
                        element.Add(dependency);
                        connectorSet.Add(connector.ConnectorGUID);
                    }
                }

                initDocumentWithPackage(element, p);
            }
        }

        private XElement createRootElement()
        {

            XElement root = new XElement(this.xmiNameSapce + "XMI",
                new XAttribute(XNamespace.Xmlns + "uml", "http://www.omg.org/spec/UML/20110701"),
                new XAttribute(XNamespace.Xmlns + "xmi", "http://www.omg.org/spec/XMI/20110701"));
            return root;
        }

        private String handleGUID(String guid)
        {
            return "EAID_" + guid.Substring(1, guid.Length - 2).Replace('-', '_');
        }
    }
}
