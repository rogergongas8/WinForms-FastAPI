using System.Drawing;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public static class UI
    {
        public static Color PrimaryColor = Color.FromArgb(45, 52, 71);
        public static Color AccentColor = Color.FromArgb(0, 122, 204); 
        public static Color BackColor = Color.WhiteSmoke;       
        public static Color LightText = Color.White;
        
        public static Font MainFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font HeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);

        public static Button CrearBoton(string texto, int x, int y, Color colorFondo)
        {
            return new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(100, 35),
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand 
            };
        }

        public static void EstilarGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = MainFont;
            dgv.RowTemplate.Height = 35;
            
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
            dgv.ColumnHeadersHeight = 40;
        }
    }
}