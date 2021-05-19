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
<<<<<<< HEAD
            try {
                Application.Run(new Form1());
            } catch (Exception e) {
=======
            try
            {
                Application.Run(new MenuForm());
            }
            catch (Exception e)
            {
>>>>>>> 3c94f40f9719ddbcbddb8abf480f54a7267aecfe
                MessageBox.Show(e.Message);
            }
        }
    }
}