using System;
using System.Windows.Forms;

namespace CapiNetTestApp
{
    internal static class Program
    {
        private static MainForm mainForm = null;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
#if NETCOREAPP5_0
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainForm = new MainForm();
            Application.Run(mainForm);
        }

        public static void LogException(Exception exception) => mainForm.LogException(exception);
        public static void Log(string message) => mainForm.Log(message);
    }
}
