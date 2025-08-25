using Microsoft.AspNetCore.Mvc;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly QLBanSachContext _context;

        public AccountController(QLBanSachContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string matKhau)
        {
            var user = _context.KhachHangs.FirstOrDefault(kh => kh.Email == email && kh.MatKhau == matKhau);
            if (user != null)
            {
                // Lưu session
                HttpContext.Session.SetInt32("MaKH", user.MaKH);
                HttpContext.Session.SetString("HoTen", user.HoTen);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai email hoặc mật khẩu!";
            return View();
        }

        // GET: Account/Signup
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng email
                var exists = _context.KhachHangs.Any(u => u.Email == model.Email);
                if (exists)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                    return View(model);
                }

               
                _context.KhachHangs.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";

                // ❌ Không tạo session tại đây
                // HttpContext.Session.SetInt32("MaKH", model.MaKH);

                // ✅ Chuyển đến trang login
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
