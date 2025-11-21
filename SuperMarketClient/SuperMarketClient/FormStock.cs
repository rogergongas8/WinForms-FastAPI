

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
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            InicializarComponentes();
            CargarDatos();
        }

        private void InicializarComponentes()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };

            chkSoloBajo = new CheckBox { Text = "Ver solo Stock Bajo (< 10)", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10) };
            chkSoloBajo.CheckedChanged += (s, e) => FiltrarLista();

            btnRefrescar = new Button { Text = "Actualizar", Location = new Point(250, 15), Width = 100, Height = 30, BackColor = Color.LightBlue };
            btnRefrescar.Click += (s, e) => CargarDatos();

            panelTop.Controls.Add(chkSoloBajo);
            panelTop.Controls.Add(btnRefrescar);

            dgvStock = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };
            
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
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void FiltrarLista()
        {
            var listaMostrar = _todosLosProductos;

            if (chkSoloBajo.Checked)
            {
                listaMostrar = listaMostrar.Where(p => p.STOCK < 10).ToList();
            }

            dgvStock.DataSource = null;
            dgvStock.DataSource = listaMostrar;

            if (dgvStock.Columns["id"] != null) dgvStock.Columns["id"].Visible = false;
            
            if (dgvStock.Columns["STOCK"] != null) 
                dgvStock.Columns["STOCK"].HeaderText = "Stock";
            
            if (dgvStock.Columns["PRECIOKG"] != null) 
            {
                dgvStock.Columns["PRECIOKG"].HeaderText = "Precio";
                dgvStock.Columns["PRECIOKG"].DefaultCellStyle.Format = "C2";
            }
        }

        private void DgvStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvStock.Rows.Count == 0) return;

            var row = dgvStock.Rows[e.RowIndex];
            var item = (Producto)row.DataBoundItem;

            // Adaptado a .STOCK
            if (item != null && item.STOCK < 10)
            {
                row.DefaultCellStyle.BackColor = Color.MistyRose;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
        }
    }
}