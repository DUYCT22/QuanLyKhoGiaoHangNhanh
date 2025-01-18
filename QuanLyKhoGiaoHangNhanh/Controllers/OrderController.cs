using NguyenNhutDuy_2122110447.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using PagedList;
using ClosedXML.Excel;
using QuanLyKhoGiaoHangNhanh.Models;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Bibliography;
namespace NguyenNhutDuy_2122110447.Controllers
{
    public class OrderController : BaseController
    {
        private static System.Data.DataTable _previewTable;
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        // GET: DonHang
        public ActionResult Index(int? page, string searchTerm, string sortOrder)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                ViewBag.Title = "Đơn hàng";
                try
                {
                    var orders = data.DonHangs.AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        orders = orders.Where(o => o.TenNguoiNhan.Contains(searchTerm));
                    }
                    switch (sortOrder)
                    {
                        case "asc":
                            orders = orders.OrderBy(o => o.GiaTriDonHang);
                            break;
                        case "desc":
                            orders = orders.OrderByDescending(o => o.GiaTriDonHang);
                            break;
                        default:
                            orders = orders.OrderByDescending(o => o.Id);
                            break;
                    }
                    int pageSize = 6;
                    int pageNumber = page ?? 1;
                    var pagedOrders = orders.ToPagedList(pageNumber, pageSize);
                    return View(pagedOrders);
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
                ViewBag.Title = "Nhập hàng";
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        if (fileExtension == ".xlsx" || fileExtension == ".xls")
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                            using (var package = new ExcelPackage(file.InputStream))
                            {
                                var worksheet = package.Workbook.Worksheets[0];
                                var dt = new System.Data.DataTable();
                                for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
                                {
                                    var columnName = worksheet.Cells[1, i].Text.Trim();
                                    dt.Columns.Add(columnName);
                                }
                                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                                {
                                    var row = dt.NewRow();
                                    for (int j = 1; j <= worksheet.Dimension.End.Column; j++)
                                    {
                                        row[j - 1] = worksheet.Cells[i, j].Text.Trim();
                                    }
                                    dt.Rows.Add(row);
                                }
                                var previewData = dt.AsEnumerable().Select(row => new PreviewItem
                                {
                                    Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                                    DiaChi = row["Địa chỉ"].ToString(),
                                    Ten = row["Tên"].ToString(),
                                    Loai = row["Loại"].ToString(),
                                    CanNang = row["Cân nặng"] != DBNull.Value ? Convert.ToDouble(row["Cân nặng"]) : 0,
                                    TheTich = row["Thể tích"] != DBNull.Value ? Convert.ToDouble(row["Thể tích"]) : 0,
                                    TenNguoiNhan = row["Tên người nhận"].ToString(),
                                    Sdt = row["Sdt"].ToString(),
                                    GiaTriDonHang = row["Giá trị đơn hàng"] != DBNull.Value ? Convert.ToDecimal(row["Giá trị đơn hàng"]) : 0,
                                    PhiVanChuyen = row["Phí vận chuyển"] != DBNull.Value ? Convert.ToDecimal(row["Phí vận chuyển"]) : 0,
                                    TongTien = row["Tổng tiền"] != DBNull.Value ? Convert.ToDecimal(row["Tổng tiền"]) : 0,
                                    GhiChu = row["Ghi chú"].ToString()
                                }).ToList();
                                Session["PreviewData"] = previewData;
                            }
                            return View("Create");
                        }
                    }
                    ModelState.AddModelError("", "Vui lòng chọn file Excel hợp lệ.");
                    return View("Create");
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
        public ActionResult ConfirmImport()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    var previewData = Session["PreviewData"] as List<PreviewItem>;
                    if (previewData != null)
                    {
                        using (var db = new QuanLyKhoGiaoHangEntities10())
                        {
                            foreach (var row in previewData)
                            {
                                // Kiểm tra trùng lặp Id
                                if (!db.DonHangs.Any(d => d.Id == row.Id))
                                {
                                    var donHang = new DonHang
                                    {
                                        Id = row.Id,
                                        DiaChi = row.DiaChi,
                                        Ten = row.Ten,
                                        Loai = row.Loai,
                                        CanNang = row.CanNang,
                                        TheTich = row.TheTich,
                                        TenNguoiNhan = row.TenNguoiNhan,
                                        Sdt = row.Sdt,
                                        GiaTriDonHang = row.GiaTriDonHang,
                                        PhiVanChuyen = row.PhiVanChuyen,
                                        TongTien = row.TongTien,
                                        GhiChu = row.GhiChu,
                                        TrangThai = "Lưu kho"
                                    };

                                    db.DonHangs.Add(donHang);
                                }
                            }

                            db.SaveChanges();
                        }

                        Session.Remove("PreviewData");
                        TempData["Notification"] = "Thêm dữ liệu thành công!";
                        TempData["NotificationType"] = "success";
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError("", "Không có dữ liệu để thêm.");
                    return View("Index");
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
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    var order = data.DonHangs.Find(Id);
                    if (order == null)
                    {
                        return Content("Không tìm thấy đơn hàng.");
                    }
                    return PartialView("Detail", order);
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
                ViewBag.Title = "Sửa đơn hàng";
                try
                {
                    var order = data.DonHangs.FirstOrDefault(d => d.Id == Id);
                    if (order == null)
                    {
                        return HttpNotFound();
                    }
                    return View(order);
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
        public ActionResult Update(int Id, DonHang updatedData)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                using (var transaction = data.Database.BeginTransaction())
                {
                    try
                    {
                        var order = data.DonHangs.FirstOrDefault(d => d.Id == Id);
                        if (order == null)
                        {
                            return HttpNotFound();
                        }
                        var delivery = data.GiaoHangs.FirstOrDefault(d => d.IdDonHang == Id);
                        if (delivery != null)
                        {
                            var lichsu = new LichSuGiaoHang
                            {
                                IdGiaoHang = delivery.Id,
                                TrangThai = updatedData.TrangThai,
                                NgayGiao = DateTime.Now,
                            };
                            delivery.TrangThai = updatedData.TrangThai;
                            data.LichSuGiaoHangs.Add(lichsu);

                        }
                        order.Ten = updatedData.Ten;
                        order.DiaChi = updatedData.DiaChi;
                        order.Loai = updatedData.Loai;
                        order.CanNang = updatedData.CanNang;
                        order.TheTich = updatedData.TheTich;
                        order.TenNguoiNhan = updatedData.TenNguoiNhan;
                        order.Sdt = updatedData.Sdt;
                        order.GiaTriDonHang = updatedData.GiaTriDonHang;
                        order.PhiVanChuyen = updatedData.PhiVanChuyen;
                        order.TongTien = updatedData.TongTien;
                        order.GhiChu = updatedData.GhiChu;
                        order.TrangThai = updatedData.TrangThai;
                        data.SaveChanges();
                        transaction.Commit();
                        var checkDelivery = data.GiaoHangs.Any(n => n.IdXe == delivery.IdXe && n.TrangThai == "Đang giao");
                        if (!checkDelivery)
                        {
                            var car = data.Xes.FirstOrDefault(n => n.Id == delivery.IdXe);
                            if (car != null)
                            {
                                car.GiaoHang = 0;
                                data.SaveChanges();
                            }
                        }
                        TempData["Notification"] = "Thay đổi thành công!";
                        TempData["NotificationType"] = "success";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                        ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                        return View("Error");
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập.";
                return View("Error");
            }
        }
        public ActionResult Distribution()
        {
            try
            {
                var carDeliverys = data.Xes.Where(n => n.GiaoHang == 0).ToList();
                var orderNotYetDelivered = data.DonHangs
                    .Where(dh => dh.TrangThai == "Lưu kho" && !data.GiaoHangs.Any(gh => gh.IdDonHang == dh.Id))
                    .ToList();
                if (!carDeliverys.Any())
                {
                    TempData["Notification"] = "Không có xe trống để phân chia!";
                    TempData["NotificationType"] = "warning";
                    return RedirectToAction("Index");
                }
                if (!orderNotYetDelivered.Any()) {
                    TempData["Notification"] = "Không có đơn hàng để phân chia!";
                    TempData["NotificationType"] = "warning";
                    return RedirectToAction("Index");
                }
                bool allOrdersAssigned = true;
                string warningMessage = string.Empty;
                foreach (var car in carDeliverys)
                {
                    double totalWeight = 0;
                    double totalVolume = 0;
                    if (totalWeight > car.TaiTrong || totalVolume > car.TheTich) continue;
                    bool isAssigned = false;
                    foreach (var order in orderNotYetDelivered)
                    {
                        var area = data.KhuVucs.FirstOrDefault(kv => kv.IdNhanVienGiaoHang == car.IdNhanVienGiaoHang);
                        if (area == null || !IsWithinArea(order.DiaChi, area.ToaDo)) continue;
                        totalWeight += Convert.ToDouble(order.CanNang);
                        totalVolume += Convert.ToDouble(order.TheTich);
                        if (totalWeight > car.TaiTrong || totalVolume > car.TheTich) continue;
                        var delivery = new GiaoHang
                        {
                            IdDonHang = order.Id,
                            IdXe = car.Id,  
                            TrangThai = "Đang giao",
                        };
                        var history = new LichSuPhanPhoi
                        {
                            IdDonHang = order.Id,
                            IdNhanVienGiaoHang = car.IdNhanVienGiaoHang,
                            NgayPhanPhoi = DateTime.Now,
                            SoTienChuoc = order.GiaTriDonHang,
                        };
                        order.TrangThai = "Đang giao";
                        data.GiaoHangs.Add(delivery);
                        data.LichSuPhanPhois.Add(history);
                        isAssigned = true;
                    }
                    if (!isAssigned)
                    {
                        allOrdersAssigned = false;
                        warningMessage += $"Xe {car.Ten} không thể phân phối đơn hàng (có thể do xe đầy hoặc không thuộc khu vực giao hàng). ";
                    }
                    car.GiaoHang = 1;
                }
                data.SaveChanges();
                TempData["Notification"] = allOrdersAssigned ? "Đã phân phối các đơn hàng thành công!" : warningMessage;
                TempData["NotificationType"] = allOrdersAssigned ? "success" : "warning";
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View("Error");
            }
        }
        private bool IsWithinArea(string diaChi, string khuVuc)
        {
            var point = diaChi.Split(',').Select(double.Parse).ToArray();
            var polygon = JsonConvert.DeserializeObject<List<List<double>>>(khuVuc);
            return IsPointInPolygon(point, polygon);
        }

        private bool IsPointInPolygon(double[] point, List<List<double>> polygon)
        {
            bool isInside = false;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                if ((polygon[i][1] > point[1]) != (polygon[j][1] > point[1]) &&
                    point[0] < (polygon[j][0] - polygon[i][0]) * (point[1] - polygon[i][1]) / (polygon[j][1] - polygon[i][1]) + polygon[i][0])
                {
                    isInside = !isInside;
                }
                j = i;
            }

            return isInside;
        }

        public ActionResult ExportToExcel()
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 3)
            {
                try
                {
                    var orders = data.DonHangs.OrderBy(x => x.Id).ToList();
                    if (orders.Count == 0)
                    {
                        TempData["Notification"] = "Không có dữ liệu!";
                        TempData["NotificationType"] = "warning";
                        return RedirectToAction("Index");
                    }
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("DonHang");
                        worksheet.Cell(1, 1).Value = "Id";
                        worksheet.Cell(1, 2).Value = "Địa chỉ";
                        worksheet.Cell(1, 3).Value = "Tên";
                        worksheet.Cell(1, 4).Value = "Loại";
                        worksheet.Cell(1, 5).Value = "Cân nặng";
                        worksheet.Cell(1, 6).Value = "Thể tích";
                        worksheet.Cell(1, 7).Value = "Tên người nhận";
                        worksheet.Cell(1, 8).Value = "Sdt";
                        worksheet.Cell(1, 9).Value = "Giá trị đơn hàng";
                        worksheet.Cell(1, 10).Value = "Phí vận chuyển";
                        worksheet.Cell(1, 11).Value = "Tổng tiền";
                        worksheet.Cell(1, 12).Value = "Ghi chú";
                        int row = 2;
                        foreach (var order in orders)
                        {
                            worksheet.Cell(row, 1).Value = order.Id;
                            worksheet.Cell(row, 2).Value = order.DiaChi;
                            worksheet.Cell(row, 3).Value = order.Ten;
                            worksheet.Cell(row, 4).Value = order.Loai;
                            worksheet.Cell(row, 5).Value = order.CanNang;
                            worksheet.Cell(row, 6).Value = order.TheTich;
                            worksheet.Cell(row, 7).Value = order.TenNguoiNhan;
                            worksheet.Cell(row, 8).Value = order.Sdt;
                            worksheet.Cell(row, 9).Value = order.GiaTriDonHang;
                            worksheet.Cell(row, 10).Value = order.PhiVanChuyen;
                            worksheet.Cell(row, 11).Value = order.TongTien;
                            worksheet.Cell(row, 12).Value = order.GhiChu;
                            row++;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var fileName = "DonHang.xlsx";
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