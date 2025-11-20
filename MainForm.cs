using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace wtrans2cnrps
{

    
    public partial class MainForm : Form
    {
        private string loadedCsvPath;
        private string loadedModelContent; 
        private string loadedModelPath;
        private DataTable csvData;
        private ToolStripStatusLabel statusLabel;  // ✅ Ajouter cette ligne
            

        public MainForm()
        {
            InitializeComponent();


        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = "Convertisseur CSV → Fichier à largeurs fixes";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized; 

            // Barre de menu
            var menuStrip = new MenuStrip();
            var fichier = new ToolStripMenuItem("Fichier");
            fichier.DropDownItems.Add("Ouvrir le fichier de données", null, LoadCsvFile);
            // fichier.DropDownItems.Add("Chargement du modèle (JSON)", null, LoadModel);
            fichier.DropDownItems.Add(new ToolStripSeparator());
            fichier.DropDownItems.Add("Quitter", null, (s, ev) => Close());

            var traitement = new ToolStripMenuItem("Traitement");
            traitement.DropDownItems.Add("Visualisation des données", null, ShowData);
            traitement.DropDownItems.Add("Génération de l'annexe 4", null, GenerateFixedFile);

            // menuAide
            var menuAide = new ToolStripMenuItem("Aide");
            menuAide.DropDownItems.Add("à propos de Générateur Annexe CNRPS", null, menuAPropos_Click);



            menuStrip.Items.Add(fichier);
            menuStrip.Items.Add(traitement);
            menuStrip.Items.Add(menuAide);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "logo.png");
                if (File.Exists(logoPath))
                {
                    pictureBoxLogo.Image = Image.FromFile(logoPath);
                }
                else
                {
                    MessageBox.Show("Logo non trouvé: " + logoPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du logo: " + ex.Message);
            }
    

            // Zone d’état
            var status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Prêt");  // ✅ Utiliser la variable de classe
            status.Items.Add(statusLabel);
            Controls.Add(status);
            status.Dock = DockStyle.Bottom;
            LoadModel();
            

                

        }

        // ---- MENU 1 : Charger CSV ----
        private void LoadCsvFile(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Fichiers CSV|*.csv|Tous les fichiers|*.*" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    csvData = CsvProcessor.ReadCsv(ofd.FileName);
                    loadedCsvPath = ofd.FileName;
                    MessageBox.Show("✅ Fichier CSV chargé avec succès !", "Chargement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur de lecture CSV : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ---- MENU 2 : Visualiser les données ----
        private void ShowData(object sender, EventArgs e)
        {
            if (csvData == null)
            {
                MessageBox.Show("Veuillez d’abord charger un fichier CSV.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var viewer = new DataViewerForm(csvData);
            viewer.ShowDialog();
        }

        // ---- MENU 3 : Charger modèle ----
        private void LoadModel()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    // string modelPath = openFileDialog.FileName;
                    string modelPath = "Resources/structure annexe cnrps 4.json";
                    

                    // ✅ Validation de l'existence du fichier
                    if (!File.Exists(modelPath))
                    {
                        MessageBox.Show("Le fichier sélectionné n'existe pas.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // ✅ Lecture synchrone du fichier
                    string modelContent = File.ReadAllText(modelPath);

                    // ✅ Vérification si le fichier est vide (AVANT la validation JSON)
                    if (string.IsNullOrWhiteSpace(modelContent))
                    {
                        MessageBox.Show("Le fichier JSON est vide.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // ✅ Validation JSON
                    try
                    {
                        System.Text.Json.JsonDocument.Parse(modelContent);
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        MessageBox.Show($"Le fichier n'est pas un JSON valide.\n\nDétails : {ex.Message}",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // ✅ Assignation des variables de classe
                    loadedModelContent = modelContent;
                    loadedModelPath = modelPath;
                    statusLabel.Text = $"Modèle chargé : {Path.GetFileName(modelPath)}";

                    // MessageBox.Show($"✅ Modèle JSON chargé avec succès !\n\nFichier : {Path.GetFileName(modelPath)}",
                        // "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du modèle :\n{ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---- MENU 4 : Générer fichier texte ----
        private void GenerateFixedFile(object sender, EventArgs e)
        {
            if (csvData == null)
            {
                MessageBox.Show("Veuillez d’abord charger un fichier CSV.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(loadedModelPath))
            {
                MessageBox.Show("Veuillez d’abord charger un modèle JSON.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog { Filter = "Fichiers texte|*.txt", FileName = "output.txt" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FixedWidthGenerator.Generate(csvData, loadedModelPath, sfd.FileName);
                    MessageBox.Show("✅ Fichier généré avec succès :\n" + sfd.FileName, "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la génération : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuAPropos_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                // Charger le logo si disponible
                try
                {
                    string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "logo.png");
                    if (File.Exists(logoPath))
                    {
                        aboutForm.Controls["pictureBoxLogo"].BackgroundImage = Image.FromFile(logoPath);
                    }
                }
                catch { }

                aboutForm.ShowDialog(this);
            }
        }


    }
}
