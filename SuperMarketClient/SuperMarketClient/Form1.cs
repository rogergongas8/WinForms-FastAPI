

namespace SuperMarketClient
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _apiClient = new ApiClient();

        public Form1()
        {
            InitializeComponent();
            this.Text = "Frutería - Menú";
            this.Size = new Size(400, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            AgregarBoton("Gestionar Productos", 40, () => new FormProductos(_apiClient).ShowDialog());
            AgregarBoton("Nueva Venta", 110, () => new FormVentas(_apiClient).ShowDialog());
            AgregarBoton("Control de Stock", 180, () => new FormStock(_apiClient).ShowDialog());
            AgregarBoton("Ver Informes", 250, () => new FormInformes(_apiClient).ShowDialog());
        }
        
        private void AgregarBoton(string texto, int y, Action accion)
        {
            var btn = new Button { Text = texto, Location = new Point(50, y), Size = new Size(280, 50) };
            btn.Click += (s, e) => accion();
            this.Controls.Add(btn);
        }
    }
}