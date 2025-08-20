using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBanSachWeb.Models;

public partial class Sach
{
    public int MaSach { get; set; }

    public string TenSach { get; set; } = null!;

    public string? TacGia { get; set; }

    public int? MaLoai { get; set; }

    public string? NXB { get; set; }

    public int? NamXB { get; set; }

    public decimal GiaBan { get; set; }

    public int? SoLuong { get; set; }

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }
    [NotMapped]
    public IFormFile? HinhAnhFile { get; set; } // cho phép null


    public virtual ICollection<CT_DonHang> CT_DonHangs { get; set; } = new List<CT_DonHang>();

    public virtual ICollection<CT_GioHang> CT_GioHangs { get; set; } = new List<CT_GioHang>();

    public virtual LoaiSach? MaLoaiNavigation { get; set; }
}
