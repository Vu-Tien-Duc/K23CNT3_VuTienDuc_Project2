using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GioHangController : Controller
    {
        private readonly QLBanSachContext _context;

        public GioHangController(QLBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/GioHang
        public async Task<IActionResult> Index()
        {
            var gioHangs = await _context.GioHangs
                .Include(g => g.MaKHNavigation)
                .Include(g => g.CT_GioHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .ToListAsync();

            return View(gioHangs);
        }

        // GET: Admin/GioHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gioHang = await _context.GioHangs
                .Include(g => g.MaKHNavigation)
                .Include(g => g.CT_GioHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .FirstOrDefaultAsync(m => m.MaGH == id);

            if (gioHang == null) return NotFound();

            return View(gioHang);
        }

        // Xóa giỏ hàng
        public async Task<IActionResult> Delete(int id)
        {
            var gioHang = await _context.GioHangs
                .Include(g => g.CT_GioHangs)
                .FirstOrDefaultAsync(g => g.MaGH == id);

            if (gioHang != null)
            {
                _context.CT_GioHangs.RemoveRange(gioHang.CT_GioHangs);
                _context.GioHangs.Remove(gioHang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
