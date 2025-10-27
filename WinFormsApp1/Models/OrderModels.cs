namespace WinFormsApp1.Models
{
    // Örn: gelen sipariş
    public class W_Order
    {
        public int Id { get; set; }
        public string Müşteri { get; set; }
        public string Adres { get; set; }
        public List<string> Ürünler { get; set; }
    }

    // Menü grubu
    public class OrderMenuGrup
    {
        public int Id { get; set; }
        public string Grup { get; set; }
        public string Ad { get; set; }
        public decimal Fiyat { get; set; }
    }
}