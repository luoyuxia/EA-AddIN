using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAddin
{
    public partial class OpenEAPForm : Form
    {
        EA.Repository repository;
        public OpenEAPForm(EA.Repository repository)
        {
            InitializeComponent();
            this.repository = repository;
        }

        private void selectEAPButton_Click(object sender, EventArgs e)
        {
            this.selectFileDialog.ShowDialog();
        }

        private void selectFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.eaInputTextBox.Text =  this.selectFileDialog.FileName;
            this.openEAButton.Enabled = true;
        }

        private void openEAButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.repository.OpenFile(this.selectFileDialog.FileName);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
