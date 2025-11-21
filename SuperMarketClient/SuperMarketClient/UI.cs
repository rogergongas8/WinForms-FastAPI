using System.Drawing;
using System.Windows.Forms;

namespace SuperMarketClient
{
    public static class UI
    {
        
        public static Color PrimaryColor = Color.FromArgb(56, 142, 60); 
        
        public static Color AccentColor = Color.FromArgb(255, 112, 67);
        
        public static Color BackColor = Color.FromArgb(250, 248, 245);
        
        public static Color TextColor = Color.FromArgb(60, 60, 60);

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
                FlatAppearance = { BorderSize = 0 } 
            };
        }

        public static void EstilarGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 201);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = MainFont;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.RowTemplate.Height = 40;
            
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
            dgv.ColumnHeadersHeight = 45;
        }
    }
}