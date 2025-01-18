
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoGiaoHangNhanh.Models
{
    public class HomeView
    {
        public List<BanGiaoVatTu> banGiaoVatTus {  get; set; }
        public List<DonHang> donHangLuuKhos { get; set; }
        public List<DonHang> donHangDangGiaos { get; set; }
        public List<DonHang> donHangDaGiaos { get; set; }
        public List<DonHang> donHangGiaDung { get; set; }
        public List<DonHang> donHangDienTu { get; set; }
        public List<DonHang> donHangVanPhongPham { get; set; }
        public List<DonHang> donHangThoiTrang { get; set; }
        public List<DonHang> donHangThucPham { get; set; }
        public List<DonHang> donHangSuCos { get; set; }
        public List<GiaoHang> giaoHangs { get; set; }
        public List<KhuVuc> khuVucs { get; set; }
        public List<LichSuBanGiaoVatTu> lichSuBanGiaoVatTuThuHois {get; set; }
        public List<LichSuGiaoHang> lichSuGiaos { get; set; }
        public List<LichSuPhanPhoi> lichSuPhanPhois { get; set; }
        public List<LoiNhanVien> loiNhanViens { get; set; }
        public List<NhanVien> nhanViens { get; set; }
        public List<NhanVien> nhanVienGiaoHang { get; set; }
        public List<NhanVien> nhanVienVanPhong { get; set; }
        public List<TaiKhoan> taiKhoanHoatDongs { get; set; }
        public List<TaiKhoan> taiKhoanKhongHoatDongs { get; set; }
        public List<VatTu> vatTuHuHongs { get; set; }
        public List<VatTu> vatTuMoi { get; set; }
        public List<VatTu> vatTuDangSuaChuas { get; set; }
        public List<Xe> xes { get; set; }
        public decimal doangThuTM { get; set; }
        public decimal doangThuTMM { get; set;}
        public decimal doangThuTMH { get; set;}
        public List<VatTu> vatTuGH { get; set; }
        public List<VatTu> vatTuVPP { get; set; }
        public List<VatTu> vatTuDT { get; set; }
        public List<VatTu> vatTuGD { get; set; }
        public List<LichSuGiaoHang> donHangHT { get; set; }
        public List<LichSuGiaoHang> donHangHL { get; set; }
    }
}