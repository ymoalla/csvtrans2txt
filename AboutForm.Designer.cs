namespace wtrans2cnrps
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label lblAppNameLabel;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblPublisherLabel;
        private System.Windows.Forms.Label lblPublisher;
        private System.Windows.Forms.Label lblLicenseExpirationLabel;
        private System.Windows.Forms.Label lblLicenseExpiration;
        private System.Windows.Forms.Label lblMachineSerialLabel;
        private System.Windows.Forms.Label lblMachineSerial;
        private System.Windows.Forms.Button btnOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.lblAppNameLabel = new System.Windows.Forms.Label();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblPublisherLabel = new System.Windows.Forms.Label();
            this.lblPublisher = new System.Windows.Forms.Label();
            this.lblLicenseExpirationLabel = new System.Windows.Forms.Label();
            this.lblLicenseExpiration = new System.Windows.Forms.Label();
            this.lblMachineSerialLabel = new System.Windows.Forms.Label();
            this.lblMachineSerial = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            
            // 
            // pictureBoxLogo
            // 
            // this.pictureBoxLogo.Location = new System.Drawing.Point(10, 20);
            // this.pictureBoxLogo.Name = "pictureBoxLogo";
            // this.pictureBoxLogo.Size = new System.Drawing.Size(100, 100);
            // this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // this.pictureBoxLogo.TabIndex = 0;
            // this.pictureBoxLogo.TabStop = false;
            
            // 
            // lblAppNameLabel
            // 
            this.lblAppNameLabel.AutoSize = true;
            this.lblAppNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblAppNameLabel.Location = new System.Drawing.Point(140, 30);
            this.lblAppNameLabel.Name = "lblAppNameLabel";
            this.lblAppNameLabel.Size = new System.Drawing.Size(85, 15);
            this.lblAppNameLabel.TabIndex = 1;
            this.lblAppNameLabel.Text = "Application :";
            
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Location = new System.Drawing.Point(240, 30);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(0, 13);
            this.lblAppName.TabIndex = 2;
            
            // 
            // lblPublisherLabel
            // 
            this.lblPublisherLabel.AutoSize = true;
            this.lblPublisherLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblPublisherLabel.Location = new System.Drawing.Point(140, 60);
            this.lblPublisherLabel.Name = "lblPublisherLabel";
            this.lblPublisherLabel.Size = new System.Drawing.Size(61, 15);
            this.lblPublisherLabel.TabIndex = 3;
            this.lblPublisherLabel.Text = "Éditeur :";
            
            // 
            // lblPublisher
            // 
            this.lblPublisher.AutoSize = true;
            this.lblPublisher.Location = new System.Drawing.Point(240, 60);
            this.lblPublisher.Name = "lblPublisher";
            this.lblPublisher.Size = new System.Drawing.Size(0, 13);
            this.lblPublisher.TabIndex = 4;
            
            // 
            // lblLicenseExpirationLabel
            // 
            this.lblLicenseExpirationLabel.AutoSize = true;
            this.lblLicenseExpirationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblLicenseExpirationLabel.Location = new System.Drawing.Point(15, 140);
            this.lblLicenseExpirationLabel.Name = "lblLicenseExpirationLabel";
            this.lblLicenseExpirationLabel.Size = new System.Drawing.Size(163, 15);
            this.lblLicenseExpirationLabel.TabIndex = 5;
            this.lblLicenseExpirationLabel.Text = "Expiration de la licence :";
            
            // 
            // lblLicenseExpiration
            // 
            this.lblLicenseExpiration.AutoSize = true;
            this.lblLicenseExpiration.Location = new System.Drawing.Point(190, 140);
            this.lblLicenseExpiration.Name = "lblLicenseExpiration";
            this.lblLicenseExpiration.Size = new System.Drawing.Size(0, 13);
            this.lblLicenseExpiration.TabIndex = 6;
            
            // 
            // lblMachineSerialLabel
            // 
            // this.lblMachineSerialLabel.AutoSize = true;
            // this.lblMachineSerialLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            // this.lblMachineSerialLabel.Location = new System.Drawing.Point(20, 170);
            // this.lblMachineSerialLabel.Name = "lblMachineSerialLabel";
            // this.lblMachineSerialLabel.Size = new System.Drawing.Size(155, 15);
            // this.lblMachineSerialLabel.TabIndex = 7;
            // this.lblMachineSerialLabel.Text = "Numéro de série (PC) :";
            
            // 
            // lblMachineSerial
            // 
            this.lblMachineSerial.AutoSize = true;
            this.lblMachineSerial.Location = new System.Drawing.Point(190, 170);
            this.lblMachineSerial.Name = "lblMachineSerial";
            this.lblMachineSerial.Size = new System.Drawing.Size(0, 13);
            this.lblMachineSerial.TabIndex = 8;
            
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(210, 220);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 30);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 270);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblMachineSerial);
            this.Controls.Add(this.lblMachineSerialLabel);
            this.Controls.Add(this.lblLicenseExpiration);
            this.Controls.Add(this.lblLicenseExpirationLabel);
            this.Controls.Add(this.lblPublisher);
            this.Controls.Add(this.lblPublisherLabel);
            this.Controls.Add(this.lblAppName);
            this.Controls.Add(this.lblAppNameLabel);
            // this.Controls.Add(this.pictureBoxLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "À propos";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            // this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}