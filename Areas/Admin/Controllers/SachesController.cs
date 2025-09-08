using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;
using X.PagedList; // cần cho ToPagedList()

using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace QLBanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SachesController : Controller
    {
        private readonly QLBanSachContext _context;
        private readonly IWebHostEnvironment _env;

        public SachesController(QLBanSachContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Saches
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.TenSach.Contains(searchString) || s.TacGia.Contains(searchString));
            }

            query = query.OrderBy(s => s.TenSach);

            var list = await query.ToListAsync();

            var sachesPaged = new StaticPagedList<Sach>(
                list.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                pageNumber,
                pageSize,
                list.Count
            );

            ViewBag.SearchString = searchString;
            return View(sachesPaged);
        }




        // GET: Admin/Saches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);

            if (sach == null) return NotFound();

            return View(sach);
        }

        // GET: Admin/Saches/Create
        public IActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(_context.LoaiSaches, "MaLoai", "TenLoai");
            return View();
        }

        // POST: Admin/Saches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sach sach)
        {
            if (ModelState.IsValid)
            {
                if (sach.HinhAnhFile != null && sach.HinhAnhFile.Length > 0)
                {
                    string fileName = Path.GetFileName(sach.HinhAnhFile.FileName);
                    string path = Path.Combine(_env.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await sach.HinhAnhFile.CopyToAsync(stream);
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

        // GET: Admin/Saches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches.FindAsync(id);
            if (sach == null) return NotFound();

            ViewBag.MaLoai = new SelectList(_context.LoaiSaches, "MaLoai", "TenLoai", sach.MaLoai);
            return View(sach);
        }

        // POST: Admin/Saches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sach sach, string? oldImage)


        {
            if (id != sach.MaSach) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSach = await _context.Saches.FindAsync(id);
                    if (existingSach == null) return NotFound();

                    // update fields
                    existingSach.TenSach = sach.TenSach;
                    existingSach.TacGia = sach.TacGia;
                    existingSach.MaLoai = sach.MaLoai;
                    existingSach.NXB = sach.NXB;
                    existingSach.NamXB = sach.NamXB;
                    existingSach.GiaBan = sach.GiaBan;
                    existingSach.SoLuong = sach.SoLuong;
                    existingSach.MoTa = sach.MoTa;

                    // upload ảnh mới
                    if (sach.HinhAnhFile != null && sach.HinhAnhFile.Length > 0)
                    {
                        string fileName = Path.GetFileName(sach.HinhAnhFile.FileName);
                        string path = Path.Combine(_env.WebRootPath, "images", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await sach.HinhAnhFile.CopyToAsync(stream);
                        }

                        existingSach.HinhAnh = fileName;
                    }
                    else
                    {
                        existingSach.HinhAnh = oldImage; // giữ ảnh cũ
                    }

                    _context.Update(existingSach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Saches.Any(e => e.MaSach == sach.MaSach))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoai = new SelectList(_context.LoaiSaches, "MaLoai", "TenLoai", sach.MaLoai);
            return View(sach);
        }

        // GET: Admin/Saches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);

            if (sach == null) return NotFound();

            return View(sach);
        }

        // POST: Admin/Saches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach != null)
            {
                _context.Saches.Remove(sach);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
