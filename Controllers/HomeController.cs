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
    public async Task<IActionResult> Index(string? searchString, int? categoryId)
    {
        // Lấy tất cả thể loại đưa sang ViewBag
        ViewBag.Categories = await _context.LoaiSaches.ToListAsync();
        ViewBag.SelectedCategory = categoryId;

        var sach = from s in _context.Saches
                   select s;

        if (!string.IsNullOrEmpty(searchString))
        {
            sach = sach.Where(s => s.TenSach.Contains(searchString));
        }

        if (categoryId.HasValue)
        {
            sach = sach.Where(s => s.MaLoai == categoryId);
        }

        return View(await sach.ToListAsync());
    }

    // GET: /Home/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var sach = await _context.Saches
            .Include(s => s.MaLoaiNavigation) // để lấy tên thể loại
            .FirstOrDefaultAsync(m => m.MaSach == id);

        if (sach == null) return NotFound();

        return View(sach);
    }
}
