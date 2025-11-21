using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _apiClient = new ApiClient();

        public Form1()
        {
            InitializeComponent();
            ConfigurarVentana();
            CrearPanelCentral();
        }

        private void ConfigurarVentana()
        {
            this.Text = "Frutería El Mercado - Gestión";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            string rutaImagen = "fondo.jpg"; 
            if (File.Exists(rutaImagen))
            {
                this.BackgroundImage = Image.FromFile(rutaImagen);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                this.BackColor = UI.PrimaryColor; 
            }
        }
        
        // Titulo
        Label lblTitulo = new Label
        {
            Text = "MENÚ PRINCIPAL",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = UI.PrimaryColor,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 50
        };

        private void CrearPanelCentral()
        {
            Panel panelMenu = new Panel
            {
                Size = new Size(350, 380),
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            
            // Panel central
            panelMenu.Location = new Point(
                (this.ClientSize.Width - panelMenu.Width) / 2,
                (this.ClientSize.Height - panelMenu.Height) / 2
            );
            panelMenu.Anchor = AnchorStyles.None; 



            panelMenu.Controls.Add(lblTitulo);

            // Botones
            AgregarBotonMenu(panelMenu, "Ver Informes", Color.Purple, () => new FormInformes(_apiClient).ShowDialog());
            AgregarBotonMenu(panelMenu, "Control de Stock", Color.Orange, () => new FormStock(_apiClient).ShowDialog());
            AgregarBotonMenu(panelMenu, "Nueva Venta", UI.AccentColor, () => new FormVentas(_apiClient).ShowDialog());
            AgregarBotonMenu(panelMenu, "Gestionar Productos", UI.PrimaryColor, () => new FormProductos(_apiClient).ShowDialog());

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
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Height = 45,
                Dock = DockStyle.Top,
                Cursor = Cursors.Hand
            };
            
            Panel espaciador = new Panel { Height = 10, Dock = DockStyle.Top };
            
            btn.Click += (s, e) => accion();

            panel.Controls.Add(espaciador);
            panel.Controls.Add(btn);
        }
    }
}