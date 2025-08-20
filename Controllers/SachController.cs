using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Controllers
{
    public class SachController : Controller
    {
        private readonly QLBanSachContext _context;

        public SachController(QLBanSachContext context)
        {
            _context = context;
        }

        // GET: Sach
        public IActionResult Index()
        {
            var saches = _context.Saches.Include(s => s.MaLoaiNavigation).ToList();

            return View(saches);
        }

        // GET: Sach/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var sach = _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefault(m => m.MaSach == id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // GET: Sach/Create
        public IActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(_context.LoaiSaches, "MaLoai", "TenLoai");
            return View();
        }

        // POST: Sach/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sach sach, IFormFile HinhAnhFile)
        {
            if (ModelState.IsValid)
            {
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    var fileName = Path.GetFileName(HinhAnhFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnhFile.CopyToAsync(stream);
                    }

                    sach.HinhAnh = fileName;
                }

                _context.Add(sach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaLoai = new SelectList(_context.LoaiSaches, "MaLoai", "TenLoai", sach.MaLoai);
            return View(sach);
        }

        // GET: Sach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches.FindAsync(id);
            if (sach == null) return NotFound();

            ViewBag.MaLoai = new SelectList(_context.LoaiSaches, "MaLoai", "TenLoai", sach.MaLoai);

            return View(sach);
        }


        // POST: Sach/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sach sach, string OldImage)
        {
            if (id != sach.MaSach) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (sach.HinhAnhFile != null && sach.HinhAnhFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(sach.HinhAnhFile.FileName);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await sach.HinhAnhFile.CopyToAsync(stream);
                        }

                        sach.HinhAnh = fileName;
                    }
                    else
                    {
                        sach.HinhAnh = OldImage; // giữ lại ảnh cũ
                    }


                    _context.Update(sach);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Saches.Any(e => e.MaSach == sach.MaSach))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.MaLoai = new SelectList(_context.LoaiSaches, "MaLoai", "TenLoai", sach.MaLoai);
            return View(sach);
        }

        // GET: Sach/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var sach = _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefault(m => m.MaSach == id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // POST: Sach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach != null)
            {
                try
                {
                    // Xóa file ảnh vật lý nếu có
                    if (!string.IsNullOrEmpty(sach.HinhAnh))
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", sach.HinhAnh);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Xóa trong database
                    _context.Saches.Remove(sach);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Bắt lỗi nếu có ràng buộc FK
                    TempData["ErrorMessage"] = "Không thể xóa sách này vì đang được sử dụng. Chi tiết: " + ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
