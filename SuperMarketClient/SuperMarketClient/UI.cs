using System.Drawing;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public static class UI
    {
        // Colores más vivos y modernos
        public static Color PrimaryColor = Color.FromArgb(46, 125, 50);   // Verde Bosque
        public static Color AccentColor = Color.FromArgb(239, 108, 0);    // Naranja Intenso
        public static Color BackColor = Color.FromArgb(245, 245, 245);    // Gris muy suave (Casi blanco)
        public static Color TextColor = Color.FromArgb(33, 33, 33);       // Gris oscuro (mejor que negro puro)
        
        public static Font MainFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font HeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);

        public static Button CrearBoton(string texto, int x, int y, Color colorFondo)
        {
            return new Button
            {
                Text = texto.ToUpper(), 
                Location = new Point(x, y),
                Size = new Size(110, 35),
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 } // Sin bordes negros
            };
        }

        public static void EstilarGrid(DataGridView dgv)
        {
            // Fondo limpio
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            
            // Quitar lineas negras
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(230, 230, 230); // Gris muy clarito para las líneas
            
            // Selección suave
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 200); // Verde muy suave
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            
            // Fuente y espaciado
            dgv.DefaultCellStyle.Font = MainFont;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.RowTemplate.Height = 45; // Filas más altas (más aire)
            
            // Cabecera
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
            dgv.ColumnHeadersHeight = 50;
        }
    }
}