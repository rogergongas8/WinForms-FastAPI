using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SuperMarketClient
{
    public class FormDetalleProducto : Form
    {
        private readonly ApiClient _apiClient;
        private readonly Producto _producto;
        private TextBox txtNombre, txtCategoria;
        private NumericUpDown numPrecio, numStock;
        private Button btnGuardar;

        public FormDetalleProducto(ApiClient apiClient, Producto producto = null)
        {
            _apiClient = apiClient;
            _producto = producto;

            this.Text = _producto == null ? "Nuevo Producto" : "Editar Producto";
            this.Size = new Size(400, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            InicializarComponentes();
            if (_producto != null) CargarDatos();
        }

        private void InicializarComponentes()
        {
            int y = 30;
            CrearCampo("Nombre:", ref txtNombre, ref y);
            CrearCampo("Categoría:", ref txtCategoria, ref y);
            CrearCampoNum("Precio (€/kg):", ref numPrecio, ref y);
            CrearCampoNum("Stock Inicial:", ref numStock, ref y);

            btnGuardar = UI.CrearBoton("GUARDAR", 120, y + 20, UI.PrimaryColor);
            btnGuardar.Width = 140; 
            btnGuardar.Height = 45;
            btnGuardar.Click += BtnGuardar_Click;

            this.Controls.Add(btnGuardar);
        }

        private void CrearCampo(string label, ref TextBox txt, ref int y)
        {
            this.Controls.Add(new Label { Text = label, Location = new Point(40, y), AutoSize = true, Font = UI.MainFont, ForeColor = Color.Gray });
            txt = new TextBox { Location = new Point(40, y + 25), Width = 300, Font = UI.MainFont, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txt);
            y += 70;
        }

        private void CrearCampoNum(string label, ref NumericUpDown num, ref int y)
        {
            this.Controls.Add(new Label { Text = label, Location = new Point(40, y), AutoSize = true, Font = UI.MainFont, ForeColor = Color.Gray });
            num = new NumericUpDown { Location = new Point(40, y + 25), Width = 150, Font = UI.MainFont, DecimalPlaces = 2, Maximum = 10000 };
            this.Controls.Add(num);
            y += 70;
        }

        private void CargarDatos()
        {
            txtNombre.Text = _producto.NOMBRE;
            txtCategoria.Text = _producto.CATEGORIA;
            numPrecio.Value = (decimal)_producto.PRECIOKG;
            numStock.Value = (decimal)_producto.STOCK;
        }

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) return;
            btnGuardar.Enabled = false;

            var nuevo = new Producto
            {
                id = _producto?.id ?? 0,
                NOMBRE = txtNombre.Text,
                CATEGORIA = txtCategoria.Text,
                PRECIOKG = (float)numPrecio.Value,
                STOCK = (float)numStock.Value
            };

            try
            {
                if (_producto == null) await _apiClient.CrearProductoAsync(nuevo);
                else await _apiClient.ActualizarProductoAsync(nuevo.id, nuevo);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            finally { btnGuardar.Enabled = true; }
        }
    }
}