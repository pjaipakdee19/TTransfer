
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
            this.staticSavePathLbl = new System.Windows.Forms.Label();
            this.savePathLbl = new System.Windows.Forms.Label();
            this.statusLbl = new System.Windows.Forms.Label();
            this.convertBtn = new System.Windows.Forms.Button();
            this.fileNameTbx = new System.Windows.Forms.TextBox();
            this.exportPathTbx = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "OpenFileDialog";
            // 
            // selectFilebtn
            // 
            this.selectFilebtn.Location = new System.Drawing.Point(97, 179);
            this.selectFilebtn.Name = "selectFilebtn";
            this.selectFilebtn.Size = new System.Drawing.Size(166, 36);
            this.selectFilebtn.TabIndex = 0;
            this.selectFilebtn.Text = "Select File";
            this.selectFilebtn.UseVisualStyleBackColor = true;
            this.selectFilebtn.Click += new System.EventHandler(this.selectFilebtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(122, 288);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(255, 23);
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Visible = false;
            // 
            // staticFilenameLbl
            // 
            this.staticFilenameLbl.AutoSize = true;
            this.staticFilenameLbl.Location = new System.Drawing.Point(60, 124);
            this.staticFilenameLbl.Name = "staticFilenameLbl";
            this.staticFilenameLbl.Size = new System.Drawing.Size(54, 13);
            this.staticFilenameLbl.TabIndex = 2;
            this.staticFilenameLbl.Text = "File Path :";
            // 
            // fileNameLbl
            // 
            this.fileNameLbl.AutoSize = true;
            this.fileNameLbl.Location = new System.Drawing.Point(159, 63);
            this.fileNameLbl.Name = "fileNameLbl";
            this.fileNameLbl.Size = new System.Drawing.Size(33, 13);
            this.fileNameLbl.TabIndex = 3;
            this.fileNameLbl.Text = "None";
            this.fileNameLbl.Visible = false;
            // 
            // staticSavePathLbl
            // 
            this.staticSavePathLbl.AutoSize = true;
            this.staticSavePathLbl.Location = new System.Drawing.Point(12, 257);
            this.staticSavePathLbl.Name = "staticSavePathLbl";
            this.staticSavePathLbl.Size = new System.Drawing.Size(68, 13);
            this.staticSavePathLbl.TabIndex = 4;
            this.staticSavePathLbl.Text = "Export Path :";
            this.staticSavePathLbl.Visible = false;
            // 
            // savePathLbl
            // 
            this.savePathLbl.AutoSize = true;
            this.savePathLbl.Location = new System.Drawing.Point(213, 63);
            this.savePathLbl.Name = "savePathLbl";
            this.savePathLbl.Size = new System.Drawing.Size(33, 13);
            this.savePathLbl.TabIndex = 5;
            this.savePathLbl.Text = "None";
            this.savePathLbl.Visible = false;
            // 
            // statusLbl
            // 
            this.statusLbl.AutoSize = true;
            this.statusLbl.Location = new System.Drawing.Point(483, 315);
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(50, 13);
            this.statusLbl.TabIndex = 6;
            this.statusLbl.Text = "Stand By";
            // 
            // convertBtn
            // 
            this.convertBtn.Location = new System.Drawing.Point(302, 179);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(166, 36);
            this.convertBtn.TabIndex = 7;
            this.convertBtn.Text = "Convert to BI format ";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // fileNameTbx
            // 
            this.fileNameTbx.Location = new System.Drawing.Point(122, 121);
            this.fileNameTbx.Name = "fileNameTbx";
            this.fileNameTbx.Size = new System.Drawing.Size(346, 20);
            this.fileNameTbx.TabIndex = 8;
            // 
            // exportPathTbx
            // 
            this.exportPathTbx.Location = new System.Drawing.Point(86, 250);
            this.exportPathTbx.Name = "exportPathTbx";
            this.exportPathTbx.Size = new System.Drawing.Size(393, 20);
            this.exportPathTbx.TabIndex = 9;
            this.exportPathTbx.Visible = false;
            // 
            // ReportConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(564, 348);
            this.Controls.Add(this.exportPathTbx);
            this.Controls.Add(this.fileNameTbx);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.statusLbl);
            this.Controls.Add(this.savePathLbl);
            this.Controls.Add(this.staticSavePathLbl);
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
        private System.Windows.Forms.Label staticSavePathLbl;
        private System.Windows.Forms.Label savePathLbl;
        private System.Windows.Forms.Label statusLbl;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.TextBox fileNameTbx;
        private System.Windows.Forms.TextBox exportPathTbx;
    }
}

