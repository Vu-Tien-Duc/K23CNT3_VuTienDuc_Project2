using System;
using System.Collections.Generic;

namespace QLBanSachWeb.Models;

public partial class LoaiSach
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
