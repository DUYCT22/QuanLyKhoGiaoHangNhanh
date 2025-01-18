using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ClosedXML.Excel;
using System.IO;
using NguyenNhutDuy_2122110447.Controllers;
namespace QuanLyKhoGiaoHangNhanh.Controllers
{
    public class DeliveryHistoryController : BaseController
    {
        // GET: DeliveryHistory
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortDeliveryHistory)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Lịch sử giao hàng";
                try
                {
                    var deliveryHistorys = data.LichSuGiaoHangs.Include("GiaoHang").AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        deliveryHistorys = deliveryHistorys.Where(o => o.TrangThai.Contains(searchTerm));
                    }
                    switch (sortDeliveryHistory)
                    {
                        case "asc":
                            deliveryHistorys = deliveryHistorys.OrderBy(o => o.NgayGiao);
                            break;
                        case "desc":
                            deliveryHistorys = deliveryHistorys.OrderByDescending(o => o.NgayGiao);
                            break;
                        default:
                            deliveryHistorys = deliveryHistorys.OrderByDescending(o => o.Id);
                            break;
                    }
                    int pageSize = 8;
                    int pageNumber = page ?? 1;
                    var pagedDeliveryHistrory = deliveryHistorys.ToPagedList(pageNumber, pageSize);
                    return View(pagedDeliveryHistrory);
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
                    var deliveryHistorys = data.LichSuGiaoHangs.ToList();
                    if (deliveryHistorys.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    data.LichSuGiaoHangs.RemoveRange(deliveryHistorys);
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
                    var deliveryHistorys = data.LichSuGiaoHangs.Include("GiaoHang").Include("Xe").Include("NhanVien").OrderBy(x => x.Id).ToList();
                    if (deliveryHistorys.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("LichSuGiaoHang");
                        worksheet.Cell(1, 1).Value = "Id";
                        worksheet.Cell(1, 2).Value = "Id giao hàng";
                        worksheet.Cell(1, 3).Value = "Id đơn hàng";
                        worksheet.Cell(1, 4).Value = "Id xe giao hàng";
                        worksheet.Cell(1, 5).Value = "Nhân viên giao hàng";
                        worksheet.Cell(1, 6).Value = "Trạng thái";
                        int row = 2;
                        foreach (var deliveryHistory in deliveryHistorys)
                        {
                            worksheet.Cell(row, 1).Value = deliveryHistory.Id;
                            worksheet.Cell(row, 2).Value = deliveryHistory.IdGiaoHang;
                            worksheet.Cell(row, 3).Value = deliveryHistory.GiaoHang.IdDonHang;
                            worksheet.Cell(row, 4).Value = deliveryHistory.GiaoHang.IdXe;
                            worksheet.Cell(row, 5).Value = deliveryHistory.GiaoHang.Xe.NhanVien.TenNhanVien;
                            worksheet.Cell(row, 6).Value = deliveryHistory.TrangThai;
                            row++;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var fileName = "LichSuGiaoHang.xlsx";
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