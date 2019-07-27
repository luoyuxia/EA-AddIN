using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace MyAddin
{
    public partial class ExportForm : Form
    {
        private EA.Repository repository;
        private FaceTypeEnumTool faceTypeEnumTool = new FaceTypeEnumTool();
        public ExportForm(EA.Repository repository)
        {
            InitializeComponent();
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
            this.exportModelButton.Enabled = true;
        }

        private void exportModelButton_Click(object sender, EventArgs e)
        {
            EA.Package topPackage = getRootPackage();
            EAPackage rootPackage = null;
            if(topPackage != null)
            {
                rootPackage = new EAPackage(topPackage.Name, topPackage.Element.Stereotype);
                initPackage(topPackage, rootPackage);
            }
            if (rootPackage == null)
            {
                MessageBox.Show(String.Format("Sorry, can't export EA model {0} to face model file for no rootpackge!", topPackage.Name));
            }
            else
            {
                this.exportToXML(rootPackage);
                MessageBox.Show(String.Format("Successfully to export EA Project {0} to face model", this.repository.ConnectionString));
                this.Dispose();
            }
        }


        private void exportToXML(EAPackage rootPackage)
        {
            FaceFileExporter faceFileExporter = new FaceFileExporter();
            faceFileExporter.export(rootPackage, this.faceXMLSelectDialog.FileName);
        }

        private void initPackage(EA.Package topPackage, EAPackage rootPackage)
        {
            foreach(EA.Package p in topPackage.Packages)
            {
                EAPackage newPackage = new EAPackage(p.Name, p.Element.Stereotype);
                EAClass previousClass = null;
                foreach (EA.Element element in p.Elements)
                {
                    EAClass eAClass = new EAClass(element.Name, element.Stereotype);
                    if (previousClass != null)
                    {
                        eAClass.addAttribute(new EAAttribute(previousClass.Name, previousClass.StereoType));
                    }
                    foreach (EA.Attribute attribute in element.Attributes)
                    {
                        EAAttribute eAAttribute = new EAAttribute(attribute.Name, attribute.Stereotype);
                        eAClass.addAttribute(eAAttribute);
                    }
                    if(faceTypeEnumTool.isChildrenElemnt(element.Stereotype))
                    {
                        previousClass = eAClass;
                    }
                    else
                    {
                        newPackage.addClass(eAClass);
                        previousClass = null;
                    } 
                }
                rootPackage.addPackage(newPackage);
                initPackage(p, newPackage);
            }
        }

        private EA.Package getRootPackage()
        {
            EA.Package rootPackage = null;
            foreach(EA.Package p in this.repository.Models)
            {
                foreach(EA.Package p1 in p.Packages)
                {
                    foreach(EA.Package p2 in p1.Packages)
                    {
                        rootPackage = p2;
                    }
                }
            }
            return rootPackage;
        }
    }
}
