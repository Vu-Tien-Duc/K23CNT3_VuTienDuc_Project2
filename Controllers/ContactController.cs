using Microsoft.AspNetCore.Mvc;
using QLBanSachWeb.Models;

namespace QLBanSachWeb.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: Gửi email (hiện tại đánh dấu ⚠)
            TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm.";

            return RedirectToAction("Index");
        }
    }
}
