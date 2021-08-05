
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
            this.staticFilenameLbl = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.statusLbl = new System.Windows.Forms.Label();
            this.convertBtn = new System.Windows.Forms.Button();
            this.fileNameTbx = new System.Windows.Forms.TextBox();
            this.idTbx = new System.Windows.Forms.TextBox();
            this.idLbl = new System.Windows.Forms.Label();
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
            // staticFilenameLbl
            // 
            this.staticFilenameLbl.AutoSize = true;
            this.staticFilenameLbl.Location = new System.Drawing.Point(60, 124);
            this.staticFilenameLbl.Name = "staticFilenameLbl";
            this.staticFilenameLbl.Size = new System.Drawing.Size(54, 13);
            this.staticFilenameLbl.TabIndex = 2;
            this.staticFilenameLbl.Text = "File Path :";
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
            this.fileNameTbx.Location = new System.Drawing.Point(134, 121);
            this.fileNameTbx.Name = "fileNameTbx";
            this.fileNameTbx.Size = new System.Drawing.Size(334, 20);
            this.fileNameTbx.TabIndex = 8;
            // 
            // idTbx
            // 
            this.idTbx.Location = new System.Drawing.Point(134, 81);
            this.idTbx.Name = "idTbx";
            this.idTbx.Size = new System.Drawing.Size(334, 20);
            this.idTbx.TabIndex = 10;
            // 
            // idLbl
            // 
            this.idLbl.AutoSize = true;
            this.idLbl.Location = new System.Drawing.Point(60, 84);
            this.idLbl.Name = "idLbl";
            this.idLbl.Size = new System.Drawing.Size(74, 13);
            this.idLbl.TabIndex = 9;
            this.idLbl.Text = "Dispenser ID :";
            // 
            // ReportConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(564, 348);
            this.Controls.Add(this.idTbx);
            this.Controls.Add(this.idLbl);
            this.Controls.Add(this.fileNameTbx);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.statusLbl);
            this.Controls.Add(this.staticFilenameLbl);
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
        private System.Windows.Forms.Label staticFilenameLbl;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label statusLbl;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.TextBox fileNameTbx;
        private System.Windows.Forms.TextBox idTbx;
        private System.Windows.Forms.Label idLbl;
    }
}

