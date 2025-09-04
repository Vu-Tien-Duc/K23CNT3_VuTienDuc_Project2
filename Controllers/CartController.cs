using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBanSachWeb.Models;
using System.Linq;

namespace QLBanSachWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly QLBanSachContext _context;

        public CartController(QLBanSachContext context)
        {
            _context = context;
        }

        // Lấy hoặc tạo giỏ hàng theo khách hàng
        private GioHang? GetOrCreateCart()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH") ?? 0;
            if (maKH == 0) return null;

            var cart = _context.GioHangs
                        .Include(g => g.CT_GioHangs)
                        .ThenInclude(ct => ct.MaSachNavigation)
                        .FirstOrDefault(g => g.MaKH == maKH);

            if (cart == null)
            {
                cart = new GioHang
                {
                    MaKH = maKH,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(cart);
                _context.SaveChanges();
            }

            return cart;
        }

        // Cập nhật số lượng giỏ hàng vào session
        private void UpdateCartCount(int maGH)
        {
            var count = _context.CT_GioHangs
                                .Where(ct => ct.MaGH == maGH)
                                .Sum(ct => ct.SoLuong);
            HttpContext.Session.SetInt32("CartCount", count);
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetOrCreateCart();
            if (cart == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để sử dụng giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            var items = _context.CT_GioHangs
                .Where(ct => ct.MaGH == cart.MaGH)
                .Include(ct => ct.MaSachNavigation)
                .ToList();

            ViewBag.Total = items.Sum(i => i.SoLuong * i.MaSachNavigation.GiaBan);
            UpdateCartCount(cart.MaGH);
            return View(items);
        }

        // Thêm sách vào giỏ
        public IActionResult AddToCart(int id)
        {
            var cart = GetOrCreateCart();
            if (cart == null) return RedirectToAction("Login", "Account");

            var sach = _context.Saches.Find(id);
            if (sach == null) return NotFound();

            var item = _context.CT_GioHangs
                        .FirstOrDefault(ct => ct.MaGH == cart.MaGH && ct.MaSach == id);

            if (item == null)
            {
                item = new CT_GioHang
                {
                    MaGH = cart.MaGH,
                    MaSach = id,
                    SoLuong = 1,
                    MaSachNavigation = sach
                };
                _context.CT_GioHangs.Add(item);
            }
            else
            {
                if (item.SoLuong < sach.SoLuong)
                {
                    item.SoLuong++;
                    _context.CT_GioHangs.Update(item);
                }
                else
                {
                    TempData["Error"] = $"Sách '{sach.TenSach}' chỉ còn {sach.SoLuong} sản phẩm trong kho.";
                    return RedirectToAction("Checkout");
                }
            }

            _context.SaveChanges();
            UpdateCartCount(cart.MaGH);

            return RedirectToAction("Checkout");
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult Update(int maSach, int soLuong)
        {
            var cart = GetOrCreateCart();
            if (cart == null) return RedirectToAction("Login", "Account");

            var item = _context.CT_GioHangs
                        .Include(ct => ct.MaSachNavigation)
                        .FirstOrDefault(ct => ct.MaGH == cart.MaGH && ct.MaSach == maSach);

            if (item != null)
            {
                int available = item.MaSachNavigation.SoLuong ?? 0;
                if (soLuong > available)
                {
                    TempData["Error"] = $"Sách '{item.MaSachNavigation.TenSach}' chỉ còn {available} sản phẩm trong kho.";
                }
                else
                {
                    item.SoLuong = soLuong;
                    _context.CT_GioHangs.Update(item);
                }

                _context.SaveChanges();
                UpdateCartCount(cart.MaGH);
            }

            return RedirectToAction("Checkout");
        }

        // Xóa khỏi giỏ
        public IActionResult Remove(int maSach)
        {
            var cart = GetOrCreateCart();
            if (cart == null) return RedirectToAction("Login", "Account");

            var item = _context.CT_GioHangs
                        .FirstOrDefault(ct => ct.MaGH == cart.MaGH && ct.MaSach == maSach);

            if (item != null)
            {
                _context.CT_GioHangs.Remove(item);
                _context.SaveChanges();
                UpdateCartCount(cart.MaGH);
            }

            return RedirectToAction("Checkout");
        }

        // GET: Checkout
        public IActionResult Checkout()
        {
            var cart = GetOrCreateCart();
            if (cart == null) return RedirectToAction("Login", "Account");

            var cartItems = _context.CT_GioHangs
                                    .Where(ct => ct.MaGH == cart.MaGH)
                                    .Include(ct => ct.MaSachNavigation)
                                    .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            return View(cartItems);
        }

        // POST: Thanh toán
        [HttpPost]
        public IActionResult CheckoutConfirm()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null) return RedirectToAction("Login", "Account");

            var cart = GetOrCreateCart();
            if (cart == null) return RedirectToAction("Index");

            var cartItems = _context.CT_GioHangs
                                    .Where(ct => ct.MaGH == cart.MaGH)
                                    .Include(ct => ct.MaSachNavigation)
                                    .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Kiểm tra tồn kho
            foreach (var item in cartItems)
            {
                if (item.SoLuong > item.MaSachNavigation.SoLuong)
                {
                    TempData["Error"] = $"Sách '{item.MaSachNavigation.TenSach}' không đủ số lượng. Tồn kho: {item.MaSachNavigation.SoLuong}";
                    return RedirectToAction("Checkout");
                }
            }

            // Tạo đơn hàng
            decimal tongTien = cartItems.Sum(i => i.SoLuong * i.MaSachNavigation.GiaBan);
            var order = new DonHang
            {
                MaKH = maKH.Value,
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                TongTien = tongTien
            };
            _context.DonHangs.Add(order);
            _context.SaveChanges();

            // Thêm chi tiết đơn hàng và trừ tồn kho
            foreach (var item in cartItems)
            {
                var ct = new CT_DonHang
                {
                    MaDH = order.MaDH,
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = item.MaSachNavigation.GiaBan
                };
                _context.CT_DonHangs.Add(ct);

                // Trừ tồn kho
                item.MaSachNavigation.SoLuong -= item.SoLuong;
                _context.Saches.Update(item.MaSachNavigation);
            }

            // Xóa giỏ hàng
            _context.CT_GioHangs.RemoveRange(cartItems);
            _context.GioHangs.Remove(cart);

            _context.SaveChanges();
            HttpContext.Session.SetInt32("CartCount", 0);

            return RedirectToAction("OrderSuccess", new { id = order.MaDH });
        }

        // Trang thông báo thành công
        public IActionResult OrderSuccess(int id)
        {
            var order = _context.DonHangs
                        .Include(d => d.MaKHNavigation)
                        .Include(d => d.CT_DonHangs)
                            .ThenInclude(ct => ct.MaSachNavigation)
                        .FirstOrDefault(d => d.MaDH == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // Danh sách đơn hàng
        public IActionResult MyOrders()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null) return RedirectToAction("Login", "Account");

            var orders = _context.DonHangs
                .Where(d => d.MaKH == maKH)
                .OrderByDescending(d => d.NgayDat)
                .ToList();

            return View(orders);
        }
    }
}
