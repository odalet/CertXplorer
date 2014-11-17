using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Delta.CapiNet;

namespace TestCapiNet
{
    internal static class Program
    {
        private static MainForm mainForm = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            mainForm = new MainForm();
            Application.Run(mainForm);
        }

        public static void LogException(Exception exception)
        {
            mainForm.LogException(exception);
        }

        public static void Log(string message)
        {
            mainForm.Log(message);
        }
    }
}
