using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAddin
{
    public partial class NewFaceForm : Form
    {
        private EA.Repository repository;
        private string faceProjectPath = "";
        private string installDir = "";
        public NewFaceForm()
        {
            InitializeComponent();
        }

        public NewFaceForm(EA.Repository repository)
        {
            InitializeComponent();
            this.installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.repository = repository;
        }

        private void NewFaceForm_Load(object sender, EventArgs e)
        {

        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            this.newFaceFileDialog.ShowDialog();
        }

        private void newFaceFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.faceProjectPath = this.newFaceFileDialog.FileName;
            this.facePathTextbox.Text = this.newFaceFileDialog.FileName;
        }

        private void newProjectButton_Click(object sender, EventArgs e)
        {
            // copy from face template file to target path
            string faceTemplatePath = Path.Combine(this.installDir, "profile_template.EAP");
            File.Copy(faceTemplatePath, this.faceProjectPath, true);
            // then open the target project
            this.repository.CreateModel(EA.CreateModelType.cmEAPFromBase, this.faceProjectPath, 0);
            this.repository.OpenFile(this.faceProjectPath);
            this.repository.Models.Refresh();
            this.Dispose();
        }
    }
}
