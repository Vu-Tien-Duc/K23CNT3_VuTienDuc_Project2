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

                // ✅ Kiểm tra mật khẩu ≥6 ký tự
                if (string.IsNullOrEmpty(model.MatKhau) || model.MatKhau.Length < 6)
                {
                    ViewBag.Error = "Mật khẩu phải lớn hơn hoặc bằng 6 ký tự.";
                    return View(model);
                }

                // ✅ Hash mật khẩu trước khi lưu
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

            var newPassword = GenerateRandomPassword(8);
            user.MatKhau = HashPassword(newPassword); // ✅ Hash lại mật khẩu mới
            _context.SaveChanges();

            Console.WriteLine($"[DEBUG] Gửi email tới {email}: Mật khẩu mới = {newPassword}");

            TempData["Success"] = "Mật khẩu mới đã được gửi đến email của bạn!";
            return RedirectToAction("Login");
        }

        // ================= SETTINGS =================
        public IActionResult Settings()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null) return RedirectToAction("Login");

            var user = _context.KhachHangs.Find(maKH);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(KhachHang model)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null) return RedirectToAction("Login");

            var user = _context.KhachHangs.Find(maKH);
            if (user == null) return NotFound();

            user.HoTen = model.HoTen;
            user.Email = model.Email;
            user.SDT = model.SDT;
            user.DiaChi = model.DiaChi;

            _context.Update(user);
            _context.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Settings");
        }

        // ================= CHANGE PASSWORD =================
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null) return RedirectToAction("Login");

            var user = _context.KhachHangs.Find(maKH);
            if (user == null) return NotFound();

            var currentHashed = HashPassword(currentPassword);
            if (user.MatKhau != currentHashed)
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng!";
                return RedirectToAction("Settings");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp!";
                return RedirectToAction("Settings");
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải lớn hơn hoặc bằng 6 ký tự!";
                return RedirectToAction("Settings");
            }

            user.MatKhau = HashPassword(newPassword);
            _context.Update(user);
            _context.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Settings");
        }

        // ================= UTILS =================
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash); // ✅ Chuẩn Base64
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
