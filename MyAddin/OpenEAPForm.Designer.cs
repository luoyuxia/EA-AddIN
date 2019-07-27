namespace MyAddin
{
    partial class OpenEAPForm
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
            this.eaInputTextBox = new System.Windows.Forms.TextBox();
            this.selectEAPButton = new System.Windows.Forms.Button();
            this.openEAButton = new System.Windows.Forms.Button();
            this.selectFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "select eap project";
            // 
            // eaInputTextBox
            // 
            this.eaInputTextBox.Enabled = false;
            this.eaInputTextBox.Location = new System.Drawing.Point(15, 40);
            this.eaInputTextBox.Name = "eaInputTextBox";
            this.eaInputTextBox.Size = new System.Drawing.Size(689, 28);
            this.eaInputTextBox.TabIndex = 1;
            // 
            // selectEAPButton
            // 
            this.selectEAPButton.Location = new System.Drawing.Point(713, 40);
            this.selectEAPButton.Name = "selectEAPButton";
            this.selectEAPButton.Size = new System.Drawing.Size(75, 28);
            this.selectEAPButton.TabIndex = 2;
            this.selectEAPButton.Text = "选择";
            this.selectEAPButton.UseVisualStyleBackColor = true;
            this.selectEAPButton.Click += new System.EventHandler(this.selectEAPButton_Click);
            // 
            // openEAButton
            // 
            this.openEAButton.Enabled = false;
            this.openEAButton.Location = new System.Drawing.Point(15, 94);
            this.openEAButton.Name = "openEAButton";
            this.openEAButton.Size = new System.Drawing.Size(782, 35);
            this.openEAButton.TabIndex = 3;
            this.openEAButton.Text = "Open EA Project";
            this.openEAButton.UseVisualStyleBackColor = true;
            this.openEAButton.Click += new System.EventHandler(this.openEAButton_Click);
            // 
            // selectFileDialog
            // 
            this.selectFileDialog.Filter = "EA项目|*.EAP";
            this.selectFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.selectFileDialog_FileOk);
            // 
            // OpenEAPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.openEAButton);
            this.Controls.Add(this.selectEAPButton);
            this.Controls.Add(this.eaInputTextBox);
            this.Controls.Add(this.label1);
            this.Name = "OpenEAPForm";
            this.Text = "OpenEAPForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox eaInputTextBox;
        private System.Windows.Forms.Button selectEAPButton;
        private System.Windows.Forms.Button openEAButton;
        private System.Windows.Forms.OpenFileDialog selectFileDialog;
    }
}