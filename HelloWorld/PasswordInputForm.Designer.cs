
namespace IOTClient
{
    partial class PasswordInputForm
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
            this.submitPWBtn = new System.Windows.Forms.Button();
            this.pwTbx = new System.Windows.Forms.TextBox();
            this.pwLbl = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.capLocLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // submitPWBtn
            // 
            this.submitPWBtn.Location = new System.Drawing.Point(342, 55);
            this.submitPWBtn.Name = "submitPWBtn";
            this.submitPWBtn.Size = new System.Drawing.Size(75, 23);
            this.submitPWBtn.TabIndex = 0;
            this.submitPWBtn.Text = "Submit";
            this.submitPWBtn.UseVisualStyleBackColor = true;
            this.submitPWBtn.Click += new System.EventHandler(this.submitPWBtn_Click);
            // 
            // pwTbx
            // 
            this.pwTbx.Location = new System.Drawing.Point(87, 70);
            this.pwTbx.Name = "pwTbx";
            this.pwTbx.Size = new System.Drawing.Size(225, 20);
            this.pwTbx.TabIndex = 1;
            this.pwTbx.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pwTbx_KeyDown);
            // 
            // pwLbl
            // 
            this.pwLbl.AutoSize = true;
            this.pwLbl.Location = new System.Drawing.Point(87, 27);
            this.pwLbl.Name = "pwLbl";
            this.pwLbl.Size = new System.Drawing.Size(237, 13);
            this.pwLbl.TabIndex = 2;
            this.pwLbl.Text = "Please submit the password to enable edit mode.";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(342, 85);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 3;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // capLocLbl
            // 
            this.capLocLbl.AutoSize = true;
            this.capLocLbl.Location = new System.Drawing.Point(150, 95);
            this.capLocLbl.Name = "capLocLbl";
            this.capLocLbl.Size = new System.Drawing.Size(91, 13);
            this.capLocLbl.TabIndex = 4;
            this.capLocLbl.Text = "CapsLock is On !!";
            this.capLocLbl.Visible = false;
            // 
            // PasswordInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 149);
            this.Controls.Add(this.capLocLbl);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.pwLbl);
            this.Controls.Add(this.pwTbx);
            this.Controls.Add(this.submitPWBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PasswordInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button submitPWBtn;
        private System.Windows.Forms.TextBox pwTbx;
        private System.Windows.Forms.Label pwLbl;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label capLocLbl;
    }
}