using ClosedXML.Excel;
using NguyenNhutDuy_2122110447.Controllers;
using PagedList;
using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoGiaoHangNhanh.Controllers
{
    public class DeliveryController : BaseController
    {
        // GET: Delivery
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortDelivery)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Giao hàng";
                try
                {
                    var deliverys = data.GiaoHangs.Include("DonHang").Include("Xe").Where(g => g.TrangThai == "Đang giao").AsQueryable();
                    var areas = data.KhuVucs.Select(kv => new
                    {
                        ToaDo = kv.ToaDo,
                        Ten = kv.Ten
                    }).ToList();
                    ViewBag.Area = JsonConvert.SerializeObject(areas);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        deliverys = deliverys.Where(o => o.DonHang.TenNguoiNhan.Contains(searchTerm));
                    }
                    switch (sortDelivery)
                    {
                        case "asc":
                            deliverys = deliverys.OrderBy(o => o.DonHang.TongTien);
                            break;
                        case "desc":
                            deliverys = deliverys.OrderByDescending(o => o.DonHang.TongTien);
                            break;
                        default:
                            deliverys = deliverys.OrderByDescending(o => o.Id);
                            break;
                    }
                    int pageSize = 8;
                    int pageNumber = page ?? 1;
                    var pagedDelivery = deliverys.ToPagedList(pageNumber, pageSize);
                    return View(pagedDelivery);
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
        public ActionResult Delete()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    var orderrs = data.DonHangs.Where(n => n.TrangThai == "Đang giao").ToList();
                    foreach (var o in orderrs) 
                    {
                        o.TrangThai = "Lưu kho";
                    }
                    var deliverys = data.GiaoHangs.ToList();
                    var deliveryHistorys = data.LichSuGiaoHangs.ToList();
                    var xe = data.Xes.Where(n => n.GiaoHang == 1);
                    if (deliverys.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    foreach (var x in xe) 
                    {
                        x.GiaoHang = 0;
                    }
                    data.LichSuGiaoHangs.RemoveRange(deliveryHistorys);
                    data.GiaoHangs.RemoveRange(deliverys);
                    data.SaveChanges();
                    TempData["Notification"] = "Xóa thành công!";
                    TempData["NotificationType"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                    TempData["Notification"] = "Xóa không thành công!";
                    TempData["NotificationType"] = "error";
                    return View("Index");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        public ActionResult ExportToExcel()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    var deliverys = data.GiaoHangs.Include("Xe").OrderBy(x => x.Id).ToList();
                    if (deliverys.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("GiaoHang");
                        worksheet.Cell(1, 1).Value = "Id";
                        worksheet.Cell(1, 2).Value = "Id đơn hàng";
                        worksheet.Cell(1, 3).Value = "Id xe giao hàng";
                        worksheet.Cell(1, 4).Value = "Nhân viên giao hàng";
                        worksheet.Cell(1, 5).Value = "Trạng thái";
                        int row = 2;
                        foreach (var delivery in deliverys)
                        {
                            worksheet.Cell(row, 1).Value = delivery.Id;
                            worksheet.Cell(row, 2).Value = delivery.IdDonHang;
                            worksheet.Cell(row, 3).Value = delivery.IdXe;
                            worksheet.Cell(row, 4).Value = delivery.Xe.NhanVien.TenNhanVien;
                            worksheet.Cell(row, 5).Value = delivery.TrangThai;
                            row++;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var fileName = "GiaoHang.xlsx";
                            stream.Position = 0;
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
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