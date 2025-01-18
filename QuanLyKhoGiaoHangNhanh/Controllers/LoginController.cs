using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using NguyenNhutDuy_2122110447.Models;
using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static NguyenNhutDuy_2122110447.Controllers.LoginController;

namespace NguyenNhutDuy_2122110447.Controllers
{
    public class LoginController : Controller
    {
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(string name, string password)
        {
            try
            {
                var user = data.TaiKhoans.FirstOrDefault(a => a.TenDangNhap == name);

                if (user != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.MatKhau);
                    if (isPasswordValid)
                    {
                        user.TrangThai = "Hoạt động";
                        data.SaveChanges();
                        Session.Timeout = 120;
                        var id = data.NhanViens.FirstOrDefault(n => n.Id == user.IdNhanVien);
                        Session["IdUser"] = user.Id;
                        Session["UserName"] = id.TenNhanVien;
                        int activeUsersCount = data.TaiKhoans.Count(a => a.TrangThai == "hoạt động");
                        Session["Online"] = activeUsersCount;
                        Session["UserGmail"] = id.Gmail;
                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View("Error");
            }
        }
        public ActionResult Logout()
        {
            try
            {
                if (Session["IdUser"] != null)
                {
                    int id = (int)Session["IdUser"];
                    var user = data.TaiKhoans.FirstOrDefault(a => a.Id == id);
                    user.TrangThai = "Không hoạt động";
                    data.SaveChanges();
                    Session.Clear();
                    return RedirectToAction("Login", "Login");
                }
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View("Error");
            }
        }
        public ActionResult ForgotPassword()
        {
            try
            {
                if (Session["LoggedInUser"] == null)
                {
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View("Error");
            }
        }
        [HttpPost]
        public JsonResult SendVerificationCode(string gmail)
        {
            try
            {
                var user = data.NhanViens.FirstOrDefault(u => u.Gmail == gmail);
                if (user == null)
                {
                    return Json(new { success = false, message = "Gmail không tồn tại trong hệ thống!" });
                }
                var account = data.TaiKhoans.FirstOrDefault(u => u.IdNhanVien == user.Id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Tài khoản không tồn tại!" });
                }
                Random random = new Random();
                int verificationCode = random.Next(100000, 999999);
                Session["VerificationCode"] = verificationCode;
                Session["UserGmail"] = gmail;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("nguyennhutduy.cv@gmail.com");
                mail.To.Add(gmail);
                mail.Subject = "Mã xác minh đặt lại mật khẩu";
                mail.Body = $"Mã xác minh của bạn là: {verificationCode}";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new System.Net.NetworkCredential("nguyennhutduy.cv@gmail.com", "btbhrkpyqdbbkhqx");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return Json(new { success = true, message = "Mã xác minh đã được gửi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
                throw;
            }
        }
        [HttpPost]
        public JsonResult VerifyCode(int code)
        {
            try
            {
                int? storedCode = Session["VerificationCode"] as int?;
                if (storedCode == code)
                {
                    return Json(new { success = true, message = "Mã xác minh hợp lệ!" });
                }
                return Json(new { success = false, message = "Mã xác minh không đúng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
                throw;
            }
        }
        [HttpPost]
        public JsonResult ResetPassword(string newPassword)
        {
            try
            {
                string gmail = Session["UserGmail"] as string;

                if (string.IsNullOrEmpty(gmail))
                {
                    return Json(new { success = false, message = "Phiên làm việc đã hết hạn, vui lòng thử lại!" });
                }
                var user = data.NhanViens.FirstOrDefault(u => u.Gmail == gmail);
                if (user == null)
                {
                    return Json(new { success = false, message = "Nhân viên không tồn tại!" });
                }
                var account = data.TaiKhoans.FirstOrDefault(u => u.IdNhanVien == user.Id);
                if (account == null)
                {
                    return Json(new { success = false, message = "Tài khoản không tồn tại!" });
                }
                else
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    account.MatKhau = hashedPassword;
                    data.SaveChanges();
                    return Json(new { success = true, message = "Đặt lại mật khẩu thành công!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}