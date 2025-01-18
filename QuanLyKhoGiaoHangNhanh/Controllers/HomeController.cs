using DocumentFormat.OpenXml.Drawing;
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
namespace NguyenNhutDuy_2122110447.Controllers
{
    public class HomeController : BaseController
    {
        QuanLyKhoGiaoHangEntities10 data = new QuanLyKhoGiaoHangEntities10();
        HomeView hv = new HomeView();
        public ActionResult Index()
        {
            ViewBag.Title = "Trang chủ";
            try
            {
                if (Convert.ToInt32(Session["IdUser"]) == 1)
                {
                    hv.donHangLuuKhos = data.DonHangs.Where(n => n.TrangThai == "Lưu kho").ToList();
                    hv.khuVucs = data.KhuVucs
                       .AsNoTracking()
                       .ToList()
                       .Select(kv => new KhuVuc
                       {
                           ToaDo = kv.ToaDo,
                           Ten = kv.Ten
                       }).ToList();
                    JsonConvert.SerializeObject(hv.khuVucs);
                    hv.taiKhoanHoatDongs = data.TaiKhoans.Where(n => n.TrangThai == "Hoạt động").ToList();
                    hv.taiKhoanKhongHoatDongs = data.TaiKhoans.Where(n => n.TrangThai == "Không hoạt động").ToList();
                    hv.nhanViens = data.NhanViens.ToList();
                    hv.xes = data.Xes.ToList();
                    hv.donHangDienTu = data.DonHangs.Where(n => n.Loai == "Thiết bị điện tử").ToList();
                    hv.donHangGiaDung = data.DonHangs.Where(n => n.Loai == "Đồ gia dụng").ToList();
                    hv.donHangThoiTrang = data.DonHangs.Where(n => n.Loai == "Thời trang").ToList();
                    hv.donHangVanPhongPham = data.DonHangs.Where(n => n.Loai == "Văn phòng phẩm").ToList();
                    hv.donHangThucPham = data.DonHangs.Where(n => n.Loai == "Thực phẩm").ToList();
                    hv.vatTuGD = data.VatTus.Where(n => n.Loai == "Đồ gia dụng").ToList();
                    hv.vatTuDT = data.VatTus.Where(n => n.Loai == "Thiết bị điện tử").ToList();
                    hv.vatTuGH = data.VatTus.Where(n => n.Loai == "Giao hàng").ToList();
                    hv.vatTuVPP = data.VatTus.Where(n => n.Loai == "Văn phòng phẩm").ToList();
                    hv.donHangHT = data.LichSuGiaoHangs.Where(n => n.TrangThai == "Đã giao" && n.NgayGiao.Year == 2024).ToList();
                    hv.donHangHL = data.LichSuGiaoHangs.Where(n => n.TrangThai == "Đã giao" && n.NgayGiao.Year == 2025).ToList();
                }
                if (Convert.ToInt32(Session["IdUser"]) == 2)
                {
                    hv.vatTuGD = data.VatTus.Where(n => n.Loai == "Đồ gia dụng").ToList();
                    hv.vatTuDT = data.VatTus.Where(n => n.Loai == "Thiết bị điện tử").ToList();
                    hv.vatTuGH = data.VatTus.Where(n => n.Loai == "Giao hàng").ToList();
                    hv.vatTuVPP = data.VatTus.Where(n => n.Loai == "Văn phòng phẩm").ToList();
                    hv.nhanViens = data.NhanViens.ToList();
                    hv.nhanVienVanPhong = data.NhanViens.Where(n => n.Khoi == "Văn phòng").ToList();
                    hv.nhanVienGiaoHang = data.NhanViens.Where(n => n.Khoi == "Giao hàng").ToList();
                    hv.loiNhanViens = data.LoiNhanViens.ToList();
                    hv.vatTuHuHongs = data.VatTus.Where(n => n.TinhTrang == "Hư hỏng").ToList();
                    hv.vatTuMoi = data.VatTus.Where(n => n.TinhTrang == "Mới").ToList();
                    hv.vatTuDangSuaChuas = data.VatTus.Where(n => n.TinhTrang == "Đang sửa chữa").ToList();
                    hv.banGiaoVatTus = data.BanGiaoVatTus.ToList();
                    hv.lichSuBanGiaoVatTuThuHois = data.LichSuBanGiaoVatTus.Where(n => n.TrangThai == "Thu hồi").ToList();
                }
                if (Convert.ToInt32(Session["IdUser"]) == 3)
                {
                    hv.khuVucs = data.KhuVucs
                        .AsNoTracking()
                        .ToList()
                        .Select(kv => new KhuVuc
                        {
                            ToaDo = kv.ToaDo,
                            Ten = kv.Ten
                        }).ToList();
                    JsonConvert.SerializeObject(hv.khuVucs);
                    hv.donHangLuuKhos = data.DonHangs.Where(n => n.TrangThai == "Lưu kho").ToList();
                    hv.donHangDangGiaos = data.DonHangs.Where(n => n.TrangThai == "Đang giao").ToList();
                    hv.donHangDaGiaos = data.DonHangs.Where(n => n.TrangThai == "Đã giao").ToList();
                    hv.donHangSuCos = data.DonHangs.Where(n => n.TrangThai == "Sự cố").ToList();
                    hv.khuVucs = data.KhuVucs.ToList();
                    hv.xes = data.Xes.ToList();
                    hv.donHangDienTu = data.DonHangs.Where(n => n.Loai == "Thiết bị điện tử").ToList();
                    hv.donHangGiaDung = data.DonHangs.Where(n => n.Loai == "Đồ gia dụng").ToList();
                    hv.donHangThoiTrang = data.DonHangs.Where(n => n.Loai == "Thời trang").ToList();
                    hv.donHangVanPhongPham = data.DonHangs.Where(n => n.Loai == "Văn phòng phẩm").ToList();
                    hv.donHangThucPham = data.DonHangs.Where(n => n.Loai == "Thực phẩm").ToList();
                    hv.donHangHT = data.LichSuGiaoHangs.Where(n => n.TrangThai == "Đã giao" && n.NgayGiao.Year == 2024).ToList();
                    hv.donHangHL = data.LichSuGiaoHangs.Where(n => n.TrangThai == "Đã giao" && n.NgayGiao.Year == 2025).ToList();
                }
                return View(hv);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View("Error");
            }
        }
    }
}