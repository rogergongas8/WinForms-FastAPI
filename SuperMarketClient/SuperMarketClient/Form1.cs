using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private Label lblTitulo;

        public Form1()
        {
            InitializeComponent();
            ConfigurarVentana();
            CrearPanelCentral();
        }

        private void ConfigurarVentana()
        {
            this.Text = "Frutería El Mercado - Gestión";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UI.PrimaryColor;

            string rutaImagen = "fondo.jpg"; 
            if (File.Exists(rutaImagen))
            {
                using (var fs = new FileStream(rutaImagen, FileMode.Open, FileAccess.Read))
                {
                    this.BackgroundImage = Image.FromStream(fs);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
        }
        
        private void CrearPanelCentral()
        {
            Panel panelMenu = new Panel
            {
                Size = new Size(380, 420),
                BackColor = Color.White,
                Padding = new Padding(30)
            };
            
            panelMenu.Location = new Point(
                (this.ClientSize.Width - panelMenu.Width) / 2,
                (this.ClientSize.Height - panelMenu.Height) / 2
            );
            panelMenu.Anchor = AnchorStyles.None;

            lblTitulo = new Label
            {
                Text = "MENÚ PRINCIPAL",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UI.PrimaryColor,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70
            };

            AgregarBotonMenu(panelMenu, "Ver Informes", Color.Purple, () => new FormInformes(_apiClient).ShowDialog());
            AgregarBotonMenu(panelMenu, "Control de Stock", Color.Orange, () => new FormStock(_apiClient).ShowDialog());
            AgregarBotonMenu(panelMenu, "Nueva Venta", UI.AccentColor, () => new FormVentas(_apiClient).ShowDialog());
            AgregarBotonMenu(panelMenu, "Gestionar Productos", UI.PrimaryColor, () => new FormProductos(_apiClient).ShowDialog());

            panelMenu.Controls.Add(lblTitulo);
            
            lblTitulo.SendToBack(); 

            this.Controls.Add(panelMenu);
        }

        private void AgregarBotonMenu(Panel panel, string texto, Color color, Action accion)
        {
            Button btn = new Button
            {
                Text = texto.ToUpper(),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Height = 50,
                Dock = DockStyle.Top,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            
            Panel espaciador = new Panel { Height = 15, Dock = DockStyle.Top, BackColor = Color.Transparent };
            
            btn.Click += (s, e) => accion();

            panel.Controls.Add(espaciador);
            panel.Controls.Add(btn);
        }
    }
}