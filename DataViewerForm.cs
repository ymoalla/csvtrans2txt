using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wtrans2cnrps
{
    public partial class DataViewerForm : Form
    {
        private DataGridView gridView;
        private Label lblInfo;
        private Panel loadingPanel;
        private Label lblLoading;
        private Panel paginationPanel;
        private Button btnFirst;
        private Button btnPrevious;
        private Button btnNext;
        private Button btnLast;
        private Label lblPageInfo;
        private TextBox txtPageSize;
        private Label lblPageSize;

        private DataTable fullData;
        private int currentPage = 0;
        private int pageSize = 25;
        private int totalPages = 0;

        public DataViewerForm(DataTable data)
        {
            InitializeComponent();
            LoadDataAsync(data);
        }

        private void InitializeComponent()
        {
            this.Text = "Visualisation des données CSV";
            this.Width = 900;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimizeBox = true;
            this.MaximizeBox = true;

            // Label d'information
            lblInfo = new Label
            {
                Text = "Aperçu des données chargées :",
                Dock = DockStyle.Top,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Height = 50
            };

            // Grille de données
            gridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AlternatingRowsDefaultCellStyle = { BackColor = System.Drawing.Color.AliceBlue },
                Visible = false
            };

            // Panel de chargement
            loadingPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White
            };

            lblLoading = new Label
            {
                Text = "Chargement en cours, veuillez patienter...",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 215)
            };
            lblLoading.Location = new System.Drawing.Point(
                (loadingPanel.Width - lblLoading.Width) / 2,
                (loadingPanel.Height - lblLoading.Height) / 2
            );
            loadingPanel.Resize += (s, e) =>
            {
                lblLoading.Location = new System.Drawing.Point(
                    (loadingPanel.Width - lblLoading.Width) / 2,
                    (loadingPanel.Height - lblLoading.Height) / 2
                );
            };
            loadingPanel.Controls.Add(lblLoading);

            // Panel de pagination
            paginationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = System.Drawing.Color.WhiteSmoke,
                Visible = false
            };

            // Boutons de navigation
            btnFirst = new Button
            {
                Text = "<<",
                Width = 40,
                Height = 30,
                Left = 10,
                Top = 7,
                FlatStyle = FlatStyle.System
            };
            btnFirst.Click += (s, e) => NavigateToPage(0);

            btnPrevious = new Button
            {
                Text = "<",
                Width = 40,
                Height = 30,
                Left = 55,
                Top = 7,
                FlatStyle = FlatStyle.System
            };
            btnPrevious.Click += (s, e) => NavigateToPage(currentPage - 1);

            lblPageInfo = new Label
            {
                Text = "Page 1 / 1",
                AutoSize = true,
                Left = 100,
                Top = 12,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            btnNext = new Button
            {
                Text = ">",
                Width = 40,
                Height = 30,
                Left = 200,
                Top = 7,
                FlatStyle = FlatStyle.System
            };
            btnNext.Click += (s, e) => NavigateToPage(currentPage + 1);

            btnLast = new Button
            {
                Text = ">>",
                Width = 40,
                Height = 30,
                Left = 245,
                Top = 7,
                FlatStyle = FlatStyle.System
            };
            btnLast.Click += (s, e) => NavigateToPage(totalPages - 1);

            // Configuration de la taille de page
            lblPageSize = new Label
            {
                Text = "Lignes/page :",
                AutoSize = true,
                Left = 310,
                Top = 12,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            txtPageSize = new TextBox
            {
                Text = "25",
                Width = 60,
                Left = 395,
                Top = 9,
                TextAlign = HorizontalAlignment.Center
            };
            txtPageSize.KeyPress += TxtPageSize_KeyPress;
            txtPageSize.Leave += TxtPageSize_Leave;
            txtPageSize.Enabled=false;

            paginationPanel.Controls.AddRange(new Control[] { 
                btnFirst, btnPrevious, lblPageInfo, btnNext, btnLast, lblPageSize, txtPageSize 
            });

            // Bouton de fermeture
            var btnClose = new Button
            {
                Text = "Fermer",
                Dock = DockStyle.Bottom,
                Height = 35,
                FlatStyle = FlatStyle.System
            };
            btnClose.Click += (s, e) => this.Close();

            // Ajout des éléments à la fenêtre
            Controls.Add(loadingPanel);
            Controls.Add(gridView);
            Controls.Add(lblInfo);
            Controls.Add(paginationPanel);
            Controls.Add(btnClose);
        }

        private void TxtPageSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ApplyPageSizeChange();
                e.Handled = true;
            }
            else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtPageSize_Leave(object sender, EventArgs e)
        {
            ApplyPageSizeChange();
        }

        private void ApplyPageSizeChange()
        {
            if (int.TryParse(txtPageSize.Text, out int newPageSize) && newPageSize > 0)
            {
                pageSize = Math.Min(newPageSize, 10000); // Max 10000 lignes par page
                txtPageSize.Text = pageSize.ToString();
                CalculateTotalPages();
                NavigateToPage(0); // Retour à la première page
            }
            else
            {
                txtPageSize.Text = pageSize.ToString();
            }
        }

        private async void LoadDataAsync(DataTable data)
        {
            if (data == null || data.Columns.Count == 0)
            {
                MessageBox.Show("Aucune donnée à afficher.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            fullData = data;

            // Mise à jour du message de chargement
            lblLoading.Text = $"Chargement de {data.Rows.Count:N0} lignes...";

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(50);
            });

            // Calculer le nombre de pages
            CalculateTotalPages();

            // Afficher la pagination si nécessaire
            if (totalPages > 1)
            {
                paginationPanel.Visible = true;
            }

            // Charger la première page
            LoadPage(0);

            // Masquer le panel de chargement et afficher la grille
            loadingPanel.Visible = false;
            gridView.Visible = true;
        }

        private void CalculateTotalPages()
        {
            if (fullData != null && fullData.Rows.Count > 0)
            {
                totalPages = (int)Math.Ceiling((double)fullData.Rows.Count / pageSize);
            }
            else
            {
                totalPages = 1;
            }
        }

        private void LoadPage(int pageIndex)
        {
            if (fullData == null || pageIndex < 0 || pageIndex >= totalPages)
                return;

            currentPage = pageIndex;

            gridView.SuspendLayout();
            
            try
            {
                // Créer une vue paginée des données
                var pagedData = fullData.Clone();
                int startRow = currentPage * pageSize;
                int endRow = Math.Min(startRow + pageSize, fullData.Rows.Count);

                for (int i = startRow; i < endRow; i++)
                {
                    pagedData.ImportRow(fullData.Rows[i]);
                }

                gridView.DataSource = pagedData;

                // Auto-dimensionner les colonnes seulement à la première page
                if (currentPage == 0)
                {
                    foreach (DataGridViewColumn col in gridView.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    gridView.AutoResizeColumns();
                    foreach (DataGridViewColumn col in gridView.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    }
                }

                // Mise à jour des informations
                lblInfo.Text = $"Aperçu des données : {fullData.Rows.Count:N0} lignes au total, {fullData.Columns.Count} colonnes | " +
                              $"Affichage : lignes {startRow + 1:N0} à {endRow:N0}";

                lblPageInfo.Text = $"Page {currentPage + 1} / {totalPages}";

                // Activer/désactiver les boutons
                btnFirst.Enabled = btnPrevious.Enabled = currentPage > 0;
                btnNext.Enabled = btnLast.Enabled = currentPage < totalPages - 1;
            }
            finally
            {
                gridView.ResumeLayout();
            }
        }

        private void NavigateToPage(int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < totalPages)
            {
                LoadPage(pageIndex);
            }
        }
    }
}