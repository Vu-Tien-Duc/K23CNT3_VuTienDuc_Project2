using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;

namespace QLBanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly QLBanSachContext _context;

        public KhachHangsController(QLBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/KhachHangs
        public async Task<IActionResult> Index()
        {
            var list = await _context.KhachHangs.ToListAsync();
            return View(list);
        }

        // GET: Admin/KhachHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var kh = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKH == id);
            if (kh == null) return NotFound();

            return View(kh);
        }

        // GET: Admin/KhachHangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                // kiểm tra email trùng
                bool emailExists = await _context.KhachHangs.AnyAsync(x => x.Email == kh.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email này đã tồn tại");
                    return View(kh);
                }

                // kiểm tra độ dài mật khẩu
                if (string.IsNullOrEmpty(kh.MatKhau) || kh.MatKhau.Length < 6)
                {
                    ModelState.AddModelError("MatKhau", "Mật khẩu phải có ít nhất 6 ký tự");
                    return View(kh);
                }

                // hash mật khẩu trực tiếp
                using (var sha = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(kh.MatKhau);
                    var hash = sha.ComputeHash(bytes);
                    kh.MatKhau = Convert.ToBase64String(hash);
                }

                _context.Add(kh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kh);
        }

        // GET: Admin/KhachHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var kh = await _context.KhachHangs.FindAsync(id);
            if (kh == null) return NotFound();

            return View(kh);
        }

        // POST: Admin/KhachHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhachHang kh)
        {
            ModelState.Remove("MatKhau"); // cho phép bỏ trống mật khẩu khi edit
            ModelState.Remove("ConfirmMatKhau");

            if (id != kh.MaKH) return NotFound();

            if (ModelState.IsValid)
            {
                var khachHang = await _context.KhachHangs.FindAsync(id);
                if (khachHang == null) return NotFound();

                khachHang.HoTen = kh.HoTen;
                khachHang.Email = kh.Email;
                khachHang.SDT = kh.SDT;
                khachHang.DiaChi = kh.DiaChi;

                if (!string.IsNullOrEmpty(kh.MatKhau))
                {
                    if (kh.MatKhau.Length < 6)
                    {
                        ModelState.AddModelError("MatKhau", "Mật khẩu phải có ít nhất 6 ký tự");
                        return View(kh);
                    }

                    // hash mật khẩu trực tiếp
                    using (var sha = SHA256.Create())
                    {
                        var bytes = Encoding.UTF8.GetBytes(kh.MatKhau);
                        var hash = sha.ComputeHash(bytes);
                        khachHang.MatKhau = Convert.ToBase64String(hash);
                    }
                }

                _context.Update(khachHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(kh);
        }

        // POST: Admin/KhachHangs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kh = await _context.KhachHangs.FindAsync(id);
            if (kh != null)
            {
                _context.KhachHangs.Remove(kh);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
