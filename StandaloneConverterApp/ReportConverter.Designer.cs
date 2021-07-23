
namespace StandaloneConverterApp
{
    partial class ReportConverter
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
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.selectFilebtn = new System.Windows.Forms.Button();
            this.outputBrowseDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.staticFilenameLbl = new System.Windows.Forms.Label();
            this.fileNameLbl = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "OpenFileDialog";
            // 
            // selectFilebtn
            // 
            this.selectFilebtn.Location = new System.Drawing.Point(169, 220);
            this.selectFilebtn.Name = "selectFilebtn";
            this.selectFilebtn.Size = new System.Drawing.Size(170, 62);
            this.selectFilebtn.TabIndex = 0;
            this.selectFilebtn.Text = "Select File";
            this.selectFilebtn.UseVisualStyleBackColor = true;
            this.selectFilebtn.Click += new System.EventHandler(this.selectFilebtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(125, 162);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(255, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // staticFilenameLbl
            // 
            this.staticFilenameLbl.AutoSize = true;
            this.staticFilenameLbl.Location = new System.Drawing.Point(104, 87);
            this.staticFilenameLbl.Name = "staticFilenameLbl";
            this.staticFilenameLbl.Size = new System.Drawing.Size(55, 13);
            this.staticFilenameLbl.TabIndex = 2;
            this.staticFilenameLbl.Text = "Filename :";
            // 
            // fileNameLbl
            // 
            this.fileNameLbl.AutoSize = true;
            this.fileNameLbl.Location = new System.Drawing.Point(166, 86);
            this.fileNameLbl.Name = "fileNameLbl";
            this.fileNameLbl.Size = new System.Drawing.Size(33, 13);
            this.fileNameLbl.TabIndex = 3;
            this.fileNameLbl.Text = "None";
            // 
            // ReportConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(510, 385);
            this.Controls.Add(this.fileNameLbl);
            this.Controls.Add(this.staticFilenameLbl);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.selectFilebtn);
            this.Name = "ReportConverter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReportConverter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.Button selectFilebtn;
        private System.Windows.Forms.FolderBrowserDialog outputBrowseDialog;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label staticFilenameLbl;
        private System.Windows.Forms.Label fileNameLbl;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

