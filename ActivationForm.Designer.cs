namespace wtrans2cnrps
{
    partial class ActivationForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblSerialLabel;
        private Label lblSerial;
        private Label lblCodeLabel;
        private TextBox txtActivationCode;
        private Button btnValidate;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblSerialLabel = new Label();
            this.lblSerial = new Label();
            this.lblCodeLabel = new Label();
            this.txtActivationCode = new TextBox();
            this.btnValidate = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Text = "Activation du programme";
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new Point(40, 20);

            // lblSerialLabel
            this.lblSerialLabel.Text = "Numéro de série de cette machine :";
            this.lblSerialLabel.AutoSize = true;
            this.lblSerialLabel.Location = new Point(80, 70);

            // lblSerial
            this.lblSerial.Font = new Font("Consolas", 10F, FontStyle.Regular);
            this.lblSerial.AutoSize = true;
            this.lblSerial.Location = new Point(40, 90);

            // lblCodeLabel
            this.lblCodeLabel.Text = "Code d’activation :";
            this.lblCodeLabel.AutoSize = true;
            this.lblCodeLabel.Location = new Point(40, 140);

            // txtActivationCode
            this.txtActivationCode.Location = new Point(40, 160);
            this.txtActivationCode.Width = 300;

            // btnValidate
            this.btnValidate.Text = "Valider";
            this.btnValidate.Location = new Point(40, 210);
            this.btnValidate.Click += new EventHandler(this.btnValidate_Click);

            // btnCancel
            this.btnCancel.Text = "Annuler";
            this.btnCancel.Location = new Point(150, 210);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // ActivationForm
            this.ClientSize = new Size(800, 280);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSerialLabel);
            this.Controls.Add(this.lblSerial);
            this.Controls.Add(this.lblCodeLabel);
            this.Controls.Add(this.txtActivationCode);
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.btnCancel);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Activation du logiciel";
            this.Load += new EventHandler(this.ActivationForm_Load);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
