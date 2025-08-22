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
    public async Task<IActionResult> Index(string? searchString, int? categoryId, string? sortOrder, int page = 1, int pageSize = 6)
    {
        var sach = _context.Saches.AsQueryable();

        // Lọc theo từ khóa
        if (!string.IsNullOrEmpty(searchString))
        {
            sach = sach.Where(s => s.TenSach.Contains(searchString));
        }

        // Lọc theo thể loại
        if (categoryId.HasValue)
        {
            sach = sach.Where(s => s.MaLoai == categoryId);
        }

        // Sắp xếp
        ViewBag.CurrentSort = sortOrder;
        ViewBag.PriceSort = sortOrder == "price_asc" ? "price_desc" : "price_asc";

        switch (sortOrder)
        {
            case "price_asc":
                sach = sach.OrderBy(s => s.GiaBan);
                break;
            case "price_desc":
                sach = sach.OrderByDescending(s => s.GiaBan);
                break;
            default:
                sach = sach.OrderBy(s => s.TenSach);
                break;
        }

        // Phân trang
        int totalItems = await sach.CountAsync();
        var items = await sach.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.CurrentPage = page;

        // Thể loại
        ViewBag.Categories = await _context.LoaiSaches.ToListAsync();
        ViewBag.SelectedCategory = categoryId;

        return View(items);
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
