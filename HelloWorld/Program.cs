using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IOTClient
{
    static class Program
    {
        private static string appGuid = "B3C75D76-8794-4F92-B6CD-18E91D6182BD";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Purpose for debugging release with below line comment.
            Application.Run(new SettingForm());

            // Show the system tray icon.
            //using (ProcessIcon pi = new ProcessIcon())
            //{
            //    pi.Display();

            //    // Make sure the application runs!
            //    using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            //    {
            //        if (!mutex.WaitOne(0, false))
            //        {
            //            MessageBox.Show("Instance already running");
            //            return;
            //        }
            //        Application.Run();
            //    }
            //}
            //lblHelloWorld.Text = "Hello World!";
        }
    }
}
