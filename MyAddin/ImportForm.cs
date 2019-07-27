using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

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
        private Dictionary<int, EAClass> eAElementEAClassMap = new Dictionary<int, EAClass>();
        private Dictionary<string, string> measurementAxisMap = new Dictionary<string, string>();
        public ImportForm(EA.Repository repository)
        {
            InitializeComponent();
            string installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testFile = Path.Combine(installDir, "test.txt");
            MessageBox.Show(File.Exists(testFile).ToString());
        //    MessageBox.Show(File.Exists("test.txt").ToString());
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
            this.faceXMLSelectDialog.ShowDialog();
        }

        private void faceXMLSelectDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.selectXMLFile.Text = this.faceXMLSelectDialog.FileName;
            this.setImportButtonStatus();
        }

        private void setImportButtonStatus()
        {
            if(this.selectNewProjectFIleDialog.FileName.Length > 0 && this.faceXMLSelectDialog.FileName.Length > 0 && this.keytext.Text.Trim().Length > 0)
            {
                this.importModelButton.Enabled = true;
            } else
            {
                this.importModelButton.Enabled = false;
            }
        }

        private void importModelButton_Click(object sender, EventArgs e)
        {
            importFaceProject();       
            this.Dispose();
        }

        private void importFaceProject()
        {
            eAIdElementIdMap.Clear();
            eAElementEAClassMap.Clear();
            measurementAxisMap.Clear();
            createNewProject();
            EA.Package topPackage = getTopPackage();
            FaceFileParser faceFileParser = new FaceFileParser(this.faceXMLSelectDialog.FileName);
            EAPackage rootPackage = faceFileParser.parse(this.decryptKey);
            if (topPackage == null || rootPackage == null)
                return;
            EAPackage additionRootPackage = new EAPackage(defaultDiagramName);
            additionRootPackage.addPackage(rootPackage);
            initPackage(topPackage, additionRootPackage);
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
    }
}
