using System;
using System.Collections.Generic;

namespace QLBanSachWeb.Models;

public partial class GioHang
{
    public int MaGH { get; set; }

    public int? MaKH { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<CT_GioHang> CT_GioHangs { get; set; } = new List<CT_GioHang>();

    public virtual KhachHang? MaKHNavigation { get; set; }
}
