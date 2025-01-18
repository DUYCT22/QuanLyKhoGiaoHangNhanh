using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;
using NguyenNhutDuy_2122110447.Controllers;

namespace QuanLyKhoGiaoHangNhanh.Controllers
{
    public class CarController : BaseController
    {
        // GET: Car
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortCar)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Xe";
                try
                {
                    var cars = data.Xes.AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cars = cars.Where(o => o.Ten.Contains(searchTerm));
                    }
                    switch (sortCar)
                    {
                        case "asc":
                            cars = cars.OrderBy(o => o.TheTich);
                            break;
                        case "desc":
                            cars = cars.OrderByDescending(o => o.TheTich);
                            break;
                        default:
                            cars = cars.OrderBy(o => o.Id);
                            break;
                    }
                    int pageSize = 6;
                    int pageNumber = page ?? 1;
                    var pagedCar = cars.ToPagedList(pageNumber, pageSize);
                    return View(pagedCar);
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
        public ActionResult Create()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Thêm xe";
                var personnels = data.NhanViens.Where(n => n.Khoi == "Giao hàng").ToList();
                ViewBag.IdNhanVienGiaoHang = new SelectList(personnels, "Id", "TenNhanVien");
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult Save(Xe xe)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    if (xe == null)
                    {
                        return HttpNotFound();
                    }
                    var car = data.Xes;
                    car.Add(xe);
                    data.SaveChanges();
                    TempData["Notification"] = "Thêm thành công!";
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
        public ActionResult Edit(int Id)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Sửa xe";
                try
                {
                    var car = data.Xes.FirstOrDefault(n => n.Id == Id);
                    if (car == null)
                    {
                        return HttpNotFound();
                    }
                    var nhanViens = data.NhanViens.Where(n => n.Khoi == "Giao hàng").ToList();
                    ViewBag.NhanVienGiaoHang = new SelectList(nhanViens, "Id", "TenNhanVien", car.IdNhanVienGiaoHang);
                    return View(car);
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
        public ActionResult Update(int Id, Xe updateData )
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    var car = data.Xes.FirstOrDefault(n => n.Id == Id);
                    if (car == null)
                    {
                        return HttpNotFound();
                    }
                    car.IdNhanVienGiaoHang = updateData.IdNhanVienGiaoHang;
                    car.Ten = updateData.Ten;
                    car.TaiTrong = updateData.TaiTrong;
                    car.TheTich = updateData.TheTich;
                    car.ThongSoKyThuat = updateData.ThongSoKyThuat;
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