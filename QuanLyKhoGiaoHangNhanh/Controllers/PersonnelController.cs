
using ClosedXML.Excel;
using NguyenNhutDuy_2122110447.Models;
using PagedList;
using QuanLyKhoGiaoHangNhanh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenNhutDuy_2122110447.Controllers
{
    public class PersonnelController : BaseController
    {
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortPersonnel)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Nhân sự";
                try
                {
                    var personnels = data.NhanViens.AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        personnels = personnels.Where(o => o.TenNhanVien.Contains(searchTerm));
                    }
                    switch (sortPersonnel)
                    {
                        case "asc":
                            personnels = personnels.OrderBy(o => o.Luong);
                            break;
                        case "desc":
                            personnels = personnels.OrderByDescending(o => o.Luong);
                            break;
                        default:
                            personnels = personnels.OrderBy(o => o.Id);
                            break;
                    }
                    int pageSize = 5;
                    int pageNumber = page ?? 1;
                    var pagedPersonnel = personnels.ToPagedList(pageNumber, pageSize);
                    return View(pagedPersonnel);
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
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Thêm nhân sự";
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult Save(NhanVien Personnel)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    if (Personnel == null)
                    {
                        return HttpNotFound();
                    }
                    var personnel = data.NhanViens;
                    personnel.Add(Personnel);
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
        public ActionResult Detail(int Id)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var personnel = data.NhanViens.Find(Id);
                    if (personnel == null)
                    {
                        return Content("Không tìm thấy nhân viên.");
                    }
                    return PartialView("Detail", personnel);
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
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Sửa nhân sự";
                try
                {
                    var personnel = data.NhanViens.FirstOrDefault(d => d.Id == Id);
                    if (personnel == null)
                    {
                        return HttpNotFound();
                    }
                    return View(personnel);
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
        public ActionResult Update(int Id, NhanVien updatedData)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
                {
                    try
                    {
                        var personnel = data.NhanViens.FirstOrDefault(d => d.Id == Id);
                        if (personnel == null)
                        {
                            return HttpNotFound();
                        }
                        personnel.TenNhanVien = updatedData.TenNhanVien;
                        personnel.ThuongTru = updatedData.ThuongTru;
                        personnel.Sdt = updatedData.Sdt;
                        personnel.Gmail = updatedData.Gmail;
                        personnel.Luong = updatedData.Luong;
                        personnel.Sdt = updatedData.Sdt;
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
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        public ActionResult Delete(int Id)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var personnel = data.NhanViens.FirstOrDefault(d => d.Id == Id);
                    if (personnel == null)
                    {
                        return HttpNotFound();
                    }
                    data.NhanViens.Remove(personnel);
                    data.SaveChanges();
                    TempData["Notification"] = "Xóa thành công!";
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
        public ActionResult Violate(int? page, string searchTerm, string sortViolate)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Lỗi nhân sự";
                try
                {
                    var violates = data.LoiNhanViens.Include("NhanVien").AsQueryable();

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        violates = violates.Where(o => o.NhanVien.TenNhanVien.Contains(searchTerm));
                    }

                    switch (sortViolate)
                    {
                        case "asc":
                            violates = violates.OrderBy(o => o.ChiPhiPhatSinh);
                            break;
                        case "desc":
                            violates = violates.OrderByDescending(o => o.ChiPhiPhatSinh);
                            break;
                        default:
                            violates = violates.OrderBy(o => o.IdNhanVien);
                            break;
                    }

                    int pageSize = 5;
                    int pageNumber = page ?? 1;
                    var pagedViolates = violates.ToPagedList(pageNumber, pageSize);

                    return View(pagedViolates);
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
        public ActionResult CreateViolate()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Ghi lỗi nhân sự";
                try
                {
                    var personnels = data.NhanViens.ToList(); 
                    ViewBag.IdNhanVien = new SelectList(personnels, "Id", "TenNhanVien");
                    return View();
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
        public ActionResult SaveViolate(LoiNhanVien personnel)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    if (personnel == null)
                    {
                        return HttpNotFound();
                    }
                    data.LoiNhanViens.Add(personnel);
                    data.SaveChanges();
                    TempData["Notification"] = "Thêm thành công!";
                    TempData["NotificationType"] = "success";
                    return RedirectToAction("Violate");
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
        public ActionResult DeleteViolate(int Id)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var personnel = data.LoiNhanViens.FirstOrDefault(d => d.Id == Id);
                    if (personnel == null)
                    {
                        return HttpNotFound();
                    }
                    data.LoiNhanViens.Remove(personnel);
                    data.SaveChanges();
                    TempData["Notification"] = "Xóa thành công!";
                    TempData["NotificationType"] = "success";
                    return RedirectToAction("Violate");
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
        public ActionResult ExportToExcelPersonnel()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var personnels = data.NhanViens.OrderBy(x => x.Id).ToList();
                    if (personnels.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("NhanVien");
                        worksheet.Cell(1, 1).Value = "Id";
                        worksheet.Cell(1, 2).Value = "Tên nhân viên";
                        worksheet.Cell(1, 3).Value = "Thường trú";
                        worksheet.Cell(1, 4).Value = "Sdt";
                        worksheet.Cell(1, 5).Value = "Gmail";
                        worksheet.Cell(1, 6).Value = "Khối";
                        worksheet.Cell(1, 7).Value = "Lương";
                        worksheet.Cell(1, 8).Value = "Ngày bắt đầu làm";
                        int row = 2;
                        foreach (var personnel in personnels)
                        {
                            worksheet.Cell(row, 1).Value = personnel.Id;
                            worksheet.Cell(row, 2).Value = personnel.TenNhanVien;
                            worksheet.Cell(row, 3).Value = personnel.ThuongTru;
                            worksheet.Cell(row, 4).Value = personnel.Sdt;
                            worksheet.Cell(row, 5).Value = personnel.Gmail;
                            worksheet.Cell(row, 6).Value = personnel.Khoi;
                            worksheet.Cell(row, 7).Value = personnel.Luong;
                            worksheet.Cell(row, 8).Value = personnel.NgayBatDauLam;
                            row++;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var fileName = "NhanVien.xlsx";
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
        public ActionResult ExportToExcelViolate()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var violates = data.LoiNhanViens.Include("NhanVien").OrderBy(x => x.Id).ToList();
                    if (violates.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("LoiNhanVien");
                        worksheet.Cell(1, 1).Value = "Id";
                        worksheet.Cell(1, 2).Value = "Tên nhân viên";
                        worksheet.Cell(1, 3).Value = "Mô tả lỗi";
                        worksheet.Cell(1, 4).Value = "Ngày phát sinh";
                        worksheet.Cell(1, 5).Value = "Chi phí phát sinh";
                        int row = 2;
                        foreach (var violate in violates)
                        {
                            worksheet.Cell(row, 1).Value = violate.Id;
                            worksheet.Cell(row, 2).Value = violate.NhanVien.TenNhanVien;
                            worksheet.Cell(row, 3).Value = violate.MoTaLoi;
                            worksheet.Cell(row, 4).Value = violate.NgayPhatSinh;
                            worksheet.Cell(row, 5).Value = violate.ChiPhiPhatSinh;
                            row++;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var fileName = "LoiNhanVien.xlsx";
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