

namespace SuperMarketClient
{
    public class FormDetalleProducto : Form
    {
        private readonly ApiClient _apiClient;
        private readonly Producto _producto;
        private TextBox txtNombre;
        private TextBox txtCategoria;
        private NumericUpDown numPrecio;
        private NumericUpDown numStock;
        private Button btnGuardar;
        private Button btnCancelar;

        public FormDetalleProducto(ApiClient apiClient, Producto producto = null)
        {
            _apiClient = apiClient;
            _producto = producto;

            this.Text = _producto == null ? "Nuevo Producto" : "Editar Producto";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            InicializarComponentes();
            if (_producto != null) CargarDatos();
        }

        private void InicializarComponentes()
        {
            Label lblNombre = new Label { Text = "Nombre:", Location = new Point(30, 30), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(120, 27), Width = 200 };

            Label lblCategoria = new Label { Text = "Categoría:", Location = new Point(30, 70), AutoSize = true };
            txtCategoria = new TextBox { Location = new Point(120, 67), Width = 200 };

            Label lblPrecio = new Label { Text = "Precio:", Location = new Point(30, 110), AutoSize = true };
            numPrecio = new NumericUpDown { Location = new Point(120, 107), Width = 100, DecimalPlaces = 2, Maximum = 1000 };

            Label lblStock = new Label { Text = "Stock:", Location = new Point(30, 150), AutoSize = true };
            numStock = new NumericUpDown { Location = new Point(120, 147), Width = 100, DecimalPlaces = 2, Maximum = 10000 };

            btnGuardar = new Button { Text = "Guardar", Location = new Point(120, 220), Width = 90, BackColor = Color.LightGreen };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button { Text = "Cancelar", Location = new Point(230, 220), Width = 90, BackColor = Color.LightGray };
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblNombre, txtNombre, lblCategoria, txtCategoria, lblPrecio, numPrecio, lblStock, numStock, btnGuardar, btnCancelar });
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
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }

            btnGuardar.Enabled = false;

            var nuevoProducto = new Producto
            {
                id = _producto?.id ?? 0,
                NOMBRE = txtNombre.Text,
                CATEGORIA = txtCategoria.Text,
                PRECIOKG = (float)numPrecio.Value,
                STOCK = (float)numStock.Value
            };

            try
            {
                if (_producto == null)
                {
                    await _apiClient.CrearProductoAsync(nuevoProducto);
                }
                else
                {
                    await _apiClient.ActualizarProductoAsync(nuevoProducto.id, nuevoProducto);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
            finally
            {
                btnGuardar.Enabled = true;
            }
        }
    }
}