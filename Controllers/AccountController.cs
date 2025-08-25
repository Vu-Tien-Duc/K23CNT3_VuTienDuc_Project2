using Microsoft.AspNetCore.Mvc;
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

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
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

        // GET: SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: SignUp
        [HttpPost]
        public IActionResult SignUp(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                // check email tồn tại chưa
                if (_context.KhachHangs.Any(u => u.Email == model.Email))
                {
                    ViewBag.Error = "Email đã được sử dụng.";
                    return View(model);
                }

                model.MatKhau = HashPassword(model.MatKhau);
              

                _context.KhachHangs.Add(model);
                _context.SaveChanges();

                // ✅ Sau khi SignUp thành công → chuyển qua trang Login
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Hàm băm mật khẩu (SHA256)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
