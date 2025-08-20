using System;
using System.Collections.Generic;

namespace QLBanSachWeb.Models;

public partial class DonHang
{
    public int MaDH { get; set; }

    public int? MaKH { get; set; }

    public DateTime? NgayDat { get; set; }

    public decimal? TongTien { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<CT_DonHang> CT_DonHangs { get; set; } = new List<CT_DonHang>();

    public virtual KhachHang? MaKHNavigation { get; set; }
}
