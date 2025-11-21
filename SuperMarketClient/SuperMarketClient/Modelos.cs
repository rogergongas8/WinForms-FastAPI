namespace SuperMarketClient
{
    public class Producto
    {
        public int id { get; set; }
        public string NOMBRE { get; set; }
        public string CATEGORIA { get; set; }
        public float PRECIOKG { get; set; }
        public float STOCK { get; set; } 
    }

    public class Venta
    {
        public int id { get; set; }
        public int producto_id { get; set; }
        public float kilos_vendidos { get; set; }
        public float total_venta { get; set; }
    }
}