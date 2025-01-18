using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenNhutDuy_2122110447.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        // Phương thức `OnActionExecuting` được ghi đè từ lớp cơ sở. Được gọi đầu tiên trước các action của Controller.
        {
            if (Session["UserName"] == null)
            // Nếu giá trị là `null`(người dùng chưa đăng nhập).
            {
                filterContext.Result = RedirectToAction("Login", "Login");
                // Người dùng sẽ được chuyển hướng đến action "Login" trong controller "Login".
            }
            base.OnActionExecuting(filterContext);
            // Gọi phương thức `OnActionExecuting` của lớp cơ sở để đảm bảo các thao tác mặc định vẫn được thực hiện.
        }
    }
}