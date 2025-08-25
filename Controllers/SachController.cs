using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;
using System.Linq;

namespace QLBanSachWeb.Controllers
{
    public class SachController : Controller
    {
        private readonly QLBanSachContext _context;

        public SachController(QLBanSachContext context)
        {
            _context = context;
        }

        // 📖 Danh sách tất cả sách (có phân trang + lọc theo loại sách)
        public IActionResult Index(int? maLoai, int page = 1, int pageSize = 6)
        {
            var query = _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .AsQueryable();

            // Nếu lọc theo loại
            if (maLoai.HasValue)
            {
                query = query.Where(s => s.MaLoai == maLoai.Value);
            }

            // Phân trang
            int totalItems = query.Count();
            var saches = query
                .OrderByDescending(s => s.NamXB)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Gửi dữ liệu cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.MaLoai = maLoai;
            ViewBag.LoaiSaches = _context.LoaiSaches.ToList();

            return View(saches);
        }

        // 🔍 Chi tiết sách
        public IActionResult Details(int id)
        {
            var sach = _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefault(s => s.MaSach == id);

            if (sach == null) return NotFound();

            return View(sach);
        }
    }
}
