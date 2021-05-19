using System;
using System.Windows.Forms;

namespace SzachyAI {

    internal static class Program {

        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try {
                Application.Run(new Form1());
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }
    }
}