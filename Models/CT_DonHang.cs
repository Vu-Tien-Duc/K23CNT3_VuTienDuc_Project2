using System;
using System.Collections.Generic;

namespace QLBanSachWeb.Models;

public partial class CT_DonHang
{
    public int MaDH { get; set; }

    public int MaSach { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual DonHang MaDHNavigation { get; set; } = null!;

    public virtual Sach MaSachNavigation { get; set; } = null!;
}
