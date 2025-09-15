using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;

public class HomeController : Controller
{
    private readonly QLBanSachContext _context;

    public HomeController(QLBanSachContext context)
    {
        _context = context;
    }

    // GET: /Home/Index
    public async Task<IActionResult> Index()
    {
        // Sách bán chạy: top 8 theo số lượng bán trong CT_DonHang
        var bestSellers = await _context.Saches
            .Include(s => s.CT_DonHangs)
            .OrderByDescending(s => s.CT_DonHangs.Sum(ct => (int?)ct.SoLuong) ?? 0)
            .Take(8)
            .ToListAsync();

        // Sách nổi bật: ví dụ lấy 8 sách mới nhất (theo năm XB)
        var featuredBooks = await _context.Saches
            .OrderByDescending(s => s.NamXB ?? 0)
            .Take(8)
            .ToListAsync();

        ViewBag.BestSellers = bestSellers;
        ViewBag.FeaturedBooks = featuredBooks;

        return View();
    }

    // GET: /Home/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var sach = await _context.Saches
            .Include(s => s.MaLoaiNavigation) // lấy thêm thể loại
            .FirstOrDefaultAsync(m => m.MaSach == id);

        if (sach == null) return NotFound();

        return View(sach);
    }
}
