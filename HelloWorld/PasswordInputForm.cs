using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IOTClient
{
    public partial class PasswordInputForm : Form
    {
        public PasswordInputForm()
        {
            InitializeComponent();
            this.pwTbx.PasswordChar = '\u25CF';
            Application.Idle += Application_Idle;
        }
        void Application_Idle(object sender, EventArgs e)
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                this.capLocLbl.Visible = true;
            }
            else
            {
                this.capLocLbl.Visible = false;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Application.Idle -= Application_Idle;
            base.OnFormClosed(e);
        }
        //remove the entire system menu:
        private const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }

        private void submitPWBtn_Click(object sender, EventArgs e)
        {
            if(pwTbx.Text == "")
            {
                MessageBox.Show("Please provide Password");
                return;
            }

            if(pwTbx.Text != "itadm1n@01")
            {
                MessageBox.Show("Incorrect Password");
                return;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
