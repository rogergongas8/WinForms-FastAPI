using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public class FormProductos : Form
    {
        private readonly ApiClient _apiClient;
        private DataGridView dgvProductos;
        private TextBox txtBuscar;
        private Button btnRefrescar;
        private List<Producto> _listaProductos = new List<Producto>();

        public FormProductos(ApiClient apiClient)
        {
            _apiClient = apiClient;
            
            this.Text = "Gestión de Productos";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            InicializarUI();
            CargarDatos();
        }

private void InicializarUI()
        {
            // 1. Configurar Fondo y Fuente
            this.BackColor = UI.BackColor;
            this.Font = UI.MainFont;

            // 2. Panel Superior
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Padding = new Padding(20) };
            
            Label lblBuscar = new Label { Text = "🔍 Buscar:", Location = new Point(20, 30), AutoSize = true, ForeColor = Color.Gray };
            txtBuscar = new TextBox { Location = new Point(90, 28), Width = 200, Font = UI.MainFont };
            txtBuscar.TextChanged += (s, e) => FiltrarLista();

            // Usamos el helper de UI para crear botones bonitos
            btnRefrescar = UI.CrearBoton("↻ Refrescar", 310, 25, Color.Gray);
            btnRefrescar.Click += (s, e) => CargarDatos();

            // Botones CRUD con colores semánticos y emojis
            var btnNuevo = UI.CrearBoton("➕ Nuevo", 520, 25, UI.AccentColor); // Azul
            btnNuevo.Click += BtnNuevo_Click;

            var btnEditar = UI.CrearBoton("✏️ Editar", 630, 25, Color.Orange); 
            btnEditar.Click += BtnEditar_Click;

            var btnEliminar = UI.CrearBoton("🗑️ Borrar", 740, 25, Color.IndianRed); 
            btnEliminar.Click += BtnEliminar_Click;

            panelTop.Controls.AddRange(new Control[] { lblBuscar, txtBuscar, btnRefrescar, btnNuevo, btnEditar, btnEliminar });

            // 3. Tabla (Usamos el estilizador)
            dgvProductos = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };
            
            UI.EstilarGrid(dgvProductos); // <--- MAGIA AQUÍ

            this.Controls.Add(dgvProductos);
            this.Controls.Add(panelTop);
            panelTop.SendToBack();
        }
        
        private Button CrearBoton(string texto, int x, Color color, EventHandler evento)
        {
            var btn = new Button { Text = texto, Location = new Point(x, 25), Size = new Size(80, 30), BackColor = color, FlatStyle = FlatStyle.Flat };
            btn.Click += evento;
            return btn;
        }

        private async void CargarDatos()
        {
            try
            {
                _listaProductos = await _apiClient.GetProductosAsync();
                FiltrarLista();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void FiltrarLista()
        {
            var busqueda = txtBuscar.Text.ToLower();
            dgvProductos.DataSource = _listaProductos
                .Where(p => p.NOMBRE.ToLower().Contains(busqueda) || p.CATEGORIA.ToLower().Contains(busqueda))
                .ToList();

            EstilarGrid(); 
        }

        private void EstilarGrid()
        {
            if (dgvProductos.Columns["id"] != null) dgvProductos.Columns["id"].Width = 40;
            if (dgvProductos.Columns["STOCK"] != null) dgvProductos.Columns["STOCK"].HeaderText = "Stock";
            if (dgvProductos.Columns["PRECIOKG"] != null) 
            {
                dgvProductos.Columns["PRECIOKG"].HeaderText = "Precio";
                dgvProductos.Columns["PRECIOKG"].DefaultCellStyle.Format = "C2";
            }
        }

        // --- Eventos Click ---
        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            new FormDetalleProducto(_apiClient).ShowDialog();
            CargarDatos();
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var item = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;
                new FormDetalleProducto(_apiClient, item).ShowDialog();
                CargarDatos();
            }
        }

        private async void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var item = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show($"¿Borrar {item.NOMBRE}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await _apiClient.EliminarProductoAsync(item.id);
                    CargarDatos();
                }
            }
        }
    }
}