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
using DocumentFormat.OpenXml.Drawing.Charts;
using QuanLyKhoGiaoHangNhanh.Models;

namespace NguyenNhutDuy_2122110447.Controllers
{
    public class SupplyController : BaseController
    {
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        public ActionResult Index(int? page, string searchTerm, string sortSupply)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Vật tư";
                try
                {
                    var supplys = data.VatTus.AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        supplys = supplys.Where(o => o.TenVatTu.Contains(searchTerm));
                    }
                    switch (sortSupply)
                    {
                        case "asc":
                            supplys = supplys.OrderBy(o => o.GiaNhap);
                            break;
                        case "desc":
                            supplys = supplys.OrderByDescending(o => o.GiaNhap);
                            break;
                        default:
                            supplys = supplys.OrderBy(o => o.IdVatTu);
                            break;
                    }
                    int pageSize = 4;
                    int pageNumber = page ?? 1;
                    var pagedSupply = supplys.ToPagedList(pageNumber, pageSize);
                    return View(pagedSupply);
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
                ViewBag.Title = "Nhập vật tư";
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
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
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

                                // Đọc header
                                for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
                                {
                                    var columnName = worksheet.Cells[1, i].Text.Trim();
                                    dt.Columns.Add(columnName);
                                }

                                // Đọc dữ liệu
                                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                                {
                                    var row = dt.NewRow();
                                    for (int j = 1; j <= worksheet.Dimension.End.Column; j++)
                                    {
                                        row[j - 1] = worksheet.Cells[i, j].Text.Trim();
                                    }
                                    dt.Rows.Add(row);
                                }

                                // Chuyển dữ liệu sang một danh sách có kiểu mạnh (PreviewItem)
                                var previewData = dt.AsEnumerable().Select(row => new PreviewItemSupply
                                {
                                    IdVatTu = row["Id vật tư"] != DBNull.Value ? Convert.ToInt32(row["Id vật tư"]) : 0,
                                    TenVatTu = row["Tên vật tư"].ToString(),
                                    Loai = row["Loại"].ToString(),
                                    SoLuong = row["Số lượng"] != DBNull.Value ? Convert.ToInt32(row["Số lượng"]) : 0,
                                    DonViTinh = row["Đơn vị tính"].ToString(),
                                    ThongSoKyThuat = row["Thông số kỹ thuật"].ToString(),
                                    GiaNhap = row["Giá Nhập"] != DBNull.Value ? Convert.ToDecimal(row["Giá nhập"]) : 0,
                                    NgayNhap = row["Ngày nhập"] != DBNull.Value ? Convert.ToDateTime(row["Ngày nhập"]) : default(DateTime),
                                    TinhTrang = row["Tình trạng"].ToString(),
                                    GiaKhauHao = row["Giá khấu hao"] != DBNull.Value ? Convert.ToDecimal(row["Giá khấu hao"]) : 0
                                }).ToList();

                                // Lưu vào Session để sử dụng trong ConfirmImport
                                Session["PreviewData"] = previewData;
                            }
                            return View("Create"); // Chuyển đến view preview
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
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var previewData = Session["PreviewData"] as List<PreviewItemSupply>;
                    if (previewData != null)
                    {
                        using (var db = new QuanLyKhoGiaoHangEntities10())
                        {
                            foreach (var row in previewData)
                            {
                                // Kiểm tra trùng lặp Id
                                if (!db.VatTus.Any(d => d.IdVatTu == row.IdVatTu))
                                {
                                    var vattu = new VatTu
                                    {
                                        IdVatTu = row.IdVatTu,
                                        TenVatTu = row.TenVatTu,
                                        Loai = row.Loai,
                                        SoLuong = row.SoLuong,
                                        DonViTinh = row.DonViTinh,
                                        ThongSoKyThuat = row.ThongSoKyThuat,
                                        GiaNhap = row.GiaNhap,
                                        NgayNhap = row.NgayNhap,
                                        TinhTrang = row.TinhTrang,
                                        GiaKhauHao = row.GiaKhauHao
                                    };

                                    db.VatTus.Add(vattu);
                                }
                            }

                            db.SaveChanges();
                        }

                        Session.Remove("PreviewData");
                        ViewBag.Message = "Dữ liệu đã được thêm thành công!";
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
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var supply = data.VatTus.Find(Id);
                    if (supply == null)
                    {
                        return Content("Không tìm thấy vật tư.");
                    }
                    return PartialView("Detail", supply);
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
        public ActionResult Edit(int IdVatTu, string format)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Sửa vật tư";
                try
                {
                    var supply = data.VatTus.FirstOrDefault(d => d.IdVatTu == IdVatTu);
                    if (supply == null)
                    {
                        return HttpNotFound();
                    }

                    return View(supply);
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
        public ActionResult Update(int IdVatTu, VatTu updatedData)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var supply = data.VatTus.FirstOrDefault(d => d.IdVatTu == IdVatTu);
                    if (supply == null)
                    {
                        return HttpNotFound();
                    }
                    supply.TenVatTu = updatedData.TenVatTu;
                    supply.Loai = updatedData.Loai;
                    supply.SoLuong = updatedData.SoLuong;
                    supply.DonViTinh = updatedData.DonViTinh;
                    supply.ThongSoKyThuat = updatedData.ThongSoKyThuat;
                    supply.GiaNhap = updatedData.GiaNhap;
                    supply.NgayNhap = updatedData.NgayNhap;
                    supply.TinhTrang = updatedData.TinhTrang;
                    supply.GiaKhauHao = updatedData.GiaKhauHao;
                    data.SaveChanges();
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
        public ActionResult Delete(int IdVatTu)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var supply = data.VatTus.FirstOrDefault(d => d.IdVatTu == IdVatTu);
                    if (supply == null)
                    {
                        return HttpNotFound();
                    }
                    data.VatTus.Remove(supply);
                    data.SaveChanges();
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
        public ActionResult DistributeSupplie(int? page, string searchTerm, string sortSupply)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Cấp vật tư";
                try
                {
                    var personnels = data.NhanViens.ToList();
                    ViewBag.IdNhanVien = new SelectList(personnels, "Id", "TenNhanVien");
                    var supplys = data.VatTus.Where(n => n.SoLuong > 0 && n.TinhTrang == "Mới").AsQueryable();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        supplys = supplys.Where(o => o.TenVatTu.Contains(searchTerm));
                    }
                    switch (sortSupply)
                    {
                        case "asc":
                            supplys = supplys.OrderBy(o => o.GiaNhap);
                            break;
                        case "desc":
                            supplys = supplys.OrderByDescending(o => o.GiaNhap);
                            break;
                        default:
                            supplys = supplys.OrderBy(o => o.IdVatTu);
                            break;
                    }
                    int pageSize = 4;
                    int pageNumber = page ?? 1;
                    var pagedSupply = supplys.ToPagedList(pageNumber, pageSize);
                    return View(pagedSupply);
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
        public JsonResult Assign(int supplyId, int nhanVienId, int soLuong)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var supply = data.VatTus.FirstOrDefault(n => n.IdVatTu == supplyId);
                    if (supply.SoLuong == 0)
                    {
                        return Json(new { success = false, message = "Vật tư đã cấp phát hết." });
                    }
                    var psersonnel = data.NhanViens.FirstOrDefault(n => n.Id == nhanVienId);
                    if (supply == null || psersonnel == null)
                    {
                        return Json(new { success = false, message = "Vật tư hoặc nhân viên không tồn tại." });
                    }
                    var banGiaoVT = new BanGiaoVatTu
                    {
                        IdVatTu = supplyId,
                        IdNhanVien = nhanVienId,
                        SoLuong = soLuong,
                        NgayBanGiao = DateTime.Now
                    };
                    var lichSuBanGiaoVT = new LichSuBanGiaoVatTu
                    {
                        IdVatTu = supplyId,
                        IdNhanVien = nhanVienId,
                        TrangThai = "Cấp phát",
                        NgayTao = DateTime.Now
                    };
                    supply.SoLuong -= soLuong;
                    data.BanGiaoVatTus.Add(banGiaoVT);
                    data.LichSuBanGiaoVatTus.Add(lichSuBanGiaoVT);
                    data.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập." });
            }
        }
        public ActionResult RecallSupply(int? page, string searchTerm, string sortSupply, int? IdNhanVien)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                ViewBag.Title = "Thu hồi vật tư";
                try
                {
                    var supplys = data.BanGiaoVatTus.Include("VatTu").Include("NhanVien").AsQueryable();
                    var psersonnels = supplys.Select(b => b.NhanVien).Distinct().ToList();
                    ViewBag.IdNhanVien = new SelectList(psersonnels, "Id", "TenNhanVien");
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        supplys = supplys.Where(o => o.VatTu.TenVatTu.Contains(searchTerm));
                    }
                    if (IdNhanVien.HasValue)
                    {
                        supplys = supplys.Where(o => o.IdNhanVien == IdNhanVien.Value);
                    }
                    switch (sortSupply)
                    {
                        case "asc":
                            supplys = supplys.OrderBy(o => o.NgayBanGiao);
                            break;
                        case "desc":
                            supplys = supplys.OrderByDescending(o => o.NgayBanGiao);
                            break;
                        default:
                            supplys = supplys.OrderBy(o => o.IdVatTu);
                            break;
                    }
                    int pageSize = 6;
                    int pageNumber = page ?? 1;
                    var pagedSupply = supplys.ToPagedList(pageNumber, pageSize);
                    return View(pagedSupply);
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
        public JsonResult Recall(int IdVatTu)
        {
            if (Convert.ToInt32(Session["IdUser"]) == 1 || Convert.ToInt32(Session["IdUser"]) == 2)
            {
                try
                {
                    var supplyRecall = data.BanGiaoVatTus.FirstOrDefault(n => n.IdVatTu == IdVatTu);
                    var supply = data.VatTus.FirstOrDefault(n => n.IdVatTu == supplyRecall.IdVatTu);
                    var distributionSupplyHistory = new LichSuBanGiaoVatTu
                    {
                        IdVatTu = IdVatTu,
                        IdNhanVien = supplyRecall.IdNhanVien,
                        TrangThai = "Thu hồi",
                        NgayTao = DateTime.Now
                    };
                    supply.SoLuong += supplyRecall.SoLuong;
                    data.BanGiaoVatTus.Remove(supplyRecall);
                    data.LichSuBanGiaoVatTus.Add(distributionSupplyHistory);
                    data.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập." });
            }
        }
    }
}