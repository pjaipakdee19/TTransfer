using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PropertySetupAction
{
    public class PropertySetupAction
    {
        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {
            FolderSelectForm folderform = new FolderSelectForm();
            if (folderform.ShowDialog() == DialogResult.Cancel)
                return ActionResult.UserExit;

            return ActionResult.Success;
        }
    }
}
