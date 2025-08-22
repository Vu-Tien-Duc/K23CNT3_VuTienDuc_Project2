using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoaiSachesController : Controller
    {
        private readonly QLBanSachContext _context;

        public LoaiSachesController(QLBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/LoaiSaches
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoaiSaches.ToListAsync());
        }

        // GET: Admin/LoaiSaches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loaiSach = await _context.LoaiSaches
                .FirstOrDefaultAsync(m => m.MaLoai == id);

            if (loaiSach == null) return NotFound();

            return View(loaiSach);
        }

        // GET: Admin/LoaiSaches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiSaches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiSach loaiSach)
        {
            if (ModelState.IsValid)
            {
                _context.LoaiSaches.Add(loaiSach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSach);
        }

        // GET: Admin/LoaiSaches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loai = await _context.LoaiSaches.FindAsync(id);
            if (loai == null) return NotFound();

            return View(loai);
        }

        // POST: Admin/LoaiSaches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiSach loaiSach)
        {
            if (id != loaiSach.MaLoai) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiSach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LoaiSaches.Any(e => e.MaLoai == loaiSach.MaLoai))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSach);
        }

        // GET: Admin/LoaiSaches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loai = await _context.LoaiSaches
                .FirstOrDefaultAsync(m => m.MaLoai == id);

            if (loai == null) return NotFound();

            return View(loai);
        }

        // POST: Admin/LoaiSaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loai = await _context.LoaiSaches.FindAsync(id);
            if (loai != null)
            {
                _context.LoaiSaches.Remove(loai);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        
    }
}
