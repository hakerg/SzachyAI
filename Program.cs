using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SzachyAI {

    internal static class Program {

        private enum ProcessDPIAwareness {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        private static void Main() {
            SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try {
                Application.Run(new MenuForm());
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }
    }
}