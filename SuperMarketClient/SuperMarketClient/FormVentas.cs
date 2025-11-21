
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
        private List<LineaVenta> _carrito;

        private class LineaVenta
        {
            public int ProductoId { get; set; }
            public string Nombre { get; set; }
            public float PrecioKg { get; set; }
            public float Kilos { get; set; }

            public float TotalLinea
            {
                get { return PrecioKg * Kilos; }
            }
        }

        public FormVentas(ApiClient apiClient)
        {
            _apiClient = apiClient;
            _carrito = new List<LineaVenta>();

            this.Text = "Registro de Ventas";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            InicializarComponentes();
            CargarProductos();
        }

        private void InicializarComponentes()
        {
            // PANEL IZQUIERDO
            Panel panelIzq = new Panel
                { Dock = DockStyle.Left, Width = 300, BackColor = Color.WhiteSmoke, Padding = new Padding(20) };

            Label lblProd = new Label
                { Text = "Producto:", Top = 30, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 10) };
            cmbProductos = new ComboBox
            {
                Top = 55, Left = 20, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            Label lblKilos = new Label
                { Text = "Kilos:", Top = 100, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 10) };
            numKilos = new NumericUpDown
            {
                Top = 125, Left = 20, Width = 100, DecimalPlaces = 2, Minimum = 0.1M, Maximum = 1000, Value = 1,
                Font = new Font("Segoe UI", 10)
            };

            btnAgregar = new Button
            {
                Text = "Añadir al Carrito >", Top = 180, Left = 20, Width = 240, Height = 40,
                BackColor = Color.LightBlue, FlatStyle = FlatStyle.Flat
            };
            btnAgregar.Click += BtnAgregar_Click;

            panelIzq.Controls.AddRange(new Control[] { lblProd, cmbProductos, lblKilos, numKilos, btnAgregar });

            // PANEL DERECHO
            Panel panelDer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            Panel panelAbajo = new Panel { Dock = DockStyle.Bottom, Height = 80 };

            lblTotal = new Label
                { Text = "Total: 0.00 €", Top = 20, AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold) };

            btnFinalizar = new Button
            {
                Text = "CONFIRMAR VENTA", Top = 10, Left = 20, Width = 200, Height = 50, BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            btnFinalizar.Click += BtnFinalizar_Click;

            panelAbajo.Controls.Add(lblTotal);
            panelAbajo.Controls.Add(btnFinalizar);

            lblTotal.Location = new Point(500, 20);

            dgvCarrito = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

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
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            var productoSeleccionado = (Producto)cmbProductos.SelectedItem;
            if (productoSeleccionado == null) return;

            float kilos = (float)numKilos.Value;

            var linea = new LineaVenta
            {
                ProductoId = productoSeleccionado.id,
                Nombre = productoSeleccionado.NOMBRE,
                PrecioKg = productoSeleccionado.PRECIOKG,
                Kilos = kilos
            };

            _carrito.Add(linea);
            ActualizarTabla();
        }

        private void ActualizarTabla()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = _carrito;

            dgvCarrito.Columns["TotalLinea"].DefaultCellStyle.Format = "C2";
            dgvCarrito.Columns["PrecioKg"].DefaultCellStyle.Format = "C2";
            dgvCarrito.Columns["ProductoId"].Visible = false;

            float total = _carrito.Sum(x => x.TotalLinea);
            lblTotal.Text = $"Total: {total:C2}";
        }

        private async void BtnFinalizar_Click(object sender, EventArgs e)
        {
            if (_carrito.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.");
                return;
            }

            btnFinalizar.Enabled = false;

            try
            {
                float totalImporte = _carrito.Sum(x => x.TotalLinea);
                string ticketTexto = "";
                ticketTexto += "================================\n";
                ticketTexto += "      FRUTERÍA EL MERCADO       \n";
                ticketTexto += "================================\n";
                ticketTexto += $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n";
                ticketTexto += "--------------------------------\n";
                ticketTexto += "PRODUCTO             KG    TOTAL\n"; 
                ticketTexto += "--------------------------------\n";

                foreach (var linea in _carrito)
                {
                    var ventaApi = new Venta
                    {
                        producto_id = linea.ProductoId,
                        kilos_vendidos = linea.Kilos
                    };
                    await _apiClient.RegistrarVentaAsync(ventaApi);
                    
                    string nombreCorto = linea.Nombre.Length > 18
                        ? linea.Nombre.Substring(0, 18)
                        : linea.Nombre.PadRight(18);

                    ticketTexto +=
                        $"{nombreCorto} {linea.Kilos.ToString("0.00").PadLeft(5)} {linea.TotalLinea.ToString("C2").PadLeft(8)}\n";
                }

                ticketTexto += "--------------------------------\n";
                ticketTexto += $"TOTAL A PAGAR:{totalImporte.ToString("C2").PadLeft(19)}\n";
                ticketTexto += "================================\n";
                ticketTexto += "      ¡Gracias por su compra!   \n";
                ticketTexto += "================================\n";

                // Guardar y abrir
                string rutaTicket = "ticket_compra.txt";
                System.IO.File.WriteAllText(rutaTicket, ticketTexto);
                System.Diagnostics.Process.Start("notepad.exe", rutaTicket);

                MessageBox.Show("¡Venta completada!");

                _carrito.Clear();
                ActualizarTabla();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                btnFinalizar.Enabled = true;
            }
        }
    }
}
