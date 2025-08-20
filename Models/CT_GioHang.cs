using System;
using System.Collections.Generic;

namespace QLBanSachWeb.Models;

public partial class CT_GioHang
{
    public int MaGH { get; set; }

    public int MaSach { get; set; }

    public int SoLuong { get; set; }

    public virtual GioHang MaGHNavigation { get; set; } = null!;

    public virtual Sach MaSachNavigation { get; set; } = null!;
}
