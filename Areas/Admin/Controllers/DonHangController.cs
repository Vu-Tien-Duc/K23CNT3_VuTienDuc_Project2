using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangController : Controller
    {
        private readonly QLBanSachContext _context;

        public DonHangController(QLBanSachContext context)
        {
            _context = context;
        }

        private void LoadTrangThaiList()
        {
            ViewBag.TrangThaiList = new List<string>
            {
                "Chờ xử lý",
                "Đang giao",
                "Đã giao",
                "Đã hủy"
            };
        }

        // GET: Admin/DonHang
        public async Task<IActionResult> Index()
        {
            var donHangs = await _context.DonHangs
                .Include(d => d.MaKHNavigation)
                .ToListAsync();
            return View(donHangs);
        }

        // GET: Admin/DonHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.MaKHNavigation)
                .Include(d => d.CT_DonHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .FirstOrDefaultAsync(m => m.MaDH == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // GET: Admin/DonHang/Create
        public IActionResult Create()
        {
            LoadTrangThaiList();
            ViewBag.KhachHangs = _context.KhachHangs.ToList();

            return View(new DonHang
            {
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xử lý"
            });
        }

        // POST: Admin/DonHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                if (!donHang.NgayDat.HasValue)
                    donHang.NgayDat = DateTime.Now;

                _context.Add(donHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadTrangThaiList();
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            return View(donHang);
        }

        // GET: Admin/DonHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return NotFound();

            LoadTrangThaiList();
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            return View(donHang);
        }

        // POST: Admin/DonHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            if (id != donHang.MaDH) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DonHangs.Any(e => e.MaDH == donHang.MaDH))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            LoadTrangThaiList();
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            return View(donHang);
        }

        // POST: Admin/DonHang/Delete/5 (xóa nhanh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.CT_DonHangs)
                .FirstOrDefaultAsync(d => d.MaDH == id);

            if (donHang != null)
            {
                if (donHang.CT_DonHangs?.Any() == true)
                    _context.CT_DonHangs.RemoveRange(donHang.CT_DonHangs);

                _context.DonHangs.Remove(donHang);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
