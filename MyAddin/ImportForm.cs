using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using MyAddin.utils;
using System.Xml;
namespace MyAddin
{
    public partial class ImportForm : Form
    {
        private EA.Repository repository;
        private string defaultPackageName = "gpslocation";
        private string defaultDiagramName = "gpslocation";
        private string DATA_TYPE = "IDLPrimitive";
        private string decryptKey;
        private Dictionary<string, int> eAIdElementIdMap = new Dictionary<string, int>();
        private Dictionary<int, string> ElementId2EAId = new Dictionary<int, string>();
        private Dictionary<string, int> eaIdPackageIdMap = new Dictionary<string, int>();
        private Dictionary<string, int> eaIdAttibuteIdMap = new Dictionary<string, int>();
        private Dictionary<int, string> PackageId2EAId = new Dictionary<int, string>();
        private Dictionary<string, int> eaIdConnectorIdMap = new Dictionary<string, int>();
        private Dictionary<string, int> eaIdDiagramIDMap = new Dictionary<string, int>();
        private Dictionary<int, string> connectorSrcMap = new Dictionary<int, string>();
        private Dictionary<int, string> connectorTgtMap = new Dictionary<int, string>();
        private Dictionary<int, HashSet<int>> packageConnectors = new Dictionary<int, HashSet<int>>();

        private Dictionary<int, EAClass> eAElementEAClassMap = new Dictionary<int, EAClass>();
        private Dictionary<string, string> measurementAxisMap = new Dictionary<string, string>();
        public ImportForm(EA.Repository repository)
        {
            InitializeComponent();
            string installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.repository = repository;
            this.initFieldValue();
        }

        private void initFieldValue()
        {
            this.eaProjectFileInput.Text = this.repository.ConnectionString
                 + String.Format("  EA_PKG_GUID=({0})", this.repository.ProjectGUID);
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            this.XMLSelectDialog.ShowDialog();
            this.setImportButtonStatus();
        }

        private void faceXMLSelectDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.selectXMLFile.Text = this.XMLSelectDialog.FileName;
            this.setImportButtonStatus();
        }

        private void setImportButtonStatus()
        {
            if(this.selectNewProjectFIleDialog.FileName.Length > 0 && this.XMLSelectDialog.FileName.Length > 0)
            {
                if(this.decrypt_checkbox.Checked && this.keytext.Text.Trim().Length <= 0)
                {
                    this.importModelButton.Enabled = false;
                    return;
                }
                this.importModelButton.Enabled = true;
                return;
            }
            this.importModelButton.Enabled = false;
        }

        private void importModelButton_Click(object sender, EventArgs e)
        {
            importFaceProject();       
            this.Dispose();
        }

        private void importFaceProject()
        {
            clearDict();
            measurementAxisMap.Clear();
            createNewProject();
            AESHelper.isEncrpted = decrypt_checkbox.Checked;
            EA.Package topPackage = getTopPackage();
            FaceFileParser faceFileParser = new FaceFileParser(this.XMLSelectDialog.FileName);
            XmlDocument xmlDoc = faceFileParser.parseXMLDoc(this.decryptKey);
        //    EAPackage rootPackage = faceFileParser.parse(this.decryptKey);
            if (topPackage == null || xmlDoc == null)
                return;
            this.addPackageOrClass(topPackage, xmlDoc.GetElementsByTagName("uml:Model")[0].FirstChild);
            this.updateConnectorEnd();
            this.updateClassAttributeType();
            this.addExtension(xmlDoc.GetElementsByTagName("xmi:Extension")[0]);

        //    EAPackage additionRootPackage = new EAPackage(defaultDiagramName);
        //    additionRootPackage.addPackage(rootPackage);
        //    initPackage(topPackage, additionRootPackage);
        }

        private void clearDict()
        {
            eAIdElementIdMap.Clear();
            eAElementEAClassMap.Clear();
            eaIdConnectorIdMap.Clear();
            eaIdPackageIdMap.Clear();
            eaIdDiagramIDMap.Clear();
            PackageId2EAId.Clear();
            ElementId2EAId.Clear();
            eaIdAttibuteIdMap.Clear();
            connectorSrcMap.Clear();
            connectorTgtMap.Clear();
            packageConnectors.Clear();
        }

        private void addPackageOrClass(EA.Package parentPackage, XmlNode xmlNode)
        {
            EA.Package p = parentPackage.Packages.AddNew(xmlNode.Attributes.GetNamedItem("name").Value, "Package");
            p.Update();
            eaIdPackageIdMap[xmlNode.Attributes.GetNamedItem("xmi:id").Value] = p.PackageID;
            PackageId2EAId[p.PackageID] = xmlNode.Attributes.GetNamedItem("xmi:id").Value;
            foreach (XmlNode childrenNode in xmlNode.ChildNodes)
            {
                string type = childrenNode.Attributes.GetNamedItem("xmi:type").Value;
                switch (type)
                {
                    case "uml:Class":
                        this.addClass(p, childrenNode);
                        break;

                    case "uml:Package":
                        this.addPackageOrClass(p, childrenNode);
                        break;
                    default: break;
                }
            }
            foreach (XmlNode childrenNode in xmlNode.ChildNodes)
            {
                string type = childrenNode.Attributes.GetNamedItem("xmi:type").Value;
                switch (type)
                {
                    case "uml:Association":
                        this.addAssociation(p, childrenNode);
                        break;
                    case "uml:Dependency":
                        this.addDependency(p, childrenNode);
                        break;
                    case "uml:Realisation":
                        this.addRealisation(p, childrenNode);
                        break;
                    case "uml:Realization":
                        this.addRealisation(p, childrenNode);
                        break;
                    case "uml:InformationFlow":
                        this.addInformationFlow(p, childrenNode);
                        break;
                    default: break;
                }
            }
        }

        private void addInformationFlow(EA.Package p, XmlNode xmlNode)
        {
            EA.Connector connector = p.Connectors.AddNew("", "InformationFlow");
            string src = xmlNode.Attributes.GetNamedItem("informationSource").Value, tgt = xmlNode.Attributes.GetNamedItem("informationTarget").Value;
            connector.ClientID = p.PackageID;
            connector.SupplierID = p.PackageID;
            connector.Update();
            connectorSrcMap[connector.ConnectorID] = src;
            connectorTgtMap[connector.ConnectorID] = tgt;
            eaIdConnectorIdMap[xmlNode.Attributes.GetNamedItem("xmi:id").Value] = connector.ConnectorID;

        }
        private void addRealisation(EA.Package p, XmlNode xmlNode)
        {
            string name = xmlNode.Attributes.GetNamedItem("name") == null ? "" : xmlNode.Attributes.GetNamedItem("name").Value;
            EA.Connector connector = p.Connectors.AddNew(name, "Realization");
            setConnectorInfo(p, connector, xmlNode);
        }

        private void setConnectorInfo(EA.Package p, EA.Connector connector, XmlNode xmlNode)
        {
            string src = xmlNode.Attributes.GetNamedItem("client").Value, tgt = xmlNode.Attributes.GetNamedItem("supplier").Value;
            connector.ClientID = p.PackageID;
            connector.SupplierID = p.PackageID;
            connector.Update();
            connectorSrcMap[connector.ConnectorID] = src;
            connectorTgtMap[connector.ConnectorID] = tgt;
            eaIdConnectorIdMap[xmlNode.Attributes.GetNamedItem("xmi:id").Value] = connector.ConnectorID;
        }

        private void addAssociation(EA.Package p, XmlNode xmlNode)
        {
            string src = "", tgt = "";
            string srcEleId = "", tgtEleId = "";
            string name = xmlNode.Attributes.GetNamedItem("name") == null ? "" : xmlNode.Attributes.GetNamedItem("name").Value;
            
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if(node.Name == "memberEnd")
                {
                    string idRef = node.Attributes.GetNamedItem("xmi:idref").Value;
                    if(idRef.Contains("src"))
                    {
                        src = idRef;
                    } else { tgt = idRef; }
                } 
            }
            foreach(XmlNode node in xmlNode.ChildNodes)
            {
                if(node.Name == "ownedEnd")
                {
                    string id = node.Attributes.GetNamedItem("xmi:id").Value;
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        if(n.Name == "type")
                        {
                            string eleId = n.Attributes.GetNamedItem("xmi:idref").Value;
                            if(id == src)
                            {
                                srcEleId = eleId;
                            } else if (id == tgt)
                            {
                                tgtEleId = eleId;
                            }
                        }
                    }
                } 
            }
            EA.Connector connector = p.Connectors.AddNew(name, "Association");
            connector.ClientID = p.PackageID;
            connector.SupplierID = p.PackageID;
            connector.Update();
            connectorSrcMap[connector.ConnectorID] = srcEleId;
            connectorTgtMap[connector.ConnectorID] = tgtEleId;
            eaIdConnectorIdMap[xmlNode.Attributes.GetNamedItem("xmi:id").Value] = connector.ConnectorID;
        }

        private void addDependency(EA.Package p, XmlNode xmlNode)
        {
            string name = xmlNode.Attributes.GetNamedItem("name") == null ? "" : xmlNode.Attributes.GetNamedItem("name").Value;
            EA.Connector connector = p.Connectors.AddNew(name, "Dependency");
            setConnectorInfo(p, connector, xmlNode);
        }

        private void updateConnectorEnd()
        {
            foreach(string eadId in eaIdConnectorIdMap.Keys)
            {
                int id = eaIdConnectorIdMap[eadId];
                EA.Connector con = this.repository.GetConnectorByID(id);
                if (eaIdPackageIdMap.ContainsKey(connectorSrcMap[id]))
                {
                    con.ClientID = repository.GetPackageByID(eaIdPackageIdMap[connectorSrcMap[id]]).Element.ElementID;
                } else if(eAIdElementIdMap.ContainsKey(connectorSrcMap[id]))
                {
                    con.ClientID = repository.GetElementByID(eAIdElementIdMap[connectorSrcMap[id]]).ElementID;
                } else
                {
                    continue;
                }

                if (eaIdPackageIdMap.ContainsKey(connectorTgtMap[id]))
                {
                    con.SupplierID = repository.GetPackageByID(eaIdPackageIdMap[connectorTgtMap[id]]).Element.ElementID;
                }
                else if (eAIdElementIdMap.ContainsKey(connectorTgtMap[id]))
                {
                    con.SupplierID = repository.GetElementByID(eAIdElementIdMap[connectorTgtMap[id]]).ElementID;
                }
                else
                {
                    continue;
                }
                con.Update();
                
            }
        }
        private void addClass(EA.Package p, XmlNode xmlNode)
        {
            EA.Element ele = p.Elements.AddNew(xmlNode.Attributes.GetNamedItem("name").Value, "Class");
            foreach(XmlNode node in xmlNode.ChildNodes)
            {
                string name = node.Attributes.GetNamedItem("name") != null ? node.Attributes.GetNamedItem("name").Value : "";
                if (name == "")
                    continue;
                string type = node.Attributes.GetNamedItem("xmi:type").Value;
                if (type.Contains("Property"))
                {
                    string visibility = node.Attributes.GetNamedItem("visibility") != null ? node.Attributes.GetNamedItem("visibility").Value : "";
                    EA.Attribute attribute = ele.Attributes.AddNew(name, "Property");
                    attribute.Visibility = visibility;
                    foreach (XmlNode att in node.ChildNodes)
                    {
                        string tagName = att.Name;
                        if (tagName == "lowerValue")
                        {
                            attribute.LowerBound = att.Attributes.GetNamedItem("value").Value;
                        }
                        else if (tagName == "upperValue")
                        {
                            attribute.UpperBound = att.Attributes.GetNamedItem("value").Value;
                        }
                        else if (tagName == "type")
                        {
                            attribute.Type = att.Attributes.GetNamedItem("xmi:idref").Value;
                        }
                    }
                    attribute.Update();
                    eaIdAttibuteIdMap[node.Attributes.GetNamedItem("xmi:id").Value] = attribute.AttributeID;
                } else if(type.Contains("Port"))
                {
                    EA.Element portEle = ele.EmbeddedElements.AddNew(name, "Port");
                    portEle.Update();
                    eAIdElementIdMap[node.Attributes.GetNamedItem("xmi:id").Value] = portEle.ElementID;
                }
            }
            ele.Stereotype = this.repository.GetPackageByID(ele.PackageID).Element.Stereotype;
            ele.Update();
            eAIdElementIdMap[xmlNode.Attributes.GetNamedItem("xmi:id").Value] = ele.ElementID;
            ElementId2EAId[ele.ElementID] = xmlNode.Attributes.GetNamedItem("xmi:id").Value;
        }

        private void updateClassAttributeType()
        {
            foreach(string key in eaIdAttibuteIdMap.Keys)
            {
                EA.Attribute attribute = this.repository.GetAttributeByID(eaIdAttibuteIdMap[key]);
                if (eAIdElementIdMap.ContainsKey(attribute.Type))
                {
                    attribute.ClassifierID = this.repository.GetElementByID(eAIdElementIdMap[attribute.Type]).ElementID;
                    attribute.Type = this.repository.GetElementByID(attribute.ClassifierID).Name;
                }
                attribute.Update();
            }
        }

        private void addExtension(XmlNode xmlNode)
        {
           foreach(XmlNode node in xmlNode.ChildNodes)
            {
                string tagName = node.Name;
                switch(tagName)
                {
                    case "diagrams":
                        this.addDiagrams(node);
                        break;
                    case "connectors":
                        this.initConnectors(node);
                        break;
                    case "elements":
                        this.initElements(node);
                        break;
                    default:
                        break;
                }
            }
        }

        private void initConnectors(XmlNode xmlNode)
        {

            foreach(XmlNode node in xmlNode.ChildNodes)
            {
                string tagName = node.Name;
                if(tagName == "connector")
                {
                    string id = node.Attributes.GetNamedItem("xmi:idref").Value;
                    EA.Connector connector = this.repository.GetConnectorByID(eaIdConnectorIdMap[id]);
                    
                    foreach(XmlNode conNode in node.ChildNodes)
                    {
                        string name = conNode.Name;
                        switch (name)
                        {
                            case "properties":
                                string direction = conNode.Attributes.GetNamedItem("direction").Value;
                           //     connector.Type = conNode.Attributes.GetNamedItem("ea_type").Value;
                                connector.Direction = direction;
                                break;
                            case "modifiers":
                                connector.IsRoot = conNode.Attributes.GetNamedItem("isRoot").Value == "true" ? true : false;
                                connector.IsLeaf = conNode.Attributes.GetNamedItem("isLeaf").Value == "true" ? true : false;
                                break;
                            case "source":
                                foreach(XmlNode sNode in conNode.ChildNodes)
                                {
                                    string souceTagname = sNode.Name;
                                    switch(souceTagname)
                                    {
                                        case "type":
                                            connector.ClientEnd.Containment = sNode.Attributes.GetNamedItem("containment").Value;
                                            connector.ClientEnd.Cardinality = sNode.Attributes.GetNamedItem("multiplicity").Value;
                                            connector.ClientEnd.Aggregation = int.Parse(sNode.Attributes.GetNamedItem("aggregation").Value);
                                            break;
                                        case "modifiers":
                                            connector.ClientEnd.IsChangeable = sNode.Attributes.GetNamedItem("changeable").Value;
                                            connector.ClientEnd.IsNavigable = sNode.Attributes.GetNamedItem("isNavigable").Value == "true" ? true : false;
                                            break;
                                        case "role":
                                            connector.ClientEnd.Visibility = sNode.Attributes.GetNamedItem("visibility").Value;
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                break;
                            case "target":
                                foreach (XmlNode sNode in conNode.ChildNodes)
                                {
                                    string souceTagname = sNode.Name;
                                    switch (souceTagname)
                                    {
                                        case "type":
                                            connector.SupplierEnd.Containment = sNode.Attributes.GetNamedItem("containment").Value;
                                            connector.SupplierEnd.Aggregation = int.Parse(sNode.Attributes.GetNamedItem("aggregation").Value);
                                            break;
                                        case "modifiers":
                                            connector.SupplierEnd.IsChangeable = sNode.Attributes.GetNamedItem("changeable").Value;
                                            connector.SupplierEnd.IsNavigable = sNode.Attributes.GetNamedItem("isNavigable").Value == "true" ? true : false;
                                            break;
                                        case "role":
                                            connector.SupplierEnd.Visibility = sNode.Attributes.GetNamedItem("visibility").Value;
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                break;
                            default:
                                break;
                        }
                    }
                    connector.Update();
                }
            }
        }
        private void addDiagrams(XmlNode xmlNode)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                EA.Diagram diagram = null;
                EA.Package p = null;
                Dictionary<string, string> elePos = new Dictionary<string, string>();
                Dictionary<string, string> eleStyle = new Dictionary<string, string>();
                foreach (XmlNode n in node.ChildNodes)
                {
                    string tagName = n.Name;
                    switch (tagName)
                    {
                        case "model":
                            string packageId = n.Attributes.GetNamedItem("package").Value;
                            p = this.repository.GetPackageByID(eaIdPackageIdMap[packageId]);
                            break;
                        case "properties":
                            string name = n.Attributes.GetNamedItem("name").Value;
                            string type = n.Attributes.GetNamedItem("type").Value;
                            diagram = p.Diagrams.AddNew(name, type);
                            diagram.Update();
                            break;
                            
                        case "elements":
                            foreach(XmlNode eleNode in n.ChildNodes)
                            {
                                string subject = eleNode.Attributes.GetNamedItem("subject").Value;
                                string geometry = eleNode.Attributes.GetNamedItem("geometry").Value;
                                string style = eleNode.Attributes.GetNamedItem("style").Value;
                                if (!geometry.StartsWith("Left")) continue;
                                elePos[subject] = geometry;
                                eleStyle[subject] = style;
                            }
                            break;
                        default:
                            break;
                    }
                }
                foreach(string uid in elePos.Keys)
                {
                    EA.DiagramObject diagramObject = null;
                    if (eaIdPackageIdMap.ContainsKey(uid))
                    {
                        EA.Package ep = repository.GetPackageByID(eaIdPackageIdMap[uid]);
                        diagramObject = diagram.DiagramObjects.AddNew(ep.Name, "Package");
                        diagramObject.ElementID = ep.Element.ElementID;
                    } else if(eAIdElementIdMap.ContainsKey(uid))
                    {
                        EA.Element ele = repository.GetElementByID(eAIdElementIdMap[uid]);
                        diagramObject = diagram.DiagramObjects.AddNew(ele.Name, ele.Type);
                        diagramObject.ElementID = ele.ElementID;
                    }
                    if(diagramObject != null)
                    {
                        string pos = elePos[uid];
                        initDiagramObjectPos(diagramObject, pos);
                        diagramObject.Style = eleStyle[uid];
                        diagramObject.Update();
                    }
                }
            }
        }
        private void initElements(XmlNode xmlNode)
        {
            foreach(XmlNode node in xmlNode)
            {
                string type = node.Attributes.GetNamedItem("xmi:type").Value;
                string id = node.Attributes.GetNamedItem("xmi:idref").Value;
                if (type.ToLower().Contains("package"))
                {
                    id = id.Replace("EAPK", "EAID");
                    EA.Package p = this.repository.GetPackageByID(eaIdPackageIdMap[id]);
                    p.IsProtected = node.Attributes.GetNamedItem("scope").Value != "public";
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "properties":
                                p.Element.Stereotype = n.Attributes.GetNamedItem("stereotype").Value;
                                p.Element.IsSpec = n.Attributes.GetNamedItem("isSpecification").Value.ToLower() == "true";
                                break;
                            case "flags":
                                p.IsControlled = n.Attributes.GetNamedItem("iscontrolled").Value.ToLower() == "true";
                                p.IsProtected = n.Attributes.GetNamedItem("isprotected").Value.ToLower() == "true";
                                p.BatchSave = int.Parse(n.Attributes.GetNamedItem("batchsave").Value);
                                p.BatchLoad = int.Parse(n.Attributes.GetNamedItem("batchload").Value);
                                p.UseDTD = n.Attributes.GetNamedItem("usedtd").Value.ToLower() == "true";
                                p.LogXML = n.Attributes.GetNamedItem("logxml").Value.ToLower() == "true";
                                p.Flags = n.Attributes.GetNamedItem("packageFlags").Value;
                                break;
                            default:
                                break;
                        }
                    }
                    p.Update();
                }
                else if (type.ToLower().Contains("class"))
                {
                    EA.Element e = this.repository.GetElementByID(eAIdElementIdMap[id]);
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "model":
                                e.TreePos = int.Parse(n.Attributes.GetNamedItem("tpos").Value);
                                e.Type = n.Attributes.GetNamedItem("ea_eleType").Value;
                                break;
                            case "properties":
                                e.IsRoot = n.Attributes.GetNamedItem("isRoot").Value == "true";
                                e.IsLeaf = n.Attributes.GetNamedItem("isLeaf").Value == "true"; ;
                                e.Abstract = n.Attributes.GetNamedItem("isAbstract").Value;
                                e.IsActive = n.Attributes.GetNamedItem("isActive").Value == "true";
                                e.IsSpec = n.Attributes.GetNamedItem("isSpecification").Value == "true";
                                e.Type = n.Attributes.GetNamedItem("sType").Value;
                                e.Stereotype = n.Attributes.GetNamedItem("stereotype").Value;
                                break;
                            case "attributes":
                                foreach (XmlNode attNode in n.ChildNodes)
                                {
                                    string attid = attNode.Attributes.GetNamedItem("xmi:idref").Value;
                                    if (!eaIdAttibuteIdMap.ContainsKey(attid))
                                        continue;

                                    EA.Attribute attribute = this.repository.GetAttributeByID(eaIdAttibuteIdMap[attid]);
                                    foreach (XmlNode valueNode in attNode.ChildNodes)
                                    {
                                        switch (valueNode.Name)
                                        {
                                            case "properties":
                                                attribute.Type = valueNode.Attributes.GetNamedItem("type").Value;
                                                attribute.IsDerived = valueNode.Attributes.GetNamedItem("derived").Value != "0";
                                                attribute.IsCollection = valueNode.Attributes.GetNamedItem("collection").Value == "true";
                                                attribute.AllowDuplicates = valueNode.Attributes.GetNamedItem("duplicates").Value != "0";
                                                attribute.IsConst = valueNode.Attributes.GetNamedItem("changeability").Value != "changeable";
                                                break;
                                            case "containment":
                                                attribute.Containment = valueNode.Attributes.GetNamedItem("containment").Value;
                                                if (valueNode.Attributes.GetNamedItem("position") != null)
                                                {
                                                    attribute.Pos = int.Parse(valueNode.Attributes.GetNamedItem("position").Value);
                                                }
                                                break;
                                            case "bounds":
                                                string lower = valueNode.Attributes.GetNamedItem("lower") != null ? valueNode.Attributes.GetNamedItem("lower").Value : "";
                                                string upper = valueNode.Attributes.GetNamedItem("upper") != null ? valueNode.Attributes.GetNamedItem("upper").Value : "";
                                                attribute.LowerBound = lower;
                                                attribute.UpperBound = upper;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "tags":
                                foreach (XmlNode tagNode in n.ChildNodes)
                                {

                                    string name = tagNode.Attributes.GetNamedItem("name").Value;
                                    string value = tagNode.Attributes.GetNamedItem("value").Value;
                                    int index = value.IndexOf("#NOTES#");
                                    EA.TaggedValue tv = null;
                                    if (index == -1)
                                    {
                                        tv = e.TaggedValues.AddNew(name, value);
                                    }
                                    else
                                    {
                                        tv = e.TaggedValues.AddNew(name, value.Substring(0, index));
                                        tv.Notes = value.Substring(index + 7);
                                    }
                                    tv.Update();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    e.Update();
                }
                
                else if (type.ToLower().Contains("port"))
                {
                    EA.Element e = this.repository.GetElementByID(eAIdElementIdMap[id]);
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        if (n.Name == "properties")
                        {
                            e.Stereotype = n.Attributes.GetNamedItem("stereotype").Value;
                            e.Update();
                        }
                    }
                } 
            }
        }

        private void initDiagramObjectPos(EA.DiagramObject diagramObject, string pos)
        {
            string[] arr = pos.Split(';');
            foreach(string pp in arr)
            {
                string p = pp.Trim();
                if (p.ToLower().StartsWith("left"))
                {
                    diagramObject.left = int.Parse(p.Substring(5));
                } else if(p.ToLower().StartsWith("top"))
                {
                    diagramObject.top = -int.Parse(p.Substring(4));
                } else if(p.ToLower().StartsWith("right"))
                {
                    diagramObject.right = int.Parse(p.Substring(6));
                } else if(p.ToLower().StartsWith("bottom"))
                {
                    diagramObject.bottom = -int.Parse(p.Substring(7));
                }
               
            }
        }

        private void createNewProject()
        {
            string newProjectPath = this.selectNewProjectFIleDialog.FileName;
            if (File.Exists(newProjectPath))
            {
                File.Delete(newProjectPath);
            }
            this.repository.CreateModel(EA.CreateModelType.cmEAPFromBase, newProjectPath, 0);
            this.repository.OpenFile(newProjectPath);
            this.repository.Models.Refresh();
        }

        private void initPackage(EA.Package topPackage, EAPackage rootPackage)
        {
            EA.Package pacakge = topPackage.Packages.AddNew(rootPackage.Name, "");
            pacakge.Update();
            pacakge.Element.Stereotype = rootPackage.StereoType;
            pacakge.Update();
            List<EAClass> remainEAClass = new List<EAClass>();
            List<int> remaineElementIDList = new List<int>();
            foreach (EAClass c in rootPackage.EAClass)
            {
                EA.Element element = createEAClass(pacakge, c);
                foreach (EAAttribute attribue in c.Attributes)
                {
                    if (attribue.IsEAClass)
                    {
                        remainEAClass.Add(new EAClass(attribue.Name, attribue.Type, attribue.ID));
                        remaineElementIDList.Add(element.ElementID);
                    }
                    else
                    {
                        EA.Attribute attribute = element.Attributes.AddNew(attribue.Name, attribue.Type);
                        attribute.Update();
                    }
                }
                element.Attributes.Refresh();
                element.Update();
                foreach(string axis in c.MeasurementAxisList)
                {
                    measurementAxisMap[axis] = c.ID;
                }
                eAIdElementIdMap[c.ID] = element.ElementID;
                eAElementEAClassMap[element.ElementID] = c;
            }
            foreach (EAClass c in remainEAClass)
            {
                EA.Element element = createEAClass(pacakge, c);
                eAIdElementIdMap[c.ID] = element.ElementID;
                eAElementEAClassMap[element.ElementID] = c;
                element.Update();
            }
            pacakge.Elements.Refresh();
            foreach (EAPackage p in rootPackage.EAPackages)
            {
                initPackage(pacakge, p);
            }
            topPackage.Packages.Refresh();
            EA.Diagram diagram = pacakge.Diagrams.AddNew(rootPackage.Name, "");
            diagram.ExtendedStyle = "ShowTags=1;";
            diagram.Update();
            EADirection eADirection = new EADirection();
            foreach (EA.Package p in pacakge.Packages)
            {
                EADirection direction = eADirection.nextEADirection();
                EA.DiagramObject d = diagram.DiagramObjects.AddNew(direction.ToString(), "");
                d.ElementID = p.Element.ElementID;;
                d.Update();
            }
           foreach(EA.Element e in pacakge.Elements)
            {
                EADirection direction = eADirection.nextEADirection();
                EA.DiagramObject d = diagram.DiagramObjects.AddNew(direction.ToString(), "");
                d.ElementID = e.ElementID;
                d.Update();
                int id = this.addRealizeRelation(e);
                if(id > 0)
                {
                    d = diagram.DiagramObjects.AddNew(direction.ToString(), "");
                    try
                    {
                        d.ElementID = this.repository.GetElementByID(id).ElementID;
                        d.Update();
                    } catch(Exception){
                    }
                    // add aixs
                    int addAixsDiagramId = this.addAxisRelation(eAElementEAClassMap[e.ElementID].RealizeClassID);
                    if (addAixsDiagramId > 0)
                    {
                        direction = eADirection.nextEADirection();
                        d = diagram.DiagramObjects.AddNew(direction.ToString(), "");
                        try
                        {
                            d.ElementID = this.repository.GetElementByID(addAixsDiagramId).ElementID;
                            d.Update();
                        }
                        catch (Exception) { }
                    }
                }
            }
           for(int i = 0; i < remainEAClass.Count; i++)
            {
                this.addMessagePortRelation(diagram, remaineElementIDList[i], eAIdElementIdMap[remainEAClass[i].ID]);
            }
        }

        private void addMessagePortRelation(EA.Diagram diagram, int startElementId, int endElementId)
        {
            EA.Element startElement = this.repository.GetElementByID(startElementId);
            EA.Connector cn = startElement.Connectors.AddNew("", "Association");
            cn.Stereotype = "UOPMessagePort";
            cn.SupplierID = endElementId;
            cn.Direction = "Source -> Destination";
            cn.Update();
            startElement.Connectors.Refresh();
        }

        private int addRealizeRelation(EA.Element element)
        {
            if (eAElementEAClassMap.ContainsKey(element.ElementID))
            {
                EAClass eAClass = eAElementEAClassMap[element.ElementID];
                string aimClassId = eAClass.RealizeClassID;
                if(aimClassId != null && eAIdElementIdMap.ContainsKey(aimClassId))
                {
                    EA.Connector cn = element.Connectors.AddNew("", "Association");
                    cn.SupplierID = eAIdElementIdMap[aimClassId];
                    cn.Stereotype = "Realize";
                    cn.Direction = "Source -> Destination";
                    cn.Update();
                    element.Connectors.Refresh();
                    return cn.SupplierID;
                }
            }
            return -1;
        }

        private int addAxisRelation(string startClassId)
        {
            if (measurementAxisMap.ContainsKey(startClassId))
            {
                string aimClassId = measurementAxisMap[startClassId];
                if (aimClassId != null && eAIdElementIdMap.ContainsKey(aimClassId))
                {
                    EA.Element startElement = this.repository.GetElementByID(eAIdElementIdMap[startClassId]);
                    EA.Element endElement = this.repository.GetElementByID(eAIdElementIdMap[aimClassId]);
                    EA.Connector cn = startElement.Connectors.AddNew("", "Association");
                    cn.Stereotype = "Axis";
                    cn.SupplierID = eAIdElementIdMap[aimClassId];
                    cn.Direction = "Source -> Destination";
                    cn.Update();
                    startElement.Connectors.Refresh();
                    return cn.SupplierID;
                }
            }
            return -1;
        }

        private EA.Element createEAClass(EA.Package package, EAClass eAClass)
        {
            EA.Element element = package.Elements.AddNew(eAClass.Name, "Class");
            element.Stereotype = eAClass.StereoType;
            if(EnumUtil.checkInEnumValues(eAClass.StereoType, typeof(PlatformDataTypeEnum)))
            {
                element.Stereotype = DATA_TYPE;
                EA.TaggedValue taggedValue =  element.TaggedValues.AddNew("IDLType", eAClass.StereoType);
                taggedValue.Update();
            }
            return element;
        }

        private EA.Package getTopPackage()
        {
            EA.Package package = null;
            foreach(EA.Package p in this.repository.Models)
            {
                package = p;
            }
            return package;
        }

        private void importPackage(EA.Package topPackage, EAPackage rootPackage)
        {
            EA.Package package = topPackage.Packages.AddNew(defaultPackageName, "");
            package.Element.Stereotype = rootPackage.StereoType;
            package.Update();
            topPackage.Packages.Refresh();
        }

        private void selectNewProjectFile_Click(object sender, EventArgs e)
        {
            this.selectNewProjectFIleDialog.ShowDialog();
            this.newProjectTextField.Text = this.selectNewProjectFIleDialog.FileName;
            this.setImportButtonStatus();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void keytext_TextChanged(object sender, EventArgs e)
        {
            this.decryptKey = this.keytext.Text.Trim();
            this.setImportButtonStatus();
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.setImportButtonStatus();
        }
    }
}
