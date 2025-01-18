using NguyenNhutDuy_2122110447.Controllers;
using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoGiaoHangNhanh.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index()
        {
            ViewBag.Title = "Cá nhân";
            try
            {
                int id = (int)Session["IdUser"];
                var user = data.NhanViens.FirstOrDefault(a => a.Id == id);
                var account = data.TaiKhoans.FirstOrDefault(a => a.IdNhanVien == user.Id);
                ViewBag.Account = account.TenDangNhap.ToString();
                return View(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View("Error");
            }
        }
        [HttpPost]
        public JsonResult ResetAccount(string password, string userName)
        {
            try
            {
                int id = (int)Session["IdUser"];
                var user = data.NhanViens.FirstOrDefault(a => a.Id == id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Nhân viên không tồn tại!" });
                }

                var account = data.TaiKhoans.FirstOrDefault(u => u.IdNhanVien == user.Id);
                if (account == null)
                {
                    return Json(new { success = false, message = "Tài khoản không tồn tại!" });
                }

                // Cập nhật userName nếu thay đổi
                if (!string.IsNullOrEmpty(userName) && account.TenDangNhap != userName)
                {
                    account.TenDangNhap = userName;
                }

                // Cập nhật password nếu không null
                if (!string.IsNullOrEmpty(password))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    account.MatKhau = hashedPassword;
                }

                // Lưu thay đổi vào DB
                data.SaveChanges();

                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }


    }
}