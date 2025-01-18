using NguyenNhutDuy_2122110447.Controllers;
using PagedList;
using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoGiaoHangNhanh.Controllers
{
    public class AreaController : BaseController
    {
        // GET: Area
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortArea)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Khu vực";
                try
                {
                    var areas = data.KhuVucs.Include("NhanVien").AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        areas = areas.Where(o => o.Ten.Contains(searchTerm));
                    }
                    switch (sortArea)
                    {
                        case "asc":
                            areas = areas.OrderBy(o => o.KhoangCach);
                            break;
                        case "desc":
                            areas = areas.OrderByDescending(o => o.KhoangCach);
                            break;
                        default:
                            areas = areas.OrderByDescending(o => o.Id);
                            break;
                    }
                    int pageSize = 5;
                    int pageNumber = page ?? 1;
                    var pagedArea = areas.ToPagedList(pageNumber, pageSize);
                    return View(pagedArea);
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