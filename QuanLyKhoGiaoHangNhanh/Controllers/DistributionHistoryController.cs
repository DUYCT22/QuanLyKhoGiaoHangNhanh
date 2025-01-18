using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using NguyenNhutDuy_2122110447.Controllers;
using ClosedXML.Excel;
using System.IO;

namespace QuanLyKhoGiaoHangNhanh.Controllers
{
    public class DistributionHistoryController : BaseController
    {
        // GET: DistributionHistory
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortDistributionHistory)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Lịch sử phân hàng";
                try
                {
                    var distributionHistorys = data.LichSuPhanPhois.Include("DonHang").Include("NhanVien").AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        distributionHistorys = distributionHistorys.Where(o => o.NhanVien.TenNhanVien.Contains(searchTerm));
                    }
                    switch (sortDistributionHistory)
                    {
                        case "asc":
                            distributionHistorys = distributionHistorys.OrderBy(o => o.SoTienChuoc);
                            break;
                        case "desc":
                            distributionHistorys = distributionHistorys.OrderByDescending(o => o.SoTienChuoc);
                            break;
                        default:
                            distributionHistorys = distributionHistorys.OrderByDescending(o => o.Id);
                            break;
                    }
                    int pageSize = 8;
                    int pageNumber = page ?? 1;
                    var pagedDistributionHistory = distributionHistorys.ToPagedList(pageNumber, pageSize);
                    return View(pagedDistributionHistory);
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
                    var distributionHistorys = data.LichSuPhanPhois.ToList();
                    if (distributionHistorys.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    data.LichSuPhanPhois.RemoveRange(distributionHistorys);
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
                    var distributionHistorys = data.LichSuPhanPhois.Include("NhanVien").OrderBy(x => x.Id).ToList();
                    if (distributionHistorys.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("LichSuPhanPhoi");
                        worksheet.Cell(1, 1).Value = "Id";
                        worksheet.Cell(1, 2).Value = "Id đơn hàng";
                        worksheet.Cell(1, 3).Value = "Nhân viên giao hàng";
                        worksheet.Cell(1, 4).Value = "Ngày phân phối";
                        worksheet.Cell(1, 5).Value = "Số tiền chuộc";
                        int row = 2;
                        foreach (var distributionHistory in distributionHistorys)
                        {
                            worksheet.Cell(row, 1).Value = distributionHistory.Id;
                            worksheet.Cell(row, 2).Value = distributionHistory.IdDonHang;
                            worksheet.Cell(row, 3).Value = distributionHistory.NhanVien.TenNhanVien;
                            worksheet.Cell(row, 4).Value = distributionHistory.NgayPhanPhoi;
                            worksheet.Cell(row, 5).Value = distributionHistory.SoTienChuoc;
                            row++;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var fileName = "LichSuPhanPhoi.xlsx";
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