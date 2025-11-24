using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public class FormStock : Form
    {
        private readonly ApiClient _apiClient;
        private DataGridView dgvStock;
        private CheckBox chkSoloBajo;
        private Button btnRefrescar;
        private List<Producto> _todosLosProductos;

        public FormStock(ApiClient apiClient)
        {
            _apiClient = apiClient;
            _todosLosProductos = new List<Producto>();

            this.Text = "Control de Stock";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UI.BackColor;

            InicializarComponentes();
            CargarDatos();
        }

        private void InicializarComponentes()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White, Padding = new Padding(10) };

            chkSoloBajo = new CheckBox { 
                Text = "⚠️ Ver solo Stock Crítico (< 10)", 
                Location = new Point(20, 25), 
                AutoSize = true, 
                Font = UI.MainFont,
                ForeColor = Color.DarkRed
            };
            chkSoloBajo.CheckedChanged += (s, e) => FiltrarLista();

            btnRefrescar = UI.CrearBoton("Actualizar", 300, 18, Color.SteelBlue);
            btnRefrescar.Click += (s, e) => CargarDatos();

            panelTop.Controls.Add(chkSoloBajo);
            panelTop.Controls.Add(btnRefrescar);

            dgvStock = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };
            
            UI.EstilarGrid(dgvStock); // Aplicar estilo limpio
            dgvStock.CellFormatting += DgvStock_CellFormatting;

            this.Controls.Add(dgvStock);
            this.Controls.Add(panelTop);
            panelTop.SendToBack();
        }

        private async void CargarDatos()
        {
            try
            {
                _todosLosProductos = await _apiClient.GetProductosAsync();
                FiltrarLista();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void FiltrarLista()
        {
            var listaMostrar = chkSoloBajo.Checked 
                ? _todosLosProductos.Where(p => p.STOCK < 10).ToList() 
                : _todosLosProductos;

            dgvStock.DataSource = null;
            dgvStock.DataSource = listaMostrar;

            if (dgvStock.Columns["id"] != null) dgvStock.Columns["id"].Visible = false;
            
            if (dgvStock.Columns["PRECIOKG"] != null) 
                dgvStock.Columns["PRECIOKG"].DefaultCellStyle.Format = "C2";
        }

        private void DgvStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvStock.Rows.Count == 0) return;

            var row = dgvStock.Rows[e.RowIndex];
            var item = (Producto)row.DataBoundItem;

            if (item != null && item.STOCK < 10)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(198, 40, 40);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 205, 210);
                row.DefaultCellStyle.SelectionForeColor = Color.Maroon;
            }
        }
    }
}