using Microsoft.AspNetCore.Mvc;
using QLBanSachWeb.Models;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace QLBanSachWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountController : Controller
    {
        private readonly QLBanSachContext _context;

        public AdminAccountController(QLBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin Login
        public IActionResult Login() => View();

        // POST: Admin Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var hashedPassword = HashPassword(password);
            // ✅ Check bảng Admin, không phải KhachHang
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email && a.MatKhau == hashedPassword);

            if (admin != null)
            {
                HttpContext.Session.SetInt32("MaAdmin", admin.MaAdmin);
                HttpContext.Session.SetString("TenAdmin", admin.HoTen ?? "Admin");
                HttpContext.Session.SetString("HinhAnh", admin.HinhAnh);
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng.";
            return View();
        }

        // GET: Admin Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        // ✅ Hàm hash chuẩn Base64 (giống User)
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
