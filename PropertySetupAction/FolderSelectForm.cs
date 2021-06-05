using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PropertySetupAction
{
    public partial class FolderSelectForm : Form
    {
        public FolderSelectForm()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            this.TopMost = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            bool valid = false;
            if (!String.IsNullOrEmpty(txtDBLocation.Text) && !String.IsNullOrEmpty(txtHistoryLocation.Text) && !String.IsNullOrEmpty(txtCustomerId.Text))
                valid = VerifyInputInfo(txtDBLocation.Text, txtHistoryLocation.Text, txtCustomerId.Text);

            if (!valid)
            {
                MessageBox.Show("You license information does not appear to be valid. Please try again.", "Invalid info");
            }
            else
            {
                this.DialogResult = DialogResult.Yes;
            }
        }

        private bool VerifyInputInfo(string dbLocation, string historyLocation, string customerId)
        {
            // Connect to License server check customerId and run algorithm to file in dblocation and historylocation

            if(customerId == "ABC123ADMIN")
            {
                return true;
            }
            return false;
        }

        private void btnDatabaseSelect_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            var fsd = new FolderSelectDialog();
            fsd.Title = "What to select";
            fsd.InitialDirectory = @"c:\";
            if (fsd.ShowDialog(IntPtr.Zero))
            {
                Console.WriteLine(fsd.FileName);
                txtDBLocation.Text = fsd.FileName;
                this.TopMost = true;
            }
        }

        private void btnCSVSelect_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            var fsd = new FolderSelectDialog();
            fsd.Title = "What to select";
            fsd.InitialDirectory = @"c:\";
            if (fsd.ShowDialog(IntPtr.Zero))
            {
                Console.WriteLine(fsd.FileName);
                txtHistoryLocation.Text = fsd.FileName;
                this.TopMost = true;
            }
        }
    }
}
