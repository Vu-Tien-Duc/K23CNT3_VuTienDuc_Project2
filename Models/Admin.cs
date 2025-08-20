using System;
using System.Collections.Generic;

namespace QLBanSachWeb.Models;

public partial class Admin
{
    public int MaAdmin { get; set; }

    public string? HoTen { get; set; }

    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;
}
