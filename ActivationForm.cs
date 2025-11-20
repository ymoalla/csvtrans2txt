using System;
using System.Windows.Forms;

namespace wtrans2cnrps
{
    public partial class ActivationForm : Form
    {
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void ActivationForm_Load(object sender, EventArgs e)
        {
            lblSerial.Text = SecureLicenseManager.GetMachineSerialNumber();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            string enteredCode = txtActivationCode.Text.Trim();

            if (string.IsNullOrEmpty(enteredCode))
            {
                MessageBox.Show("Veuillez saisir le code d’activation.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (SecureLicenseManager.Activate(enteredCode))
            {
                MessageBox.Show("Activation réussie !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Code d’activation invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Application.Exit(); // Quitte l’application
        }
    }
}
