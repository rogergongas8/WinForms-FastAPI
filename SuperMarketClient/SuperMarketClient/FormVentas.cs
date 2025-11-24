using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public class FormVentas : Form
    {
        private readonly ApiClient _apiClient;
        private ComboBox cmbProductos;
        private NumericUpDown numKilos;
        private DataGridView dgvCarrito;
        private Label lblTotal;
        private Button btnAgregar, btnFinalizar;

        private List<Producto> _productosDisponibles;
        private List<LineaVenta> _carrito = new List<LineaVenta>();

        private class LineaVenta
        {
            public int ProductoId { get; set; }
            public string Nombre { get; set; }
            public float PrecioKg { get; set; }
            public float Kilos { get; set; }
            public float TotalLinea => PrecioKg * Kilos;
        }

        public FormVentas(ApiClient apiClient)
        {
            _apiClient = apiClient;
            this.Text = "Registro de Ventas";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            InicializarComponentes();
            CargarProductos();
        }

        private void InicializarComponentes()
        {
            // PANEL IZQUIERDO (Entrada datos)
            Panel panelIzq = new Panel { Dock = DockStyle.Left, Width = 320, BackColor = UI.BackColor, Padding = new Padding(20) };

            Label lblProd = new Label { Text = "Producto:", Top = 30, Left = 20, AutoSize = true, Font = UI.HeaderFont };
            cmbProductos = new ComboBox { Top = 60, Left = 20, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = UI.MainFont, FlatStyle = FlatStyle.Flat };

            Label lblKilos = new Label { Text = "Cantidad (Kg):", Top = 110, Left = 20, AutoSize = true, Font = UI.HeaderFont };
            numKilos = new NumericUpDown { Top = 140, Left = 20, Width = 120, DecimalPlaces = 2, Minimum = 0.1M, Maximum = 1000, Value = 1, Font = UI.MainFont };

            btnAgregar = UI.CrearBoton("Añadir  >", 20, 200, UI.PrimaryColor);
            btnAgregar.Width = 260; // Botón ancho
            btnAgregar.Height = 45;
            btnAgregar.Click += BtnAgregar_Click;

            panelIzq.Controls.AddRange(new Control[] { lblProd, cmbProductos, lblKilos, numKilos, btnAgregar });

            // PANEL DERECHO (Carrito)
            Panel panelDer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0) };
            Panel panelAbajo = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.WhiteSmoke };

            lblTotal = new Label { Text = "Total: 0.00 €", Location = new Point(30, 30), AutoSize = true, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = UI.PrimaryColor };

            btnFinalizar = new Button { 
                Text = "✅ CONFIRMAR VENTA", 
                Location = new Point(400, 25), 
                Size = new Size(220, 50), 
                BackColor = UI.AccentColor, 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat, 
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFinalizar.FlatAppearance.BorderSize = 0;
            btnFinalizar.Click += BtnFinalizar_Click;

            panelAbajo.Controls.Add(lblTotal);
            panelAbajo.Controls.Add(btnFinalizar);

            dgvCarrito = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            UI.EstilarGrid(dgvCarrito);

            panelDer.Controls.Add(dgvCarrito);
            panelDer.Controls.Add(panelAbajo);

            this.Controls.Add(panelDer);
            this.Controls.Add(panelIzq);
        }

        private async void CargarProductos()
        {
            try
            {
                _productosDisponibles = await _apiClient.GetProductosAsync();
                cmbProductos.DataSource = _productosDisponibles;
                cmbProductos.DisplayMember = "NOMBRE";
                cmbProductos.ValueMember = "id";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            var prod = (Producto)cmbProductos.SelectedItem;
            if (prod == null) return;

            _carrito.Add(new LineaVenta { ProductoId = prod.id, Nombre = prod.NOMBRE, PrecioKg = prod.PRECIOKG, Kilos = (float)numKilos.Value });
            ActualizarTabla();
        }

        private void ActualizarTabla()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = _carrito;
            dgvCarrito.Columns["ProductoId"].Visible = false;
            dgvCarrito.Columns["TotalLinea"].DefaultCellStyle.Format = "C2";
            dgvCarrito.Columns["PrecioKg"].DefaultCellStyle.Format = "C2";

            lblTotal.Text = $"Total: {_carrito.Sum(x => x.TotalLinea):C2}";
        }

        private async void BtnFinalizar_Click(object sender, EventArgs e)
        {
            if (_carrito.Count == 0) return;
            btnFinalizar.Enabled = false;

            try
            {
                string ticket = "FRUTERÍA EL MERCADO\n------------------\n";
                foreach (var item in _carrito)
                {
                    await _apiClient.RegistrarVentaAsync(new Venta { producto_id = item.ProductoId, kilos_vendidos = item.Kilos });
                    ticket += $"{item.Nombre} x {item.Kilos}kg = {item.TotalLinea:C2}\n";
                }
                ticket += $"\nTOTAL: {_carrito.Sum(x => x.TotalLinea):C2}";

                string path = "ticket.txt";
                System.IO.File.WriteAllText(path, ticket);
                System.Diagnostics.Process.Start("notepad.exe", path);

                _carrito.Clear();
                ActualizarTabla();
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            finally { btnFinalizar.Enabled = true; }
        }
    }
}