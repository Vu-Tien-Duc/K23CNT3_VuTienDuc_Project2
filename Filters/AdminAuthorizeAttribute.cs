using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QLBanSachWeb.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var adminId = session.GetInt32("MaAdmin");

            if (adminId == null)
            {
                // Chưa đăng nhập → chuyển về trang Login
                context.Result = new RedirectToActionResult("Login", "AdminAccount", new { area = "Admin" });
            }
        }
    }
}
