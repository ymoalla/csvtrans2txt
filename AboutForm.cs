using System;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

namespace wtrans2cnrps
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            LoadInformation();
        }

        private void LoadInformation()
        {
            // Charger les informations
            lblAppName.Text = "Générateur Annexe CNRPS";
            lblPublisher.Text = "Schheider Consulting";
            
            // Date d'expiration de la licence (à adapter selon votre système)
            var licenseInfo = SecureLicenseManager.GetLicenseInfo();
            lblLicenseExpiration.Text = licenseInfo.ExpirationDate.ToString("dd/MM/yyyy");
                
           
            // Numéro de série de la machine
            // lblMachineSerial.Text = GetMachineSerialNumber();
        }

        public static string GetMachineSerialNumber()
        {
            // return GetMachineSerial();
            var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["SerialNumber"].ToString();
            }
            return "UNKNOWN";
            
        }



        private string GetMachineSerial()
        {
            try
            {
                string serialNumber = string.Empty;

                // Essayer d'obtenir le numéro de série de la carte mère
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                foreach (ManagementObject mo in searcher.Get())
                {
                    serialNumber = mo["SerialNumber"]?.ToString();
                    if (!string.IsNullOrEmpty(serialNumber))
                        break;
                }

                // Si pas de numéro de série de carte mère, utiliser le disque dur
                if (string.IsNullOrEmpty(serialNumber))
                {
                    searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        serialNumber = mo["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(serialNumber))
                            break;
                    }
                }

                return string.IsNullOrEmpty(serialNumber) ? "Non disponible" : serialNumber.Trim();
            }
            catch (Exception ex)
            {
                return "Erreur: " + ex.Message;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}