using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly QLBanSachContext _context;

        public KhachHangsController(QLBanSachContext context)
        {
            _context = context;
        }

        // GET: KhachHang
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhachHangs.ToListAsync());
        }

        // GET: KhachHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var kh = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKH == id);

            if (kh == null) return NotFound();

            return View(kh);
        }

        // GET: KhachHang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhachHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email trùng
                bool emailExists = await _context.KhachHangs
                    .AnyAsync(x => x.Email == kh.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email này đã tồn tại");
                    return View(kh); // Trả lại view với lỗi hiển thị ở asp-validation-for="Email"
                }

                // Hash mật khẩu nếu cần
                // kh.MatKhau = _passwordHasher.HashPassword(kh.MatKhau);

                _context.Add(kh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(kh);
        }


        // GET: KhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var kh = await _context.KhachHangs.FindAsync(id);
            if (kh == null) return NotFound();

            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhachHang kh)
        {
            // Bỏ qua lỗi required của MatKhau khi edit
            ModelState.Remove("MatKhau");

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
                    khachHang.MatKhau = kh.MatKhau; // hash nếu cần
                }

                _context.Update(khachHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(kh);
        }



        // POST: KhachHang/Delete/5
        [HttpPost]
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
