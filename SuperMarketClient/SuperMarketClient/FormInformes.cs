using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public class FormInformes : Form
    {
        private readonly ApiClient _apiClient;
        private Label lblTotalDinero, lblTotalKilos, lblTopProducto;

        public FormInformes(ApiClient apiClient)
        {
            _apiClient = apiClient;
            this.Text = "Informes";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UI.BackColor;

            Inicializar();
            CargarDatos();
        }

        private void Inicializar()
        {
            Label lblTitulo = new Label { Text = "Resumen de Ventas", Top = 20, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = UI.TextColor };
            this.Controls.Add(lblTitulo);

            CrearTarjeta("Ingresos Totales", ref lblTotalDinero, 20, 70, Color.DarkGreen);
            CrearTarjeta("Kilos Vendidos", ref lblTotalKilos, 300, 70, Color.DarkBlue);
            
            // Tarjeta grande
            Panel pnlTop = new Panel { Location = new Point(20, 200), Size = new Size(540, 100), BackColor = Color.White };
            Label lblH = new Label { Text = "Producto Estrella 🌟", Location = new Point(15, 15), AutoSize = true, Font = UI.HeaderFont, ForeColor = Color.Gray };
            lblTopProducto = new Label { Text = "-", Location = new Point(15, 45), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = UI.AccentColor };
            pnlTop.Controls.AddRange(new Control[] { lblH, lblTopProducto });
            this.Controls.Add(pnlTop);

            Button btn = UI.CrearBoton("Actualizar", 220, 330, Color.Gray);
            btn.Click += (s, e) => CargarDatos();
            this.Controls.Add(btn);
        }

        private void CrearTarjeta(string titulo, ref Label lblValor, int x, int y, Color colorTexto)
        {
            Panel p = new Panel { Location = new Point(x, y), Size = new Size(260, 110), BackColor = Color.White };
            Label lT = new Label { Text = titulo, Location = new Point(15, 15), AutoSize = true, Font = UI.HeaderFont, ForeColor = Color.Gray };
            lblValor = new Label { Text = "...", Location = new Point(15, 50), AutoSize = true, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = colorTexto };
            p.Controls.Add(lT);
            p.Controls.Add(lblValor);
            this.Controls.Add(p);
        }

        private async void CargarDatos()
        {
            try
            {
                var ventas = await _apiClient.GetVentasAsync();
                var productos = await _apiClient.GetProductosAsync();

                lblTotalDinero.Text = ventas.Sum(v => v.total_venta).ToString("C2");
                lblTotalKilos.Text = $"{ventas.Sum(v => v.kilos_vendidos):N2} kg";

                var top = ventas.GroupBy(v => v.producto_id).OrderByDescending(g => g.Sum(v => v.kilos_vendidos)).FirstOrDefault();
                if (top != null)
                {
                    var nombre = productos.FirstOrDefault(p => p.id == top.Key)?.NOMBRE ?? "Desconocido";
                    lblTopProducto.Text = $"{nombre} ({top.Sum(v => v.kilos_vendidos)} kg)";
                }
            }
            catch {}
        }
    }
}