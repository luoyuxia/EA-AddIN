namespace MyAddin
{
    partial class ExportForm
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
            this.selectXMLFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.eaProjectFileInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.faceXMLSelectDialog = new System.Windows.Forms.OpenFileDialog();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.exportModelButton = new System.Windows.Forms.Button();
            this.selectFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectXMLFile
            // 
            this.selectXMLFile.CausesValidation = false;
            this.selectXMLFile.Location = new System.Drawing.Point(12, 115);
            this.selectXMLFile.Name = "selectXMLFile";
            this.selectXMLFile.ReadOnly = true;
            this.selectXMLFile.Size = new System.Drawing.Size(864, 28);
            this.selectXMLFile.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 18);
            this.label2.TabIndex = 9;
            this.label2.Text = "FACE XML File:";
            // 
            // eaProjectFileInput
            // 
            this.eaProjectFileInput.Location = new System.Drawing.Point(12, 32);
            this.eaProjectFileInput.Name = "eaProjectFileInput";
            this.eaProjectFileInput.ReadOnly = true;
            this.eaProjectFileInput.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.eaProjectFileInput.Size = new System.Drawing.Size(954, 28);
            this.eaProjectFileInput.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 18);
            this.label1.TabIndex = 7;
            this.label1.Text = "EA Project File:";
            // 
            // faceXMLSelectDialog
            // 
            this.faceXMLSelectDialog.CheckFileExists = false;
            this.faceXMLSelectDialog.CheckPathExists = false;
            this.faceXMLSelectDialog.DefaultExt = "face";
            this.faceXMLSelectDialog.Filter = "FACE Data Model Files|*.face";
            this.faceXMLSelectDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.faceXMLSelectDialog_FileOk);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 216);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(954, 318);
            this.richTextBox1.TabIndex = 13;
            this.richTextBox1.Text = "";
            // 
            // exportModelButton
            // 
            this.exportModelButton.Enabled = false;
            this.exportModelButton.Location = new System.Drawing.Point(12, 159);
            this.exportModelButton.Name = "exportModelButton";
            this.exportModelButton.Size = new System.Drawing.Size(954, 41);
            this.exportModelButton.TabIndex = 12;
            this.exportModelButton.Text = "Export FACE Model Data XML";
            this.exportModelButton.UseVisualStyleBackColor = true;
            this.exportModelButton.Click += new System.EventHandler(this.exportModelButton_Click);
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(882, 115);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(84, 28);
            this.selectFileButton.TabIndex = 11;
            this.selectFileButton.Text = "选择";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 544);
            this.Controls.Add(this.selectXMLFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.eaProjectFileInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.exportModelButton);
            this.Controls.Add(this.selectFileButton);
            this.Name = "ExportForm";
            this.Text = "Enterprise Architect FACE v2.1 Data Model XML Import Utility";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox selectXMLFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox eaProjectFileInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog faceXMLSelectDialog;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button exportModelButton;
        private System.Windows.Forms.Button selectFileButton;
    }
}