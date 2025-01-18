using NguyenNhutDuy_2122110447.Controllers;
using QuanLyKhoGiaoHangNhanh.Models;
using System;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoGiaoHangNhanh.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortAccount)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1)
            {
                ViewBag.Title = "Tài Khoản";
                try
                {
                    var areas = data.TaiKhoans.Include("NhanVien").AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm)) 
                    {
                        areas = areas.Where(o => o.NhanVien.TenNhanVien.Contains(searchTerm));
                    }
                    switch (sortAccount)
                    {
                        default:
                            areas = areas.OrderByDescending(o => o.Id);
                            break;
                    }
                    int pageSize = 5;
                    int pageNumber = page ?? 1;
                    var pagedAccount = areas.ToPagedList(pageNumber, pageSize);
                    return View(pagedAccount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                    ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        public ActionResult Edit(int Id)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1)
            {
                ViewBag.Title = "Sửa tài khoản";
                try
                {
                    var account = data.TaiKhoans.FirstOrDefault(tk => tk.Id == Id);
                    if (account == null)
                    {
                        return HttpNotFound();
                    }
                    var nhanViens = data.NhanViens.ToList();
                    ViewBag.IdNhanVien = new SelectList(nhanViens, "Id", "TenNhanVien", account.IdNhanVien);
                    return View(account);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                    ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult Update(int Id, int IdNhanVien, string TenDangNhap)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1)
            {
                try
                {
                    var account = data.TaiKhoans.FirstOrDefault(tk => tk.Id == Id);
                    if (account == null)
                    {
                        return HttpNotFound();
                    }
                    account.IdNhanVien = IdNhanVien;
                    account.TenDangNhap = TenDangNhap;
                    data.SaveChanges();
                    TempData["Notification"] = "Sửa thành công!";
                    TempData["NotificationType"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                    ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }

    }
}