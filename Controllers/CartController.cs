using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly QLBanSachContext _context;

        public CartController(QLBanSachContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            var session = HttpContext.Session.GetString("Cart");
            if (session != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(session) ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }

        // Lưu giỏ hàng vào Session
        private void SaveCart(List<CartItem> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", json);
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(x => x.ThanhTien);
            return View(cart);
        }

        // Thêm vào giỏ
        public IActionResult AddToCart(int id)
        {
            var sach = _context.Saches.Find(id);
            if (sach == null) return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSach == id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    MaSach = sach.MaSach,
                    TenSach = sach.TenSach,
                    HinhAnh = sach.HinhAnh,
                    GiaBan = sach.GiaBan,
                    SoLuong = 1
                });
            }
            else
            {
                item.SoLuong++;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Xóa khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSach == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult Update(int id, int soLuong)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSach == id);
            if (item != null)
            {
                item.SoLuong = soLuong;
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }
    }
}
