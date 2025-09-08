using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QLBanSachWeb.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace QLBanSachWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly QLBanSachContext _context;

        public AccountController(QLBanSachContext context)
        {
            _context = context;
        }

        // ================= LOGIN =================
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var hashedPassword = HashPassword(password);
            var user = _context.KhachHangs.FirstOrDefault(u => u.Email == email && u.MatKhau == hashedPassword);

            if (user != null)
            {
                HttpContext.Session.SetInt32("MaKH", user.MaKH);
                HttpContext.Session.SetString("TenKH", user.HoTen);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng.";
            return View();
        }

        // ================= SIGNUP =================
        public IActionResult SignUp() => View();

        [HttpPost]
        public IActionResult SignUp(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                if (_context.KhachHangs.Any(u => u.Email == model.Email))
                {
                    ViewBag.Error = "Email đã được sử dụng.";
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.MatKhau) || model.MatKhau.Length < 6)
                {
                    ViewBag.Error = "Mật khẩu phải lớn hơn hoặc bằng 6 ký tự.";
                    return View(model);
                }

                model.MatKhau = HashPassword(model.MatKhau);
                _context.KhachHangs.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ================= FORGOT PASSWORD =================
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.KhachHangs.FirstOrDefault(kh => kh.Email == email);
            if (user == null)
            {
                TempData["Error"] = "Email không tồn tại trong hệ thống!";
                return View();
            }

            // Tạo mật khẩu mới
            var newPassword = GenerateRandomPassword(8);
            user.MatKhau = HashPassword(newPassword);
            _context.SaveChanges();

            // Hiển thị trực tiếp mật khẩu mới cho khách
            TempData["Success"] = $"Mật khẩu mới của bạn là: {newPassword}";
            return View();
        }

        // ================= UTILS =================
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
     
        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
