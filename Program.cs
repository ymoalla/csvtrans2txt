using wtrans2cnrps;
using System;
using System.Windows.Forms;

namespace wtrans2cnrps
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configuration de l'application
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Vérification de la licence
            if (!SecureLicenseManager.IsActivated())
            {
                Application.Run(new ActivationForm());
            }
            else
            {
                Application.Run(new MainForm());
            }
        }
    }
}