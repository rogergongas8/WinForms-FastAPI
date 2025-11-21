

namespace SuperMarketClient
{
    public class FormInformes : Form
    {
        private readonly ApiClient _apiClient;
        private Label lblTotalDinero;
        private Label lblTotalKilos;
        private Label lblTopProducto;
        private Button btnRefrescar;

        public FormInformes(ApiClient apiClient)
        {
            _apiClient = apiClient;
            this.Text = "Informes y Estadísticas";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            InicializarComponentes();
            CargarDatos();
        }

        private void InicializarComponentes()
        {
            // Titlo
            Label lblTitulo = new Label { Text = "Resumen del Negocio", Top = 20, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold) };

            // Dinero Total
            GroupBox gbDinero = new GroupBox { Text = "Ingresos Totales", Location = new Point(20, 70), Size = new Size(200, 100) };
            lblTotalDinero = new Label { Text = "0.00 €", Location = new Point(20, 40), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Green };
            gbDinero.Controls.Add(lblTotalDinero);

            // Kilos Totales
            GroupBox gbKilos = new GroupBox { Text = "Kilos Vendidos", Location = new Point(240, 70), Size = new Size(200, 100) };
            lblTotalKilos = new Label { Text = "0.00 kg", Location = new Point(20, 40), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Blue };
            gbKilos.Controls.Add(lblTotalKilos);

            // Producto Estrella
            GroupBox gbTop = new GroupBox { Text = "Producto Más Vendido (ID)", Location = new Point(20, 190), Size = new Size(420, 80) };
            lblTopProducto = new Label { Text = "-", Location = new Point(20, 30), AutoSize = true, Font = new Font("Segoe UI", 12) };
            gbTop.Controls.Add(lblTopProducto);

            btnRefrescar = new Button { Text = "Actualizar Datos", Location = new Point(150, 300), Size = new Size(180, 40), BackColor = Color.LightGray };
            btnRefrescar.Click += (s, e) => CargarDatos();

            this.Controls.AddRange(new Control[] { lblTitulo, gbDinero, gbKilos, gbTop, btnRefrescar });
        }

        private async void CargarDatos()
        {
            try
            {
                var ventas = await _apiClient.GetVentasAsync();
                var productos = await _apiClient.GetProductosAsync();

                // Calcular Total Dinero
                float totalDinero = ventas.Sum(v => v.total_venta);
                lblTotalDinero.Text = totalDinero.ToString("C2");

                // Calcular Total Kilos
                float totalKilos = ventas.Sum(v => v.kilos_vendidos);
                lblTotalKilos.Text = $"{totalKilos:N2} kg";

                // Producto mas vendido
                if (ventas.Count > 0)
                {
                    var topVenta = ventas.GroupBy(v => v.producto_id)
                                         .OrderByDescending(g => g.Sum(v => v.kilos_vendidos))
                                         .FirstOrDefault();
                    
                    if (topVenta != null)
                    {
                        int idTop = topVenta.Key;
                        float kilosTop = topVenta.Sum(v => v.kilos_vendidos);
                        
                        // Buscar nombre del producto
                        var prodNombre = productos.FirstOrDefault(p => p.id == idTop)?.NOMBRE ?? "Desconocido";
                        
                        lblTopProducto.Text = $"{prodNombre} ({kilosTop} kg vendidos)";
                    }
                }
                else
                {
                    lblTopProducto.Text = "No hay ventas aún.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculando informes: " + ex.Message);
            }
        }
    }
}