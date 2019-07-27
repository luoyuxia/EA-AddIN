namespace MyAddin
{
    partial class NewFaceForm
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
            this.facePathTextbox = new System.Windows.Forms.TextBox();
            this.newFaceFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.selectFileButton = new System.Windows.Forms.Button();
            this.newProjectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Face项目路径";
            // 
            // facePathTextbox
            // 
            this.facePathTextbox.Enabled = false;
            this.facePathTextbox.Location = new System.Drawing.Point(12, 49);
            this.facePathTextbox.Name = "facePathTextbox";
            this.facePathTextbox.Size = new System.Drawing.Size(421, 28);
            this.facePathTextbox.TabIndex = 1;
            // 
            // newFaceFileDialog
            // 
            this.newFaceFileDialog.CheckFileExists = false;
            this.newFaceFileDialog.CheckPathExists = false;
            this.newFaceFileDialog.DefaultExt = "EAP";
            this.newFaceFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.newFaceFileDialog_FileOk);
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(439, 45);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(89, 35);
            this.selectFileButton.TabIndex = 2;
            this.selectFileButton.Text = "选择路径";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            // 
            // newProjectButton
            // 
            this.newProjectButton.Location = new System.Drawing.Point(439, 103);
            this.newProjectButton.Name = "newProjectButton";
            this.newProjectButton.Size = new System.Drawing.Size(89, 37);
            this.newProjectButton.TabIndex = 3;
            this.newProjectButton.Text = "新建项目";
            this.newProjectButton.UseVisualStyleBackColor = true;
            this.newProjectButton.Click += new System.EventHandler(this.newProjectButton_Click);
            // 
            // NewFaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 152);
            this.Controls.Add(this.newProjectButton);
            this.Controls.Add(this.selectFileButton);
            this.Controls.Add(this.facePathTextbox);
            this.Controls.Add(this.label1);
            this.Name = "NewFaceForm";
            this.Text = "新建Face项目";
            this.Load += new System.EventHandler(this.NewFaceForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox facePathTextbox;
        private System.Windows.Forms.OpenFileDialog newFaceFileDialog;
        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.Button newProjectButton;
    }
}