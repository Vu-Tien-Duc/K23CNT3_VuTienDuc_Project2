namespace QLBanSachWeb.Models
{
    public class CartItem
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; } = null!;
        public string? HinhAnh { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien => GiaBan * SoLuong;
    }
}
