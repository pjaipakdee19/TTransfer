
namespace IOTClient
{
    partial class SettingForm
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
            this.components = new System.ComponentModel.Container();
            this.btnExport1 = new System.Windows.Forms.Button();
            this.lblHelloWorld = new System.Windows.Forms.Label();
            this.databaseFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnDatabaseSelect = new System.Windows.Forms.Button();
            this.btnSeletectHistoryCVS = new System.Windows.Forms.Button();
            this.lblDatabasePath = new System.Windows.Forms.Label();
            this.databaseLocationTextbox = new System.Windows.Forms.TextBox();
            this.posHistoryLocationTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxShopDispenVal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblDatabaseVersion = new System.Windows.Forms.Label();
            this.lblDatabaseVersionText = new System.Windows.Forms.Label();
            this.lblDatabaseTime = new System.Windows.Forms.Label();
            this.lblDatabaseCheckVal = new System.Windows.Forms.Label();
            this.btnCheckShopID = new System.Windows.Forms.Button();
            this.HistoryExportDateTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SaveInputData = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnDownloadUpdate = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // btnExport1
            // 
            this.btnExport1.Location = new System.Drawing.Point(409, 381);
            this.btnExport1.Name = "btnExport1";
            this.btnExport1.Size = new System.Drawing.Size(145, 23);
            this.btnExport1.TabIndex = 0;
            this.btnExport1.Text = "ส่งออกประวัติ POS";
            this.btnExport1.UseVisualStyleBackColor = true;
            this.btnExport1.Click += new System.EventHandler(this.btnExport1_ClickAsync);
            // 
            // lblHelloWorld
            // 
            this.lblHelloWorld.AutoSize = true;
            this.lblHelloWorld.Location = new System.Drawing.Point(351, 183);
            this.lblHelloWorld.Name = "lblHelloWorld";
            this.lblHelloWorld.Size = new System.Drawing.Size(0, 13);
            this.lblHelloWorld.TabIndex = 1;
            // 
            // btnDatabaseSelect
            // 
            this.btnDatabaseSelect.Location = new System.Drawing.Point(607, 140);
            this.btnDatabaseSelect.Name = "btnDatabaseSelect";
            this.btnDatabaseSelect.Size = new System.Drawing.Size(75, 23);
            this.btnDatabaseSelect.TabIndex = 2;
            this.btnDatabaseSelect.Text = "...";
            this.btnDatabaseSelect.UseVisualStyleBackColor = true;
            this.btnDatabaseSelect.Click += new System.EventHandler(this.btnDatabaseSelect_Click);
            // 
            // btnSeletectHistoryCVS
            // 
            this.btnSeletectHistoryCVS.Location = new System.Drawing.Point(607, 182);
            this.btnSeletectHistoryCVS.Name = "btnSeletectHistoryCVS";
            this.btnSeletectHistoryCVS.Size = new System.Drawing.Size(75, 23);
            this.btnSeletectHistoryCVS.TabIndex = 3;
            this.btnSeletectHistoryCVS.Text = "...";
            this.btnSeletectHistoryCVS.UseVisualStyleBackColor = true;
            this.btnSeletectHistoryCVS.Click += new System.EventHandler(this.btnSeletectHistoryCVS_Click);
            // 
            // lblDatabasePath
            // 
            this.lblDatabasePath.AutoSize = true;
            this.lblDatabasePath.Location = new System.Drawing.Point(110, 149);
            this.lblDatabasePath.Name = "lblDatabasePath";
            this.lblDatabasePath.Size = new System.Drawing.Size(76, 13);
            this.lblDatabasePath.TabIndex = 5;
            this.lblDatabasePath.Text = "ฐานข้อมูล POS";
            // 
            // databaseLocationTextbox
            // 
            this.databaseLocationTextbox.Location = new System.Drawing.Point(198, 142);
            this.databaseLocationTextbox.Name = "databaseLocationTextbox";
            this.databaseLocationTextbox.Size = new System.Drawing.Size(383, 20);
            this.databaseLocationTextbox.TabIndex = 6;
            // 
            // posHistoryLocationTextBox
            // 
            this.posHistoryLocationTextBox.Location = new System.Drawing.Point(198, 185);
            this.posHistoryLocationTextBox.Name = "posHistoryLocationTextBox";
            this.posHistoryLocationTextBox.Size = new System.Drawing.Size(383, 20);
            this.posHistoryLocationTextBox.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(110, 192);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "ประวัติ POS";
            // 
            // tbxShopDispenVal
            // 
            this.tbxShopDispenVal.Location = new System.Drawing.Point(198, 101);
            this.tbxShopDispenVal.Name = "tbxShopDispenVal";
            this.tbxShopDispenVal.Size = new System.Drawing.Size(184, 20);
            this.tbxShopDispenVal.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(110, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "รหัสร้านค้า";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(74, 381);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "ครวจสอบรุ่นฐานข้อมูล";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // lblDatabaseVersion
            // 
            this.lblDatabaseVersion.AutoSize = true;
            this.lblDatabaseVersion.Location = new System.Drawing.Point(74, 324);
            this.lblDatabaseVersion.Name = "lblDatabaseVersion";
            this.lblDatabaseVersion.Size = new System.Drawing.Size(71, 13);
            this.lblDatabaseVersion.TabIndex = 12;
            this.lblDatabaseVersion.Text = "รุ่นฐานข้อมูล :";
            // 
            // lblDatabaseVersionText
            // 
            this.lblDatabaseVersionText.AutoSize = true;
            this.lblDatabaseVersionText.Location = new System.Drawing.Point(152, 324);
            this.lblDatabaseVersionText.Name = "lblDatabaseVersionText";
            this.lblDatabaseVersionText.Size = new System.Drawing.Size(31, 13);
            this.lblDatabaseVersionText.TabIndex = 13;
            this.lblDatabaseVersionText.Text = "0.0.1";
            // 
            // lblDatabaseTime
            // 
            this.lblDatabaseTime.AutoSize = true;
            this.lblDatabaseTime.Location = new System.Drawing.Point(74, 349);
            this.lblDatabaseTime.Name = "lblDatabaseTime";
            this.lblDatabaseTime.Size = new System.Drawing.Size(81, 13);
            this.lblDatabaseTime.TabIndex = 14;
            this.lblDatabaseTime.Text = "ตรวจสอบล่าสุด :";
            // 
            // lblDatabaseCheckVal
            // 
            this.lblDatabaseCheckVal.AutoSize = true;
            this.lblDatabaseCheckVal.Location = new System.Drawing.Point(161, 349);
            this.lblDatabaseCheckVal.Name = "lblDatabaseCheckVal";
            this.lblDatabaseCheckVal.Size = new System.Drawing.Size(133, 13);
            this.lblDatabaseCheckVal.TabIndex = 15;
            this.lblDatabaseCheckVal.Text = "อาทิตย์ 15 พฤษภาคม 2564";
            // 
            // btnCheckShopID
            // 
            this.btnCheckShopID.Location = new System.Drawing.Point(409, 98);
            this.btnCheckShopID.Name = "btnCheckShopID";
            this.btnCheckShopID.Size = new System.Drawing.Size(75, 23);
            this.btnCheckShopID.TabIndex = 16;
            this.btnCheckShopID.Text = "ตรวจสอบ";
            this.btnCheckShopID.UseVisualStyleBackColor = true;
            this.btnCheckShopID.Visible = false;
            this.btnCheckShopID.Click += new System.EventHandler(this.btnCheckShopID_Click);
            // 
            // HistoryExportDateTime
            // 
            this.HistoryExportDateTime.AutoSize = true;
            this.HistoryExportDateTime.Location = new System.Drawing.Point(493, 349);
            this.HistoryExportDateTime.Name = "HistoryExportDateTime";
            this.HistoryExportDateTime.Size = new System.Drawing.Size(169, 13);
            this.HistoryExportDateTime.TabIndex = 18;
            this.HistoryExportDateTime.Text = "07:20 , อาทิตย์ 15 พฤษภาคม 2564";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(406, 349);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "เวลาส่งออกล่าสุด :";
            // 
            // SaveInputData
            // 
            this.SaveInputData.Location = new System.Drawing.Point(219, 233);
            this.SaveInputData.Name = "SaveInputData";
            this.SaveInputData.Size = new System.Drawing.Size(265, 23);
            this.SaveInputData.TabIndex = 19;
            this.SaveInputData.Text = "บันทึก";
            this.SaveInputData.UseVisualStyleBackColor = true;
            this.SaveInputData.Click += new System.EventHandler(this.SaveInputData_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "IOTClient";
            this.notifyIcon1.Visible = true;
            // 
            // btnDownloadUpdate
            // 
            this.btnDownloadUpdate.Location = new System.Drawing.Point(257, 380);
            this.btnDownloadUpdate.Name = "btnDownloadUpdate";
            this.btnDownloadUpdate.Size = new System.Drawing.Size(94, 23);
            this.btnDownloadUpdate.TabIndex = 20;
            this.btnDownloadUpdate.Text = "TestDownload";
            this.btnDownloadUpdate.UseVisualStyleBackColor = true;
            this.btnDownloadUpdate.Visible = false;
            this.btnDownloadUpdate.Click += new System.EventHandler(this.btnDownloadUpdate_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(251, 409);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 23);
            this.progressBar1.TabIndex = 21;
            this.progressBar1.Visible = false;
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnDownloadUpdate);
            this.Controls.Add(this.SaveInputData);
            this.Controls.Add(this.HistoryExportDateTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCheckShopID);
            this.Controls.Add(this.lblDatabaseCheckVal);
            this.Controls.Add(this.lblDatabaseTime);
            this.Controls.Add(this.lblDatabaseVersionText);
            this.Controls.Add(this.lblDatabaseVersion);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbxShopDispenVal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.posHistoryLocationTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.databaseLocationTextbox);
            this.Controls.Add(this.lblDatabasePath);
            this.Controls.Add(this.btnSeletectHistoryCVS);
            this.Controls.Add(this.btnDatabaseSelect);
            this.Controls.Add(this.lblHelloWorld);
            this.Controls.Add(this.btnExport1);
            this.Name = "SettingForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TOA IOT Setting";
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExport1;
        private System.Windows.Forms.Label lblHelloWorld;
        private System.Windows.Forms.FolderBrowserDialog databaseFolderBrowserDialog;
        private System.Windows.Forms.Button btnDatabaseSelect;
        private System.Windows.Forms.Button btnSeletectHistoryCVS;
        private System.Windows.Forms.Label lblDatabasePath;
        private System.Windows.Forms.TextBox databaseLocationTextbox;
        private System.Windows.Forms.TextBox posHistoryLocationTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxShopDispenVal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblDatabaseVersion;
        private System.Windows.Forms.Label lblDatabaseVersionText;
        private System.Windows.Forms.Label lblDatabaseTime;
        private System.Windows.Forms.Label lblDatabaseCheckVal;
        private System.Windows.Forms.Button btnCheckShopID;
        private System.Windows.Forms.Label HistoryExportDateTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button SaveInputData;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnDownloadUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

