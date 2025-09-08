using Microsoft.AspNetCore.Mvc;
using QLBanSachWeb.Models;
using System.Linq;
using Newtonsoft.Json; // dùng để serialize JSON
using QLBanSachWeb.Filters;
namespace QLBanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class HomeController : Controller
    {
        private readonly QLBanSachContext _context;

        public HomeController(QLBanSachContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var doanhThuTheoThang = _context.DonHangs
                .Where(d => d.NgayDat != null)
                .Select(d => new
                {
                    d.NgayDat,
                    TongTien = d.CT_DonHangs.Sum(ct => ct.SoLuong * ct.DonGia)
                })
                .GroupBy(d => new { Thang = d.NgayDat.Value.Month, Nam = d.NgayDat.Value.Year })
                .Select(g => new
                {
                    Month = g.Key.Thang,
                    Year = g.Key.Nam,
                    TongDoanhThu = g.Sum(x => x.TongTien)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            // Chuyển dữ liệu sang JSON để Chart.js dùng
            ViewBag.DoanhThuTheoThang = JsonConvert.SerializeObject(doanhThuTheoThang);

            return View();
        }
    }
}
