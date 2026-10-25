using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TrungTamNgoaiNgu.Filters
{
    /// <summary>
    /// Action Filter kiểm tra quyền truy cập ở cấp độ Server-side.
    /// Không chỉ ẩn menu giao diện mà còn ngăn chặn triệt để học viên gõ trực tiếp URL Admin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32("MaTaiKhoan");
            var vaiTro = session.GetString("VaiTro");

            // 1. Chưa đăng nhập
            if (!maTaiKhoan.HasValue || string.IsNullOrEmpty(vaiTro))
            {
                var returnUrl = context.HttpContext.Request.Path.Value;
                context.Result = new RedirectToActionResult("Login", "TaiKhoan", new { returnUrl });
                return;
            }

            // 2. Kiểm tra vai trò được phép
            if (_allowedRoles != null && _allowedRoles.Length > 0)
            {
                bool isAuthorized = _allowedRoles.Any(r => string.Equals(r, vaiTro, StringComparison.OrdinalIgnoreCase));
                if (!isAuthorized)
                {
                    // Từ chối truy cập, chuyển hướng về trang chủ hoặc thông báo lỗi
                    var controller = context.Controller as Controller;
                    if (controller != null)
                    {
                        controller.TempData["Error"] = "Bạn không có quyền truy cập vào chức năng hoặc trang này!";
                    }

                    if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Result = new RedirectToActionResult("Index", "Home", null);
                    }
                    else
                    {
                        context.Result = new RedirectToActionResult("Login", "TaiKhoan", null);
                    }
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
