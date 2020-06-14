namespace MyAddin
{
    partial class ImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.eaProjectFileInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.selectXMLFile = new System.Windows.Forms.TextBox();
            this.XMLSelectDialog = new System.Windows.Forms.OpenFileDialog();
            this.selectFileButton = new System.Windows.Forms.Button();
            this.importModelButton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.newProjectTextField = new System.Windows.Forms.TextBox();
            this.selectNewProjectFile = new System.Windows.Forms.Button();
            this.selectNewProjectFIleDialog = new System.Windows.Forms.OpenFileDialog();
            this.keytext = new System.Windows.Forms.TextBox();
            this.key = new System.Windows.Forms.Label();
            this.decrypt_checkbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "EA Project File:";
            // 
            // eaProjectFileInput
            // 
            this.eaProjectFileInput.Location = new System.Drawing.Point(12, 30);
            this.eaProjectFileInput.Name = "eaProjectFileInput";
            this.eaProjectFileInput.ReadOnly = true;
            this.eaProjectFileInput.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.eaProjectFileInput.Size = new System.Drawing.Size(954, 28);
            this.eaProjectFileInput.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "XMI File:";
            // 
            // selectXMLFile
            // 
            this.selectXMLFile.CausesValidation = false;
            this.selectXMLFile.Location = new System.Drawing.Point(12, 113);
            this.selectXMLFile.Name = "selectXMLFile";
            this.selectXMLFile.ReadOnly = true;
            this.selectXMLFile.Size = new System.Drawing.Size(864, 28);
            this.selectXMLFile.TabIndex = 3;
            // 
            // XMLSelectDialog
            // 
            this.XMLSelectDialog.Filter = "Files|*.xml";
            this.XMLSelectDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.faceXMLSelectDialog_FileOk);
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(882, 113);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(84, 28);
            this.selectFileButton.TabIndex = 4;
            this.selectFileButton.Text = "选择";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            // 
            // importModelButton
            // 
            this.importModelButton.Enabled = false;
            this.importModelButton.Location = new System.Drawing.Point(12, 257);
            this.importModelButton.Name = "importModelButton";
            this.importModelButton.Size = new System.Drawing.Size(954, 41);
            this.importModelButton.TabIndex = 5;
            this.importModelButton.Text = "Import XMI";
            this.importModelButton.UseVisualStyleBackColor = true;
            this.importModelButton.Click += new System.EventHandler(this.importModelButton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 304);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(961, 228);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(224, 18);
            this.label3.TabIndex = 7;
            this.label3.Text = "EA New Project File Path";
            // 
            // newProjectTextField
            // 
            this.newProjectTextField.Enabled = false;
            this.newProjectTextField.Location = new System.Drawing.Point(15, 189);
            this.newProjectTextField.Name = "newProjectTextField";
            this.newProjectTextField.Size = new System.Drawing.Size(861, 28);
            this.newProjectTextField.TabIndex = 8;
            // 
            // selectNewProjectFile
            // 
            this.selectNewProjectFile.Location = new System.Drawing.Point(882, 189);
            this.selectNewProjectFile.Name = "selectNewProjectFile";
            this.selectNewProjectFile.Size = new System.Drawing.Size(84, 28);
            this.selectNewProjectFile.TabIndex = 9;
            this.selectNewProjectFile.Text = "选择";
            this.selectNewProjectFile.UseVisualStyleBackColor = true;
            this.selectNewProjectFile.Click += new System.EventHandler(this.selectNewProjectFile_Click);
            // 
            // selectNewProjectFIleDialog
            // 
            this.selectNewProjectFIleDialog.AddExtension = false;
            this.selectNewProjectFIleDialog.CheckFileExists = false;
            this.selectNewProjectFIleDialog.CheckPathExists = false;
            this.selectNewProjectFIleDialog.DefaultExt = "EAP";
            // 
            // keytext
            // 
            this.keytext.Location = new System.Drawing.Point(15, 223);
            this.keytext.Name = "keytext";
            this.keytext.Size = new System.Drawing.Size(182, 28);
            this.keytext.TabIndex = 10;
            this.keytext.TextChanged += new System.EventHandler(this.keytext_TextChanged);
            // 
            // key
            // 
            this.key.AutoSize = true;
            this.key.Location = new System.Drawing.Point(203, 228);
            this.key.Name = "key";
            this.key.Size = new System.Drawing.Size(44, 18);
            this.key.TabIndex = 11;
            this.key.Text = "密码";
            this.key.Click += new System.EventHandler(this.label4_Click);
            // 
            // decrypt_checkbox
            // 
            this.decrypt_checkbox.AutoSize = true;
            this.decrypt_checkbox.Location = new System.Drawing.Point(882, 227);
            this.decrypt_checkbox.Name = "decrypt_checkbox";
            this.decrypt_checkbox.Size = new System.Drawing.Size(70, 22);
            this.decrypt_checkbox.TabIndex = 12;
            this.decrypt_checkbox.Text = "解密";
            this.decrypt_checkbox.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.decrypt_checkbox.UseVisualStyleBackColor = true;
            this.decrypt_checkbox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 544);
            this.Controls.Add(this.decrypt_checkbox);
            this.Controls.Add(this.key);
            this.Controls.Add(this.keytext);
            this.Controls.Add(this.selectNewProjectFile);
            this.Controls.Add(this.newProjectTextField);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.importModelButton);
            this.Controls.Add(this.selectFileButton);
            this.Controls.Add(this.selectXMLFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.eaProjectFileInput);
            this.Controls.Add(this.label1);
            this.Name = "ImportForm";
            this.Text = "Enterprise Architect FACE v2.1 Data Model XML Export Utility";
            this.Load += new System.EventHandler(this.ImportForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox eaProjectFileInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox selectXMLFile;
        private System.Windows.Forms.OpenFileDialog XMLSelectDialog;
        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.Button importModelButton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox newProjectTextField;
        private System.Windows.Forms.Button selectNewProjectFile;
        private System.Windows.Forms.OpenFileDialog selectNewProjectFIleDialog;
        private System.Windows.Forms.TextBox keytext;
        private System.Windows.Forms.Label key;
        private System.Windows.Forms.CheckBox decrypt_checkbox;
    }
}